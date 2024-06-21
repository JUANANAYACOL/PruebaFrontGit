using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;

namespace Control.Endeavour.FrontEnd.Models.Models.GenericDtos
{
    public class NotificationEvalutionDtoResponse
    {
        public bool Succeed { get; set; } = new();
        public ModalType ModalType { get; set; } = new();
        public string NotificationMessage { get; set; } = string.Empty;
    }
}