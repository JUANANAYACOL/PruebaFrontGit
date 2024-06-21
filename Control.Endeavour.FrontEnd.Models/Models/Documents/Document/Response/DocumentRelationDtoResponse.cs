namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response
{
    public class DocumentRelationDtoResponse
    {
        public int DocumentRelationId { get; set; }
        public int DocumentId1 { get; set; }
        public int DocumentId2 { get; set; }
        public string? RelationOrigin { get; set; }
    }
}
