using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;


namespace Control.Endeavour.FrontEnd.Components.Modals.SystemConfiguration
{
    public partial class ActionsModal
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


        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Parameters
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }


        #endregion

        #region Models
        private ActionsDtoResponse actionsDtoResponse = new();
        private ActionsDtoResponse actionsDtoResponseEdit = new();

        #endregion

        #region Environments

        #region Environments(String)
        private string OriginText = "SelectAnOption";
        private string ClassNameText = "SelectAnOption";
        private string FlowStateText = "SelectAnOption";

        #endregion

        #region Environments(Numeric)


        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool IsEditForm = false;
        private bool modalStatus = false;
        private bool enableButton = true;

        #endregion

        #region Environments(List & Dictionary)

        private List<VSystemParamDtoResponse> classCodeList = new();
        private List<VSystemParamDtoResponse> flowStateList = new();
        private List<VSystemParamDtoResponse> originList = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            try
            {
                await GetClasses();
                await GetFlowState();
                await GetOrigin();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar la inicialización: {ex.Message}");
            }

        }


        #endregion

        #region Methods

        #region HandleMethods
        private void HandleModalClosed(bool status)
        {

            modalStatus = status;
            actionsDtoResponse = new ();
            enableButton = true;
            OriginText = "SelectAnOption";
            ClassNameText = "SelectAnOption";
            FlowStateText = "SelectAnOption";
            IsEditForm = false;
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
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (ValidateFields())
                {
                    var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Actions/CreateActions", actionsDtoResponse);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ActionsDtoResponse>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                        await OnChangeData.InvokeAsync(true);
                        IsEditForm = false;
                        ResetFormAsync();
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Information, Translation["CreateErrorMessage"], true, Translation["Accept"],buttonTextCancel:"");
                    }
                }

            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        private async Task HandleFormUpdate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (ValidateFields())
                {
                    
                    var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Actions/UpdateActions", actionsDtoResponse);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ActionsDtoResponse>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        IsEditForm = false;
                        ResetFormAsync();
                        notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                        await OnChangeData.InvokeAsync(true);
                        
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Information, Translation["CannotUpdate"], true, Translation["Accept"],buttonTextCancel:"");
                    }
                }

            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);

        }
        private void ResetFormAsync()
        {
            if (!IsEditForm)
            {
                actionsDtoResponse = new();
            }
            else
            {
                actionsDtoResponse = new()
                {
                    ActionId = actionsDtoResponseEdit.ActionId,
                    Name = actionsDtoResponseEdit.Name,
                    ClassCode = actionsDtoResponseEdit.ClassCode,
                    FlowStateCode = actionsDtoResponseEdit.FlowStateCode,
                    Origin = actionsDtoResponseEdit.Origin,
                    ActiveState = actionsDtoResponseEdit.ActiveState
                };
            }
        }
        #endregion

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

        #region OthersMethods

        #region ValidateFields
        private bool ValidateFields()
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(actionsDtoResponse.Name))
            {
                errors.Add(string.Format(Translation["CharacterEmptyValidation"], Translation["Name"]));
            }

            if (string.IsNullOrEmpty(actionsDtoResponse.ClassCode))
            {
                errors.Add(string.Format(Translation["CharacterEmptyValidation"], Translation["Class"]));
            }

            if (string.IsNullOrEmpty(actionsDtoResponse.FlowStateCode))
            {
                errors.Add(string.Format(Translation["CharacterEmptyValidation"], Translation["FlowState"]));
            }

            if (string.IsNullOrEmpty(actionsDtoResponse.Origin))
            {
                errors.Add(string.Format(Translation["CharacterEmptyValidation"], Translation["Origin"]));
            }

            if (errors.Count > 0)
            {
                var message = string.Join(" ", errors);
                notificationModal.UpdateModal(ModalType.Error, Translation["TheFollowingFieldsHaveErrors"] + "\n" + message, true);
                return false;
            }

            return true;
        }

        private void ValidateEnableButton()
        {
            enableButton = string.IsNullOrWhiteSpace(actionsDtoResponse.Name) ||
                   string.IsNullOrWhiteSpace(actionsDtoResponse.ClassCode) ||
                   string.IsNullOrWhiteSpace(actionsDtoResponse.FlowStateCode) ||
                   string.IsNullOrWhiteSpace(actionsDtoResponse.Origin);
        }

        #endregion

        #region GetData

        private async Task GetClasses()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "CL");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    classCodeList = deserializeResponse.Data;
                }
                else { classCodeList = new(); }

            }
            catch (Exception ex)
            {
               
                notificationModal.UpdateModal(ModalType.Error, $"Error al obtener los tipos de clase: {ex.Message}", true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        private async Task GetOrigin()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "ORI");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    originList = deserializeResponse.Data;
                }
                else { originList = new(); }

            }
            catch (Exception ex)
            {

                notificationModal.UpdateModal(ModalType.Error, $"Error al obtener los tipos de origen", true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task GetFlowState()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "ES");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    flowStateList = deserializeResponse.Data;
                }
                else { flowStateList = new(); }

            }
            catch (Exception ex)
            {

                notificationModal.UpdateModal(ModalType.Error, $"Error al obtener los tipos de estado de flujo", true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion



        #region ModalMethods

        public void ReceiveRecord(ActionsDtoResponse response)
        {
            IsEditForm = true;
            actionsDtoResponse.ActionId = response.ActionId;
            actionsDtoResponse.Name = response.Name;
            actionsDtoResponse.FlowStateCode = response.FlowStateCode;
            actionsDtoResponse.ClassCode = response.ClassCode;
            actionsDtoResponse.Origin = response.Origin;
            actionsDtoResponse.ActiveState = response.ActiveState;
            OriginText = "SelectAnOption";
            ClassNameText = "SelectAnOption";
            FlowStateText = "SelectAnOption";

            actionsDtoResponseEdit = new()
            {
                ActionId = response.ActionId,
                Name = response.Name,
                FlowStateCode = response.FlowStateCode,
                ClassCode = response.ClassCode,
                Origin = response.Origin,
                ActiveState = response.ActiveState
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
