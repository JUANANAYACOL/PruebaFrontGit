using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response
{
    public class DataCardDtoResponse
    {
        public int withoutProcessing { get; set; }
        public int inProgress { get; set; }
        public int SuccessfullManagement { get; set; }
        public int copies { get; set; }
        public string? WithoutProcessingWord { get; set; }
        public string? InProgressWord { get; set; }
        public string? SuccessfullManagementWord { get; set; }
        public string? copiesWord { get; set; }
    }
}
