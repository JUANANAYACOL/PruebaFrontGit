using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class DocumentaryTypologiesBehaviorsAllDtoRequest
    {
        public int? DocumentaryTypologyId { get; set; }
        public string ClassCode { get; set; } = null!;
        public string CorrespondenceType { get; set; } = null!;
        public List<TypologyManagersDtoRequest> TypologyManagers { get; set; } = new();
        public List<BehaviorTypologiesDtoRequest> BehaviorTypologies { get; set; } = new();
        public List<UserCopyDtoRequest>? UserCopies { get; set; } = new();
    }
}