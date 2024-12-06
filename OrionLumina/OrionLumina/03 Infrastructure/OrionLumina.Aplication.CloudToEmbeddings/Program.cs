using Orion.Lumina.Application;
using Orion.Lumina.Domain;

namespace OrionLumina.Infrastructure.CloudToEmbeddings
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);


            var driveCredentials = builder.Configuration.GetSection("installed").Get<Credentials>();
            builder.Services.AddSingleton(driveCredentials); // Register as a singleton service

            builder.Services.AddSingleton<GoogleDriveManager>();
            builder.Services.AddSingleton<FileReaderFactory>();
            builder.Services.AddTransient<IFileReader, PdfFileReader>();
            builder.Services.AddTransient<IFileReader, DocxFileReader>();
            builder.Services.AddTransient<IFileReader, ExcelFileReader>();

            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();
            host.Run();
        }
    }



}