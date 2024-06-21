namespace Control.Endeavour.FrontEnd.Models.Models.Administration.MetaValues.Request
{
    public class MetaValueFilterDtoRequest
    {
        public int MetaFieldId { get; set; }

        public int? ValueOrder { get; set; }

        public string? ValueText { get; set; }
    }
}