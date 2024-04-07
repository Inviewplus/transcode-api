using Oci.ObjectstorageService.Models;
using Oci.ObjectstorageService.Responses;

namespace Transcoder.Infrastructure.OciClient
{
    public interface IOracleClient
    {
        Task<List<ObjectSummary>> GetFiles();
        Task<GetObjectResponse> GetFile(string filename);
        Task PutFile(string idProduct, string objectName, MemoryStream memoryStream);
        Task<string> GetUrl(string productId);
    }
}
