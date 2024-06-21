using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request
{
    public class DocumentUserQueryFilter : PaginationRequest
    {
        public int UserId { get; set; }
        public bool ProcessDocument { get; set; }
    }
}
