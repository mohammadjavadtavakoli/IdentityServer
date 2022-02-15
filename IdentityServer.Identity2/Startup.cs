using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework.DbContexts;
using System.Linq;
using IdentityServer4.EntityFramework.Mappers;

namespace IdentityServer.Identity
{
    public class Startup
    {
        public IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
          
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connection = Configuration.GetConnectionString("IdentityDbConnection");
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connection,
                            optionBuilder => optionBuilder.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connection,
                            optionBuilder => optionBuilder.MigrationsAssembly(migrationsAssembly));
                });
            #region InMemory

            // services.AddIdentityServer()
            //     .AddDeveloperSigningCredential()
            //     .AddInMemoryApiScopes(new List<ApiScope>()
            //     {
            //         new ApiScope("api.read")
            //     })
            //     .AddInMemoryApiResources(new List<ApiResource>
            //     {
            //         new ApiResource("peopleApi", "people-api")
            //         {
            //             Scopes = new List<string>() {"api.read"}
            //         }
            //     })
            //     .AddInMemoryClients(new List<Client>
            //     {
            //         new Client()
            //         {
            //             ClientId = "m2m.postman",
            //             ClientName = "post man client",
            //             ClientSecrets =
            //             {
            //                 new Secret("password".Sha256())
            //             },
            //             AllowedGrantTypes = {GrantType.ClientCredentials},
            //             AllowedScopes = {"api.read"},
            //             Claims = new List<ClientClaim>()
            //             {
            //                 new ClientClaim("UserEmail", "test@gmail.com"),
            //                 new ClientClaim("UserMobile", "09120000000")
            //             }
            //         },
            //         //just for test
            //         new Client()
            //         {
            //             ClientId = "ROP-Client",
            //             ClientSecrets =
            //             {
            //                 new Secret("password".Sha256())
            //             },
            //             AllowedGrantTypes = {GrantType.ResourceOwnerPassword},
            //             AllowedScopes = {"api.read"},
            //             Claims = new List<ClientClaim>()
            //             {
            //                 new ClientClaim("UserEmail", "test@gmail.com"),
            //                 new ClientClaim("UserMobile", "09120000000")
            //             }
            //         }
            //     })
            //     .AddResourceOwnerValidator<UserValidator.UserValidator>();

            #endregion

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            initDataBase(app);
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

        public void initDataBase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var ctx = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                ctx.Database.Migrate();
                if (!ctx.Clients.Any())
                {

                    var client = new Client()
                    {
                        ClientId = "ROP-Client",
                        ClientSecrets =
                                 {
                                     new Secret("password".Sha256())
                                 },
                        AllowedGrantTypes = { GrantType.ResourceOwnerPassword },
                        AllowedScopes = { "api.read" },
                        Claims = new List<ClientClaim>()
                                 {
                                     new ClientClaim("UserEmail", "test@gmail.com"),
                                     new ClientClaim("UserMobile", "09120000000")
                                 }
                    };
                    ctx.Clients.Add(client.ToEntity());
                    ctx.SaveChanges();
                }

                if (!ctx.ApiResources.Any())
                {
                    var ApiResource = new ApiResource("peopleApi", "people-api")
                    {
                        Scopes = new List<string>() { "api.read" }
                    };
                    ctx.ApiResources.Add(ApiResource.ToEntity());
                    ctx.SaveChanges();
                }

                if (!ctx.ApiScopes.Any())
                {
                    var ApiScopes = new ApiScope("api.read");
 
                    ctx.ApiScopes.Add(ApiScopes.ToEntity());
                    ctx.SaveChanges();
                }

            }
        }
    }
}