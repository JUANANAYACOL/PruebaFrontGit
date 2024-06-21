using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Authentication.PasswordRecovery.Request
{
    public class PasswordCodeRecoveryDtoRequest
    {
        public string? Email { get; set; }
        public string? UUID { get; set; }

    }
}
