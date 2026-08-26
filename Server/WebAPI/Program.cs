using Core.Models;
using Core.Repository;
using Core.Services;
using Data;
using Data.DataRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// הגדרות JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
// רישום ה-DbContext
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Vacation apartments")));

// רישום Repositories
builder.Services.AddScoped<IPropertiesReposiory, PropertiesRepository>();
builder.Services.AddScoped<IAmenitiesRepository, AmenitiesRepository>();
builder.Services.AddScoped<IOwnersRepository, OwnersRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ILoginRequestService, LoginRequestService>();
builder.Services.AddScoped<IImagesRepository, ImagesRepository>();

// רישום Services
builder.Services.AddScoped<IPropertiesService, PropertiesService>();
builder.Services.AddScoped<IAmenitiesService, AmenitiesService>();
builder.Services.AddScoped<IOwnersService, OwnerService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IImagesService, ImagesService>();

// הגדרות Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// רישום AutoMapper
builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());

// הגדרת CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// הגשת קבצים סטטיים מתוך wwwroot (כולל תיקיית images שבתוכה)
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// הפעלת ה-CORS
app.UseCors("AllowAngular");

app.UseAuthorization();
app.MapControllers();
app.Run();