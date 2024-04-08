using Oci.Common.Auth;
using Oci.ObjectstorageService;

namespace Transcoder.Infrastructure.OciClient
{
    public class OracleProvider
    {
        public ObjectStorageClient ObjectStoreProvider()
        {
            var profileProvider = new ConfigFileAuthenticationDetailsProvider(
                                           Environment.GetEnvironmentVariable("OCI_AUTH_PATH"), 
                                           Environment.GetEnvironmentVariable("OCI_AUTH_PROFILE")
                                           );

            return new ObjectStorageClient(profileProvider);
        }
    }
}
