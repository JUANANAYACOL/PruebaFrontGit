namespace Control.Endeavour.FrontEnd.Models.Models.Administration.NoWorkDays
{
    public class NoWorkDaysDtoResponse
    {
        public int NoWorkDayId { get; set; }
        public DateTime NoWorkDay1 { get; set; }
        public string? Reason { get; set; }
        public string? ReasonName { get; set; }
        public bool Active { get; set; }
        public bool ActiveState { get; set; }
        public string? CreateUser { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
