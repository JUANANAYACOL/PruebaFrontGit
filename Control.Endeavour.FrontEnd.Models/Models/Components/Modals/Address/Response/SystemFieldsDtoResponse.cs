using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response
{
    public class SystemFieldsDtoResponse
    {
        public int SystemFieldId { get; set; }
        public int SystemParamId { get; set; }
        public string Code { get; set; } = null!;
        public string Value { get; set; } = null!;
        public string? Coment { get; set; }
        public bool ActiveState { get; set; }
        public string? CreateUser { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
