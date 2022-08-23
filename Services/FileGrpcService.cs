using Grpc.Core;
using Librarian_CSharp;
using System.Diagnostics;

namespace Librarian_CSharp.Services
{
    public class FileGrpcService : FileGrpc.FileGrpcBase
    {
        private readonly ILogger<FileGrpcService> _logger;
        private string? _name;
        private long _size;
        //private MemoryStream? _ms;
        private string? _dirPath;
        private string? _fileName;

        public FileGrpcService(ILogger<FileGrpcService> logger)
        {
            _logger = logger;
        }

        async Task<long> ProcessUploadStream(IAsyncStreamReader<UploadRequest> reqStream, IServerStreamWriter<UploadResponse> respStream)
        {
            _name = reqStream.Current.MetaData.Name;
            _size = reqStream.Current.MetaData.Size;
            //_ms = new MemoryStream();

            _fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmssffffff")}_{_name}";
            string filePath = Path.Combine(_dirPath, _fileName);
            using BinaryWriter bw = new BinaryWriter(File.OpenWrite(filePath));
            int offset = 0;
            while (await reqStream.MoveNext())
            {
                //Task.Delay(200).Wait();
                var req = reqStream.Current;
                if (req.ContentCase != UploadRequest.ContentOneofCase.FileBytes)
                    break;
                int size = req.FileBytes.Content.ToByteArray().Length;
                //await _ms.WriteAsync(req.FileBytes.Content.ToByteArray(), 0, size);
                bw.Write(req.FileBytes.Content.ToByteArray());
                Debug.WriteLine($"offset = {offset}, size = {size}");
                offset += size;
                await respStream.WriteAsync(new UploadResponse
                {
                    Status = Status.InProgress,
                    ReceivedSize = offset,
                });
            }
            //var ret = _ms.Length;
            //_ms.Close();
            bw.Flush();
            bw.Close();
            var ret = new FileInfo(filePath).Length;
            return ret;
        }

        public override async Task Upload(IAsyncStreamReader<UploadRequest> reqStream, IServerStreamWriter<UploadResponse> respStream, ServerCallContext context)
        {
            var resp = new UploadResponse();
            _dirPath = @"C:\Users\gyx\Desktop\Temp\FileGrpcServiceUpload";
            //_dirPath = @"/var/www/html/ariang/dl/FileGrpcServiceUpload";
            try
            {
                await reqStream.MoveNext();
                var req = reqStream.Current;
                if (req == null)
                    throw new Exception("request is null");
                long actSize;
                switch (req.ContentCase)
                {
                    case UploadRequest.ContentOneofCase.None:
                        throw new Exception("content is none");
                    case UploadRequest.ContentOneofCase.FileBytes:
                        throw new Exception("no metadata");
                    case UploadRequest.ContentOneofCase.MetaData:
                        actSize = await ProcessUploadStream(reqStream, respStream);
                        resp.ReceivedSize = actSize;
                        Debug.WriteLine($"actSize = {actSize}");
                        Debug.WriteLine($"Name = {_name}, Size = {_size}");
                        break;
                    default:
                        throw new Exception("unknown content type");
                }
                resp.Name = _name;
                resp.Status = Status.Success;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                resp.Name = _name;
                resp.Status = Status.Failed;
                
                string filePath = Path.Combine(_dirPath, _fileName);
                File.Delete(filePath);
            }
            await respStream.WriteAsync(resp);
        }
    }
}