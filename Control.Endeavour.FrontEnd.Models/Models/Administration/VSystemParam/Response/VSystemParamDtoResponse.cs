using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response
{
    public class VSystemParamDtoResponse
    {
        public string? Code { get; set; }
        public string? ParamCode { get; set; }
        public string? FieldCode { get; set; }
        public string? ParamName { get; set; }
        public string? Value { get; set; }
        public bool ActiveParam { get; set; }
        public bool ActiveField { get; set; }
    }
}
