using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Attachments;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Generic.Attachments
{
    public partial class AttachmentsModal
    {
        #region Variables

        #region Inject

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Modals

        private NotificationsComponentModal notificationModal = new();

        #endregion Modals

        #region Parameters

        [Parameter]
        public EventCallback<List<AttachmentsDtoRequest>> OnReturnFilingAttachmentsModel { get; set; }

        [Parameter] public EventCallback<MyEventArgs<List<AttachmentsDtoRequest>>> OnStatusChanged { get; set; }

        [Parameter] public AttachmentsType Type { get; set; } = AttachmentsType.Generic;

        #endregion Parameters

        #region Models

        public AttachmentsType GenericType { get; set; } = AttachmentsType.Generic;

        #endregion Models

        #region Environments

        #region Environments(String)

        private string CodeInput { get; set; } = string.Empty;
        private string DescriptionInput { get; set; } = string.Empty;
        private string AttachmentType { get; set; } = string.Empty;

        #endregion Environments(String)

        #region Environments(Numeric)

        private Decimal contadorcarac = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool upFile = true;
        private bool isEnableActionButton = true;
        private bool isEnableTextArea = true;
        private bool modalStatus = false;
        private bool DisableBtnAdjuntar = true;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<FileInfoData> fileDataList = new List<FileInfoData>();
        private List<AttachmentsDtoRequest> filingAttachments = new List<AttachmentsDtoRequest>();
        private List<VSystemParamDtoResponse> systemFieldsList { get; set; }

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ////EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
                await GetAttachmentType();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar la initialización: {ex.Message}");
            }
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        #region HandleModal

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            StateHasChanged();
        }

        private async Task HandleAttachment()
        {
            foreach (var item in fileDataList)
            {
                AttachmentsDtoRequest attachment = new()
                {
                    DataFile = item.Base64Data,
                    ArchiveName = item.Name,
                    ArchiveExt = item.Extension.Replace(".", ""),
                    ExhibitCode = AttachmentType,
                    ExhibitCodeName = systemFieldsList.Where(x => x.Code == AttachmentType).Select(x => x.Value).FirstOrDefault(),
                    AttCode = CodeInput,
                    AttDescription = DescriptionInput,
                    Size = item.Size,
                    IconPath = item.IconPath
                };

                filingAttachments.Add(attachment);
            }
            if (Type == AttachmentsType.Filing)
            {
                await HandleSendAttachments();
            }
            else
            {
                await resetModal();
            }
        }

        private async Task HandleDeleteAttachment(AttachmentsDtoRequest attachment)
        {
            filingAttachments.Remove(attachment);
        }

        private async Task HandleSendAttachments()
        {
            if (filingAttachments.Count != 0)
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["AddAttachmentsQuestion"], true, Translation["Yes"], Translation["No"]);
            }
        }

        private async Task HandleFilesList(List<FileInfoData> newList)
        {
            if (newList.Any())
            {
                fileDataList = newList;
                ActivateAttach(false);
            }
            else
            {
                ActivateAttach(true);
            }
        }

        #endregion HandleModal

        #region HandleModalNotificacions

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                await OnReturnFilingAttachmentsModel.InvokeAsync(filingAttachments);

                var eventArgs = new MyEventArgs<List<AttachmentsDtoRequest>>
                {
                    Data = filingAttachments,
                    ModalStatus = false
                };

                await resetModal();
                await OnStatusChanged.InvokeAsync(eventArgs);
                UpdateModalStatus(false);
            }
            else
            {
                var eventArgs = new MyEventArgs<List<AttachmentsDtoRequest>>();

                await OnStatusChanged.InvokeAsync(eventArgs);
            }
        }

        #endregion HandleModalNotificacions

        #region Multilanguage

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion Multilanguage

        #endregion HandleMethods

        #region OthersMethods

        #region GetDataMethods

        private async Task GetAttachmentType()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "02");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                if (deserializeResponse!.Succeeded)
                {
                    systemFieldsList = deserializeResponse.Data!;
                }
                else
                {
                    systemFieldsList = new();
                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la notificación: {ex.Message}");
            }
        }

        #endregion GetDataMethods

        #region ModalActionsMethods

        public void UpdateModalStatus(bool newValue, List<AttachmentsDtoRequest> filingAttachmentsRemove = null)
        {
            //filingAttachments = filingAttachmentsRemove == null ? new List<AttachmentsDtoRequest>() : filingAttachmentsRemove;
            if (filingAttachmentsRemove != null)
            {
                filingAttachments = filingAttachmentsRemove;
            }

            modalStatus = newValue;
            StateHasChanged();
        }

        public async Task resetModal()
        {
            AttachmentType = "";
            CodeInput = "";
            DescriptionInput = "";
            contadorcarac = 0;
            fileDataList = new();

            await GetAttachmentType();
        }

        #endregion ModalActionsMethods

        #region GeneralMethods

        private void ContarCaracteres(ChangeEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value.ToString()))
            {
                contadorcarac = e.Value.ToString().Length;

                if (contadorcarac >= 10)
                {
                    ActivateAttach(false);
                    upFile = false;
                }
                else { ActivateAttach(true); upFile = true; }
            }
            else
            {
                contadorcarac = 0;
                upFile = true;
            }
        }

        private void ActivateAttach(bool disable)
        {
            if (fileDataList.Count > 0 && !String.IsNullOrEmpty(DescriptionInput) && !String.IsNullOrEmpty(AttachmentType) && !disable)
            {
                DisableBtnAdjuntar = disable;
            }
            else
            {
                DisableBtnAdjuntar = true;
            }
        }

        #endregion GeneralMethods

        #endregion OthersMethods

        #endregion Methods
    }
}