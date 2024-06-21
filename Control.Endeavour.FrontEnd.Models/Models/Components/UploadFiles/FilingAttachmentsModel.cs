using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles
{
    public class FilingAttachmentsModel
    {
        public string? Details { get; set; }
        public string? CertificateOfDeliveries { get; set; }
        public string? ReasonForReturn { get; set; }
        public string? Observations { get; set; }
        public string? GuideNumber { get; set; }
        public List<FileInfoData>? Files { get; set; }
    }
}
