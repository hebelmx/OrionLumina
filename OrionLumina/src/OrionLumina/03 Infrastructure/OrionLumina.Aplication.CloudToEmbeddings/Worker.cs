namespace OrionLumina.Infrastructure.CloudToEmbeddings
{
    public class Worker(ILogger<Worker> logger,
        GoogleDriveManager googleDriveManager,
        Credentials driveCredentials) : BackgroundService
    {



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {



            while (!stoppingToken.IsCancellationRequested)
            {

                await googleDriveManager.ListAsync();





                //todo test rag 
                //todo test fine tune


                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }


    }
}
