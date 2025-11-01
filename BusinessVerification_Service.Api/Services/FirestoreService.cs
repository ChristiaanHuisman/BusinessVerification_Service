using BusinessVerification_Service.Api.Interfaces.ServicesInterfaces;
using Google.Cloud.Firestore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BusinessVerification_Service.Api.Services
{
    public class FirestoreService : IFirestoreService
    {
        // Inject dependencies
        private readonly FirestoreDb _firestoreDb;

        // Constructor for dependency injection
        public FirestoreService (FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        // Generic method
        //
        // Need to carefully specify path to document when calling the method
        //
        // Retrieve relevant model from Firestore spcified document
        public async Task<T?> GetDocumentFromFirestore<T>(string documentPath)
            where T : class
        {
            try
            {
                // Get a snapshot of the specific Firestore document
                DocumentReference documentReference = _firestoreDb.Document(documentPath);
                DocumentSnapshot documentSnapshot = await
                    documentReference.GetSnapshotAsync();
                if (!documentSnapshot.Exists) return null;

                // Get raw dictionary
                Dictionary<string, object> documentDictionary
                    = documentSnapshot.ToDictionary();

                // Do conversions so that model is populated without being case sensitive
                string json = JsonSerializer.Serialize(documentDictionary);
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters =
                    {
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true)
                    }
                };

                // Return the dictionary as the relevant model
                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch
            {
                // If the document does not exist
                return null;
            }
        }

        // Generic method
        //
        // Need to carefully specify path to document when calling the method
        //
        // Overwrite and merge relevant model to Firestore spcified document
        public async Task SetDocumentByFirestorePath<T>(string documentPath, T document)
            where T : class
        {
            // Overwrite existing fields, create fields that do not exist, and
            // do not remove other fields
            DocumentReference documentReference = _firestoreDb.Document(documentPath);
            await documentReference.SetAsync(document, SetOptions.MergeAll);
        }

        // For successful conversion between model enums and Firestore strings
        public class FirestoreEnumStringConverter<TEnum> : IFirestoreConverter<TEnum>
            where TEnum : struct, Enum
        {
            public TEnum FromFirestore(object value)
            {
                if (value is string s && Enum.TryParse<TEnum>(s, true, out var result))
                {
                    return result;
                }
                throw new InvalidOperationException($"Cannot convert '{value}' to enum {typeof(TEnum)}");
            }

            public object ToFirestore(TEnum value)
            {
                return value.ToString();
            }
        }
    }
}
