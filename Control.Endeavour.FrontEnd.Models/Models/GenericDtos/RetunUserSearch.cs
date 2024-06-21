using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.GenericDtos
{
    public class RetunUserSearch
    {
        public string TypeOfUser { get; set; }
        public int UserId { get; set; }

        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPosition { get; set; }

        public string? UserAdministrativeUnitName {  get; set; }
        public string? UserProductionOfficeName { get; set; }

        public string? UserIdentificationNumber {  get; set; }   


        public bool IsSelected { get; set; }
        public bool IsCopy { get; set; }

        public bool? IsSender { get; set; }
        public bool? IsRecipient { get; set; }
    }
}
