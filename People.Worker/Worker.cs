using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using People.Worker.Services;

namespace People.Worker;

public sealed class Worker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<Worker> _logger;

    public Worker(
        IServiceScopeFactory scopeFactory,
        ILogger<Worker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var processor = scope.ServiceProvider
                    .GetRequiredService<OutboxProcessor>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing outbox.");
            }

            // intervalo entre leituras
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}
