using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Services.Interfaces.DriveService
{
    public interface IGraphService
    {
        Task<List<Value>> GetUserDocs(string objectId, string token);
        Task<List<Value>> GetFoldersByUser(GraphFolderDtoResponse folder, string objectId, string token);
        Task<List<Value>> getGroupDocuments(string groupId, string token);
        Task<List<Value>> getFolderInfo(GraphFolderDtoResponse folder, string groupId, string token);
        Task<List<Value>> getFolderData(GraphDocumentDtoResponse folder, string groupId, string token);
    }
}
