namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response
{
    public class DocRelationDtoResponse
    {
        public int DocumentId { get; set; }
        public int ControlId { get; set; }
        public int DocumentaryTypologyBehaviorId { get; set; }
        public int? RecordId { get; set; }
        public int? FileId { get; set; }
        public string FilingCode { get; set; } = null!;
        public string? DocDescription { get; set; }
        public DateTime? DocDate { get; set; }
        public long Pages { get; set; }
        public string? ReceptionCode { get; set; }
        public string ClassCode { get; set; } = null!;
        public DateTime? DueDate { get; set; }
        public string? PriorityCode { get; set; }

        public int? DocumentRelationId { get; set; }
        public int? DocumentId1 { get; set; }
        public int? DocumentId2 { get; set; }
        public string? RelationOrigin { get; set; }
    }
}
