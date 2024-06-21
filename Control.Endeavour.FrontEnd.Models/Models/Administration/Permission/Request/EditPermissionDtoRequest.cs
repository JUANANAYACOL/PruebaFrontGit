using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Request
{
    public class EditPermissionDtoRequest
    {
        public int PermissionId { get; set; }
        public bool AccessF { get; set; }
        public bool CreateF { get; set; }
        public bool ModifyF { get; set; }
        public bool ConsultF { get; set; }
        public bool DeleteF { get; set; } = false;
        public bool PrintF { get; set; }
        public bool ActiveState { get; set; }
        public string User { get; set; }
    }
}
