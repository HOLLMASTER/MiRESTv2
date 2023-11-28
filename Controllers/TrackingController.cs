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
    [Route("v2/brief")]
    public class TrackingController : ControllerBase
    {
        private readonly DatabaseContext _dbContext;

        public TrackingController(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: tracking/{trackingId}
        [HttpGet("{trackingId}")]
        public IActionResult GetTrackingById(string trackingId)
        {
            var tracking = _dbContext.Trackings.FirstOrDefault(t => t.TrackingId == trackingId);
            if (tracking == null)
            {
                return NotFound();
            }
            var result = new
            {
                tracking.TrackingId,
                CreatedDate = tracking.CreatedDate.HasValue ? tracking.CreatedDate.Value.ToString("yyyy-MM-dd - HH:mm:ss") : null,
            };
            return Ok(result);
        }

        // GET: tracking
        [HttpGet]
        public IActionResult GetAllTrackings()
        {
            var trackings = _dbContext.Trackings.ToList();
            var results = trackings.Select(t => new
            {
                t.TrackingId,
                CreatedDate = t.CreatedDate.HasValue ? t.CreatedDate.Value.ToString("yyyy-MM-dd - HH:mm:ss") : null,}).ToList();
            return Ok(results);
        }
    }


}