using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace HotelBookingEngineDashboard.WarmupFunction
{
    public class WarmupFunction
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WarmupFunction> _logger;
        private readonly string _healthUrl;

        public WarmupFunction(
            IHttpClientFactory httpClientFactory,
            ILogger<WarmupFunction> logger,
            IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("WarmupClient");
            _logger = logger;
            _healthUrl = configuration["HealthCheckUrl"]
                ?? throw new InvalidOperationException("HealthCheckUrl is not configured.");
        }

        [Function("WarmupPing")]
        public async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo timer)
        {
            _logger.LogInformation("Warmup ping started at {Time}", DateTime.UtcNow);

            try
            {
                var response = await _httpClient.GetAsync(_healthUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation(
                        "Warmup ping successful. Status: {Status}, Body: {Body}",
                        response.StatusCode, content);
                }
                else
                {
                    _logger.LogWarning(
                        "Warmup ping returned non-success status: {Status}",
                        response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Warmup ping failed: {Message}", ex.Message);
            }
        }
    }
}
