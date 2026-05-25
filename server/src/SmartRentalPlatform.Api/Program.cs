using Microsoft.AspNetCore.Authentication;
using SmartRentalPlatform.Api.Authentication;
using SmartRentalPlatform.Api.Configuration;
using SmartRentalPlatform.Api.Middleware;
using SmartRentalPlatform.Application;
using SmartRentalPlatform.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "DevAuth";
    options.DefaultChallengeScheme = "DevAuth";
    options.DefaultForbidScheme = "DevAuth";
}).AddScheme<AuthenticationSchemeOptions, DevAuthHandler>("DevAuth", _ => { });

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:5174")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAdminApprovalServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ClientApp");
app.UseDevAdminAuth();
app.UseAdminApprovalMiddleware();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
