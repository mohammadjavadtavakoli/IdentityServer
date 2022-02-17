using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IdentityServer.Identity.Data.Application.Context;
using IdentityServer.Identity.Services;
using IdentityServer.Identity.UserValidator;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer.Identity
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {


            services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                config.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                config.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(config =>
            {
                config.LoginPath = "/Account/Login";
                config.LogoutPath = "/Account/Logout";
                config.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });



            var connection = _configuration.GetConnectionString("IdentityServerDbConnection");

            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;


            //            services.AddIdentityServer()
            //                .AddDeveloperSigningCredential()
            //                .AddConfigurationStore(options =>
            //                {
            //                    options.ConfigureDbContext = builder => builder.UseSqlServer(connection,
            //                        optionsBuilder => optionsBuilder.MigrationsAssembly(migrationAssembly));
            //                })
            //                .AddOperationalStore(options =>
            //                {
            //                    options.ConfigureDbContext = builder => builder.UseSqlServer(connection,
            //                        optionsBuilder => optionsBuilder.MigrationsAssembly(migrationAssembly));
            //                })
            //                .AddResourceOwnerValidator<UserValidator.UserValidator>();
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(new List<IdentityResource>
                {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile()
                })
                .AddInMemoryApiScopes(new List<ApiScope>()
                {
                    new ApiScope("api.Read")
                })
                .AddInMemoryApiResources(new List<ApiResource>
                {
                    new ApiResource("PeopleApi","people api")
                    {
                        Scopes = new List<string>(){ "api.Read" }
                    }
                })
                .AddInMemoryClients(new List<Client>
                {
                    new Client()
                    {
                        ClientId = "mtm.PostMan",
                        ClientName = "post man client",
                        ClientSecrets =
                        {
                            new Secret("Password".Sha256())
                        },
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        AllowedScopes = { "api.Read" },
                        Claims = new List<ClientClaim>
                        {
                            new ClientClaim("UserEmail","test@gmail.com"),
                            new ClientClaim("UserMobile","0912098823")
                        }
                    },
                    new Client()
                    {
                        ClientId = "ROP-client",
                        ClientSecrets =
                        {
                            new Secret("Password".Sha256())
                        },
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                        AllowedScopes = { "api.Read" },
                        Claims = new List<ClientClaim>
                        {
                            new ClientClaim("UserEmail","test@gmail.com"),
                        }
                    },
                    new Client()
                    {
                        ClientId = "code-client",
                        ClientSecrets =
                        {
                            new Secret("Password".Sha256())
                        },
                        AllowedGrantTypes = GrantTypes.Code,

                        RedirectUris =
                        {
                            "https://localhost:44312/signin-oidc"
                        },
                        PostLogoutRedirectUris = { "https://localhost:44312/signin-oidc/signout-callback-oidc" },


                        AllowedScopes = { "api.Read","openid","profile" },
                    }
                })
                .AddResourceOwnerValidator<UserValidator.UserValidator>();

            services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(_configuration.GetConnectionString("IdentityDbConnection"));
            });

            services.AddTransient<IUserService, UserService>();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //InitDataBase(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(route => { route.MapDefaultControllerRoute(); });


            app.UseIdentityServer();
        }

        public void InitDataBase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    Client c = new Client()
                    {
                        ClientId = "ROP-client",
                        ClientSecrets =
                        {
                            new Secret("Password".Sha256())
                        },
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                        AllowedScopes = { "api.Read" },
                        Claims = new List<ClientClaim>
                        {
                            new ClientClaim("UserEmail", "test@gmail.com"),
                        }
                    };
                    context.Clients.Add(c.ToEntity());
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    var r = new ApiResource("PeopleApi", "people api")
                    {
                        Scopes = new List<string>() { "api.Read" }
                    };

                    context.ApiResources.Add(r.ToEntity());
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    var s = new ApiScope("api.Read");
                    context.ApiScopes.Add(s.ToEntity());
                    context.SaveChanges();
                }


            }
        }
    }
}
