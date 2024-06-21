namespace Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response
{
    public class OverrideTrayManagerDtoResponse
    {
        public int CancelationManagerId { get; set; }
        public int UserId { get; set; }
        public string TypeCode { get; set; }
        public string NameUser { get; set; }
        public bool ActiveState { get; set; }
        public string NameTypeCode { get; set; }
        public string FullName { get; set; }
        public string ChargeName { get; set; }
    }
}