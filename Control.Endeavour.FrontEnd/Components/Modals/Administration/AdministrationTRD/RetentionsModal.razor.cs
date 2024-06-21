using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD
{
    public partial class RetentionsModal
    {
        #region Variables

        #region Inject

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        [Parameter]
        public EventCallback<bool> OnStatusChanged { get; set; }

        [Parameter] public EventCallback<MyEventArgs<RetentionSSDtoRequest>> OnStatusChanged2 { get; set; }

        #endregion Inject

        #region Modals

        private NotificationsComponentModal notificationModal = new();

        #endregion Modals

        #region Parameters

        [Parameter] public EventCallback<RetentionSSDtoRequest> OnReturnRetencionObject { get; set; }

        #endregion Parameters

        #region Models

        private RetentionSSDtoRequest retentionSSDtoRequest = new();
        private RetentionSSDtoResponse _selectedRecord = new();
        private EditContext? editContext;

        #endregion Models

        #region Environments

        #region Environments(String)

        private string TimeFileCentral = string.Empty;
        private string TimeFileManagement = string.Empty;

        #endregion Environments(String)

        #region Environments(Numeric)

        private int characterCounterComment = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool BtnSubmitStatus = true;
        private bool TimeFileManagementStatus = false;
        private bool TimeFileCentralStatus = false;

        private bool totalConservationStatus { get; set; } = true;
        private bool eliminationStatus { get; set; } = true;
        private bool techEnvironmentStatus { get; set; } = true;
        private bool selectionStatus { get; set; } = true;

        #endregion Environments(Bool)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        #region ModalMethods

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }
        }

        public async Task UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            ResetFormAsync();
        }

        private async Task HandleModalClosed(bool status)
        {
            modalStatus = status;

            var eventArgs = new MyEventArgs<RetentionSSDtoRequest>
            {
                Data = null,
                ModalStatus = false
            };
            await OnStatusChanged2.InvokeAsync(eventArgs);
        }

        #endregion ModalMethods

        #region FormMethods

        private async Task HandleValidSubmit()
        {
            retentionSSDtoRequest.TimeFileManagement = string.IsNullOrEmpty(TimeFileManagement) ? 0 : (int.Parse(TimeFileManagement));
            retentionSSDtoRequest.TimeFileCentral = string.IsNullOrEmpty(TimeFileCentral) ? 0 : (int.Parse(TimeFileCentral));

            var eventArgs = new MyEventArgs<RetentionSSDtoRequest>
            {
                Data = retentionSSDtoRequest,
                ModalStatus = false
            };
            await OnStatusChanged2.InvokeAsync(eventArgs);
        }

        public void ResetFormAsync()
        {
            TimeFileManagement = "0";
            TimeFileCentral = "0";

            retentionSSDtoRequest.TotalConservation = false;
            retentionSSDtoRequest.Elimination = false;
            retentionSSDtoRequest.TechEnvironment = false;
            retentionSSDtoRequest.Selection = false;
            retentionSSDtoRequest.ProcedureRet = string.Empty;

            totalConservationStatus = true;
            eliminationStatus = true;
            selectionStatus = true;
            selectionStatus = true;
        }

        public void UpdateSelectedRecord(RetentionSSDtoResponse response)
        {
            _selectedRecord = response;

            TimeFileManagement = _selectedRecord.TimeFileManagement.ToString();
            TimeFileCentral = _selectedRecord.TimeFileCentral.ToString();

            retentionSSDtoRequest.TimeFileManagement = _selectedRecord.TimeFileManagement;
            retentionSSDtoRequest.TimeFileCentral = _selectedRecord.TimeFileCentral;
            retentionSSDtoRequest.TotalConservation = _selectedRecord.TotalConservation;
            retentionSSDtoRequest.Elimination = _selectedRecord.Elimination;
            retentionSSDtoRequest.TechEnvironment = _selectedRecord.TechEnvironment;
            retentionSSDtoRequest.Selection = _selectedRecord.Selection;
            retentionSSDtoRequest.ProcedureRet = _selectedRecord.ProcedureRet;
            characterCounterComment = string.IsNullOrEmpty(_selectedRecord.ProcedureRet) ? 0 : _selectedRecord.ProcedureRet.Length;

            EnableSaveButton();
        }

        #endregion FormMethods

        #endregion HandleMethods

        #region OthersMethods

        #region ValidationMethods

        private void CountCharacters(ChangeEventArgs e, ref int charactersCounterVariable)
        {
            String value = e.Value.ToString() ?? String.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                charactersCounterVariable = value.Length;
            }
            else
            {
                charactersCounterVariable = 0;
            }
        }

        public void CheckStates()
        {
            totalConservationStatus = !(retentionSSDtoRequest.Elimination || retentionSSDtoRequest.Selection);
            techEnvironmentStatus = !(retentionSSDtoRequest.Elimination || retentionSSDtoRequest.Selection);
            eliminationStatus = !(retentionSSDtoRequest.TotalConservation || retentionSSDtoRequest.TechEnvironment);
            selectionStatus = !(retentionSSDtoRequest.TotalConservation || retentionSSDtoRequest.TechEnvironment);
        }

        public void EnableSaveButton()
        {
            int x = 0;

            TimeFileManagement = (!string.IsNullOrEmpty(TimeFileManagement) && int.TryParse(TimeFileManagement, out x)) ? x.ToString() : "0";
            TimeFileCentral = (!string.IsNullOrEmpty(TimeFileCentral) && int.TryParse(TimeFileCentral, out x)) ? x.ToString() : "0";

            CheckStates();

            bool anyDispositionTrue = retentionSSDtoRequest.Elimination ||
                             retentionSSDtoRequest.Selection ||
                             retentionSSDtoRequest.TechEnvironment ||
                             retentionSSDtoRequest.TotalConservation;

            if (string.IsNullOrEmpty(TimeFileManagement) || string.IsNullOrEmpty(TimeFileCentral) || TimeFileManagement.Equals("0") || TimeFileCentral.Equals("0") || !anyDispositionTrue)
            {
                BtnSubmitStatus = true;
            }
            else
            {
                BtnSubmitStatus = false;
            }

            StateHasChanged();
        }

        #endregion ValidationMethods

        #endregion OthersMethods

        #endregion Methods
    }
}