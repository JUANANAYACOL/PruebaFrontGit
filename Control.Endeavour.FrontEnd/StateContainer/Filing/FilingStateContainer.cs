using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response;

namespace Control.Endeavour.FrontEnd.StateContainer.Filing
{
    public class FilingStateContainer
    {
        #region Properties
        public bool? ActiveFiling { get; set; }
        public string? FilingNumber { get; set; }
        public string? DocumentId { get; set; }
        public string? Recipients { get; set; }
        public string? Folios { get; set; }
        public string? Annexes { get; set; }
        public bool? Verify { get; set; }

        public List<int> listId { get; set; }
        public List<OverrideTrayRequestDtoResponse> RequestList { get; set; }
        #endregion

        #region Attributes
        public event Action? ComponentChange;
        #endregion

        #region Methods
        public void Parametros(bool activeFiling)
        {
            ActiveFiling = activeFiling;
            ExecuteAction();
        }

        private void ExecuteAction() => ComponentChange?.Invoke();
        #endregion
    }
}
