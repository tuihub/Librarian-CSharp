using Minio;

namespace Librarian.Sephirah.Utils
{
    public static class MinioClientUtil
    {
        public static MinioClient GetMinioClient()
        {
            return new MinioClient()
                       .WithEndpoint(GlobalContext.SystemConfig.MinioEndpoint)
                       .WithCredentials(
                           GlobalContext.SystemConfig.MinioAccessKey,
                           GlobalContext.SystemConfig.MinioSecretKey)
                       .WithSSL(GlobalContext.SystemConfig.MinioWithSSL)
                       .Build();
        }
    }
}
