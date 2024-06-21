

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class SubSerieDtoRequest
    {
        public int SeriesId { get; set; }
        public string Code { get; set; } = null!;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool ActiveState { get; set; }
        public string? Class { get; set; }
        public string? SecurityLevel { get; set; }
        public RetentionSSDtoRequest Retention { get; set; } = new RetentionSSDtoRequest();
    }
}
