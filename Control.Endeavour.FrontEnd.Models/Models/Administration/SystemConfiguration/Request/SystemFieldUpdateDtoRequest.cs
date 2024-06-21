

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request
{
    public class SystemFieldUpdateDtoRequest
    {
        public int SystemFieldId { get; set; }
        public int SystemParamId { get; set; }
        public string? Code { get; set; }
        public string? Value { get; set; } 
        public string? Coment { get; set; }
        public string? UpdateUser { get; set; } 
        public bool ActiveState { get; set; }
    }
}
