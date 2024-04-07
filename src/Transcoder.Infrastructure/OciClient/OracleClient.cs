using Oci.ObjectstorageService.Models;
using Oci.ObjectstorageService.Requests;
using Oci.ObjectstorageService.Responses;

namespace Transcoder.Infrastructure.OciClient
{
    public class OracleClient : IOracleClient
    {
        private readonly OracleProvider _oracleProvider;

        public OracleClient(OracleProvider oracleProvider)
        {
            _oracleProvider = oracleProvider;
        }

        public async Task<GetObjectResponse> GetFile(string filename)
        {
            var getObjectRequest = new GetObjectRequest
            {
                NamespaceName = Environment.GetEnvironmentVariable("NAMESPACENAME_ORACLE"),
                BucketName = Environment.GetEnvironmentVariable("BUCKET_PRODUCTION"),
                ObjectName = filename
            };

            var response = await _oracleProvider.ObjectStoreProvider().GetObject(getObjectRequest);

            return response;
        }

        public async Task<List<ObjectSummary>> GetFiles()
        {
            var listObjectsRequest = new ListObjectsRequest
            {
                NamespaceName = Environment.GetEnvironmentVariable("NAMESPACENAME_ORACLE"),
                BucketName = Environment.GetEnvironmentVariable("BUCKET_PRODUCTION")
            };

            var listObjectsResponse = (await _oracleProvider.ObjectStoreProvider().ListObjects(listObjectsRequest)).ListObjects.Objects.ToList();

            return listObjectsResponse;
        }

        public async Task<string> GetUrl(string productId)
        {
            var createPreauthenticatedRequestDetails = new CreatePreauthenticatedRequestDetails
            {
                Name = $"PreSign-{DateTime.Now.ToString()}",
                BucketListingAction = Oci.ObjectstorageService.Models.PreauthenticatedRequest.BucketListingActionEnum.Deny,
                ObjectName = $"{productId}/",
                AccessType = Oci.ObjectstorageService.Models.CreatePreauthenticatedRequestDetails.AccessTypeEnum.AnyObjectRead,
                TimeExpires = DateTime.Now.AddHours(2),
            };
            var createPreauthenticatedRequestRequest = new Oci.ObjectstorageService.Requests.CreatePreauthenticatedRequestRequest
            {
                NamespaceName = Environment.GetEnvironmentVariable("NAMESPACENAME_ORACLE"),
                BucketName = Environment.GetEnvironmentVariable("BUCKET_PRODUCTION_POS"),
                CreatePreauthenticatedRequestDetails = createPreauthenticatedRequestDetails,
            };

            var signedUrl = await _oracleProvider.ObjectStoreProvider().CreatePreauthenticatedRequest(createPreauthenticatedRequestRequest);

            return signedUrl.PreauthenticatedRequest.FullPath + $"{productId}/master.m3u8";
        }

        public async Task PutFile(string idProduct, string objectName, MemoryStream memoryStream)
        {
            var putrequest = new PutObjectRequest
            {
                NamespaceName = Environment.GetEnvironmentVariable("NAMESPACENAME_ORACLE"),
                BucketName = Environment.GetEnvironmentVariable("BUCKET_PRODUCTION_POS"),
                ObjectName = $"{idProduct}/{objectName}",
                PutObjectBody = memoryStream
            };

            await _oracleProvider.ObjectStoreProvider().PutObject(putrequest);
        }
    }
}
