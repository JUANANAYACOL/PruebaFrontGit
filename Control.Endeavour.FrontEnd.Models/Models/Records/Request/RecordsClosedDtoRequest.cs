namespace Control.Endeavour.FrontEnd.Models.Models.Records.Request
{
    public class RecordsClosedDtoRequest
    {
        public int? RecordId { get; set; }
        public string? ClosedType { get; set; }
        public string? Justification { get; set; }
    }
}