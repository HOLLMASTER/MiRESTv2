using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Reflection;
using System.Net;
using System.Xml.Linq;
using System.IO;
using System.Net.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authentication;



namespace MiRest_V2.Controllers
{
    [ApiController]
    [Route("/")]
    public class EinlieferungController : ControllerBase
    {
        private static readonly Dictionary<int, string> FileTypeMap = new Dictionary<int, string>
        {
            { 0, "pdf" },
            { 1, "zip" },
            { 2, "rar" },
            { 3, "7-zip" },
            { 4, "tar" },
            { 5, "gz" },
            { 6, "ps" },
            { 7, "rtf" },
            { 8, "doc" },
            { 9, "docx" },
            { 10, "xls" },
            { 11, "xlsx" },
            { 12, "txt" },
            { 13, "otf" },
            { 14, "csv" },
            { 15, "gpg" },
            { 16, "pgp" },
            { 17, "undefiniert" }
        };
        private readonly DatabaseContext _dbContext;
        public EinlieferungController(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        private bool IsAuthorized(string authorizationHeader)
        {
            if (authorizationHeader == null || string.IsNullOrEmpty(authorizationHeader))
            {
                return false;
            }

            try
            {
                var encodedCredentials = authorizationHeader.Replace("Basic", "");
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
                var username = credentials.Split(':')[0];
                var password = credentials.Split(':')[1];
                var user = _dbContext.User.FirstOrDefault(u => u.Username == username && u.Password == password);
                if (user == null)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        private string GenerateTrackingId()
        {
            return Guid.NewGuid().ToString("N");
        }

        [HttpPost]
        [Route("v2/brief")]
        public IActionResult Einlieferung([FromBody] Einlieferung einlieferung) 
        {            
            var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (!IsAuthorized(authorizationHeader))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }          

            try
            {
                var fileTypeText = FileTypeMap.ContainsKey(einlieferung.filetype) ? FileTypeMap[einlieferung.filetype] : "undefiniert";
                var trackingId = GenerateTrackingId();
                var tracking = new Tracking
                {
                    TrackingId = trackingId,
                    CreatedDate = DateTime.Now
                };
                _dbContext.Tracking.Add(tracking);
                _dbContext.SaveChanges();


                var xmlData = new XDocument(
                        new XElement("OutputXML",
                        new XElement("Filename", $"{trackingId}.{fileTypeText.ToLower()}"),
                        new XElement("Filetype", fileTypeText),
                        new XElement("TrackingID", trackingId),
                        new XElement("PLZ", einlieferung.plz),
                        new XElement("LKZ", einlieferung.lkz),
                        new XElement("RecipientID", einlieferung.recipientid),
                        new XElement("CostUnit", einlieferung.costunit),
                        new XElement("PrintMode", einlieferung.printmode),
                        new XElement("ColorMode", einlieferung.colormode),
                        new XElement("PaperMode", einlieferung.papermode),
                        einlieferung.CustomFields != null && einlieferung.CustomFields.Count > 0 ?
                        new XElement("CustomFields", einlieferung.CustomFields.Select(field => new XElement("CustomField", new XAttribute("name", field.Key), field.Value))) : null
                    )
                );

                var xmlFilePath = $"{trackingId}.xml";
                using (var fileStream = new FileStream(xmlFilePath, FileMode.Create))
                {
                    xmlData.Save(fileStream);
                }

                byte[] base64Bytes = Convert.FromBase64String(einlieferung.base64file);
                var base64FilePath = $"{trackingId}.{fileTypeText.ToLower()}";
                System.IO.File.WriteAllBytes(base64FilePath, base64Bytes);

                var response = new
                {
                    TrackingID = trackingId
                };

                return Ok(response);
            }

            catch (HttpRequestException)
            {
                return StatusCode(404, "Der aufgerufene URL ist nicht korrekt.");
            }
            catch (ValidationException ex)
            {
                return StatusCode(412, "Der übermittelte Body ist nicht korrekt: " + ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, "Die Authentifizierung ist fehlgeschlagen.");
            }                       
            catch
            {
                return StatusCode(500, "Interner Fehler.");
            }
        }

    }

    public class Einlieferung
    {
        public Einlieferung()
        {
            plz = string.Empty;
            lkz = string.Empty;
            recipientid = string.Empty;
            costunit = string.Empty;
            printmode = 0;
            colormode = 0;
            papermode = 0;
            filetype = 0;
            base64file = string.Empty;
            custom_ = string.Empty;
            CustomFields = new Dictionary<string, string>();
        }
        [Required]
        [RegularExpression(@"^\d{4,5}$", ErrorMessage = "PLZ muss 4 bis 5 Zahlen enthalten.")]
        public string plz { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "LKZ muss aus 2 großen Buchstaben bestehen.")]
        public string lkz { get; set; }

        [StringLength(10, MinimumLength = 8, ErrorMessage = "Recipientid muss 8 bis 10 Zeichen lang sein.")]
        public string recipientid { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "Costunit darf nur Zahlen enthalten.")]
        public string costunit { get; set; }

        [Range(1, 2, ErrorMessage = "Printmode muss 1 oder 2 sein.")]
        public int printmode { get; set; }

        [Range(0, 2, ErrorMessage = "Colormode muss zwischen 0 und 2 liegen.")]
        public int colormode { get; set; }

        [Range(0, 2, ErrorMessage = "Papermode muss zwischen 0 und 2 liegen.")]
        public int papermode { get; set; }

        [Range(0, 17, ErrorMessage = "Ungültiger Dateityp.")]
        public int filetype { get; set; }

        [Required]
        public string base64file { get; set; }
        public string custom_ { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }
    }

}

