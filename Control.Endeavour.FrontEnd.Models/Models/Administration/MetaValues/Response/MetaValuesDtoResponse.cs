namespace Control.Endeavour.FrontEnd.Models.Models.Administration.MetaValues.Response
{
    public class MetaValuesDtoResponse
    {
        public int MetaValueId { get; set; }

        public int MetaFieldId { get; set; }

        public int? ValueOrder { get; set; }

        public string? ValueText { get; set; }

        public bool ActiveState { get; set; }
    }
}