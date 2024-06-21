using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response
{
    public class CancelationRequestInformationDtoResponse
    {
        public string TypeRequestCode { get; set; }
        public string TypeCode { get; set; }
        public int? CancelationReasonId { get; set; }
        public string RequestComment { get; set; }
        public List<AttachmentInformation> Attachments { get; set; } = new List<AttachmentInformation>();

        public List<DocumentInformation> Documents { get; set; } = new List<DocumentInformation>();
    }

    public class DocumentInformation
    {
        public int ControlId { get; set; }

        public string FilingCode { get; set; }

        public DateTime CreateDate { get; set; }
    }

    public class AttachmentInformation
    {
        public int FileId { get; set; }

        public string ArchiveName { get; set; }

        public string ArchiveExt { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
