using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minio;

namespace Librarian.Common.Utils
{
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
}
