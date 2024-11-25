using Microsoft.Extensions.AI;

namespace OrionLumina.Application.CloudToEmbeddings
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


                Console.WriteLine("Me: "); //prints Me: on screen
                var inputStr = Console.ReadLine(); //accepts user input
                if (inputStr == "exit()") //if input is 'exit()' application exits
                {
                    break;
                }




                var uriString = "http://localhost:11434/";
                var modelId = "tinyllama";

                IChatClient client = new OllamaChatClient(new Uri(uriString), modelId); //Initialize chat client
                var response =
                    await client.CompleteAsync(inputStr,
                        cancellationToken: stoppingToken); //receives output asynchronously
                Console.WriteLine(); //Appends new line
                Console.WriteLine($"AI: {response.Message}"); //prints response message

                IEmbeddingGenerator<string, Embedding<float>> embed = new OllamaEmbeddingGenerator(new Uri(uriString), modelId);


                var messageLines = response.Message.ToString()
                    .Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

                var results = await embed.GenerateAsync(messageLines, cancellationToken: stoppingToken); //Generates embeddings asynchronously

                foreach (var result in results)
                {
                    Console.WriteLine($"Embedding: {result.Vector}");
                }


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
