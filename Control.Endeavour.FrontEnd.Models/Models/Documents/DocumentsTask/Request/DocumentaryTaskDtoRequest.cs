using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request
{
    public class DocumentaryTaskDtoRequest
    {
#nullable enable
        public int? RecordId { get; set; } = 0;
        public int? DocumentRelationId { get; set; } = 0;
        public string ClassCode { get; set; } = null!;
        public int? DocumentaryTypologyBehaviorId { get; set; }
        public string? ShipingMethod { get; set; }
        public int TemplateId { get; set; }
        public string TaskDescription { get; set; } = null!;
        public string DocDescription { get; set; } = null!;
        public DateTime TaskDate { get; set; }
        public DateTime DueDate { get; set; }
        public int CreateUserId { get; set; }

        public TaskManagementDtoRequest TaskManagementRequest { get; set; } = new();
        public List<AttachmentsDtoRequest> Attachments { get; set; } = new();
        public List<DestinationsDtoRequest> Destinations { get; set; } = new();
        public List<SignatureDtoRequest> Signatures { get; set; } = new();
        public Dictionary<String, Object> Parameters { get; set; } = new Dictionary<String, Object>();
    }

    public class DestinationsDtoRequest
    {
        public int DestinyId { get; set; }
        public string DestinyType { get; set; } = null!;
        public bool ItsCopy { get; set; } = false;
    }
}
