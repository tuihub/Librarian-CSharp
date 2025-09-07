using Minio;

namespace Librarian.Common.Utils;

public static class MinioClientUtil
{
    public static IMinioClient GetMinioClient()
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