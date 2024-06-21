using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Documents;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Generic;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using Microsoft.JSInterop;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Resources;
using Microsoft.Extensions.Localization;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using System.Drawing;
using System;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.DocumentaryTask
{
    public partial class SendDocumentModal
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
        private GenericUserSearchModal genericUserSearch = new();

        #endregion

        #region Parameters

        [Parameter]
        public EventCallback<SendDocumentDtoResponse> senddocuments { get; set; }

        [Parameter] public EventCallback<MyEventArgs<SendDocumentDtoResponse>> OnStatusChanged { get; set; }

        #endregion

        #region Models

        private RetunUserSearch vUserSelected = new();
        private SendDocumentDtoResponse sendDocument = new();
        private MetaModel meta = new();
        TelerikRadioGroup<SystemFieldsDtoResponse, string> radioGroupRef { get; set; }

        #endregion

        #region Environments

        #region Environments(String)
        private string instructionCode { get; set; }
        public string descriptionInput { get; set; }

        string GetButtonClass(InstructionCodeEnum option) => selectedOption == option ? "selected" : "";

        private string instructionName = "";

        #endregion

        #region Environments(Numeric)

        private int contadorcarac = 0;
        private InstructionCodeEnum selectedOption = 0;

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool enableButton = true;
        private bool UserModalStatus = false;
        #endregion

        #region Environments(List & Dictionary)

        private List<VSystemParamDtoResponse> systemFieldsList = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetInstructionCode();
        }

        #endregion

        #region Methods

        #region HandleMethods

        private void HandleSelection(InstructionCodeEnum option)
        {
            selectedOption = option;
            GetInstruction(selectedOption);
        }

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
        }

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        private async Task OpenNewModalUser()
        {
            UserModalStatus = true;
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {

                var eventArgs = new MyEventArgs<SendDocumentDtoResponse>
                {
                    Data = sendDocument,
                    ModalStatus = false
                };

                await OnStatusChanged.InvokeAsync(eventArgs);

            }
            else
            {
                var eventArgs = new MyEventArgs<SendDocumentDtoResponse>();
                await OnStatusChanged.InvokeAsync(eventArgs);
            }

        }

        #endregion

        #region OthersMethods
        private void HandleSelectedUserData(List<RetunUserSearch> selectedUsers)
        {
            vUserSelected = selectedUsers[0];
            ValidateEnableButton();
        }
       

        #region GetInstructionCode
        public async Task GetInstructionCode()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");
                HttpClient?.DefaultRequestHeaders.Add("ParamCode", "TAINS");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");

                if (deserializeResponse.Data != null)
                {
                    systemFieldsList = deserializeResponse.Data.Where(x => !x.FieldCode.Equals("PR")).Select(x => x).ToList() ?? new();
                    meta = deserializeResponse.Meta;
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las acciones: {ex.Message}");
            }
        }

        private void GetInstruction(InstructionCodeEnum instruction)
        {
            if (instruction == InstructionCodeEnum.Review)
            {
                instructionCode = InstructionCodeEnum.Review.GetCoreValue();
                instructionName = "Revisar";
            }
            else if (instruction == InstructionCodeEnum.Approve)
            {
                instructionCode = InstructionCodeEnum.Approve.GetCoreValue();
                instructionName = "Aprobar";
            }
            else if (instruction == InstructionCodeEnum.Signature)
            {
                instructionCode = InstructionCodeEnum.Signature.GetCoreValue();
                instructionName = "Firmar";
            }
            ValidateEnableButton();
        }

        #endregion

        #region CountChar

        private void ContarCaracteres(ChangeEventArgs e)
        {
            String value = e.Value.ToString() ?? String.Empty;
            descriptionInput = value;

            if (!string.IsNullOrEmpty(e.Value.ToString()))
            {
                contadorcarac = e.Value.ToString().Length;
            }
            else
            {
                contadorcarac = 0;
            }

            ValidateEnableButton();
        }

        #endregion

        #region Data Modal



        #endregion

        #region SendDocument

        private async Task SelectSendDocument()
        {
            sendDocument.Description = descriptionInput;
            sendDocument.Instruction = instructionCode;
            sendDocument.InstructionName = instructionName;
            sendDocument.Recivers = vUserSelected;

            await senddocuments.InvokeAsync(sendDocument);

            if (sendDocument != null)
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["WishContinue"], true);
            }
        }

        #endregion

        #region ValidationMethods

        private void ValidateEnableButton()
        {
            enableButton = string.IsNullOrWhiteSpace(descriptionInput) ||
                   string.IsNullOrWhiteSpace(instructionCode) ||
                   string.IsNullOrWhiteSpace(instructionName) ||
                   vUserSelected == null;
        }

        #endregion


        #region CloseModalSearchUser
        private void HandleModalUserClosed(bool status)
        {

            UserModalStatus = status;

        }

        #endregion

        #endregion

        #endregion
    }
}
