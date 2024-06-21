namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Response
{
    public class ReplacementDtoResponse
    {
        public int? ReplacementId { get; set; }

        public int? UserId { get; set; }
        public int? UserReplacementId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Reason { get; set; } = null!;
    }
}