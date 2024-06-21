using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{

    public class TypologyManagersDtoRequestAll
    {
        public int TypologyManagerId { get; set; }
        public int ManagerId { get; set; }
        public string? InstructionCode { get; set; } = null;
        public bool Leader { get; set; }
    }
}
