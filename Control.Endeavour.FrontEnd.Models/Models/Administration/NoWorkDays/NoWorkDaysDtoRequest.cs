namespace Control.Endeavour.FrontEnd.Models.Models.Administration.NoWorkDays
{
    public class NoWorkDaysDtoRequest
    {
        public int NoWorkDayId { get; set; }
        public DateTime? NoWorkDay1 { get; set; }
        public string? Reason { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsBulk { get; set; } = false;
        public bool ActiveState { get; set; } = true;
    }
}
