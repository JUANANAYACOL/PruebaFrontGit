using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Authentication.PasswordRecovery.Request
{
    public class ChangePasswordDtoRequest
    {
        public string? Email { get; set; }
        public string? Code { get; set; }
        public string? NewPassword { get; set; }
        public string? UUID { get; set; }
        public string? Ip { get; set; }
    }
}
