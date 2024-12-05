namespace OrionLumina.Application.CloudToEmbeddings;

public class Worker(ILogger<Worker> logger) : BackgroundService
{



    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {



        while (!stoppingToken.IsCancellationRequested)
        {


            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}


public class TrainingSession(ILogger<TrainingSession> logger)
{
    public string ModelName { get; set; } = "Meta-Llama-3.1-70B-Instruct";


    public async Task<Result> TrainAsync()
    {

        logger.LogInformation("Training Session started {Time}", DateTime.Now.ToLocalTime());


        return new Result();

    }

}