using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.ViewDocuments;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Records.Request;
using Control.Endeavour.FrontEnd.Models.Models.Records.Response;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.Blazor.Primitives.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.ComponentModel.Design;
using System.Net.Http.Json;
using System.Net.Mail;
using Telerik.Blazor.Components;
using Newtonsoft.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Generic.Attachments
{
    public partial class AttachmentTrayModal
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

        #region Components

        public TelerikPdfViewer? PdfViewerRef { get; set; }

        #endregion Components

        #region Modals

        private AttachmentsModal attachmentsModal = new();
        private ViewDocumentModal viewDocumentModal = new();
        private NotificationsComponentModal notificationModal = new();

        #endregion Modals

        #region Parameters

        [Parameter] public EventCallback<MyEventArgs<List<AttachmentsDtoRequest>>> OnStatusChanged { get; set; }

        [Parameter] public bool IsDocumentAttachment { get; set; }
        [Parameter] public bool IsControlPanel { get; set; } = false;

        #endregion Parameters

        #region Models

        private FileDtoResponse fileInfo = new();
        private AttachmentRecordsDtoResponse attachentRecord = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string panelGridClass = "";
        private byte[] FileData = Array.Empty<byte>();

        #endregion Environments(String)

        #region Environments(Numeric)

        public int ActiveTabIndex { get; set; } = 0;
        private int ControlId = 0;
        private int DocumentId = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool SeenAttachments = false;
        private bool validUploadFile = false;
        private bool isTaskAttachment = true;
        private bool isDocumentAttachment = false;
        private bool isControlPanel = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<AttachmentsDtoRequest> attachmentList = new();
        private List<AttachmentsDtoResponse> AttachmentsList = new();
        private List<AttachmentsDtoResponse> AttachmentDeleteList = new();
        private List<AttachmentDocumentDtoResponse> AttachmentDocumentList = new();
        private List<AttachmentDocumentDtoResponse> AttachmentDocumentTrueList = new();
        private List<AttachmentRecordsDtoResponse> AttachmentControlPanelList = new();
        private List<AttachmentsDtoResponse> AttachmentDocumentCPList = new();
        private List<AttachmentsDtoRequest> listAttachment { get; set; } = new();
        private List<AttachmentDocumentDtoRequest> AttachmentRequest = new();
        private List<AttachmentRecordsDtoRequest> AttachmentControlPanelListRequest = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            if (IsDocumentAttachment)
            {
                isTaskAttachment = false;
                isDocumentAttachment = true;
                isControlPanel = false;
            }
            else if (IsControlPanel)
            {
                isTaskAttachment = false;
                isDocumentAttachment = false;
                isControlPanel = true;
            }
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            StateHasChanged();
        }

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        private async Task TabChangedHandler(int newIndex)
        {
            ActiveTabIndex = newIndex;
            if (ActiveTabIndex == 3 && AttachmentDocumentTrueList.Count == 0)
            {
                await UpdateAttachmentDocument(ControlId, true);
            }
        }

        private void ShowAttachmentModal()
        {
            attachmentsModal.UpdateModalStatus(true);
        }

        #region ViewAttachment

        private async Task HandleViewAttachment(AttachmentsDtoResponse data)
        {
            if (data.ArchiveExt.Equals("pdf"))
            {
                await GetFile(data.FileId);
                viewDocumentModal.DataContainer(Convert.ToBase64String(FileData));
                viewDocumentModal.UpdateModalStatus(true);
            }
            else
            {
                await viewDocumentModal.DownloadFile(data.FileId);
            }
        }

        private async Task HandleViewAttachmentDocument(AttachmentDocumentDtoResponse data)
        {
            if (data.ArchiveExt.Equals("pdf"))
            {
                await GetFile(data.FileId);
                viewDocumentModal.DataContainer(Convert.ToBase64String(FileData));
                viewDocumentModal.UpdateModalStatus(true);
            }
            else
            {
                await viewDocumentModal.DownloadFile(data.FileId);
            }
        }

        private async Task HandleViewAttachmentRecords(AttachmentRecordsDtoResponse data)
        {
            if (data.FileExt.Equals("pdf"))
            {
                await GetFile(data.FileId);
                viewDocumentModal.DataContainer(Convert.ToBase64String(FileData));
                viewDocumentModal.UpdateModalStatus(true);
            }
            else
            {
                await viewDocumentModal.DownloadFile(data.FileId);
            }
        }

        private async Task HandleViewDocumentRecords(AttachmentsDtoResponse data)
        {
            if (data.ArchiveExt.Equals("pdf"))
            {
                await GetFile(data.FileId);
                viewDocumentModal.DataContainer(Convert.ToBase64String(FileData));
                viewDocumentModal.UpdateModalStatus(true);
            }
            else
            {
                await viewDocumentModal.DownloadFile(data.FileId);
            }
        }

        #endregion ViewAttachment

        #region HandleModalNotificacions

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            switch (args.ModalOrigin)
            {
                case "ManagementTray":
                    break;
                case "Records":
                    if (args.IsAccepted && attachentRecord != null)
                    {
                        if (attachentRecord?.AttachmentId != null)
                        {
                            var deleteValidation = await DeleteAttachmentRecords(attachentRecord.AttachmentId);

                            if (deleteValidation)
                            {
                                notificationModal.UpdateModal(ModalType.Success, Translation["AttachDeleteSuccess"], true, Translation["Accept"], modalOrigin: "ManagementTray");
                                await UpdateAttachmentControlPanel(DocumentId);
                            }
                        }
                        else
                        {
                            notificationModal.UpdateModal(ModalType.Error, Translation["FileAttachError"], true);
                        }
                    }
                    break;
                case "":
                    if (args.IsAccepted)
                    {
                        var eventArgs = new MyEventArgs<List<AttachmentsDtoRequest>>
                        {
                            Data = attachmentList,
                            ModalStatus = false
                        };

                        await OnStatusChanged.InvokeAsync(eventArgs);
                        UpdateModalStatus(false);
                    }
                    else
                    {
                        var eventArgs = new MyEventArgs<List<AttachmentsDtoRequest>>();

                        await OnStatusChanged.InvokeAsync(eventArgs);
                    }
                    break;
               
            }
            
        }

        #endregion HandleModalNotificacions

        #endregion HandleMethods

        #region OthersMethods

        #region GetFile

        private async Task GetFile(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                fileInfo = deserializeResponse.Data;

                if (fileInfo != null)
                {
                    FileData = fileInfo.DataFile;
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener archivo: {ex.Message}");
            }
        }

        #endregion GetFile

        #region GetAttachmentData

        public void GetAttachmentData(List<AttachmentsDtoRequest> attachment)
        {
            attachmentList.AddRange(attachment);
            AttachmentsList.AddRange(attachment.Select(x => new AttachmentsDtoResponse { ArchiveName = x.ArchiveName, ExibitCodeName = x.ExhibitCodeName, AttDescription = x.AttDescription }).ToList());
        }
        #endregion GetAttachmentData

        #region UpdateAttachments

        public async Task UpdateAttachment(int id, bool value, List<AttachmentsDtoRequest> attachments)
        {
            SeenAttachments = !value;
            validUploadFile = true;
            AttachmentsList = new();
            AttachmentDeleteList = new();

            HttpClient?.DefaultRequestHeaders.Remove("TaskId");
            HttpClient?.DefaultRequestHeaders.Add("TaskId", $"{id}");
            var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<AttachmentsDtoResponse>>>("documentarytasks/DocumentaryTask/GetAttachments");
            HttpClient?.DefaultRequestHeaders.Remove("TaskId");

            if (deserializeResponse.Data != null)
            {
                AttachmentsList.AddRange(deserializeResponse.Data.Where(x => x.Active));
                AttachmentDeleteList.AddRange(deserializeResponse.Data.Where(x => !x.Active));
            }
            else { Console.WriteLine("no se encontraron adjuntos"); }

            if (attachments.Count > 0)
            {
                AttachmentsList.AddRange(attachments.Select(x => new AttachmentsDtoResponse { ArchiveName = x.ArchiveName, ExibitCodeName = x.ExhibitCodeName, AttDescription = x.AttDescription }).ToList());
            }
        }

        public async Task UpdateAttachmentDocument(int id, bool AttGenerated = false)
        {
            try
            {
                ControlId = id;

                var data = new
                {
                    controlId = id,
                    attGenerated = AttGenerated,
                };
                var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Document/ByAttFilter", data);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<AttachmentDocumentDtoResponse>>>();
                if (deserializeResponse?.Data != null)
                {
                    switch (AttGenerated)
                    {
                        case true:
                            AttachmentDocumentTrueList = deserializeResponse.Data;
                            ActiveTabIndex = 3;
                            break;

                        case false:
                            AttachmentDocumentList = deserializeResponse.Data;
                            ActiveTabIndex = 2;
                            break;
                    }
                }
                else
                {
                    notificationModal.UpdateModal(Models.Enums.Components.Modals.ModalType.Information, Translation["NoAttachments4Document"], true, buttonTextCancel: "", modalOrigin: "ManagementTray");
                    switch (AttGenerated)
                    {
                        case true:
                            AttachmentDocumentTrueList = new();
                            ActiveTabIndex = 3;
                            break;

                        case false:
                            AttachmentDocumentList = new();
                            ActiveTabIndex = 2;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener adjuntos: {ex.Message}");
            }
        }

        public async Task UpdateAttachmentControlPanel(int _DocumentId)
        {
            try
            {
                DocumentId = _DocumentId;
                HttpClient?.DefaultRequestHeaders.Remove("documentId");
                HttpClient?.DefaultRequestHeaders.Add("documentId", $"{DocumentId}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FilesDocumentDtoResponse>>("records/ControlBoard/AttachmentsByDocumentId");
                HttpClient?.DefaultRequestHeaders.Remove("documentId");

                if (deserializeResponse?.Data != null)
                {
                    FilesDocumentDtoResponse dataByDocument = deserializeResponse.Data;
                    AttachmentControlPanelList = deserializeResponse.Data.Attachments;

                    await GetFile(AttachmentControlPanelList.First().FileId);

                    AttachmentDocumentCPList = new();

                    AttachmentsDtoResponse model = new AttachmentsDtoResponse()
                    {
                        FileId = dataByDocument.FileId,
                        ArchiveName = fileInfo.FileName,
                        ArchiveExt = fileInfo.FileExt,
                        AttDescription = dataByDocument.DocDescription,
                        CreateUser = dataByDocument.CreateUser,
                        CreateDate = dataByDocument.CreateDate.ToString()
                    };

                    AttachmentDocumentCPList.Add(model);
                    ActiveTabIndex = 5;
                }
                else
                {
                    notificationModal.UpdateModal(Models.Enums.Components.Modals.ModalType.Information, Translation["NoAttachments4Document"], true, buttonTextCancel: "", modalOrigin: "ManagementTray");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener adjuntos: {ex.Message}");
            }
        }

        #endregion UpdateAttachments

        #region Call Modal

        private void SendAttachments()
        {
            if (attachmentList.Count != 0)
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["AddAttachmentsQuestion"], true, Translation["Yes"], Translation["No"]);
            }
        }

        private async void HandleAttachmentChanged(MyEventArgs<List<AttachmentsDtoRequest>> data)
        {
            if (isDocumentAttachment)
            {
                await HandleGestioAttachmentChanged(data);
            }
            else if (isControlPanel)
            {
                await HandleAttachmentRecordsChanged(data);
            }
            else
            {
                GetAttachmentData(data.Data);
                attachmentsModal.UpdateModalStatus(data.ModalStatus);
            }
            
        }
        
        public async Task HandleGestioAttachmentChanged(MyEventArgs<List<AttachmentsDtoRequest>> data)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            GetAttachmentData(data.Data);
            listAttachment = data.Data;
            AttachmentRequest = listAttachment.Select(a => new AttachmentDocumentDtoRequest
            {
                CompanyId = 17,
                DataFile = a.DataFile ?? throw new ArgumentNullException(nameof(a.DataFile)),
                ArchiveName = a.ArchiveName ?? throw new ArgumentNullException(nameof(a.ArchiveName)),
                ArchiveExt = a.ArchiveExt ?? throw new ArgumentNullException(nameof(a.ArchiveExt)),
                ControlId = ControlId,
                ExhibitCode = a.ExhibitCode ?? throw new ArgumentNullException(nameof(a.ExhibitCode)),
                AttDescription = a.AttDescription ?? throw new ArgumentNullException(nameof(a.AttDescription)),
                AttNumber = 1,
                AttCode = a.AttCode ?? throw new ArgumentNullException(nameof(a.AttCode)),
                CreateUser = "Front"
            }).ToList();
            var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Document/CreateAttachment", AttachmentRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<AttachmentDocumentDtoResponse>>>();
            if (deserializeResponse.Succeeded)
            {
                notificationModal.UpdateModal(ModalType.Success, Translation["FileAttachSuccess"], true, Translation["Accept"],modalOrigin: "ManagementTray");
                await UpdateAttachmentDocument(ControlId);
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["FileAttachError"], true, modalOrigin: "ManagementTray");
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
        }

        public async Task HandleAttachmentRecordsChanged(MyEventArgs<List<AttachmentsDtoRequest>> data)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            GetAttachmentData(data.Data);
            listAttachment = data.Data;

            AttachmentControlPanelListRequest = listAttachment.Select(a => new AttachmentRecordsDtoRequest
            {
                DataFile = a.DataFile ?? throw new ArgumentNullException(nameof(a.DataFile)),
                ArchiveName = a.ArchiveName ?? throw new ArgumentNullException(nameof(a.ArchiveName)),
                ArchiveExt = a.ArchiveExt ?? throw new ArgumentNullException(nameof(a.ArchiveExt)),
                ExhibitCode = a.ExhibitCode ?? throw new ArgumentNullException(nameof(a.ExhibitCode)),
                AttDescription = a.AttDescription ?? throw new ArgumentNullException(nameof(a.AttDescription)),
                AttCode = a.AttCode ?? throw new ArgumentNullException(nameof(a.AttCode)),
                DocumentId = DocumentId
            }).ToList();

            var responseApi = await HttpClient.PostAsJsonAsync("records/ControlBoard/AttachmentsDocumentBulks", AttachmentControlPanelListRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();
            if (deserializeResponse.Succeeded)
            {
                notificationModal.UpdateModal(ModalType.Success, Translation["FileAttachSuccess"], true, Translation["Accept"], modalOrigin: "ManagementTray");
                await UpdateAttachmentControlPanel(DocumentId);
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["FileAttachError"], true, modalOrigin: "ManagementTray");
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
        }

        #endregion Call Modal

        #region DeleteAttachments

        private async Task<bool> DeleteAttachment(int taskId, int id)
        {
            AttachmentsDeleteDtoRequest attachment = new AttachmentsDeleteDtoRequest()
            {
                TaskId = taskId,
                AttchmentId = id
            };

            var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/DeleteAttchments", attachment);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();

            if (deserializeResponse.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> DeleteAttachmentRecords(int id)
        {
            HttpClient?.DefaultRequestHeaders.Remove("attachmentId");
            HttpClient?.DefaultRequestHeaders.Add("attachmentId", $"{id}");
            var responseApi = await HttpClient.PostAsJsonAsync<object>("records/ControlBoard/DeleteAttachmentsDocument", null);
            HttpClient?.DefaultRequestHeaders.Remove("attachmentId");
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();

            if (deserializeResponse.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion DeleteAttachments

        #region RemoveAttachments

        private async Task DeleteAttachments(AttachmentsDtoResponse attachment)
        {
            if (attachment.TaskId != null)
            {
                var deleteValidation = await DeleteAttachment(attachment.TaskId.Value, attachment.AttachmentId);

                if (deleteValidation)
                {
                    AttachmentDeleteList.Add(attachment);
                    AttachmentsList.Remove(attachment);
                }
            }
            else
            {
                AttachmentDeleteList.Add(attachment);
                AttachmentsList.Remove(attachment);
            }
        }

        private async Task DeleteAttachmentsRecords(AttachmentRecordsDtoResponse attachment)
        {
            attachentRecord = attachment;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteAttachmentMessage"], true, Translation["Yes"], Translation["No"], modalOrigin:"Records");
        }

        #endregion RemoveAttachments

        #endregion OthersMethods

        #endregion Methods
    }
}