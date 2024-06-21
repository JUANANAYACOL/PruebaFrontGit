namespace Control.Endeavour.FrontEnd.Models.Models.Records.Response
{
    public class VolumeDtoResponse
    {
        public int VolumeId { get; set; }
        public int RecordId { get; set; }
        public string? Code { get; set; }
        public string? NameTome { get; set; }
        public int? FileId { get; set; }
        public decimal? TotalFolios { get; set; }
        public decimal? UsedFolios { get; set; }
        public int? TomeNumber { get; set; }
        public string? StateVolume { get; set; }
        public bool Active { get; set; }
    }
}
