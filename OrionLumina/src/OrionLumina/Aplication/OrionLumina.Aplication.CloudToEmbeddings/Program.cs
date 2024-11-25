namespace OrionLumina.Application.CloudToEmbeddings
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);


            var driveCredentials = builder.Configuration.GetSection("installed").Get<Credentials>();
            builder.Services.AddSingleton(driveCredentials); // Register as a singleton service

            builder.Services.AddSingleton<GoogleDriveManager>();


            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();
            host.Run();
        }
    }
}