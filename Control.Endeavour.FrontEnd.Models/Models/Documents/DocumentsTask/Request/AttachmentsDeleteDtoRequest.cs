using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request
{
    public class AttachmentsDeleteDtoRequest
    {
        public int AttchmentId { get; set; }
        public int TaskId { get; set; }
        public string? User { get; set; }
    }
}
