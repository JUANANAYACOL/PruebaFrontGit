using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request
{
    public class FileCompanyDtoRequest
    {
        public int? CompanyId { get; set; }
        public string FileExt { get; set; } = null!;
        public string? FileName { get; set; }
        public byte[] DataFile { get; set; } = null!;
    }
}
