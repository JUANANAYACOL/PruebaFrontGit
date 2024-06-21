using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.DocumentaryTypologiesBag.Request
{
    public class DocumentaryTypologiesBagUpdateDtoRequest
    {
        public int DocumentaryTypologyBagId { get; set; }
        public string TypologyName { get; set; } = null!;
        public string TypologyDescription { get; set; } = null;
        public bool ActiveState { get; set; }
    }
}