using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using AspNetCore.Authentication.Basic;

namespace MiRest_V2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddDbContext<DatabaseContext>();

            var app = builder.Build();

            //Configure the HTTP request pipeline.
            app.UseMiddleware<HttpMethodNotAllowedMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
    
    public class HttpMethodNotAllowedMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpMethodNotAllowedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method != "POST" && context.Request.Method != "GET") //&& context.Request.Method != "GET")
            {
                context.Response.StatusCode = 405; // Method Not Allowed
                await context.Response.WriteAsync("Die gewählte HTTP Methode ist nicht akzeptiert.");
            }
            else
            {
                await _next(context);
            }
        }
    }
}