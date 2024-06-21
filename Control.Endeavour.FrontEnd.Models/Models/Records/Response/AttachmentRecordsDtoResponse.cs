using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Records.Response
{
    public class AttachmentRecordsDtoResponse
    {
        public int AttachmentId { get; set; }
        public int FileId { get; set; }
        public string FileExt { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string ExhibitCode { get; set; } = null!;
        public string AttCode { get; set; } = null!;
        public string AttDescription { get; set; } = null!;
        public decimal? AttNumber { get; set; }
        public string CreateUser { get; set; } = null!;
        public DateTime CreateDate { get; set; }
    }
}
