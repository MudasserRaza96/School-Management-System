using FastReport.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SchoolApiService.Services;
using SchoolApiService.Services.Interfaces;
using SchoolApiService.Services.Implementations;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels.SecurityModels;
using System.Text;
using System.Text.Json.Serialization;

namespace SchoolApiService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors();

            builder.Services.TryAddScoped<ImageUploadService, ImageUploadService>();

            // Register Business Services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<IStaffService, StaffService>();
            builder.Services.AddScoped<IStaffExperienceService, StaffExperienceService>();
            builder.Services.AddScoped<IStaffSalaryService, StaffSalaryService>();
            builder.Services.AddScoped<IAttendanceService, AttendanceService>();
            builder.Services.AddScoped<IExamTypeService, ExamTypeService>();
            builder.Services.AddScoped<IExamScheduleService, ExamScheduleService>();
            builder.Services.AddScoped<IExamScheduleStandardService, ExamScheduleStandardService>();
            builder.Services.AddScoped<IMarkService, MarkService>();
            builder.Services.AddScoped<IMarkEntryService, MarkEntryService>();
            builder.Services.AddScoped<IFeeService, FeeService>();
            builder.Services.AddScoped<IFeeTypeService, FeeTypeService>();
            builder.Services.AddScoped<IDueBalanceService, DueBalanceService>();
            builder.Services.AddScoped<IMonthlyPaymentService, MonthlyPaymentService>();
            builder.Services.AddScoped<IOthersPaymentService, OthersPaymentService>();
            builder.Services.AddScoped<IStandardService, StandardService>();
            builder.Services.AddScoped<ISubjectService, SubjectService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IAcademicMonthService, AcademicMonthService>();
            builder.Services.AddScoped<ICommonService, CommonService>();
            builder.Services.AddScoped<IWebReportService, WebReportService>();

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            var connectionString = builder.Configuration.GetConnectionString("LocalDbConnection");
            builder.Services.AddDbContext<SchoolDbContext>(options =>
              options.UseSqlServer(connectionString));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<SchoolDbContext>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Demo API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Description = "Please insert JWT token into field"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            builder.Services.AddScoped<ITokenService, TokenService>();

            var validIssuer = builder.Configuration.GetValue<string>("JwtTokenSettings:ValidIssuer");
            var validAudience = builder.Configuration.GetValue<string>("JwtTokenSettings:ValidAudience");
            var symmetricSecurityKey = builder.Configuration.GetValue<string>("JwtTokenSettings:SymmetricSecurityKey");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.UseSecurityTokenValidators = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = validIssuer,
                    ValidAudience = validAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(symmetricSecurityKey)
                    ),
                    SaveSigninToken = true
                };
            });

            FastReport.Utils.RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));

            builder.Services.AddFastReport();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseStatusCodePages();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(opt =>
            {
                opt.AllowAnyHeader();
                opt.AllowAnyMethod();
                opt.AllowAnyOrigin();
            });

            app.MapControllers();
            app.UseFastReport();

            app.Run();
        }
    }
}
