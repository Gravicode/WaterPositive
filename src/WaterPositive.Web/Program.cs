using WaterPositive.Web.Components;
using WaterPositive.Web.Data;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Blazored.Toast;
using WaterPositive.Tools;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
using WaterPositive.Web.Helpers;
using System.Net;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using ServiceStack.Redis;
using WaterPositive.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;

namespace WaterPositive.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddRazorPages();
            //builder.Services.AddServerSideBlazor();
            builder.Services.AddMudServices();

            // ******
            // BLAZOR COOKIE Auth Code (begin)
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            // BLAZOR COOKIE Auth Code (end)

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<HttpContextAccessor>();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<HttpClient>();
            builder.Services.AddTransient<AzureBlobHelper>();
            builder.Services.AddTransient<UserProfileService>();
            builder.Services.AddTransient<WaterDepotService>();
            builder.Services.AddTransient<CCTVService>();
            builder.Services.AddTransient<DataCounterService>();
            builder.Services.AddTransient<SensorDataService>();
            builder.Services.AddTransient<WaterUsageService>();
            builder.Services.AddTransient<UserProfileService>();
            builder.Services.AddTransient<WaterPriceService>();
            builder.Services.AddTransient<WaterTankDataService>();
            builder.Services.AddTransient<ReportService>();
            builder.Services.AddTransient<UsageLimitService>();
            
            builder.Services.AddTransient<IRestApiService, RestApiService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().WithMethods("GET, PATCH, DELETE, PUT, POST, OPTIONS"));
            });
            var configBuilder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false);
            IConfiguration Configuration = configBuilder.Build();

            AppConstants.SQLConn = Configuration["ConnectionStrings:SqlConn"];
            AppConstants.RedisCon = Configuration["ConnectionStrings:RedisCon"];
            AppConstants.BlobConn = Configuration["ConnectionStrings:BlobConn"];
            AppConstants.GMapApiKey = Configuration["GmapKey"];

            AppConstants.ReportPeopleCounter = Configuration["Reports:ReportPeopleCounter"];
            AppConstants.ReportWeather = Configuration["Reports:ReportWeather"];
            AppConstants.ReportKartu = Configuration["Reports:Kartu"];

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddBlazoredToast();

            MailService.MailUser = Configuration["MailSettings:MailUser"];
            MailService.MailPassword = Configuration["MailSettings:MailPassword"];
            MailService.MailServer = Configuration["MailSettings:MailServer"];
            MailService.MailPort = int.Parse(Configuration["MailSettings:MailPort"]);
            MailService.SetTemplate(Configuration["MailSettings:TemplatePath"]);
            MailService.SendGridKey = Configuration["MailSettings:SendGridKey"];
            MailService.UseSendGrid = true;


            SmsService.UserKey = Configuration["SmsSettings:ZenzivaUserKey"];
            SmsService.PassKey = Configuration["SmsSettings:ZenzivaPassKey"];
            SmsService.TokenKey = Configuration["SmsSettings:TokenKey"];
            AppConstants.StorageEndpoint = Configuration["Storage:Endpoint"];
            AppConstants.StorageAccess = Configuration["Storage:Access"];
            AppConstants.StorageSecret = Configuration["Storage:Secret"];
            AppConstants.StorageBucket = Configuration["Storage:Bucket"];
            AppConstants.UploadUrlPrefix = Configuration["UploadUrlPrefix"];
            var setting = new StorageSetting() { };
            setting.Bucket = AppConstants.StorageBucket;
            setting.SecretKey = AppConstants.StorageSecret;
            setting.AccessKey = AppConstants.StorageAccess;
            builder.Services.AddSingleton(setting);
            builder.Services.AddTransient<StorageObjectService>();
            builder.Services.AddSingleton(new RedisManagerPool(AppConstants.RedisCon));

            builder.Services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("103.189.234.66"));
            });

            builder.Services.AddSignalR(hubOptions =>
            {
                hubOptions.MaximumReceiveMessageSize = 128 * 1024; // 1MB
            });


            builder.Services.AddControllers();

            #region Rest API
            var securityScheme = new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JSON Web Token based security",
            };

            var securityReq = new OpenApiSecurityRequirement()
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }
};

            var contact = new OpenApiContact()
            {
                Name = "Water Positive Team",
                Email = "waterpositive@alkademi.com",
                Url = new Uri("https://alkademi.com")
            };

            var license = new OpenApiLicense()
            {
                Name = "Water Positive API License",
                Url = new Uri("https://alkademi.com/license.html")
            };

            var info = new OpenApiInfo()
            {
                Version = "v1",
                Title = "Water Positive Data API v1.0",
                Description = "API for accessing Water Positive data",
                TermsOfService = new Uri("https://alkademi.com/terms.html"),
                Contact = contact,
                License = license
            };

            // Add JWT configuration (support multi auth scheme, jwt and cookie)
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/auth/login";
                    options.AccessDeniedPath = "/auth/forbidden";
                })
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey
                            (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    CookieAuthenticationDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder =
                    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", info);
                o.AddSecurityDefinition("Bearer", securityScheme);
                o.AddSecurityRequirement(securityReq);
            });

            #endregion

            var app = builder.Build();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            //swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Water Positive Data API V1");
            });
            //end swagger
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAntiforgery();

            // ******
            // BLAZOR COOKIE Auth Code (begin)
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();
            // BLAZOR COOKIE Auth Code (end)
            // ******

            app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true) // allow any origin  
            .AllowCredentials());               // allow credentials 

            // BLAZOR COOKIE Auth Code (begin)
            app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            // BLAZOR COOKIE Auth Code (end)

            //app.MapBlazorHub();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();
            app.MapRazorPages();

            var db = new WaterPositiveDB();
            db.Database.EnsureCreated();

            app.Run();
        }
    }
}
