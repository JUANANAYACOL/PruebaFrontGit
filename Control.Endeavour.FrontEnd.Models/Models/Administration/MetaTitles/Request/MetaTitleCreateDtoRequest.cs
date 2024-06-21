namespace Control.Endeavour.FrontEnd.Models.Models.Administration.MetaTitles.Request
{
    public class MetaTitleCreateDtoRequest
    {
        public int MetaTitleId { get; set; }

        public int MetaFieldId { get; set; }

        public int? DocumentaryTypologyBagId { get; set; }

        public int? SubSeriesId { get; set; }
        public int? SeriesId { get; set; }

        public int OrderData { get; set; }
        public string NameMetaField { get; set; } = null!;

        public string FieldTypeValue { get; set; } = null!;
        public bool ActiveState { get; set; }
        public string? CreateUser { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}