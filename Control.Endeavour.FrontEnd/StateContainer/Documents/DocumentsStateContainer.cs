using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using System;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.StateContainer.Documents
{
    public class DocumentsStateContainer
    {
        #region Propiedades

        public string? Code { get; set; }
        public string? UserType { get; set; }
        public List<string> Codes { get; set; } = new List<string>();
        public List<bool> Values { get; set; } = new List<bool>();

        public int Identification { get; set; }
        public int Value { get; set; }
        public bool PositionCard { get; set; }

        public DocumentResponseComentaryClosedDtoResponse documentResponseComentaryClosed { get; set; } = new();

        public bool? ActiveTask { get; set; }

        #endregion Propiedades

        #region Atributos

        public event Action? ComponentChange;

        #endregion Atributos

        #region Metodos

        public void Parametros(string code, string userType, List<string> codes, List<bool> values)
        {
            Code = code; UserType = userType; Codes = codes; Values = values;
            ExecuteAction();
        }

        public void ParametrosVisor(int identification, bool positionCard)
        {
            Identification = identification;
            PositionCard = positionCard;
            ExecuteAction();
        }

        public void ParametrosManagementTray(DocumentResponseComentaryClosedDtoResponse documentResponse, int ValueAction)
        {
            documentResponseComentaryClosed = documentResponse;
            documentResponseComentaryClosed.ActionId = ValueAction;
            ExecuteAction();
        }

        public void ValidNavigation(bool activeFiling)
        {
            ActiveTask = activeFiling;
            ExecuteAction();
        }

        private void ExecuteAction() => ComponentChange?.Invoke();

        #endregion Metodos
    }
}