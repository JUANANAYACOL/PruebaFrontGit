using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.User.Response
{
    public class BlockedUsersDtoResponse
    {
        public int UserNoveltyId { get; set; }

        public int UserId { get; set; }

        public string Fullname { get; set; }

        public DateTime? DateBlock { get; set; }

    }
}
