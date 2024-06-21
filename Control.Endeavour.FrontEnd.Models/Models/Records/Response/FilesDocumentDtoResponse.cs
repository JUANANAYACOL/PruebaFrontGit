using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Records.Response
{
    public class FilesDocumentDtoResponse
    {
        public int DocumentId { get; set; }
        public int ControlId { get; set; }
        public int CreateUserId { get; set; }
        public string? CreateUser { get; set; } = null;
        public string DocDescription { get; set; } = null!;
        public int FileId { get; set; }
        //public string FilingCode { get; set; } = null!;
        //public string Functionality { get; set; } = null!;
        public DateTime? CreateDate { get; set; }
        public List<AttachmentRecordsDtoResponse> Attachments { get; set; } = new ();
    }

}
