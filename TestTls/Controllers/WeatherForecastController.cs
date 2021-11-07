using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;

namespace TestTls.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    [Authorize(AuthenticationSchemes = CertificateAuthenticationDefaults.AuthenticationScheme)]    
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }
/*
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
*/
        //[HttpPost]
        [HttpPost("GetUserInfo")]      
        public Dictionary<string, object> Post()
        {
            var body = HttpContext.Request.ToString();
            return new Dictionary<string, object>(){
                {"FirstName", "Steve"},
                {"LastName", "Eisner"},
                {"Login", "Steve Eisner"},
                {"IsAdministrator", true}
 /*
                Activated = true,
                Id = "eisnerw",
                Login = certSubject.ToLower().Contains("joan") ? "Joan Eisner" : "Bill Eisner",
                LangKey = "en",
                CreatedBy = "System",
                CreatedDate = new System.DateTime()*/
            };
 
        } 
    }
}
