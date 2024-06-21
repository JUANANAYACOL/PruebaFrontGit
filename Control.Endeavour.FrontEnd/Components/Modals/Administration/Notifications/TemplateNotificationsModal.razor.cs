using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Notifications.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Notifications.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.Notifications
{
    public partial class TemplateNotificationsModal
    {

        #region Variables

        #region Inject 
        

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

        private TempleteSendGridDtoRequest TemplateDtoRequest = new();

        #endregion

        #region Environments

        #region Environments(String)
        private string CreateTemplatesUrl = "https://mc.sendgrid.com/dynamic-templates/new";
        private string ResponseRecordClosed = "No";

        private string _numberOfTemplateFields = string.Empty;
        public string NumberOfTemplateFields
        {
            get => _numberOfTemplateFields;
            set
            {
                if (_numberOfTemplateFields != value)
                {
                    _numberOfTemplateFields = value;
                    GenerateTemplates(); 
                }
            }
        }

        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool IsEditForm = false;
        private bool IsEnabledBtn = true;

        #endregion

        #region Environments(List & Dictionary)
        private HashSet<int> selectedOption = new HashSet<int>();
        private List<TemplateTemp> templates = new List<TemplateTemp>();
        private List<string> optionsRecordClosed = new List<string>() { "Sí", "No" };


        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{

            optionsRecordClosed = new List<string>() { Translation["Yes"], Translation["No"] };
            ResponseRecordClosed = Translation["No"];
        }


        #endregion

        #region Methods

        #region HandleMethods
        #region FormMethods
        private async Task HandleValidSubmit()
        {
            
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
                TemplateDtoRequest.Fields = templates;
                var responseApi = await HttpClient.PostAsJsonAsync("notification/Templates/CreateTemplateSendGrid", TemplateDtoRequest);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<TemplateDtoResponse>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                        await OnChangeData.InvokeAsync(true);
                        IsEditForm = false;
                        ResetFormAsync();
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Information, Translation["CreateErrorMessage"], true, Translation["Accept"], buttonTextCancel: "");
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
                //if (ValidateFields())
                //{

                //    var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Actions/UpdateActions", actionsDtoResponse);

                //    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ActionsDtoResponse>>();

                //    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                //    {
                //        IsEditForm = false;
                //        ResetFormAsync();
                //        notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                //        await OnChangeData.InvokeAsync(true);

                //    }
                //    else
                //    {
                //        notificationModal.UpdateModal(ModalType.Information, Translation["CannotUpdate"], true, Translation["Accept"], buttonTextCancel: "");
                //    }
                //}

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
                TemplateDtoRequest = new();
                NumberOfTemplateFields = string.Empty;
                templates = new();

            }
            else
            {
                //actionsDtoResponse = actionsDtoResponseEdit;
            }
        }
        #endregion

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }


        }
        private void HandleModalClosed(bool status)
        {

            modalStatus = status;

            IsEditForm = false;
            StateHasChanged();
        }
        #endregion

        #region OthersMethods
        #region ModalMethods

        public void ReceiveRecord(ActionsDtoResponse response)
        {
            
        }
        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        private async Task GoCreateTemplateSendgridAsync()
        {
            await Js.InvokeVoidAsync("openUrlInNewTab", CreateTemplatesUrl);
        }
        private void UpdateSelectedOption(int index, int optionValue)
        {
            if(optionValue == 3)
            {
                optionValue = 1;
            }
            templates[index].KeyType = optionValue;
        }
        private void GenerateTemplates()
        {
            if (int.TryParse(NumberOfTemplateFields, out int count) && count > 0)
            {
                templates = new List<TemplateTemp>(count);
                for (int i = 0; i < count; i++)
                {
                    templates.Add(new TemplateTemp());
                }
            }
            else
            {
                templates.Clear();
            }
        }
        private void OnValueChangedRecordClosed(string newValue)
        {
            ResponseRecordClosed = newValue;
            IsEnabledBtn = newValue == Translation["Yes"] ? false : true;
        }

        #endregion

        #endregion

        #endregion

    }
}
