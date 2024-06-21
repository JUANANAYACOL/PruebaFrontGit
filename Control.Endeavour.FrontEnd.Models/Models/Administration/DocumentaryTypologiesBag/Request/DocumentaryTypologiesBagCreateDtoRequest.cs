namespace Control.Endeavour.FrontEnd.Models.Models.Administration.DocumentaryTypologiesBag.Request
{
    public class DocumentaryTypologiesBagCreateDtoRequest
    {
        public string TypologyName { get; set; } = null!;
        public string TypologyDescription { get; set; } = null;
        public bool ActiveState { get; set; }
        public string User { get; set; }
    }
}