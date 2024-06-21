namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request
{
    public class DocumentRelationDtoRequest
    {
        public string FilingCode1 { get; set; } = null!;
        public string FilingCode2 { get; set; } = null!;
        public string RelationOrigin { get; set; } = null!;
        public string? CreateUser { get; set; }
    }
}
