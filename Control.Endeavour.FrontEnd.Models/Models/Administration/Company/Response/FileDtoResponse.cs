using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response
{
    public class FileDtoResponse
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string FileExt { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public bool Active { get; set; }
        public byte[] DataFile { get; set; }
    }
}
