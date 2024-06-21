namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response
{
    public class DocumentaryTypologiesBehaviorsAllDtoResponse
    {
        public int DocumentaryTypologyBehaviorId { get; set; }
        public int? DocumentaryTypologyId { get; set; }
        public string DocumentaryTypologyName { get; set; }
        public string ClassCode { get; set; }
        public string ClassCodeName { get; set; }
        public string CorrespondenceType { get; set; }
        public string CorrespondenceTypeName { get; set; }
        public List<TypologyManagersDtoResponse> TypologyManagers { get; set; }
        public List<BehaviorTypologiesDtoResponse> BehaviorTypologies { get; set; }
        public string CreateUser { get; set; } = null!;
        public string UpdateUser { get; set; } = null!;
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}