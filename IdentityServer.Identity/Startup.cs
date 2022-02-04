using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

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
                    new ApiResource("peopleApi", "people-api")
                    {
                        Scopes = new List<string>() {"api.read"}
                    }
                })
                .AddInMemoryClients(new List<Client>
                {
                    new Client()
                    {
                        ClientId = "m2m.postman",
                        ClientName = "post man client",
                        ClientSecrets =
                        {
                            new Secret("password".Sha256())
                        },
                        AllowedGrantTypes = {GrantType.ClientCredentials},
                        AllowedScopes = {"api.read"},
                        Claims = new List<ClientClaim>()
                        {
                            new ClientClaim("UserEmail", "test@gmail.com"),
                            new ClientClaim("UserMobile", "09120000000")
                        }
                    },
                    //just for test
                    new Client()
                    {
                        ClientId = "ROP-Client",
                        ClientSecrets =
                        {
                            new Secret("password".Sha256())
                        },
                        AllowedGrantTypes = {GrantType.ResourceOwnerPassword},
                        AllowedScopes = {"api.read"},
                        Claims = new List<ClientClaim>()
                        {
                            new ClientClaim("UserEmail", "test@gmail.com"),
                            new ClientClaim("UserMobile", "09120000000")
                        }
                    }
                })
                .AddResourceOwnerValidator<UserValidator.UserValidator>();
            ;
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