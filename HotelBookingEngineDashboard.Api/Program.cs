using HotelBookingEngineDashboard.Api.Extensions;
using HotelBookingEngineDashboard.Api.Middleware;
using HotelBookingEngineDashboard.Application.Services;
using HotelBookingEngineDashboard.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

// Services 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerWithJwt();               
builder.Services.AddJwtAuthentication(builder.Configuration);  
builder.Services.AddCorsPolicy();                   

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate =
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
