using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Control.Endeavour.FrontEnd.Components.Modals.SystemConfiguration
{
    public partial class SystemFieldsModal
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

        private InputModalComponent inputCode = new();
        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Parameters
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }


        #endregion

        #region Models
        private SystemFieldsDtoRequest systemFieldsDtoRequest = new SystemFieldsDtoRequest();
        private SystemFieldsDtoRequest systemFieldsDtoRequestEdit = new SystemFieldsDtoRequest();
        
        #endregion

        #region Environments

        #region Environments(String)
        private string systemFieldId = string.Empty;
        private string systemParamId = string.Empty;
        #endregion

        #region Environments(Numeric)

        private int intSystemParamId;
        int characterCounterValue = 0;
        int characterCounterComment = 0;
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool IsEditForm = false;
        private bool modalStatus = false;
        private bool IsDisabledCode = false;
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
        private void HandleModalClosed(bool status)
        {

            modalStatus = status;
            systemFieldsDtoRequest = new SystemFieldsDtoRequest();
            enableButton = true;
            systemFieldId = string.Empty;
            systemParamId = string.Empty;
            IsDisabledCode = false;
            IsEditForm = false;
            ResetForm();
            StateHasChanged();
        }

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }


        }
        #region FormMethods
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
            try
            {
                if (!string.IsNullOrEmpty(systemFieldsDtoRequest.Value) && IsTextAreaValue(systemFieldsDtoRequest.Value) && IsTextAreaValue(systemFieldsDtoRequest.Value) && inputCode.IsInputValid)
                {
                    systemFieldsDtoRequest.SystemParamId = intSystemParamId;
                    var responseApi = await HttpClient.PostAsJsonAsync("params/SystemFields/CreateSystemFields", systemFieldsDtoRequest);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<AppKeysDtoResponse>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                        IsEditForm = false;
                        ResetForm();
                        IsDisabledCode = false;
                        await OnChangeData.InvokeAsync(true);
                    }
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                }

            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }
        private async Task HandleFormUpdate()
        {
            try
            {
                if (!string.IsNullOrEmpty(systemFieldsDtoRequest.Value) && IsTextAreaValue(systemFieldsDtoRequest.Value) && IsTextAreaValue(systemFieldsDtoRequest.Value))
                {
                 
                    SystemFieldUpdateDtoRequest updateDtoRequest = new();
                    updateDtoRequest.SystemFieldId = int.Parse(systemFieldId);
                    updateDtoRequest.SystemParamId = intSystemParamId;
                    updateDtoRequest.Code = systemFieldsDtoRequest.Code;
                    updateDtoRequest.Value = systemFieldsDtoRequest.Value;
                    updateDtoRequest.Coment = systemFieldsDtoRequest.Coment;
                    updateDtoRequest.ActiveState = systemFieldsDtoRequest.ActiveState;
                    var responseApi = await HttpClient.PostAsJsonAsync("params/SystemFields/UpdateSystemFields", updateDtoRequest);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<SystemFieldsDtoResponse>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                        IsEditForm = false;
                        ResetForm();
                        IsDisabledCode = false;
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
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }

        }
        private void ResetForm()
        {
            if (!IsEditForm)
            {
                systemFieldsDtoRequest = new SystemFieldsDtoRequest();
                characterCounterValue = 0;
                characterCounterComment = 0;
            }
            else
            {
                systemFieldsDtoRequest = new()
                {
                             SystemParamId = systemFieldsDtoRequestEdit.SystemParamId,
                             ParamCode = systemFieldsDtoRequestEdit.ParamCode,
                             Code = systemFieldsDtoRequestEdit.Code,
                             Value = systemFieldsDtoRequestEdit.Value,
                             Coment = systemFieldsDtoRequestEdit.Coment,
                             ActiveState = systemFieldsDtoRequestEdit.ActiveState
                };
                characterCounterComment = systemFieldsDtoRequest.Coment?.Length ?? 0;
                characterCounterValue = systemFieldsDtoRequest.Value?.Length ?? 0;

            }
        }
        #endregion

        #endregion

        #region OthersMethods

        #region ValidationMethods
        private void CountCharacters(ChangeEventArgs e, ref int charactersCounterVariable,string type)
        {
            
            String value = e.Value.ToString() ?? String.Empty;
            if (type.Equals("1"))
            {
                systemFieldsDtoRequest.Value = value;
            }
            else
            {
                systemFieldsDtoRequest.Coment = value;
            }


            if (!string.IsNullOrEmpty(value))
            {
                charactersCounterVariable = value.Length;

            }
            else
            {
                charactersCounterVariable = 0;
            }
            if (type.Equals("1"))
            {
                ValidateEnableButton();
            }
            
        }
        private void ValidateEnableButton()
        {
            
            if (!string.IsNullOrWhiteSpace(systemFieldsDtoRequest.Code) &&
                !string.IsNullOrWhiteSpace(systemFieldsDtoRequest.Value))
            {
                enableButton = false;
            }
            else
            {
                enableButton = true; 
            }
        }

        private bool IsTextAreaValue(string value)
        {
            return !string.IsNullOrEmpty(value);
        }
        #endregion

        #region ModalMethods
        public void UpdateParamId(int paramId)
        {
            intSystemParamId = paramId;
        }
        public void ReceiveRecord(SystemFieldsDtoResponse response)
        {
            IsEditForm = true;
            IsDisabledCode = true;
            systemFieldId = response.SystemFieldId.ToString();
            systemParamId = response.SystemParamId.ToString();
            systemFieldsDtoRequest.Value = response.Value;
            systemFieldsDtoRequest.Code = response.Code;
            systemFieldsDtoRequest.Coment = response.Coment;
            systemFieldsDtoRequest.ActiveState = response.ActiveState;
            characterCounterComment = systemFieldsDtoRequest.Coment?.Length ?? 0;
            characterCounterValue = systemFieldsDtoRequest.Value?.Length ?? 0;
            //systemFieldsDtoRequestEdit = systemFieldsDtoRequest;
            systemFieldsDtoRequestEdit = new()
            {
                SystemParamId = systemFieldsDtoRequest.SystemParamId,
                ParamCode = systemFieldsDtoRequest.ParamCode,
                Code = systemFieldsDtoRequest.Code,
                Value = systemFieldsDtoRequest.Value,
                Coment = systemFieldsDtoRequest.Coment,
                ActiveState = systemFieldsDtoRequest.ActiveState
            };
        }
        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        #endregion

        #endregion

        #endregion

    }
}
