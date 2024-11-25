﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace OrionLumina.Application.CloudToEmbeddings;


public class GoogleDriveManager(ILogger<Worker> GoogleDriveDownloadFile, Credentials driveCredentials)
{

    public async Task ListAsync()
    {
        string[] scopes = { DriveService.Scope.Drive };
        string applicationName = "OrionLumina";

        // Authentication with Google
        UserCredential credential;

        using (var stream = new MemoryStream())
        {
            // Convert credentials to JSON format
            var credentialsJson = $@"
            {{
                ""installed"": {{
                    ""client_id"": ""{driveCredentials.Client_Id}"",
                    ""project_id"": ""{driveCredentials.Project_Id}"",
                    ""auth_uri"": ""{driveCredentials.Auth_Uri}"",
                    ""token_uri"": ""{driveCredentials.Token_Uri}"",
                    ""auth_provider_x509_cert_url"": ""{driveCredentials.Auth_Provider_X509_Cert_Url}"",
                    ""client_secret"": ""{driveCredentials.Client_Secret}"",
                    ""redirect_uris"": [""http://localhost""]
                }}
            }}";

            // Write credentials to memory stream
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                writer.Write(credentialsJson);
                writer.Flush();
                stream.Position = 0; // Reset stream position for reading
            }

            // Authenticate the user
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                scopes,
                "user",
                CancellationToken.None,
                new FileDataStore("OrionLuminaDriveToken", true));
        }

        // Create the Drive API service
        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = applicationName,
        });

        // List root folders
        Console.WriteLine("Listing root folders...");
        var request = service.Files.List();
        request.Q = "'root' in parents and mimeType = 'application/vnd.google-apps.folder' and trashed = false";
        request.Fields = "nextPageToken, files(id, name)";

        var result = await request.ExecuteAsync();
        if (result.Files != null && result.Files.Count > 0)
        {
            Console.WriteLine("Folders found:");
            foreach (var file in result.Files)
            {
                Console.WriteLine($"- {file.Name} (ID: {file.Id})");
            }
        }
        else
        {
            Console.WriteLine("No folders found.");
        }
    }
    static async Task Download()
    {
        string[] Scopes = { DriveService.Scope.DriveFile };
        string ApplicationName = "OrionLumina";
        var credentialsFile = "credentials.json";
        var user = "exxerpro@exxerpro.com";
        var clientId = "896742639071-l28vlph1hjcu9sg83fg4np7j636uhd36.apps.googleusercontent.com";


        // Authenticate and create the Drive API service.
        UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            new FileStream(credentialsFile, FileMode.Open, FileAccess.Read),
            Scopes,
            "user",
            CancellationToken.None);

        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        // File metadata and file creation.
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = "SampleFile.txt"
        };

        FilesResource.CreateMediaUpload request;
        using (var stream = new FileStream("path/to/your/file.txt", FileMode.Open))
        {
            request = service.Files.Create(fileMetadata, stream, "text/plain");
            request.Fields = "id";
            await request.UploadAsync();
        }

        var file = request.ResponseBody;
        Console.WriteLine($"File ID: {file.Id}");
    }
}
