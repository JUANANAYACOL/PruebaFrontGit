namespace Control.Endeavour.FrontEnd.Models.Models.Records.Request
{
    public class VolumeDtoRequest
    {
        public int? VolumeId { get; set; }
        public string Archive { get; set; } = null!;
        public string? User { get; set; }
        public int? TomeNumber { get; set; }
        public int? RecordId { get; set; }

        
    }
}
