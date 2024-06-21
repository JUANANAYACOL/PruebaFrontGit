using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ImporterTrdTvd.Request
{
    public class ImporterErrorReportDtoRequest
    {
        public int DocumentalVersionId { get; set; }
        public byte[] DataFile { get; set; }
        public string Identifier { get; set; }
        public string LanguageCode { get; set; }
    }
}
