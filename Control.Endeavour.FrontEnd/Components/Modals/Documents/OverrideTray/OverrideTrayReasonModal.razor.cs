using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.OverrideTray
{
    public partial class OverrideTrayReasonModal
    {
        #region Variables

        #region Inject

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Parameters

        [Parameter] public EventCallback<bool> OnChangeData { get; set; }

        #endregion Parameters

        #region Components

        #endregion Components

        #region Environments

        #region Environments(String)

        private string NameReason = "";
        private string ReasonCode = "";
        private string TypeCode = "";
        private string placeHolderDefault = "";
        private string DTReason = "";
        private string DTTypeCode = ""; 

        #endregion Environments(String)

        #region Environments(Bool)

        private bool Habilitar;
        private bool modalStatus;
        public bool Temp;
        private bool activeState = true;
        private bool enableButton = true;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<VSystemParamDtoResponse> TypeCodeList = new();
        private List<VSystemParamDtoResponse> ReasonCodeList = new();
        private NotificationsComponentModal notificationModal = new();
        private OverrideTrayReasonDtoResponse _selectedRecord = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            placeHolderDefault = Translation["EnterCancellationReason"];
            DTReason = Translation["SelectAnOption"];
            DTTypeCode = Translation["SelectAnOption"];
            await GetReasonCode();
            await GetTypeCode();
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region MyRegion

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion MyRegion

        #region GetTypeCode

        private async Task GetTypeCode()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "TCA");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse.Succeeded)
                {
                    Habilitar = true;

                    TypeCodeList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<VSystemParamDtoResponse>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las funcionalidades: {ex.Message}");
            }
        }

        #endregion GetTypeCode

        #region GetReasonCode

        private async Task GetReasonCode()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "RCA");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse.Succeeded)
                {
                    ReasonCodeList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<VSystemParamDtoResponse>();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las funcionalidades: {ex.Message}");
            }
        }

        #endregion GetReasonCode

        #region HandleModalClosed

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            enableButton = true;
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #region HandleModalNotiClose

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }
        }

        #endregion HandleModalNotiClose

        #region UpdateSelectedRecord

        public void UpdateSelectedRecord(OverrideTrayReasonDtoResponse? record)
        {
            _selectedRecord = record;
            NameReason = _selectedRecord.NameReason;
            ReasonCode = _selectedRecord.ReasonCode;
            TypeCode = _selectedRecord.TypeCode;
            activeState = _selectedRecord.ActiveState;
        }

        #endregion UpdateSelectedRecord

        #region Reset

        public void reset()
        {
            placeHolderDefault = Translation["EnterCancellationReason"];
            ReasonCode = "";
            TypeCode = "";
            NameReason = "";
            enableButton = true;
            activeState = true;
        }

        #endregion Reset

        #region PostReason

        private async Task PostReason()
        {
            OverrideTrayReasonDtoRequest Reason = new();
            Reason.NameReason = NameReason;
            Reason.ReasonCode = ReasonCode;
            Reason.TypeCode = TypeCode;
            Reason.ActiveState = activeState;

            var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationReason/CreateCancelationReason", Reason);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<OverrideTrayReasonDtoRequest>>();
            if (deserializeResponse.Succeeded)
            {
                //Logica Exitosa
                notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                await OnChangeData.InvokeAsync(true);
                reset();
            }
            else
            {
                //Logica no Exitosa
                notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
            }
        }

        #endregion PostReason

        #region PutReason

        private async Task PutReason()
        {
            OverrideTrayReasonEditDtoRequest ReasonEdit = new();
            ReasonEdit.CancelationReasonId = _selectedRecord.CancelationReasonId;
            Console.WriteLine(_selectedRecord.CancelationReasonId);
            ReasonEdit.NameReason = NameReason == null ? _selectedRecord.NameReason : NameReason;
            ReasonEdit.ReasonCode = ReasonCode == null ? _selectedRecord.ReasonCode : ReasonCode;
            ReasonEdit.TypeCode = TypeCode == null ? _selectedRecord.TypeCode : TypeCode;
            ReasonEdit.ActiveState = activeState;

            var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationReason/UpdateCancelationReason", ReasonEdit);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<OverrideTrayReasonEditDtoRequest>>();
            if (deserializeResponse.Succeeded)
            {
                //Logica Exitosa
                notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                await OnChangeData.InvokeAsync(true);
                reset();
            }
            else
            {
                //Logica no Exitosa
                notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
            }
        }

        #endregion PutReason

        #region Save

        private async Task Save()
        {
            try
            {
                if (Temp)
                {
                    await PostReason();
                }
                else
                {
                    await PutReason();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar un administrador {ex.Message}");
            }
        }

        #endregion Save

        #region BindReasonCode

        public void BindReasonCode(string value)
        {
            ReasonCode = value;
            ValidateEnableButton();
        }

        #endregion BindReasonCode

        #region BindTypeCode

        public void BindTypeCode(string value)
        {
            TypeCode = value;
            ValidateEnableButton();
        }

        #endregion BindTypeCode

        #region ValidationMethods

        private void ValidateEnableButton()
        {
            enableButton = string.IsNullOrWhiteSpace(ReasonCode) ||
                   string.IsNullOrWhiteSpace(TypeCode) ||
                   string.IsNullOrWhiteSpace(NameReason);
        }

        #endregion

        #endregion Methods
    }
}