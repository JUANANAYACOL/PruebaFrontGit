using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request
{
    public class OverrideTrayRequestDtoRequest
    {
        public int ControlId { get; set; }
        public int CancelationReasonId { get; set; }
        public int UserId { get; set; } //Puede ser quien lo va a gestionar o al usuario que se va entregar
        public string RequestComment { get; set; }
        public string TypeRequestCode { get; set; }
        public string TypeCode { get; set; }
        public ActionType ActionType { get; set; }
        public List<FileCompanyDtoRequest> FileDtoRequests { get; set; }
    }

    public enum ActionType
    {
        GESTOR = 0,
        USUARIO = 1,
    }
}