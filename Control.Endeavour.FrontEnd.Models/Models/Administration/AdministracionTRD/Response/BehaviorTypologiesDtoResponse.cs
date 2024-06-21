namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response
{
    public class BehaviorTypologiesDtoResponse
    {
        public int BehaviorTypologyId { get; set; }

        public int BehaviorBagId { get; set; }

        public int DocumentaryTypologyBehaviorId { get; set; }

        public string BehaviorValue { get; set; }
        public string CreateUser { get; set; } = null!;
        public string UpdateUser { get; set; } = null!;
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}