using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ImporterTrdTvd.Request
{
    public class ImporterHistoryDtoRequest : PaginationRequest
    {
        public int DocumentalVersionId { get; set; }
        public int FileId { get; set; }
    }
}
