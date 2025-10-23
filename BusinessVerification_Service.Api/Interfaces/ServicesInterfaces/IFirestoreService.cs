namespace BusinessVerification_Service.Api.Interfaces.ServicesInterfaces
{
    // Include methods from the helper
    public interface IFirestoreService
    {
        Task<T?> GetDocumentFromFirestore<T>(string documentPath) where T : class;
    }
}
