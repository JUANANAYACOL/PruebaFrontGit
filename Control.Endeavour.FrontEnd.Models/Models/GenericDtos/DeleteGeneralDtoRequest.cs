using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.GenericDtos
{
    public class DeleteGeneralDtoRequest
    {
        public int Id { get; set; }
        public string? User { get; set; }
    }
}
