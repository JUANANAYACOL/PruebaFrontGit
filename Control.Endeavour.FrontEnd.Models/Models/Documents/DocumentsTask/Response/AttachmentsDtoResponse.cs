using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response
{
    public class AttachmentsDtoResponse
    {
        public int AttachmentId { get; set; }
        public int? TaskId { get; set; }
        public string AttCode { get; set; }
        public int FileId { get; set; }
        public string ArchiveName { get; set; }
        public string ArchiveExt { get; set; }
        public string ExhibitCode { get; set; }
        public string ExibitCodeName { get; set; }
        public decimal AttNumber { get; set; }
        public string AttDescription { get; set; }
        public string CreateUser { get; set; }
        public string CreateDate { get; set; }
        public bool Active { get; set; }
    }
}
