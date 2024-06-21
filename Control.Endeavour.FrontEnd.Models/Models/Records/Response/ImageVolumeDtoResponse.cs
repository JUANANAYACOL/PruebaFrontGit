using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Records.Response
{
    public class ImageVolumeDtoResponse
    {
        public int VolumeId { get; set; }
        public int FileId { get; set; }
        public byte[]? DataFile { get; set; }
    }
}
