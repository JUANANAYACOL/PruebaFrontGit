using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request
{
    public class DocumentaryTypologiesBehaviorsAllUpdateDtoRequest
    {
        public int DocumentaryTypologyBehaviorId { get; set; }
        public int? DocumentaryTypologyId { get; set; }
        public string ClassCode { get; set; } = null!;
        public string CorrespondenceType { get; set; } = null!;
        public List<TypologyManagersDtoRequestAll>? TypologyManagers { get; set; } = new();
        public List<BehaviorTypologiesDtoRequestAll>? BehaviorTypologies { get; set; } = new();
        public List<UserCopyDtoRequestAll>? UserCopies { get; set; } = new();
    }

}
