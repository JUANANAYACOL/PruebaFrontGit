using Control.Endeavour.FrontEnd.Models.Enums.Documents;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;

namespace Control.Endeavour.FrontEnd.StateContainer.ManagementTray
{
    public class ManagementTrayStateContainer
    {
        #region Propiedades

        public DocumentStatusEnum Status { get; set; }
        
        public int UserId { get; set; }
        public bool AnotherUser {  get; set; }
        public string? UserFullName { get; set; }

        #endregion

        #region Atributos

        public event Action? ComponentChange;

        #endregion

        #region Metodos

        public void Parametros(DocumentStatusEnum status)
        {
            Status = status;
            ExecuteAction();
        }
        
        public void ManagementTrayParameters(int AssingUserId, bool IsAnotherUser, string UserName)
        {
            UserId = AssingUserId;
            AnotherUser = IsAnotherUser;
            UserFullName = UserName;
            ExecuteAction();
        }

        private void ExecuteAction() => ComponentChange?.Invoke();

        #endregion
    }
}
