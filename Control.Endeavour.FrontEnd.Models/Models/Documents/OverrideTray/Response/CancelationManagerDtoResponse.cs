using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response
{
    public class CancelationManagerDtoResponse
    {
#nullable enable
        public string? TypeCode { get; set; }
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Charge { get; set; }
        public string? AdministrativeUnitName { get; set; }
        public string? ProductionOfficeName { get; set; }
    }
}