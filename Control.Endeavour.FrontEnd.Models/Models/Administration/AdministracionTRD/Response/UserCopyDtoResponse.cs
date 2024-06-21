using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response
{

    public class UserCopyDtoResponse
    {
        public int UserCopiesId { get; set; }
        public int DocumentaryTypologyBehaviorId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string CreateUser { get; set; } = null!;
        public string UpdateUser { get; set; } = null!;
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
