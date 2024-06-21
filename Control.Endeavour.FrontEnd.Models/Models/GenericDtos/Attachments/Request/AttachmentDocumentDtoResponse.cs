namespace Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request
{
    public class AttachmentDocumentDtoResponse
    {
        public int AttachmentId { get; set; }
        public int FileId { get; set; }
        public string? ArchiveName { get; set; }
        public string? ArchiveExt { get; set; }
        public string? ExhibitCode { get; set; }
        public string? ExhibitCodeName { get; set; }
        public string? AttCode { get; set; }
        public string? AttDescription { get; set; }
        public decimal AttNumber { get; set; }
        public bool AttGenerated { get; set; }
        public string? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
