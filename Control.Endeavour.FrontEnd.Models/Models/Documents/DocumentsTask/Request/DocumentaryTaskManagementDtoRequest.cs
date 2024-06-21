using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request
{
    public class DocumentaryTaskManagementDtoRequest : TaskManagementDtoRequest
    {
        public int TaskId { get; set; }
        public string? ClassCode { get; set; }
        public string? ShipingMethod { get; set; }
        public int? DocumentaryTypologyBehaviorId { get; set; }
        public int? DocumentRelationId { get; set; }
        public int? RecordId { get; set; }
        public string? DocDescription { get; set; }
        public int CreateUserId { get; set; }
        public List<AttachmentsDtoRequest>? Attachments { get; set; }
        public List<DestinationsDtoRequest>? Destinations { get; set; }
        public List<SignatureDtoRequest>? Signatures { get; set; }
    }
}
