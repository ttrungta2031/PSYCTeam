using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PsychologicalCounseling.Models;
using PsychologicalCounseling.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Aspose;
using System.IO;
using Microsoft.AspNetCore.Http;
using Hangfire;
using static PsychologicalCounseling.Services.DashboardAuthorization;
//using Hangfire;
//using Hangfire.Dashboard;

namespace PsychologicalCounseling
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        //    Aspose.Words.License lic = new Aspose.Words.License();
        //    lic.SetLicense(@"Aspose.Words.lic.xml"); 
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Enable CORS
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod()
                 .AllowAnyHeader());
            });


         
            services.AddHangfire(x => x.UseSqlServerStorage(@"Server=tcp:psycteam.database.windows.net,1433;Initial Catalog=PsychologicalCouseling;Persist Security Info=False;User ID=psycteam;Password=Admin@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));

            services.AddHangfireServer();
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddMvc();




            services.AddControllers();

            //format json
            /* services.AddControllers().AddJsonOptions(x => {
                 x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
             });*/

            /* services.AddControllers().AddNewtonsoftJson(x =>
             x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

             services.AddNewtonsoftJson(options =>
     options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
 );
 */
/*            services.AddControllers().AddNewtonsoftJson(x =>
            x.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd"
            ) ;*/
                


            //setup dich vu background











            //set up dich vu Authentication


            var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidIssuers = new string[] { Configuration["Jwt:Issuer"] },
                        ValidAudiences = new string[] { Configuration["Jwt:Audience"] },
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true
                    };
                });
            IdentityModelEventSource.ShowPII = true;



            //  var connection = Configuration.GetConnectionString("");
            services.AddDbContext<PsychologicalCouselingContext>(options =>
       options.UseSqlServer(Configuration.GetConnectionString("PsychologicalCouselingDatabase")));
            //services.AddDbContextPool<bookings3Context>(options => options.UseSqlServer(connection));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PsychologicalCounseling", Version = "v1",
                    Description = "An API design by Team 3TK",                
                    Contact = new OpenApiContact
                    {
                        Name = "Tran Ta",
                        Email = "ttrungta2031@gmail.com",
                        Url = new Uri("https://www.facebook.com/ta.tran.779"),                       
                    }
                    

                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth",
                          Name = "Bearer",
                          In = ParameterLocation.Header,
                        },
                        new List<string>()
                      }
                    });
            });

            // format json 
            /*    services.AddControllers().AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                });*/

         //   services.Configure<DBScannerSettings>(Configuration.GetSection("DBScannerSettings"));


       //   services.AddHostedService<AutoService>();
    



            //thêm mail setting cho Email service 
            //  services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            //  services.AddTransient<IMailService, MailService>();
            //  //services.AddHostedService<AutoService>();
            //   services.AddTransient<IAutoService, AutoService>();
            //  services.AddSingleton<IAutoService, AutoService>();
            //  services.AddTransient<IGetallService, Getallservice>();
            //  services.AddTransient<UserManager<User>, UserManager<User>>();



            services.AddOptions();
            services.AddSingleton<IAgoraProvider, AgoraProvider>();
            services.AddSingleton<IDrawnatalchart, Drawnatalchart>();
            //  services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor();
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<ISendNotiService, SendNotiService>();
            services.AddTransient<IDrawnatalchart, Drawnatalchart>();

            services.AddScoped<IJobTestService, JobTestService>();

            //services.AddTransient<IGetallService, Getallservice>();
            //add license aspose for word
            // auto service dbscanner




        }










        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(builder => builder
               .AllowAnyHeader()
               .AllowAnyMethod()
               .SetIsOriginAllowed((host) => true)
               .AllowCredentials().WithOrigins("https://localhost:5001",
               "http://www.psychologicalcounselingv1.somee.com",
               "https://www.psychologicalcounselingv1.somee.com",
               "https://psychological-counseling-delta.vercel.app",
               "https://psychological-counseling-admin.vercel.app")
           );


          if (env.IsDevelopment() || env.IsProduction())
           {

                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PsychologicalCounseling v1")
          ) ;
            }
            app.UseSession();
          
            app.UseHttpsRedirection();
            //authen truoc author
            app.UseAuthentication();
            app.UseRouting();
            //add license aspose for word
        
            app.UseAuthorization();
            // var option = new DashboardOptions
            // {
            //      Authorization = new[] { new MyAuthorizationFilter() }
            //   };

            // app.UseHangfireDashboard("/dashboard/adminpsyc2810", option);


            var options = new DashboardOptions
            {
                Authorization = new[] {
                    new DashboardAuthorization(new[]
                    {
                        new HangfireUserCredentials
                        {
                            Username = "adminpsyc",
                            Password = "admin1245"
                        }
                    })
                }
            };


            app.UseHangfireDashboard("/dashboard/adminpsyc2611", options);
          

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
