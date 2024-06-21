

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class SubSeriesUpdateDtoRequest
    {
        public int SubSeriesId { get; set; }
        public int SeriesId { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public bool? ActiveState { get; set; }
        public string? Class { get; set; }
        public string? SecurityLevel { get; set; }
        public int? ProductionOfficeId { get; set; }
        public List<int>? UsersId { get; set; }
        public RetentionSSDtoRequest Retention { get; set; } = null;
    }
}
