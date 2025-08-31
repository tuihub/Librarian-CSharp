//using Grpc.Core;
//using Librarian.Common.Utils;
//using Microsoft.AspNetCore.Authorization;
//using TuiHub.Protos.Librarian.Sephirah.V1.Angela;
//using TuiHub.Protos.Librarian.V1;
//using Porter = TuiHub.Protos.Librarian.Porter.V1;

//namespace Librarian.Angela.Services
//{
//    public partial class AngelaService
//    {
//        [Authorize]
//        public override Task<ListPortersResponse> ListPorters(ListPortersRequest request, ServerCallContext context)
//        {
//            // Verify that the user is an administrator
//            UserUtil.VerifyUserAdminAndThrow(context, _dbContext);

//            var response = new ListPortersResponse();

//            // Mock getting a Porter list
//            // In a real environment, this should retrieve Porter information from a database or other storage
//            var porterList = new List<Porter>();

//            // If there is an actual Porter list data source, data should be retrieved from here
//            // This is just an example implementation

//            response.Paging = new PagingResponse { TotalSize = porterList.Count };
//            response.Porters.AddRange(porterList);

//            return Task.FromResult(response);
//        }
//    }
//}
