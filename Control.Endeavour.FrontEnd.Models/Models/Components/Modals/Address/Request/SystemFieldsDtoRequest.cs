

namespace Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request
{
    public class SystemFieldsDtoRequest
    {
        public int SystemParamId { get; set; }
        public string ParamCode { get; set; } = null!;
        public string? Code { get; set; }
        public string? Value { get; set; }
        public string? Coment { get; set; }
        public bool ActiveState { get; set; } = true;
    }
}
