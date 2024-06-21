using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request
{
    public class AttachmentDocumentDtoRequest
    {
        public int CompanyId { get; set; }

        public byte[] DataFile { get; set; } = null!;

        public string ArchiveName { get; set; } = null!;

        public string ArchiveExt { get; set; } = null!;

        public int ControlId { get; set; }

        public string ExhibitCode { get; set; } = null!;

        public string AttDescription { get; set; } = null!;

        public decimal? AttNumber { get; set; }

        public string AttCode { get; set; } = null!;
        public string? CreateUser { get; set; }
    }
}
