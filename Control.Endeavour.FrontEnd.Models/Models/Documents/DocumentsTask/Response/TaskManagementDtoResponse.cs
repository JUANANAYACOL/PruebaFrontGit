using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response
{
    public class TaskManagementDtoResponse
    {
        public int TaskManagementId { get; set; }
        public int DocumentaryTaskId { get; set; }
        public int ManagementOrder { get; set; }
        public string InstructionCode { get; set; }
        public string ProcessCode { get; set; }
        public long Pages { get; set; }
    }
}
