using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Identity
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
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryApiScopes(new List<ApiScope>()
                {
                    new ApiScope("api.read")
                })
                .AddInMemoryApiResources(new List<ApiResource>
                {
                    new ApiResource("peopleApi","people-api")
                    {
                        Scopes=new List<string>(){"api.read"}
                    }
                })
                .AddInMemoryClients(new List<Client>
                {
                    new Client()
                    {
                        ClientId="m2m.postman",
                        ClientName="post man client",
                        ClientSecrets =
                        {
                            new Secret("password".Sha256())
                        },
                        AllowedGrantTypes={GrantType.ClientCredentials},
                        AllowedScopes={ "api.read" }
                    }
                });;
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseIdentityServer();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages();
            //});
        }
    }
}
