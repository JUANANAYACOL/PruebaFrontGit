using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Authentication.CodeRecovery.Request
{
    public class SendVerificationCodeDtoRequest
    {

        public string? User { get; set; }
        public string? Uuid { get; set; }
    }
}
