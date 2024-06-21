namespace Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request
{
    public class AttachmentsDtoRequest
    {
        public byte[]? DataFile { get; set; }

        public string? ArchiveName { get; set; }

        public string? ArchiveExt { get; set; }

        public string? ExhibitCode { get; set; }

        public string? ExhibitCodeName { get; set; }

        public string? AttCode { get; set; }

        public string? AttDescription { get; set; }
        public string? IconPath { get; set; }
        public long? Size { get; set; }
    }
}
