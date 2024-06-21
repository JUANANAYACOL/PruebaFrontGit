using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Audit.Request
{
    public class LogByFilterDtoRequest : PaginationRequest
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int ClientId { get; set; }
        public int UserId { get; set; }
        public string? Module { get; set; }
        public string? Micro { get; set; }
        public string? Class { get; set; }
        public string? Method { get; set; }
        public string? TableName { get; set; }
        public string? LogType { get; set; }
        public string? Detail { get; set; }
    }
}