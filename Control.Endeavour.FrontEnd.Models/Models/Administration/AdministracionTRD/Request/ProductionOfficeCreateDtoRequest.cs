namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class ProductionOfficeCreateDtoRequest
    {
        public int AdministrativeUnitId { get; set; }
        public int? BossId { get; set; }
        public string Code { get; set; } = null!;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool ActiveState { get; set; }
    }
}