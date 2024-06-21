namespace Control.Endeavour.FrontEnd.Models.Models.Administration.DocumentaryTypologiesBag.Response
{
    public class DocumentaryTypologiesBagDtoResponse
    {
        public int DocumentaryTypologyBagId { get; set; }
        public string TypologyName { get; set; } = null!;
        public string TypologyDescription { get; set; } = null;
        public bool ActiveState { get; set; }
        public string? CreateUser { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}