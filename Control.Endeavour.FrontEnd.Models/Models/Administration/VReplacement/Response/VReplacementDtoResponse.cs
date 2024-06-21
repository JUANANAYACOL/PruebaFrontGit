namespace Control.Endeavour.FrontEnd.Models.Models.Administration.VReplacement.Response
{
    public class VReplacementDtoResponse
    {
        public int ReplacementId { get; set; }

        public int UserId { get; set; }

        public string? UserFirstName { get; set; }

        public string? UserMiddleName { get; set; }

        public string? UserLastName { get; set; }

        public string UserFullName { get; set; } = null!;

        public int UserReplacementId { get; set; }

        public string? ReplacementFirstName { get; set; }

        public string? ReplacementMiddleName { get; set; }

        public string? ReplacementLastName { get; set; }

        public string ReplacementFullName { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Reason { get; set; }

        public string? CreateUser { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int? CompanyId { get; set; }
    }
}