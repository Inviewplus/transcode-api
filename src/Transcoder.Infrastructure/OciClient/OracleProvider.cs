using Oci.Common.Auth;
using Oci.ObjectstorageService;

namespace Transcoder.Infrastructure.OciClient
{
    public class OracleProvider
    {
        public ObjectStorageClient ObjectStoreProvider()
        {
            return new ObjectStorageClient(new ConfigFileAuthenticationDetailsProvider("./oracle/.oci/config", "DEFAULT"));
        }
    }
}
