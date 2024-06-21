
namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Request
{
    public class AdministrativeUnitUpdateDtoRequest
    {
        public int AdministrativeUnitId { get; set; }
        public int DocumentalVersionId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool ActiveState { get; set; }
    }
}
