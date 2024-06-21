using Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Request
{
    public class QuickFilingDtoRequest
    {
        public string? ClassCode { get; set; }
        public List<AttachmentsDtoRequest> Attachment { get; set; } = new();
        public string? PhysicalDescription { get; set; }
        public int NumberOfRecipients { get; set; } = 1;
    }
}
