using BusinessVerification_Service.Api.Interfaces.ServicesInterfaces;
using Google.Cloud.Firestore;

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
            // Get a snapshot of the specific Firestore document
            DocumentReference documentReference = _firestoreDb.Document(documentPath);
            DocumentSnapshot documentSnapshot = await
                documentReference.GetSnapshotAsync();

            // Return the snapshot as the relevant model
            return documentSnapshot.Exists
                ? documentSnapshot.ConvertTo<T>()
                : null;
        }
    }
}
