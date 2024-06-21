namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Request
{
    public class ReplacementCreateDtoRequest
    {
        public int UserId { get; set; }

        public int UserReplacementId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Reason { get; set; }
        public string? CreateUser { get; set; }
    }
}