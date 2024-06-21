using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Control.Endeavour.FrontEnd.Components.Modals.SystemConfiguration
{
    public partial class AppKeysModal : ComponentBase
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

        #endregion

        #region Components

        private InputModalComponent inputKeyName = new();
        private InputModalComponent inputValue1 = new();
        private InputModalComponent inputValue2 = new();
        private InputModalComponent inputValue3 = new();
        private InputModalComponent inputValue4 = new();

        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Parameters
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }

        #endregion

        #region Models
        private AppKeysDtoRequest appKeysDtoRequest { get; set; } = new();
        private AppKeysDtoRequest appKeysDtoRequestEdit { get; set; } = new();

        #endregion

        #region Environments

        #region Environments(String)

        #endregion

        #region Environments(Numeric)
        private int appFunctionId { get; set; }
        private int appKeyId { get; set; }
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)
        private bool IsEditForm { get; set; }
        private bool modalStatus = false;
        private bool enableButton = true;
        #endregion

        #region Environments(List & Dictionary)

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
			//EventAggregator.LanguageChangedEvent += HandleLanguageChanged;

		}


        #endregion

        #region Methods

        #region HandleMethods
        #region Form Methods
        private async Task HandleValidSubmit()
        {
            // Lógica de envío del formulario
            if (IsEditForm)
            {
                await HandleFormUpdate();
            }
            else
            {
                await HandleFormCreate();
            }

            StateHasChanged();

        }

        private async Task HandleFormCreate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                appKeysDtoRequest.CompanyId = 17;
                if (!string.IsNullOrEmpty(appKeysDtoRequest.KeyName) && !string.IsNullOrEmpty(appKeysDtoRequest.Value1))
                {
                    if (appKeysDtoRequest.Value1.Length < 3)
                    {
                        notificationModal.UpdateModal(ModalType.Warning, Translation["Value1ValidateMessage"], true, Translation["Accept"], buttonTextCancel: "");
                    }

                    appKeysDtoRequest.AppFunctionId = appFunctionId;
                    var responseApi = await HttpClient.PostAsJsonAsync("params/AppKeys/CreateAppKeys", appKeysDtoRequest);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<AppKeysDtoResponse>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                        await OnChangeData.InvokeAsync(true);
                        IsEditForm = false;
                        ResetForm();
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                    }

                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                }

            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message,  true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task HandleFormUpdate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (!string.IsNullOrEmpty(appKeysDtoRequest.KeyName) && !string.IsNullOrEmpty(appKeysDtoRequest.Value1))
                {
                    Dictionary<string, dynamic> headers = new() { { "AppFunctionId", appFunctionId }, { "AppKeyId", appKeyId } };
                    AppKeysUpdateDtoRequest appKeysUpdate = new();
                    appKeysUpdate.AppFunctionId = appFunctionId;
                    appKeysUpdate.AppKeyId = appKeyId;
                    appKeysUpdate.KeyName = appKeysDtoRequest.KeyName;
                    if (appKeysDtoRequest.Value1.Length < 3)
                    {
                        notificationModal.UpdateModal(ModalType.Warning, Translation["Value1ValidateMessage"], true, Translation["Accept"], buttonTextCancel: "");
                    }
                    appKeysUpdate.Value1 = appKeysDtoRequest.Value1;
                    appKeysUpdate.Value2 = appKeysDtoRequest.Value2;
                    appKeysUpdate.Value3 = appKeysDtoRequest.Value3;
                    appKeysUpdate.Value4 = appKeysDtoRequest.Value4;
                    appKeysUpdate.CompanyId = 17;
                    appKeysUpdate.ActiveState = appKeysDtoRequest.ActiveState;
                     //Cambiar por varibale de usuario

                    var responseApi = await HttpClient.PostAsJsonAsync("params/AppKeys/UpdateAppKeys", appKeysUpdate);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<AppKeysDtoResponse>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                        IsEditForm = false;
                        ResetForm();


                        await OnChangeData.InvokeAsync(true);
                    }




                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
                }

            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message,  true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        private void ResetForm()
        {
            if (!IsEditForm)
            {
                appKeysDtoRequest = new AppKeysDtoRequest();
            }
            else
            {
                appKeysDtoRequest = new()
                {
                    KeyName = appKeysDtoRequestEdit.KeyName,
                    Value1 = appKeysDtoRequestEdit.Value1,
                    Value2 = appKeysDtoRequestEdit.Value2,
                    Value3 = appKeysDtoRequestEdit.Value3,
                    Value4 = appKeysDtoRequestEdit.Value4,
                    CompanyId = appKeysDtoRequestEdit.CompanyId,
                    ActiveState = appKeysDtoRequestEdit.ActiveState
                };
                
            }
        }
        #endregion
        private async Task HandleModalClosedAsync(bool status)
        {

            modalStatus = status;
            StateHasChanged();
            IsEditForm = false;
            ResetForm();
            enableButton = true;




        }
        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatusAsync(args.ModalStatus);
            }


        }


        #endregion

        #region OthersMethods

        #region ModalMethods
        public void ReceiveRecord(AppKeysDtoResponse response)
        {
            appFunctionId = response.AppFunctionId;
            appKeyId = response.AppKeyId;
            appKeysDtoRequest.KeyName = response.KeyName;
            appKeysDtoRequest.Value1 = response.Value1;
            appKeysDtoRequest.Value2 = response.Value2;
            appKeysDtoRequest.Value3 = response.Value3;
            appKeysDtoRequest.Value4 = response.Value4;
            appKeysDtoRequest.CompanyId = 17;
            appKeysDtoRequest.ActiveState = response.ActiveState;
            appKeysDtoRequestEdit = new()
            {
                KeyName = response.KeyName,
                Value1 = response.Value1,
                Value2 = response.Value2,
                Value3 = response.Value3,
                Value4 = response.Value4,
                CompanyId = 17,
                ActiveState = response.ActiveState
            };
            IsEditForm = true;
        }
        public async Task UpdateModalStatusAsync(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
            
        }
        public void AppFunctionId(int appFunctionIdRecord)
        {
            appFunctionId = appFunctionIdRecord;
        }

        #endregion

        #region ValidationMethods
        private bool IsTextAreaValue(string value)
        {
            return !string.IsNullOrEmpty(value);
        }
        public void ValidateEnableButton()
        {
            enableButton = string.IsNullOrWhiteSpace(appKeysDtoRequest.KeyName) ||
                           string.IsNullOrWhiteSpace(appKeysDtoRequest.Value1);
        }
        #endregion

        #endregion

        #endregion

    }
}
