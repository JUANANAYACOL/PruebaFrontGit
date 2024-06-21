using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response
{
    public class ProfilesDtoResponse
    {
        public int ProfileId { get; set; }
        public int CompanyId { get; set; }
        public string? ProfileCode { get; set; }
        public string Profile1 { get; set; } = null!;
        public string? Description { get; set; }
        public bool Active { get; set; }
        public bool ActiveState { get; set; }
        public DateTime CreateDate { get; set; }
        public int ProfileUserId { get; set; }



    }
}
