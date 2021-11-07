using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.Certificate;
using System.Security.Cryptography.X509Certificates;

namespace TestTls
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services
                .AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
                .AddCertificate(options =>
                {
                    // Only allow chained certs, no self signed
                    options.AllowedCertificateTypes = CertificateTypes.Chained;
                    // Don't perform the check if a certificate has been revoked - requires an "online CA", which was not set up in our case.
                    options.RevocationMode = X509RevocationMode.NoCheck;
                    options.Events = new CertificateAuthenticationEvents()
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetService<ILogger<Startup>>();

                            logger.LogError(context.Exception, "Failed auth.");

                            return Task.CompletedTask;
                        },
                        OnCertificateValidated = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetService<ILogger<Startup>>();

                            logger.LogInformation("Within the OnCertificateValidated portion of Startup");

                            var caValidator = context.HttpContext.RequestServices.GetService<CertificateAuthorityValidator>();
                            if (!caValidator.IsValid(context.ClientCertificate))
                            {
                                const string failValidationMsg = "The client certificate failed to validate";
                                logger.LogWarning(failValidationMsg);
                                context.Fail(failValidationMsg);
                            }
                            
                            logger.LogInformation("You did it my dudes!");

                            return Task.CompletedTask;
                        } 
                    };
                });
            services.AddSingleton<CertificateAuthorityValidator>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TestTls", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TestTls v1"));
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseHttpsRedirection();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
    
}
public interface ICertificateAuthorityValidator
{
    bool IsValid(X509Certificate2 clientCert);
}
public class CertificateAuthorityValidator : ICertificateAuthorityValidator
{
    private readonly ILogger<CertificateAuthorityValidator> _logger;

    public CertificateAuthorityValidator(ILogger<CertificateAuthorityValidator> logger)
    {
        _logger = logger;
    }

    public bool IsValid(X509Certificate2 clientCert)
    {
        _logger.LogInformation($"Validating certificate within the {nameof(CertificateAuthorityValidator)}");
        return  clientCert.Issuer ==  "CN=serversite.com, S=Virginia, C=US";
    }
}
