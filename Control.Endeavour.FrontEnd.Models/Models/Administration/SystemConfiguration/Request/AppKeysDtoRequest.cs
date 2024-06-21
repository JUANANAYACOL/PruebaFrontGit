

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request
{
    public class AppKeysDtoRequest
    {
        public int AppFunctionId { get; set; }
        public string? KeyName { get; set; }
        public string? Value1 { get; set; }
        public string? Value2 { get; set; }
        public string? Value3 { get; set; }
        public string? Value4 { get; set; }
        public int CompanyId { get; set; }
        public string? CreateUser { get; set; }
        public bool ActiveState { get; set; } = true;
    }
}
