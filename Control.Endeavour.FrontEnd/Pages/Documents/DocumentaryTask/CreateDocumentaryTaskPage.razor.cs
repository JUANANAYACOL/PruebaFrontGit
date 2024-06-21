using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.DocumentaryTask;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.Attachments;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.TypologySearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VDocumentaryTypology.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
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
using DevExpress.Blazor.Primitives.Internal;
using DevExpress.Blazor.RichEdit;
using DevExpress.Utils.About;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Numerics;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Pages.Documents.DocumentaryTask
{
    public partial class CreateDocumentaryTaskPage
    {
        #region Variables

        #region Inject

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private NavigationManager navigation { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        [Inject]
        private DocumentsStateContainer? documentsStateContainer { get; set; }

        #endregion Inject

        #region Components

        private DxRichEdit richEditControl = new();
        private Selection? selection;

        #endregion Components

        #region Modals

        private FormatMasterListModal formatMasterListModal = new();
        private DocumentClasificationModal documentClasificationModal = new();
        private SendDocumentModal sendDocumentsModal = new();
        private DocumentRelationModal docRelationModal = new();
        private AttachmentsModal attachmentsModal = new();
        private CopiesModal copyModal = new();
        private NotificationsComponentModal notificationModal = new();
        private NotificationsComponentModal notificationModal2 = new();
        private AIServiceModal AIServiceModal = new();

        #endregion Modals

        #region Models

        private DocumentClasificationDtoResponse docClasContainer { get; set; } = new();
        private SendDocumentDtoResponse sendDocumentContainer { get; set; } = new();
        private SecondPasswordModal modalSecondPass = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string ProcessCode = "";
        private string panelTitle = "";
        private string panelButton = "";
        private string TypeSign = "";
        private string destinationsNames = "";
        private string title = "";
        private string filingCode = "";
        private string SignatureType = "";
        private string action = "";
        private string DropdownMenuRecords = "dropdown-menu-show-no";
        private string DropdownActions = "dropdown-menu-show-no";
        private string iconAction = "fa-solid fa-angle-down";

        #endregion Environments(String)

        #region Environments(Numeric)

        private int TemplateId = 0;
        private int TemporalTemplateId = 0;
        private int docRelationContainer { get; set; }

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool panel_2 = false;
        private bool panel_3 = false;
        private bool panel_4 = false;
        private bool isDropdownOpen = false;
        private bool isDropdownActionOpen = false;
        private bool panel_Radicado = false;
        private bool MT_value = false;
        private bool cancelTask = false;
        private bool isFirstLoad = true;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<VSystemParamDtoResponse> systemFieldsTAINSList = new();
        public List<SignatureDtoRequest> signatures = new();
        private byte[] TaskManagementFile = Array.Empty<byte>();
        private List<DestinationsDtoRequest> _Copys = new();
        private List<AttachmentsDtoRequest>? attachmentsContainer = new();
        private byte[] fileContent = Array.Empty<byte>();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            panelTitle = @Translation["ButtonSend"];
            panelButton = @Translation["ButtonSend"];
            title = @Translation["LinkDocument"];
            action = @Translation["Action"];
            //documentsStateContainer = await EncriptService.DecryptData<DocumentsStateContainer>(HttpClient, Js, "container");

            //HttpClient?.DefaultRequestHeaders.Remove("ControlId");
            //HttpClient?.DefaultRequestHeaders.Add("ControlId", $"{documentsStateContainer.Identification}");
            //var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<DocumentResponseComentaryClosedDtoResponse>>("documentmanagement/DocumentManagement/GetDocumentClosedBehavior");
            //HttpClient?.DefaultRequestHeaders.Remove("ControlId");

            //if (deserializeResponse.Succeeded)
            //{
            //    documentsStateContainer.documentResponseComentaryClosed = deserializeResponse.Data;
            //}

            await GetAction();
            documentsStateContainer.ValidNavigation(true);

            if (documentsStateContainer.documentResponseComentaryClosed.ControlId != 0)
            {
                panel_Radicado = true;
                filingCode = documentsStateContainer.documentResponseComentaryClosed.FilingCode;
                docRelationContainer = documentsStateContainer.documentResponseComentaryClosed.ControlId;

                if (documentsStateContainer.documentResponseComentaryClosed.DocumentaryTypologiesBehaviorId != 0)
                {
                    panel_2 = true;
                    panel_3 = true;
                }
            }
            else
            {
                panel_2 = false;
                panel_3 = false;
                panel_4 = false;
                panel_Radicado = false;
            }
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
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

        private void ShowCopiesModal()
        {
            copyModal.UpdateModalStatus(true);
        }

        private void ShowFormatMasterModal()
        {
            formatMasterListModal.UpdateModalStatus(true);
        }

        private void ShowDocClasificationModal()
        {
            documentClasificationModal.UpdateModalStatus(true);
        }

        private void ShowSendDocsModal()
        {
            sendDocumentsModal.UpdateModalStatus(true);
        }

        private void ShowDocRelationModal()
        {
            docRelationModal.UpdateModalStatus(true);
            docRelationModal.resetModal();
        }

        private void ShowAttachmentModal()
        {
            attachmentsModal.UpdateModalStatus(true);
        }

        #endregion ModalConfiguration Methods

        private async Task HandleNotiCloseModal(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                await CreateDocumentTask();
            }
        }

        private async Task HandleSecondNotiCloseModal(ModalClosedEventArgs args)
        {
            switch (args.ModalOrigin)
            {
                case "managementTray":
                    if (args.IsAccepted && cancelTask && args.ModalOrigin == "managementTray")
                    {
                        navigation.NavigateTo("/ManagementTray");
                    }

                    break;

                case "":
                    if (args.IsAccepted && cancelTask)
                    {
                        navigation.NavigateTo("/DocumentaryTaskTray");
                    }

                    break;

                case "template":
                    if (args.IsAccepted)
                    {
                        TemplateId = TemporalTemplateId;
                        documentClasificationModal.resetModal(false);
                        ProcessCode = "";
                        panel_3 = false;
                        panel_4 = false;
                        _Copys = new();
                        attachmentsContainer = new();
                        docRelationContainer = 0;
                        await HandleSelectedTemplate(fileContent);
                    }

                    break;

                default:
                    notificationModal.UpdateModal(ModalType.Error, "ModalOrigin no reconocido", true);
                    break;
            }
        }

        private async Task HandleValidatePasswordAsync(MyEventArgs<bool> validate)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            if (validate.Data)
            {
                modalSecondPass.UpdateModalStatus(validate.ModalStatus);

                SignatureFilterDtoRequest signature = new() { SignatureType = TypeSign };

                var responseApi = await HttpClient.PostAsJsonAsync("security/Signature/ByFilterSignatures", signature);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<UserSignatureDtoResponse>>>();

                if (deserializeResponse != null)
                {
                    var identifyFile = deserializeResponse.Data.FirstOrDefault();

                    if (identifyFile!.FileId == null) { notificationModal.UpdateModal(ModalType.Error, Translation["CannotUploadSignature"], true); }

                    byte[] bytes = await GetSign(identifyFile.FileId!.Value);

                    if (!bytes.Any()) { notificationModal.UpdateModal(ModalType.Error, Translation["EnterSignToSystem"], true); }

                    SignatureDtoRequest signRegister = new();
                    signRegister.SignatureDate = DateTime.Now;
                    signRegister.SignatureType = SignatureType;

                    signatures.Add(signRegister);

                    Document documentAPI = richEditControl.DocumentAPI;

                    Stream file = new MemoryStream(bytes);

                    DocumentImageSource documentImageSource = DocumentImageSource.LoadFromStream(file, 3000, 3000);

                    await documentAPI.InlineImages.CreateAsync(richEditControl.Selection.CaretPosition, documentImageSource);
                }
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #region DataContainer Methods

        private void HandleCopys(MyEventArgs<CopyDtoResponse> data)
        {
            copyModal.UpdateModalStatus(data.ModalStatus);
            _Copys.AddRange((data.Data.DestinationsUser != null) ? data.Data.DestinationsUser.Select(x => new DestinationsDtoRequest { DestinyId = x.UserId, DestinyType = "TDF,U", ItsCopy = true }).ToList() : new());
            _Copys.AddRange((data.Data.DestinationsAdministration != null) ? data.Data.DestinationsAdministration.Select(x => new DestinationsDtoRequest { DestinyId = (int)(x.ThirdPartyId ?? x.ThirdUserId), DestinyType = x.Type, ItsCopy = true }).ToList() : new());
        }

        private void HandleDocumentClasification(DocumentClasificationDtoResponse data)
        {
            documentClasificationModal.UpdateModalStatus(false);
            docClasContainer = data;
            panel_3 = true;

            destinationsNames = string.Join(",", docClasContainer.DestinationsUser.Select(x => x.FullName));
            destinationsNames += string.Join(",", docClasContainer.DestinationsAdministration.Select(x => x.FullName));
        }

        private async Task HandleAIResponseAsync(string response)
        {
            //AIServiceModal.UpdateModalStatus(false);
            Document documentAPI = richEditControl.DocumentAPI;
            await documentAPI.AddTextAsync(richEditControl.Selection.CaretPosition, response);
            AIServiceModal.UpdateModalStatus(false);
        }

        private void HandleSendDocuments(MyEventArgs<SendDocumentDtoResponse> data)
        {
            sendDocumentsModal.UpdateModalStatus(data.ModalStatus);
            sendDocumentContainer = data.Data;

            if (sendDocumentContainer != null)
            {
                if (!string.IsNullOrEmpty(sendDocumentContainer.Description) && !string.IsNullOrEmpty(sendDocumentContainer.Instruction) && sendDocumentContainer.Recivers != null)
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation["Summary"], true, "", Translation["Cancel"], Translation["ConfirmAction"]);
                }
                else
                {
                    notificationModal2.UpdateModal(ModalType.Information, Translation["FillModal"], true, buttonTextCancel: "");
                }
            }
        }

        private void HandleDocumentRelation(MyEventArgs<int> data)
        {
            docRelationModal.UpdateModalStatus(data.ModalStatus);
            docRelationContainer = data.Data;
        }

        private void HandleAttachmentList(MyEventArgs<List<AttachmentsDtoRequest>> data)
        {
            attachmentsModal.UpdateModalStatus(data.ModalStatus);
            attachmentsContainer = data.Data;
        }

        #region LoadFileFromMasterList

        private async Task HandleSelectedFile(MyEventArgs<TemplateModelDtoResponse> file)
        {
            formatMasterListModal.UpdateModalStatus(file.ModalStatus);

            TemporalTemplateId = file.Data.TemplateId;

            if (TemplateId != TemporalTemplateId && TemplateId != 0)
            {
                notificationModal2.UpdateModal(ModalType.Information, Translation["ChangingTemplate"], true, modalOrigin: "template");
                fileContent = file.Data.Archive;
            }
            else if (TemplateId == TemporalTemplateId)
            {
                notificationModal2.UpdateModal(ModalType.Information, Translation["SameTemplate"], true, buttonTextCancel: "");
            }
            else
            {
                TemplateId = TemporalTemplateId;
                await HandleSelectedTemplate(file.Data.Archive);
            }
        }

        private async Task HandleSelectedTemplate(byte[] file)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            panel_2 = true;
            MT_value = true;
            StateHasChanged();
            await richEditControl.NewDocumentAsync();

            byte[] tempFileContent = file;

            await richEditControl.LoadDocumentAsync(tempFileContent, DocumentFormat.OpenXml);
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion LoadFileFromMasterList

        #endregion DataContainer Methods

        #endregion HandleMethods

        #region OthersMethods

        #region Prepare modals

        private async Task ShowDocClasificationMT()
        {
            documentClasificationModal.UpdateModalStatus(true);
            if (isFirstLoad)
            {
                await documentClasificationModal.DocClasificationMT(documentsStateContainer.documentResponseComentaryClosed);
                isFirstLoad = false;
            }
        }

        private async Task ShowDocRelationMT()
        {
            docRelationModal.UpdateModalStatus(true);
            docRelationModal.DocRelationMT(documentsStateContainer.documentResponseComentaryClosed, false);
        }

        #endregion Prepare modals

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
                    systemFieldsTAINSList = deserializeResponse.Data.Where(x => x.FieldCode.Equals("PR") || x.FieldCode.Equals("FR")).ToList();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Translation["ErrorMessage"] + $": { ex.Message}");
            }
        }

        #endregion GetActions

        #region CallActions

        public void CallAction(string value)
        {
            ProcessCode = value;

            if (value.Equals("PR"))
            {
                action = @Translation["ToProject"];
                iconAction = "fa-solid fa-paper-plane";
                panelTitle = @Translation["ButtonSend"];
                panelButton = @Translation["ButtonSend"];
                panel_4 = true;
            }
            else if (value.Equals("FR"))
            {
                action = @Translation["Sign"];
                iconAction = "fa-solid fa-pen-to-square";
                panelTitle = @Translation["Radicate"];
                panelButton = @Translation["Radicate"];
                panel_4 = true;
            }
        }

        #endregion CallActions

        #region CreateDocumentTask

        private async Task CreateDocumentTask()
        {
            #region Creación de Objeto

            if (signatures.Count == 0 && ProcessCode.Equals("FR"))
            {
                notificationModal2.UpdateModal(ModalType.Information, @Translation["SignNecessary"], true, buttonTextCancel: "");
            }
            else if (string.IsNullOrEmpty(docClasContainer.Description))
            {
                notificationModal2.UpdateModal(ModalType.Information, @Translation["SubjectCategorizingDoc"], true, buttonTextCancel: "");
                await ShowDocClasificationMT();
            }
            else if (TemplateId == 0)
            {
                notificationModal2.UpdateModal(ModalType.Information, @Translation["SelectTemplate"], true, buttonTextCancel: "");
                ShowFormatMasterModal();
            }
            else
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                var DocumentaryTask = new DocumentaryTaskDtoRequest
                {
                    //En el primer paso se debe de capturar el Template
                    TemplateId = TemplateId,

                    //En el Segundo paso se captura: el id del Behavior, el asunto de la radicacion, el destinatario de la radicación
                    DocumentaryTypologyBehaviorId = docClasContainer.IdTypology,
                    ClassCode = docClasContainer.ClassCode,
                    DocDescription = docClasContainer.Description ?? string.Empty,
                    ShipingMethod = string.IsNullOrEmpty(docClasContainer.ShipingMethod) ? null : docClasContainer.ShipingMethod
                };

                //Se agregan los desinatarios que proviene de la clasificacion y pertenecen a los destinatarios de la radicación
                List<DestinationsDtoRequest> Destinations = new();

                if (docClasContainer.DestinationsUser.Count > 0)
                {
                    foreach (var userDes in docClasContainer.DestinationsUser)
                    {
                        var tempUser = new DestinationsDtoRequest { DestinyId = userDes.UserId, DestinyType = userDes.Type, ItsCopy = false };
                        Destinations.Add(tempUser);
                    }
                }
                else
                {
                    foreach (var userDes in docClasContainer.DestinationsAdministration)
                    {
                        var tempUser = new DestinationsDtoRequest { DestinyId = (int)(userDes.ThirdPartyId ?? userDes.ThirdUserId), DestinyType = userDes.Type, ItsCopy = false };
                        Destinations.Add(tempUser);
                    }
                }

                //En el Tercer Paso se captura : El process code
                if (docRelationContainer != 0)
                {
                    DocumentaryTask.DocumentRelationId = docRelationContainer;
                }

                if (attachmentsContainer != null)
                {
                    DocumentaryTask.Attachments = attachmentsContainer;
                }

                DocumentaryTask.TaskManagementRequest.ProcessCode = "TAINS," + ProcessCode;

                if (ProcessCode.Equals("PR"))
                {
                    //En el Cuarto Paso se captura:  destinatario, instruction code, task description

                    DocumentaryTask.TaskManagementRequest.UserForwardId = sendDocumentContainer.Recivers.UserId;
                    DocumentaryTask.TaskManagementRequest.InstructionCode = sendDocumentContainer.Instruction;
                    DocumentaryTask.TaskManagementRequest.Comentary = sendDocumentContainer.Description ?? string.Empty;
                }

                //DocumentaryTask.TaskManagementRequest.UserTaskId = UserId;
                DocumentaryTask.TaskDescription = docClasContainer.Description ?? string.Empty;
                //Captura datos adicionales del Documentary Task
                //DocumentaryTask.CreateUserId = UserId;

                //Fecha de Vencimiento es un parametro de APPKEYS para agregar al momento de la creacion del documento los dias que tiene para vencimiento.
                DocumentaryTask.TaskDate = DateTime.Now;
                DocumentaryTask.DueDate = DateTime.Now.AddDays(10);

                await richEditControl.SaveDocumentAsync();
                TaskManagementFile = await richEditControl.ExportDocumentAsync(DocumentFormat.OpenXml);

                // Captura de datos del TaskManagemetn
                DocumentaryTask.TaskManagementRequest.Archive = TaskManagementFile;

                //Captura de Firmas
                DocumentaryTask.Signatures = signatures;

                DocumentaryTask.Destinations = Destinations;

                if (_Copys.Any())
                {
                    DocumentaryTask.Destinations.AddRange(_Copys);
                }

                #endregion Creación de Objeto

                var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/CreateDocumentaryTask", DocumentaryTask);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<DocumentaryTaskDtoResponse>>();

                if (deserializeResponse.Succeeded)
                {
                    documentsStateContainer.ActiveTask = false;

                    if (documentsStateContainer.documentResponseComentaryClosed.ControlId != 0)
                    {
                        var model = new ManagementTrayDtoRequest()
                        {
                            ControlId = documentsStateContainer.documentResponseComentaryClosed.ControlId,
                            ActionId = documentsStateContainer.documentResponseComentaryClosed.ActionId,
                            CommentaryClosed = $"CERRADO CON TAREA DOCUMENTAL {deserializeResponse.Data.TaskId}"
                        };

                        var responseApiProcess = await HttpClient.PostAsJsonAsync("documentmanagement/DocumentManagement/CreateProcess", model);
                        var deserializeResponseProcess = await responseApiProcess.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();

                        if (deserializeResponseProcess.Succeeded)
                        {
                            documentsStateContainer.documentResponseComentaryClosed = new();
                            notificationModal2.UpdateModal(ModalType.Success, string.Format(Translation["CreateSuccessfullDocumentaryTask"], deserializeResponse.Data.TaskId), true, modalOrigin: "managementTray");
                            cancelTask = true;
                        }
                    }
                    else
                    {
                        documentsStateContainer.documentResponseComentaryClosed = new();
                        notificationModal2.UpdateModal(ModalType.Success, string.Format(Translation["CreateSuccessfullDocumentaryTask"], deserializeResponse.Data.TaskId), true);
                        cancelTask = true;
                    }
                }
                else
                {
                    notificationModal2.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true);
                    cancelTask = false;
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
        }

        #endregion CreateDocumentTask

        #region SaveDocumentContent

        private async Task OnDocumentContentChanged(byte[] content)
        {
            TaskManagementFile = content;
        }

        #endregion SaveDocumentContent

        #region CustomizedSigns and Load/Validate Signs

        public void OnCustomizeRibbon(IRibbon ribbon)
        {
            IRibbonTab signTab = ribbon.Tabs.AddCustomTab(@Translation["SignaturesOthers"]);
            IRibbonTab AIService = ribbon.Tabs.AddCustomTab(@Translation["AIService"]);

            IBarGroup secodtGroup = AIService.Groups.AddCustomGroup();
            IBarGroup firstGroup = signTab.Groups.AddCustomGroup();

            IBarButton LoadSignMec = firstGroup.Items.AddCustomButton(@Translation["MechanicalSignature"], () => LoadSign("ME"));
            LoadSignMec.IconCssClass = "bi bi-vector-pen";
            LoadSignMec.Tooltip = @Translation["MechanicalSignature"];

            IBarButton LoadSignRub = firstGroup.Items.AddCustomButton(@Translation["RubricSignature"], () => LoadSign("RU"));
            LoadSignRub.IconCssClass = "bi bi-pencil-square";
            LoadSignRub.Tooltip = @Translation["RubricSignature"];

            IBarButton LoadAIServiceTextGenerator = secodtGroup.Items.AddCustomButton(@Translation["InsertText"], () => ShowAIServiceTextGenerator());
            LoadAIServiceTextGenerator.IconCssClass = "fa-solid fa-signature";
            LoadAIServiceTextGenerator.Tooltip = @Translation["InsertText"];

            IBarButton LoadAIServiceSpeechToText = secodtGroup.Items.AddCustomButton(@Translation["InsertVoice"], () => ShowAIServiceSpeechToText());
            LoadAIServiceSpeechToText.IconCssClass = "fa-solid fa-microphone";
            LoadAIServiceSpeechToText.Tooltip = @Translation["InsertVoice"];

            IBarButton LoadAIServiceSpeechToTranscription = secodtGroup.Items.AddCustomButton(@Translation["UploadFile"], () => ShowAIServiceSpeechToTranscription());
            LoadAIServiceSpeechToTranscription.IconCssClass = "fa-solid fa-arrow-up-from-bracket";
            LoadAIServiceSpeechToTranscription.Tooltip = @Translation["UploadFile"];
        }

        private async Task LoadSign(string type)
        {
            if (type.Equals("ME", StringComparison.CurrentCultureIgnoreCase))
            {
                TypeSign = "TYFR,FIME";
                SignatureType = TypeSign;
                modalSecondPass.UpdateModalStatus(true);
            }
            else if (type.Equals("TU", StringComparison.CurrentCultureIgnoreCase))
            {
                TypeSign = "TYFR,FIRU";
                SignatureType = TypeSign;
                modalSecondPass.UpdateModalStatus(true);
            }
        }

        private async Task<byte[]> GetSign(int FileId)
        {
            HttpClient?.DefaultRequestHeaders.Remove("FileId");
            HttpClient?.DefaultRequestHeaders.Add("FileId", FileId.ToString());
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
            HttpClient?.DefaultRequestHeaders.Remove("FileId");

            if (deserializeResponse != null) { return deserializeResponse.Data!.DataFile; }

            return Array.Empty<byte>();
        }

        #endregion CustomizedSigns and Load/Validate Signs

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