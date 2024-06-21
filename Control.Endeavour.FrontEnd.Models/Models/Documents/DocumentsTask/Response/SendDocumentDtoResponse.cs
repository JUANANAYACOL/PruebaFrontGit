using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response
{
    public class SendDocumentDtoResponse
    {
        public string Description { get; set; } = string.Empty;
        public string Instruction { get; set; } = string.Empty;
        public string InstructionName { get; set; } = string.Empty;
        public RetunUserSearch? Recivers { get; set; }
    }
}
