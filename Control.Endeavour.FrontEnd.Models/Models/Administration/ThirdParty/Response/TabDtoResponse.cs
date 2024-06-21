using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdParty.Response
{
    public class TabDtoResponse
    {
        public string Title { get; set; } = null!;
        public List<ThirdPartyDtoResponse> FilteredData { get; set; } = null!;
        public MetaModel Meta { get; set; } = new() { PageSize = 10 };
    }
}
