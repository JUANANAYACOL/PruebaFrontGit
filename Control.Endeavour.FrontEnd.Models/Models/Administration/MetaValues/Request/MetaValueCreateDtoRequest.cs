namespace Control.Endeavour.FrontEnd.Models.Models.Administration.MetaValues.Request
{
    public class MetaValueCreateDtoRequest
    {
        public int MetaValueId { get; set; }

        public int MetaFieldId { get; set; }

        public int? ValueOrder { get; set; }

        public string? ValueText { get; set; }

        public bool ActiveState { get; set; }

        public string? User { get; set; }
    }
}