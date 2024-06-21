
namespace Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response
{
    public class AppFunctionDtoResponse
    {
        public int AppFunctionId { get; set; }
        public string? FunctionName { get; set; }
        public string? Description { get; set; }
        public string? CreateUser { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
