using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Components.Modals
{
    public class ModalClosedEventArgs
    {
        public bool IsAccepted { get; set; }
        public bool IsCancelled { get; set; }

        public bool ModalStatus { get; set; }
        public string? ModalOrigin { get; set; }
    }
}
