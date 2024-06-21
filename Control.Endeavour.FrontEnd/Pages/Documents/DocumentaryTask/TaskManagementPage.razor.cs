using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.DocumentaryTask;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.Attachments;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.TypologySearch;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.ViewDocuments;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VDocumentaryTypology.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Documents;
using DevExpress.Blazor.Office;
using DevExpress.Blazor.RichEdit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;
using static Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response.SystemFieldsDtoResponse;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;

namespace Control.Endeavour.FrontEnd.Pages.Documents.DocumentaryTask
{
    public partial class TaskManagementPage
    {
        #region Variables

        #region Inject

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private DocumentsStateContainer? documentsStateContainer { get; set; }

        [Inject]
        private NavigationManager navigation { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Modals

        private SecondPasswordModal secondPasswordModal = new();
        private DocumentClasificationModal documentClasificationModal = new();
        private SendDocumentModal sendDocumentsModal = new();
        private DocumentRelationModal docRelationModal = new();
        private AttachmentTrayModal attachmentTrayModal = new();
        private CopiesModal copyModal = new();
        private NotificationsComponentModal notificationModal = new();
        private NotificationsComponentModal notificationModal2 = new();
        private AIServiceModal AIServiceModal = new();

        #endregion Modals

        #region Models

        public TelerikPdfViewer? PdfViewerRef { get; set; }
        private DocumentClasificationDtoResponse? ProcedureDocClasContainer { get; set; }
        private CopyDtoResponse ProcedureCopiesContainer { get; set; } = new();
        private SendDocumentDtoResponse ProceduresendDocContainer { get; set; } = new();
        private int ProcedureNRadicado { get; set; }

        private DxRichEdit richEdit = new();
        private Selection selection;
        public byte[] FileData = Array.Empty<byte>();
        private FileDtoResponse fileInfo = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string panelTitle = "";
        private string panelButton = "";
        private string ProcessCode = "";
        private string TypeSign = "";
        private string destinationsNames = "";
        private string DisplayDocument = "col-md-6";
        private string DisplayTable = "col-md-6";
        private string docIcon = "fa-solid fa-eye";
        private string tableIcon = "fa-solid fa-eye";
        private string docText = "";
        private string tableText = "";
        private string title = "";
        private string SignatureType = "";
        private string DocClassComunication = "";
        private string DocClassTypologyName = "";
        private string action = "";
        private string DropdownMenuRecords = "dropdown-menu-show-no";
        private string DropdownActions = "dropdown-menu-show-no";
        private string iconAction = "fa-solid fa-angle-down";

        #endregion Environments(String)

        #region Environments(Numeric)

        private int idTask;

        private int UserId = 4055;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        public bool? SeenTask { get; set; }
        public bool fileWord { get; set; } = false;

        private bool panel_3 = false;
        private bool panel_4 = false;
        private bool cancelTask = false;
        private bool validDisplayTable = false;
        private bool validDisplayDocument = false;
        private bool isDropdownOpen = false;
        private bool isDropdownActionOpen = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<VSystemParamDtoResponse> systemFieldsTAINSList = new();
        private List<DocumentWorkFlowDtoResponse> documentWorkFlows = new();
        private List<DestinationsDtoRequest> ProcedureCopiesList = new();
        private List<SignatureDtoRequest> ProcedureSignatures = new();
        private List<AttachmentsDtoRequest> ProcedureAttachments = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            AppKeysFilterDtoRequest appKeysFilter = new();
            appKeysFilter.FunctionName = "ContenedorStorage";
            var responseApi = await HttpClient.PostAsJsonAsync("params/AppKeys/ByFilter", appKeysFilter);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<AppKeysDtoResponse>>>();
            if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
            {
                documentsStateContainer = await Js.InvokeAsync<DocumentsStateContainer>("decryptDataReturn", "data", deserializeResponse.Data[0].Value2);
            }

            //documentsStateContainer = await EncriptService.DecryptData<DocumentsStateContainer>(HttpClient, Js, "container");

            SpinnerLoaderService.ShowSpinnerLoader(Js);

            action = Translation["Action"];
            panelTitle = @Translation["ButtonSend"];
            panelButton = @Translation["ButtonSend"];
            title = @Translation["LinkDocument"];
            tableText = Translation["HideWorkflow"];
            docText = Translation["ShowDocument"];

            if (documentsStateContainer.Identification != 0)
            {
                idTask = documentsStateContainer.Identification;
                await GetAction();
                ValidUser(documentsStateContainer.PositionCard);
                await PrepareDocClasification(documentsStateContainer.Identification);

                if (SeenTask.HasValue && SeenTask.Value)
                {
                    await GetWorkFlow(documentsStateContainer.Identification);
                    await ShowWord(documentWorkFlows[documentWorkFlows.Count - 1].FileId.Value);
                }
                else
                {
                    await GetWorkFlow(documentsStateContainer.Identification);
                    await ShowPdf(documentWorkFlows[documentWorkFlows.Count - 1].FilePdfId.Value);
                }
            }

            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private async Task HandleSecondNotiCloseModal(ModalClosedEventArgs args)
        {
            if (args.IsAccepted && cancelTask)
            {
                navigation.NavigateTo("/DocumentaryTaskTray");
            }
        }

        private async Task HandleNotiCloseModal(ModalClosedEventArgs args)
        {
            if (args.IsAccepted && args.ModalOrigin.Equals("taskManagement"))
            {
                await CreateTaskManagement();
            }
        }

        private async Task HandleAIResponseAsync(string response)
        {
            //AIServiceModal.UpdateModalStatus(false);
            Document documentAPI = richEdit.DocumentAPI;
            await documentAPI.AddTextAsync(richEdit.Selection.CaretPosition, response);
            AIServiceModal.UpdateModalStatus(false);
        }

        #region ModalConfiguration Methods

        private Task ShowAIServiceTextGenerator()
        {
            AIServiceModal.UpdateModalStatus(true, "TXT");
            return Task.CompletedTask;
        }

        private Task ShowAIServiceSpeechToText()
        {
            AIServiceModal.UpdateModalStatus(true, "AUD");
            return Task.CompletedTask;
        }

        private Task ShowAIServiceSpeechToTranscription()
        {
            AIServiceModal.UpdateModalStatus(true, "FIL");
            return Task.CompletedTask;
        }

        private void ShowSendDocModal()
        {
            sendDocumentsModal.UpdateModalStatus(true);
        }

        private async Task ShowDocClasificationModal()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await documentClasificationModal.UpdateDocClasification(documentsStateContainer.Identification, SeenTask.Value, ProcedureDocClasContainer);
            documentClasificationModal.UpdateModalStatus(true);
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        public async Task ShowCopiesModal()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await copyModal.UpdateCopys(documentsStateContainer.Identification, SeenTask.Value, ProcedureCopiesContainer);
            copyModal.UpdateModalStatus(true);
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        public async Task ShowAttachmenTrayModal()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await attachmentTrayModal.UpdateAttachment(documentsStateContainer.Identification, SeenTask.Value, ProcedureAttachments);
            attachmentTrayModal.UpdateModalStatus(true);
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        public async Task ShowDocRelationModal()
        {
            if(ProcedureNRadicado > 0)
            {
                docRelationModal.UpdateModalStatus(true);
                docRelationModal.resetModal();
            }
            else
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                await docRelationModal.UpdateDocumentRelation(documentsStateContainer.Identification, SeenTask.Value);
                docRelationModal.UpdateModalStatus(true);
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
        }

        #endregion ModalConfiguration Methods

        #region ModalLoadData Methods

        private void HandleCopys(MyEventArgs<CopyDtoResponse> data)
        {
            if (data.Data != null)
            {
                copyModal.UpdateModalStatus(data.ModalStatus);
                ProcedureCopiesContainer = data.Data;
                ProcedureCopiesList.AddRange((data.Data.DestinationsUser != null) ? data.Data.DestinationsUser.Select(x => new DestinationsDtoRequest { DestinyId = x.UserId, DestinyType = "TDF,U", ItsCopy = true }).ToList() : new());
                ProcedureCopiesList.AddRange((data.Data.DestinationsAdministration != null) ? data.Data.DestinationsAdministration.Select(x => new DestinationsDtoRequest { DestinyId = (int)(x.ThirdPartyId ?? x.ThirdUserId), DestinyType = x.Type, ItsCopy = true }).ToList() : new());
            }
            else
            {
                copyModal.UpdateModalStatus(data.ModalStatus);
                ProcedureCopiesContainer = new();
            }
        }

        private void HandleDocumentClasification(DocumentClasificationDtoResponse data)
        {
            documentClasificationModal.UpdateModalStatus(false);
            ProcedureDocClasContainer = data;
            DocClassComunication = ProcedureDocClasContainer.ComunicationClass;
            DocClassTypologyName = ProcedureDocClasContainer.TypologyName;

            if (ProcedureDocClasContainer.DestinationsUser != null)
            {
                destinationsNames = string.Join(",", ProcedureDocClasContainer.DestinationsUser.Select(x => x.FullName));
            }
            else { destinationsNames += string.Join(",", ProcedureDocClasContainer.DestinationsAdministration.Select(x => x.FullName)); }
        }

        private void HandleSendDocuments(MyEventArgs<SendDocumentDtoResponse> data)
        {
            sendDocumentsModal.UpdateModalStatus(data.ModalStatus);
            ProceduresendDocContainer = data.Data;

            if (ProceduresendDocContainer != null)
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["CreateDocumentTask"], true, modalOrigin: "taskManagement");
            }
            else
            {
                Console.WriteLine("no se pudo crear el tramite de la tarea");
                notificationModal.UpdateModal(ModalType.Error, Translation["CannotCreate"], true);
            }
        }

        private void HandleDocumentRelation(MyEventArgs<int> data)
        {
            docRelationModal.UpdateModalStatus(data.ModalStatus);
            ProcedureNRadicado = data.Data;
        }

        #endregion ModalLoadData Methods

        #endregion HandleMethods

        #region OthersMethods

        #region PrepareDocClasification

        public async Task PrepareDocClasification(int idTask)
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("TaskId");
                HttpClient?.DefaultRequestHeaders.Add("TaskId", $"{idTask}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<DocumentClasificationDtoResponse>>("documentarytasks/DocumentaryTask/GetClasificationTask");
                HttpClient?.DefaultRequestHeaders.Remove("TaskId");

                if (deserializeResponse != null)
                {
                    DocClassComunication = deserializeResponse.Data.ComunicationClass;
                    DocClassTypologyName = deserializeResponse.Data.TypologyName;

                    if (deserializeResponse.Data.DestinationsUser != null)
                    {
                        destinationsNames = string.Join(",", deserializeResponse.Data.DestinationsUser.Select(x => x.FullName));
                    }
                    else { destinationsNames += string.Join(",", deserializeResponse.Data.DestinationsAdministration.Select(x => x.FullName)); }
                }
                else
                {
                    Console.WriteLine("No se encontro una clasificacion de documento para esa tarea documental");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar la clasificacion del documento: {ex.Message}");
            }
        }

        #endregion PrepareDocClasification

        #region GetActions

        public async Task GetAction()
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
                    systemFieldsTAINSList = deserializeResponse.Data.Where(x => x.FieldCode != "PR").ToList();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las acciones: {ex.Message}");
            }
        }

        #endregion GetActions

        #region CallActions

        public void CallAction(string value)
        {
            ProcessCode = value;

            if (!value.Equals("FR"))
            {
                action = systemFieldsTAINSList.Where(x => x.FieldCode == value).Select(x => x.Value).FirstOrDefault();
                iconAction = "fa-solid fa-pen-to-square";
                panelTitle = @Translation["ButtonSend"];
                panelButton = @Translation["ButtonSend"];
                panel_4 = true;

                if (value.Equals("AP"))
                {
                    iconAction = "fa-solid fa-file-circle-check";
                }
                else { iconAction = "fa-solid fa-check"; }
            }
            else
            {
                action = @Translation["Sign"];
                iconAction = "fa-solid fa-pen-to-square";
                panelTitle = @Translation["Radicate"];
                panelButton = @Translation["Radicate"];
                panel_4 = true;
            }
        }

        #endregion CallActions

        #region GetWorkFlow

        private async Task GetWorkFlow(int idTask)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                var workflow = new
                {
                    TaskId = idTask,
                };

                var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/GetWorkFlow", workflow);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<DocumentWorkFlowDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    documentWorkFlows = deserializeResponse.Data;
                }
                else
                {
                    Console.WriteLine("no se encontraro un flujo de trabajo para esa tarea documental");
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el flujo de trabajo: {ex.Message}");
            }
        }

        #endregion GetWorkFlow

        #region ValidUser

        private void ValidUser(bool positionCard)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                if (!positionCard)
                {
                    SeenTask = false;
                    panel_3 = false;
                    StateHasChanged();
                }
                else
                {
                    SeenTask = true;
                    panel_3 = true;
                    StateHasChanged();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al identificar usuario: {ex.Message}");
            }
        }

        #endregion ValidUser

        #region ShowWord

        private async Task ShowWord(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                fileWord = true;

                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                fileInfo = deserializeResponse.Data;

                if (fileInfo != null)
                {
                    FileData = fileInfo.DataFile;
                    await richEdit.LoadDocumentAsync(FileData, DocumentFormat.OpenXml);
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener archivo: {ex.Message}");
            }
        }

        #endregion ShowWord

        #region ShowNewWord

        private async Task ShowNewWord(DocumentWorkFlowDtoResponse docflow)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                SeenTask = true;

                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{docflow.FileId}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                fileInfo = deserializeResponse.Data;

                if (fileInfo != null)
                {
                    FileData = fileInfo.DataFile;
                    await richEdit.LoadDocumentAsync(FileData, DocumentFormat.OpenXml);
                    StateHasChanged();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener archivo: {ex.Message}");
            }
        }

        #endregion ShowNewWord

        #region ShowPdf

        private async Task ShowPdf(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                fileWord = false;

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

        #endregion ShowPdf

        #region ShowNewPdf

        private async Task ShowNewPdf(DocumentWorkFlowDtoResponse docflow)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                SeenTask = false;

                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{docflow.FilePdfId}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                fileInfo = deserializeResponse.Data;

                if (fileInfo != null)
                {
                    FileData = fileInfo.DataFile;
                    StateHasChanged();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener archivo: {ex.Message}");
            }
        }

        #endregion ShowNewPdf

        #region EditorComponent

        #region Customización de las Firmas

        public void OnCustomizeRibbon(IRibbon ribbon)
        {
            IRibbonTab signTab = ribbon.Tabs.AddCustomTab(Translation["SignaturesOthers"]);
            IRibbonTab AIService = ribbon.Tabs.AddCustomTab(Translation["AIService"]);

            IBarGroup secodtGroup = AIService.Groups.AddCustomGroup();
            IBarGroup firstGroup = signTab.Groups.AddCustomGroup();

            IBarButton LoadSignMec = firstGroup.Items.AddCustomButton(Translation["MechanicalSignature"], () => LoadSign("ME"));
            LoadSignMec.IconCssClass = "bi bi-vector-pen";
            LoadSignMec.Tooltip = Translation["MechanicalSignature"];

            IBarButton LoadSignRub = firstGroup.Items.AddCustomButton(Translation["RubricSignature"], () => LoadSign("RU"));
            LoadSignRub.IconCssClass = "bi bi-pencil-square";
            LoadSignRub.Tooltip = Translation["RubricSignature"];

            IBarButton LoadAIServiceTextGenerator = secodtGroup.Items.AddCustomButton(Translation["InsertText"], () => ShowAIServiceTextGenerator());
            LoadAIServiceTextGenerator.IconCssClass = "fa-solid fa-signature";
            LoadAIServiceTextGenerator.Tooltip = Translation["InsertText"];

            IBarButton LoadAIServiceSpeechToText = secodtGroup.Items.AddCustomButton(Translation["InsertVoice"], () => ShowAIServiceSpeechToText());
            LoadAIServiceSpeechToText.IconCssClass = "fa-solid fa-microphone";
            LoadAIServiceSpeechToText.Tooltip = Translation["InsertVoice"];

            IBarButton LoadAIServiceSpeechToTranscription = secodtGroup.Items.AddCustomButton(@Translation["UploadFile"], () => ShowAIServiceSpeechToTranscription());
            LoadAIServiceSpeechToTranscription.IconCssClass = "fa-solid fa-arrow-up-from-bracket";
            LoadAIServiceSpeechToTranscription.Tooltip = @Translation["UploadFile"];
        }

        #endregion Customización de las Firmas

        #region Cargue - Validación de Firmas

        private async Task LoadSign(string type)
        {
            if (type.Equals("ME", StringComparison.CurrentCultureIgnoreCase))
            {
                TypeSign = "TYFR,FIME";
                secondPasswordModal.UpdateModalStatus(true);
                SignatureType = TypeSign;
            }
            else if (type.Equals("TU", StringComparison.CurrentCultureIgnoreCase))
            {
                TypeSign = "TYFR,FIRU";
                secondPasswordModal.UpdateModalStatus(true);

                SignatureType = TypeSign;
            }
        }

        private async Task HandleValidatePasswordAsync(MyEventArgs<bool> validate)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            if (validate.Data)
            {
                secondPasswordModal.UpdateModalStatus(validate.ModalStatus);

                SignatureFilterDtoRequest signature = new() { SignatureType = TypeSign };

                var responseApi = await HttpClient.PostAsJsonAsync("security/Signature/ByFilterSignatures", signature);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<UserSignatureDtoResponse>>>();

                if (deserializeResponse != null)
                {
                    var identifyFile = deserializeResponse.Data.FirstOrDefault();

                    if (identifyFile!.FileId == null) { notificationModal.UpdateModal(ModalType.Error, Translation["CannotUploadSignature"], true); }

                    byte[] bytes = await GetSign(identifyFile.FileId.Value);

                    if (!bytes.Any()) { notificationModal.UpdateModal(ModalType.Error, Translation["EnterSignToSystem"], true); }

                    SignatureDtoRequest signRegister = new();
                    signRegister.SignatureDate = DateTime.Now;
                    signRegister.SignatureType = SignatureType;

                    ProcedureSignatures.Add(signRegister);

                    Document documentAPI = richEdit.DocumentAPI;

                    Stream file = new MemoryStream(bytes);

                    DocumentImageSource documentImageSource = DocumentImageSource.LoadFromStream(file, 3000, 3000);

                    await documentAPI.Images.CreateAsync(richEdit.Selection.CaretPosition, documentImageSource);
                }
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task<byte[]> GetSign(int FileId)
        {
            HttpClient?.DefaultRequestHeaders.Remove("FileId");
            HttpClient?.DefaultRequestHeaders.Add("FileId", $"{FileId}");
            var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
            HttpClient?.DefaultRequestHeaders.Remove("FileId");

            if (deserializeResponse != null) { return deserializeResponse.Data.DataFile; }

            return Array.Empty<byte>();
        }

        private async Task OnDocumentContentChanged(byte[] content)
        {
            FileData = content;
        }

        #endregion Cargue - Validación de Firmas

        #endregion EditorComponent

        #region send data to other modals

        private void HandleAttachmentChanged(MyEventArgs<List<AttachmentsDtoRequest>> data)
        {
            ProcedureAttachments = data.Data;
            attachmentTrayModal.UpdateModalStatus(data.ModalStatus);
        }

        #endregion send data to other modals

        #region CreateTaskManagement

        private async Task CreateTaskManagement()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            if (ProcedureSignatures.Count == 0 && ProcessCode.Equals("FR"))
            {
                notificationModal2.UpdateModal(ModalType.Information, Translation["SignNecessary"], true, buttonTextCancel: "");
            }
            else
            {
                await richEdit.SaveDocumentAsync();

                var taskManagement = new DocumentaryTaskManagementDtoRequest();

                taskManagement.Archive = FileData;
                taskManagement.ProcessCode = "TAINS," + ProcessCode;
                taskManagement.CreateUserId = UserId;

                if (!ProcessCode.Equals("FR"))
                {
                    taskManagement.UserTaskId = UserId;
                    taskManagement.UserForwardId = ProceduresendDocContainer.Recivers.UserId;
                    taskManagement.InstructionCode = ProceduresendDocContainer.Instruction;
                    taskManagement.Comentary = ProceduresendDocContainer.Description ?? string.Empty;
                }

                if (ProcedureNRadicado > 0)
                {
                    taskManagement.DocumentRelationId = ProcedureNRadicado;
                }

                taskManagement.TaskId = documentsStateContainer.Identification;

                List<DestinationsDtoRequest> Destinations = new();

                //Esto Aplica siempre y cuando alla objeto
                if (ProcedureDocClasContainer != null)
                {
                    taskManagement.Destinations = new();
                    taskManagement.ClassCode = ProcedureDocClasContainer.ClassCode;
                    taskManagement.DocDescription = ProcedureDocClasContainer.Description ?? string.Empty;
                    taskManagement.ShipingMethod = string.IsNullOrEmpty(ProcedureDocClasContainer.ShipingMethod) ? null : ProcedureDocClasContainer.ShipingMethod;
                    taskManagement.DocumentaryTypologyBehaviorId = ProcedureDocClasContainer.IdTypology;

                    if (ProcedureDocClasContainer.DestinationsUser.Count > 0)
                    {
                        foreach (var userDes in ProcedureDocClasContainer.DestinationsUser)
                        {
                            var tempUser = new DestinationsDtoRequest { DestinyId = userDes.UserId, DestinyType = userDes.Type, ItsCopy = false };
                            Destinations.Add(tempUser);
                        }
                    }
                    else
                    {
                        foreach (var userDes in ProcedureDocClasContainer.DestinationsAdministration)
                        {
                            var tempUser = new DestinationsDtoRequest { DestinyId = (int)(userDes.ThirdPartyId ?? userDes.ThirdUserId), DestinyType = userDes.Type, ItsCopy = false };
                            Destinations.Add(tempUser);
                        }
                    }

                    taskManagement.Destinations.AddRange(Destinations);
                }

                if (ProcedureSignatures.Count > 0) { taskManagement.Signatures = ProcedureSignatures; }

                if (ProcedureCopiesContainer != null)
                {
                    if (taskManagement.Destinations == null) { taskManagement.Destinations = new(); }

                    if (ProcedureCopiesContainer.DestinationsUser != null && ProcedureCopiesContainer.DestinationsUser.Count > 0)
                    {
                        taskManagement.Destinations.AddRange(ProcedureCopiesContainer.DestinationsUser.Select(x => new DestinationsDtoRequest { DestinyId = x.UserId, DestinyType = "TDF,U", ItsCopy = true }).ToList());
                    }

                    if (ProcedureCopiesContainer.DestinationsAdministration != null && ProcedureCopiesContainer.DestinationsAdministration.Count > 0)
                    {
                        taskManagement.Destinations.AddRange(ProcedureCopiesContainer.DestinationsAdministration.Select(x => new DestinationsDtoRequest { DestinyId = (int)(x.ThirdPartyId ?? x.ThirdUserId), DestinyType = x.Type, ItsCopy = true }).ToList());
                    }
                }

                if (ProcedureAttachments.Any())
                {
                    taskManagement.Attachments = ProcedureAttachments;
                }

                var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/TasksManagement", taskManagement);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<TaskManagementDtoResponse>>();

                if (deserializeResponse.Succeeded)
                {
                    notificationModal2.UpdateModal(ModalType.Success, string.Format(Translation["CreateSuccessfullDocumentaryTaskProcess"], taskManagement.TaskId), true);
                    cancelTask = true;
                }
                else
                {
                    notificationModal2.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true);
                    cancelTask = false;
                }
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion CreateTaskManagement

        #region Display Table or Document

        private void ShowTable()
        {
            validDisplayTable = !validDisplayTable;

            if (validDisplayTable && DisplayDocument.Equals("col-md-6") || (!validDisplayTable && DisplayDocument.Equals("col-md-12") || validDisplayTable && DisplayDocument.Equals("col-md-12")) || !validDisplayTable && DisplayDocument.Equals("col-md-6"))
            {
                DisplayTable = "col-md-12";
                DisplayDocument = "d-none";
                tableIcon = "fa-solid fa-eye-slash";
                tableText = Translation["HideWorkflow"];
                docIcon = "fa-solid fa-eye";
                docText = Translation["ShowDocument"];
            }
            else
            {
                tableText = Translation["ShowWorkflow"];
                DisplayTable = "col-md-6";
                DisplayDocument = "col-md-6";
                tableIcon = "fa-solid fa-eye";
            }
        }

        private void ShowDocument()
        {
            validDisplayDocument = !validDisplayDocument;

            if (validDisplayDocument && DisplayTable.Equals("col-md-6") || (!validDisplayDocument && DisplayTable.Equals("col-md-12") || validDisplayDocument && DisplayTable.Equals("col-md-12")))
            {
                DisplayTable = "d-none";
                DisplayDocument = "col-md-12";
                docIcon = "fa-solid fa-eye-slash";
                docText = Translation["HideDocument"];
                tableIcon = "fa-solid fa-eye";
                tableText = Translation["ShowWorkflow"];
            }
            else if (!validDisplayDocument && DisplayTable.Equals("col-md-6"))
            {
                tableText = Translation["ShowWorkflow"];
                docText = Translation["HideDocument"];
                DisplayTable = "d-none";
                DisplayDocument = "col-md-12";
                docIcon = "fa-solid fa-eye-slash";
                tableIcon = "fa-solid fa-eye";
            }
            else
            {
                docText = Translation["ShowDocument"];
                DisplayTable = "col-md-6";
                DisplayDocument = "col-md-6";
                docIcon = "fa-solid fa-eye";
            }
        }

        #endregion Display Table or Document

        #region metodo de prueba

        private void ToggleDropdown(ref bool dropDownOpenRef, ref string classdropdown)
        {
            if (dropDownOpenRef)
            {
                classdropdown = "dropdown-menu-show-no";
                dropDownOpenRef = false;
            }
            else
            {
                classdropdown = "dropdown-menu dropdown-menu-show-yes";
                dropDownOpenRef = true;
            }
        }

        #endregion metodo de prueba

        #endregion OthersMethods

        #endregion Methods
    }
}