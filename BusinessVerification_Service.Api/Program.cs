using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Nager.PublicSuffix;
using Nager.PublicSuffix.RuleProviders;

namespace BusinessVerification_Service.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            try
            {
                // Register FirestoreDb as a Singleton
                //
                // Only done once per application lifetime as it is for prototyping purposes
                //
                // Having a periodic refresh of the credentials can be added in future
                // versions of the API for performance improvements when the application
                // scales to a more permanent hosting solution
                string credentialPath = builder.Configuration["Firestore:CredentialsPath"];
                string projectId = builder.Configuration["Firestore:ProjectId"];
                GoogleCredential googleCredential = GoogleCredential.FromFile(credentialPath);
                FirestoreClient firestoreClient = new FirestoreClientBuilder
                {
                    Credential = googleCredential
                }.Build();
                FirestoreDb firestoreDb = FirestoreDb.Create(projectId, client: firestoreClient);
                builder.Services.AddSingleton(firestoreDb);
                Console.WriteLine("Connected to Firestore successfully.");
            }
            catch
            {
                // Stop the program if the connection to Firestore fails
                Console.WriteLine("Failed to connect to Firestore.");
                throw;
            }

            try
            {
                // Register IDomainParser as a Singleton asynchronously
                //
                // Only done once per application lifetime as it is for prototyping purposes
                //
                // Caching the public suffix list periodically can be added in future
                // verisons for performance improvements when the application scales
                // to a more permanent hosting solution
                SimpleHttpRuleProvider ruleProvider = new SimpleHttpRuleProvider();
                await ruleProvider.BuildAsync();
                DomainParser domainParser = new DomainParser(ruleProvider);
                builder.Services.AddSingleton<IDomainParser>(domainParser);
                Console.WriteLine("Downloaded the public suffix list successfully.");
            }
            catch
            {
                // Stop the program if getting the public suffix list fails
                Console.WriteLine("Failed to download the public suffix list.");
                throw;
            }

            // Add interfaces of services and helpers to the container

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
