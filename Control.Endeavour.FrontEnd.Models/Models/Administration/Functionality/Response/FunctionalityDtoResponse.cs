namespace Control.Security.Core.DTOs.Response
{
#nullable enable

    public class FunctionalityDtoResponse
    {
        public int FunctionalityId { get; set; }

        public string Functionality1 { get; set; } = null!;

        public string? Description { get; set; }

        public bool Active { get; set; }

        public string? CreateUser { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public bool? Create { get; set; }
        public bool? Update{ get; set; }
        public bool? Read { get; set; }
        public bool? Delete { get; set; }
        public bool? Print { get; set; }
        public bool? Append{ get; set; }
    }
}