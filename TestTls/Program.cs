using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;

namespace TestTls
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    // vvv requires client certificate when connecting vvv 
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.ConfigureHttpsDefaults(configureOptions =>
                        {
                            configureOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                            configureOptions.ServerCertificate = new X509Certificate2(@"C:\OpenSSL\serversite.com.p12", "pass");
                        });
                    });
                    // ^^^ requires client certificate when connecting ^^^
                });        
    }
}
