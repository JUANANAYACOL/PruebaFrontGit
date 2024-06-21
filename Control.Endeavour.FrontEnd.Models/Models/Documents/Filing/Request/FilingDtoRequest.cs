using Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Request
{
    public class FilingDtoRequest
    {
        public int DocumentaryTypologyBehaviorId { get; set; }
        public int RecordId { get; set; }
        public int FileId { get; set; }
        public string DocDescription { get; set; }
        public int Pages { get; set; }
        public string ReceptionCode { get; set; }
        public string PriorityCode { get; set; }
        public DateTime DocDate { get; set; }
        public Dictionary<String, Object> Parameters { get; set; }
        public List<DocumentSignatory> DocumentSignatory { get; set; }
        public List<DocumentReceiver> DocumentReceiver { get; set; }
        public List<AttachmentsDtoRequest> Attachment { get; set; }
        public List<MetaData> metaData { get; set; }
        public string AutomaticShipping { get; set; }
        public string EndManagement { get; set; }
        public string NameSignatory { get; set; }
        public int DueDays { get; set; }
        public int DueHours { get; set; }
        public string InternalDocument { get; set; }
        public string GuideNumber { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public string User { get; set; }
        public int UserId { get; set; }
        public int UserAssingId { get; set; }
    }

    public class DocumentSignatory //Remitente - Firmante
    {
        public int SignatoryId { get; set; }
        public string TypeSignatory { get; set; }
    }

    public class DocumentReceiver // Destinatarios
    {
        public int ReceiverId { get; set; }
        public string TypeReceiver { get; set; }
        public bool ItsCopy { get; set; }
    }

    public class MetaData
    {
        public int MetaFieldId { get; set; }
        public string DataText { get; set; }
        public string ColorData { get; set; }
    }
}