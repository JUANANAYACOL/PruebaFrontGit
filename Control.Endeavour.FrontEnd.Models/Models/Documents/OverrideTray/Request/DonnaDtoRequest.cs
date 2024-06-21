using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request
{
    public class DonnaDtoRequest
    {
        public string SegmentName { get; set; }
        public string Category { get; set; }
        public double SegmentValue { get; set; }
        public string color { get; set; }
        public bool ShouldShowInLegend { get; set; } = true;


    }
}
