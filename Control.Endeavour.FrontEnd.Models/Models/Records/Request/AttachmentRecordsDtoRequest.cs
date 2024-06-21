using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Records.Request
{
    public class AttachmentRecordsDtoRequest
    {
        public byte[]? DataFile { get; set; }
        public string? ArchiveName { get; set; }
        public string? ArchiveExt { get; set; }
        public string? ExhibitCode { get; set; }
        public string? AttCode { get; set; }
        public string? AttDescription { get; set; }
        public int? DocumentId { get; set; }
    }
}
