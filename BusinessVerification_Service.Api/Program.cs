using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

namespace BusinessVerification_Service.Api
{
    public class Program
    {
        public static void Main(string[] args)
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
                var credentialPath = builder.Configuration["Firestore:CredentialsPath"];
                var projectId = builder.Configuration["Firestore:ProjectId"];
                var googleCredential = GoogleCredential.FromFile(credentialPath);
                var firestoreClient = new FirestoreClientBuilder
                {
                    Credential = googleCredential
                }.Build();
                var firestoreDb = FirestoreDb.Create(projectId, client: firestoreClient);
                builder.Services.AddSingleton(firestoreDb);
                Console.WriteLine("Connected to Firestore successfully.");
            }
            catch
            {
                // Stop the program if the connection to Firestore fails
                Console.WriteLine("Failed to connect to Firestore.");
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
