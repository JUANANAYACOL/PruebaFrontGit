using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Notifications.Request
{
    public class TempleteSendGridDtoRequest
    {
        public string SendGridTempId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int CompanyId { get; set; } = 17;
        public int TypeId { get; set; } = 9;
        public List<TemplateTemp> Fields { get; set; }

    }

    public class TemplateTemp
    {
        public int KeyType { get; set; }

        public string? KeyName { get; set; }

        public string? Value {  get; set; } = string.Empty;

        
    }
}
