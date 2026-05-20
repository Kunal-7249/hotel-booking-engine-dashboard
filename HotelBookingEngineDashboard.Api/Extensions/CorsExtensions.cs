namespace HotelBookingEngineDashboard.Api.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                    policy.WithOrigins("http://localhost:4200", 
                    "https://hotel-booking-engine-dashboard.vercel.app"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

            return services;
        }
    }
}
