using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.NoWorkDays;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Drawing;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;
using static DevExpress.Pdf.Native.PdfValidationLogger;

namespace Control.Endeavour.FrontEnd.Components.Modals.SystemConfiguration
{
    public partial class NoWorkDaysModals
    {

        #region Variables

        #region Inject 
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }
        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
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
        private NoWorkDaysDtoRequest NoWorkDaysDtoRequest = new();
        private NoWorkDaysDtoRequest NoWorkDaysUpdateDtoRequest = new();

        #endregion

        #region Environments

        #region Environments(String)
        private string ResponseRecordClosed = "No";
        private string QuestionRangeClass = "d-none";
        private string EndDateClass = "d-none";
        private string DateText = "";

        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)
        private DateTime Min = new DateTime(2020, 1, 1, 19, 30, 45);
        private DateTime Max = new DateTime(2080, 12, 31);

        #endregion

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool IsEditForm = false;

        private bool IsEnableToUpdate = true;
        private bool enableButton = true;

        #endregion

        #region Environments(List & Dictionary)

        private List<VSystemParamDtoResponse> ReasonNoWorkDays = new();
        private List<string> optionsRecordClosed = new List<string>() { "Sí", "No" };

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            ResponseRecordClosed = Translation["No"];
            optionsRecordClosed = new List<string>() { Translation["Yes"], Translation["No"] };
            DateText = Translation["DateTimePicker_Date"];
            await GetNoWorkDaysReasons();
        }


        #endregion

        #region Methods

        #region HandleMethods
        private void HandleModalClosed(bool status)
        {

            modalStatus = status;
            IsEditForm = false;
            enableButton = true;
            ResetFormAsync();
            StateHasChanged();
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
                if (ValidateNoWorkDaysFields())
                {
                    var responseApi = await HttpClient.PostAsJsonAsync("params/NoWorkDay/CreateNoWorkDay", NoWorkDaysDtoRequest);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        if (deserializeResponse.Data.Equals("1"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                        }
                        else
                        {
                            var message = string.Format(Translation["CreateSuccesfullMassiveRecord"], deserializeResponse.Data);
                            notificationModal.UpdateModal(ModalType.Success, message, true, Translation["Accept"]);
                            
                        }
                        
                        await OnChangeData.InvokeAsync(true);
                        IsEditForm = false;
                        ResetFormAsync();
                    }
                    else
                    {
                        if (deserializeResponse.Data.Equals("0"))
                        {
                            
                                notificationModal.UpdateModal(ModalType.Information, Translation["RepeatedRecordsMessage"], true, Translation["Accept"], buttonTextCancel: "");
                        }
                        else
                        {
                            notificationModal.UpdateModal(ModalType.Information, Translation["CreateErrorMessage"], true, Translation["Accept"], buttonTextCancel: "");
                        }
                        
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
                if (ValidateNoWorkDaysFields())
                {

                    var responseApi = await HttpClient.PostAsJsonAsync("params/NoWorkDay/UpdateNoWorkDay", NoWorkDaysDtoRequest);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<NoWorkDaysDtoRequest>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                        await OnChangeData.InvokeAsync(true);
                        IsEditForm = false;
                        ResetFormAsync();
                        UpdateModalStatus(false);
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Information, Translation["CannotUpdate"], true, Translation["Accept"], buttonTextCancel: "");
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
                NoWorkDaysDtoRequest = new();
                NoWorkDaysDtoRequest.NoWorkDay1 = null;
                NoWorkDaysDtoRequest.EndDate = null;
                IsEditForm = false;
                IsEnableToUpdate = true;
            }
            else
            {
                NoWorkDaysDtoRequest = new()
                {
                    NoWorkDayId = NoWorkDaysUpdateDtoRequest.NoWorkDayId,
                    NoWorkDay1 = NoWorkDaysUpdateDtoRequest.NoWorkDay1,
                    Reason = NoWorkDaysUpdateDtoRequest.Reason,
                    ActiveState = NoWorkDaysUpdateDtoRequest.ActiveState,
                };
                IsEditForm = true;
                if (NoWorkDaysUpdateDtoRequest.Reason.Equals("RNWD,SNDAY") || NoWorkDaysUpdateDtoRequest.Reason.Equals("RNWD,STDAY") || NoWorkDaysUpdateDtoRequest.Reason.Equals("RNWD,HLDAY"))
                {
                    IsEnableToUpdate = false;
                }
                else
                {
                    IsEnableToUpdate = true;
                }
            }
            
            
            QuestionRangeClass = "d-none";
            EndDateClass = "d-none";
            ResponseRecordClosed = Translation["No"];
        }
        #endregion

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            


        }
        #endregion

        #region OthersMethods

        #region ModalMethods
        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        public void ReceiveRecord(NoWorkDaysDtoResponse response)
        {
            IsEditForm = true;
            enableButton = false;
            NoWorkDaysDtoRequest.NoWorkDayId = response.NoWorkDayId;
            NoWorkDaysDtoRequest.NoWorkDay1 = response.NoWorkDay1;
            NoWorkDaysDtoRequest.Reason = response.Reason;
            NoWorkDaysDtoRequest.ActiveState = response.ActiveState;
            NoWorkDaysUpdateDtoRequest = new() {
                NoWorkDayId = response.NoWorkDayId,
                NoWorkDay1 = response.NoWorkDay1,
                Reason = response.Reason,
                ActiveState = response.ActiveState,
            }
                ;
            if (NoWorkDaysUpdateDtoRequest.Reason.Equals("RNWD,SNDAY") || NoWorkDaysUpdateDtoRequest.Reason.Equals("RNWD,STDAY") || NoWorkDaysUpdateDtoRequest.Reason.Equals("RNWD,HLDAY")){                IsEnableToUpdate = false;
            }
            else
            {
                IsEnableToUpdate = true;
            }
        }
        #endregion

        #region GetDataMethods

        private async Task GetNoWorkDaysReasons()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "RNWD");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    ReasonNoWorkDays = deserializeResponse.Data;
                }
                else
                {
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    ReasonNoWorkDays = new();
                }

            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }

        }

        #endregion

        #region ActionMethods
        private void OnValueChangedRecordClosed(string newValue)
        {
            ResponseRecordClosed = newValue;
            if (newValue == Translation["Yes"])
            {
                NoWorkDaysDtoRequest.IsBulk = true;
                EndDateClass = string.Empty;
                DateText = Translation["StartDate"];
            }
            else if (newValue == Translation["No"])
            {
                NoWorkDaysDtoRequest.IsBulk = false;
                NoWorkDaysDtoRequest.EndDate = null;
                EndDateClass = "d-none";
                DateText = Translation["DateTimePicker_Date"];
            }
            ValidateEnableButton();
        }
        private void DatesFilterMethod(DateTime? value, string type)
        {
            
            switch (type)
            {
                case "StartDate":
                    NoWorkDaysDtoRequest.NoWorkDay1 = value.Value.Date;
                    break;
                case "EndDate":
                    NoWorkDaysDtoRequest.EndDate = value.Value.Date;
                    break;
            }
            ValidateEnableButton();
        }
        private void EnablePanels(string? reason)
        {
            
            NoWorkDaysDtoRequest.Reason = reason;
            if (reason.Equals("RNWD,SNDAY") || reason.Equals("RNWD,STDAY") && !IsEditForm)
            {
                QuestionRangeClass = string.Empty;
            }
            else
            {
                QuestionRangeClass = "d-none";
                EndDateClass = "d-none";
            }
            ValidateEnableButton();
        }
        #endregion

        #region ValidateMethods
        public void ValidateEnableButton()
        {
            bool isEndDateRequired = NoWorkDaysDtoRequest.IsBulk;
            enableButton = string.IsNullOrWhiteSpace(NoWorkDaysDtoRequest.Reason) ||
                           !NoWorkDaysDtoRequest.NoWorkDay1.HasValue;
            if (isEndDateRequired)
            {
                enableButton = enableButton || !NoWorkDaysDtoRequest.EndDate.HasValue;
            }
        }



        private bool ValidateNoWorkDaysFields()
        {
            var errors = new List<string>();

            if (!NoWorkDaysDtoRequest.NoWorkDay1.HasValue)
            {
                errors.Add(string.Format(Translation["CharacterSelectionValidation"], Translation["DateTimePicker_Date"]));
            }

            if (string.IsNullOrEmpty(NoWorkDaysDtoRequest.Reason))
            {
                errors.Add(string.Format(Translation["CharacterSelectionValidation"], Translation["Motive"]));
            }

            if (NoWorkDaysDtoRequest.IsBulk)
            {
                if (!NoWorkDaysDtoRequest.EndDate.HasValue)
                {
                    errors.Add(string.Format(Translation["CharacterSelectionValidation"], Translation["EndDate"]));
                }
                else if (NoWorkDaysDtoRequest.EndDate.Value < NoWorkDaysDtoRequest.NoWorkDay1.Value)
                {
                    errors.Add(Translation["InvalidDateRange"]); 
                }
            }

            if (errors.Count > 0)
            {
                var message = string.Join(" ", errors);
                notificationModal.UpdateModal(ModalType.Error, Translation["TheFollowingFieldsHaveErrors"] + "\n" + message, true);
                return false;
            }

            return true;
        }


        #endregion

        #endregion

        #endregion

    }
}
