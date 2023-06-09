using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using GraduationProject.DataBase.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GraduationProject.Service.Interfaces;
using GraduationProject.Service.Services;
using GraduationProject.DataBase.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<GraduationProjectDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IClinicService, ClinicService>();
builder.Services.AddScoped<IDynamic_AttributeService, Dynamic_AttributeService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<ISecretaryService, SecretaryService>();
builder.Services.AddScoped<IAttributeViewManagement, AttributeViewManagement>();
builder.Services.AddScoped<IClinicService, ClinicService>();
builder.Services.AddScoped<IWorking_DaysService, Working_DaysService>();
builder.Services.AddScoped<IMedicalInformationService, MedicalInformationService>();
builder.Services.AddScoped<IMyClinicService, MyClinicService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddDateOnlyTimeOnlyStringConverters();
builder.Services.AddSwaggerGen(c => c.UseDateOnlyTimeOnlyStringConverters());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://localhost:7242/",
            ValidAudience = "https://localhost:7242/",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("veryVerySecretKey"))
        };
    });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();