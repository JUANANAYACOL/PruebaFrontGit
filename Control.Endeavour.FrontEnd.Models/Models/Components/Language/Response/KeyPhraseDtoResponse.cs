using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Components.Language.Response
{
    public class KeyPhraseDtoResponse
    {
        public int KeyPhraseId { get; set; }
        public string KeyName { get; set; } = null!;
        public bool Active { get; set; }
    }
    
}
