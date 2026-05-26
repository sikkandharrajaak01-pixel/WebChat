using Chat_App.Services.Interfaces;
namespace Chat_App.Services.Implementations
{
    public class MomentCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MomentCleanupService> _logger;
        public MomentCleanupService(IServiceProvider serviceProvider, ILogger<MomentCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Moment cleanup service started");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var momentService = scope.ServiceProvider.GetRequiredService<IMomentService>();
                    await momentService.DeleteExpiredMoments();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting expired moments");
                }
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}