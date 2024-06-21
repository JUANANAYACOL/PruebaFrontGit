

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response
{
    public class SystemParamsDtoResponse
    {
        public int SystemParamId { get; set; }
        public string? ParamCode { get; set; }

        public string? ParamName { get; set; }

        public string? Description { get; set; }
        public string? CreateUser { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
