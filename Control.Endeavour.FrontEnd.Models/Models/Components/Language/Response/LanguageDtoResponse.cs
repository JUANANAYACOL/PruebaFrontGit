using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Components.Language.Response
{
    public class LanguageDtoResponse
    {
        public int LanguageId { get; set; }
        public string CodeLanguage { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string NameTraslated { get; set; } = null!;
    }
}
