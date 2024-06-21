using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Components.Modals
{
    public class MyEventArgs<T>
    {
        public T? Data { get; set; }
        public bool ModalStatus { get; set; }
    }
}
