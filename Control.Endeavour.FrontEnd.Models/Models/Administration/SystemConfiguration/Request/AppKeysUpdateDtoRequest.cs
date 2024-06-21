

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request
{
    public class AppKeysUpdateDtoRequest
    {
        public int AppKeyId { get; set; }
        public int AppFunctionId { get; set; }
        public string? KeyName { get; set; }
        public string? Value1 { get; set; }
        public string? Value2 { get; set; }
        public string? Value3 { get; set; }
        public string? Value4 { get; set; }
        public int CompanyId { get; set; }
        public string? UpdateUser { get; set; }
        public bool ActiveState { get; set; }
    }
}
