using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.DocumentaryTask;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.Filing;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.MetaData;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.Attachments;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.Label;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.TypologySearch;
using Control.Endeavour.FrontEnd.Models.Enums.Attachments;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Country.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaData.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Radication;
using Control.Endeavour.FrontEnd.Models.Models.Administration.State.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VDocumentaryTypology.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Filing;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Globalization;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Documents.Filing
{
    public partial class FilingSentPage
    {
        #region Variables

        #region Inject

        //[Inject] private EventAggregatorService? EventAggregator { get; set; }
        [Inject] private HttpClient? HttpClient { get; set; }

        [Inject] private FilingStateContainer? FilingSC { get; set; }
        [Inject] private IJSRuntime Js { get; set; }
        [Inject] private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Components

        private string DocDescription { get; set; } = string.Empty;
        private string Pages { get; set; } = string.Empty;
        private string ReceptionCode { get; set; } = string.Empty;
        private string PriorityCode { get; set; } = "RPRI,N";
        private DateTime? DueDate { get; set; } = null;
        private string InternalDocument { get; set; } = string.Empty;
        private string GuideNumber { get; set; } = string.Empty;
        private int CountryId { get; set; } = 0;
        private int StateId { get; set; } = 0;
        private int CityId { get; set; } = 0;
        private string? ValueNotificacion { get; set; }

        #endregion Components

        #region Models

        private VDocumentaryTypologyDtoResponse TRDSelected = new();
        private FilingDtoRequest FilingRequest = new();
        private AttachmentsDtoRequest RecordToDelete = new();
        private AttachmentsType Filing = AttachmentsType.Filing;
        private AppKeysDtoResponse DefaultLocation = new();

        #endregion Models

        #region Modals

        private MetaDataRelationModal metaDataRelationModal { get; set; } = new();

        private MetaModel Meta = new();
        private GenericDocTypologySearchModal DocTypologySearchModal = new();
        private AttachmentsModal ModalAttachments = new();
        private MetaDataValueModal metaDataValueModal { get; set; } = new();
        private GenericUserSearchModal genericUserSearch = new();
        private NotificationsComponentModal notificationModal = new();
        private UploadPDFModal UploadPDFModal = new();
        private LabelModal LabelModal = new();
        private DocumentRelationModal docRelationModal = new();
        private ValidateDocumentGeneralInfoModal validateDocumentInfo = new();

        #endregion Modals

        #region Environments

        #region Environments(String)

        private string titleView { get; set; } = string.Empty;
        private string title { get; set; } = string.Empty;

        // <--[ Variables para ocultar pasos de la radicación ]-->

        private string panel_1 = ""; //Paso 1
        private string panel_2 = "d-none"; //Paso 2
        private string panel_3 = "d-none"; //Paso 3
        private string panel_4 = "d-none"; //Paso 4
        private string panel_5 = "d-none"; //Paso 5
        private string panel_6 = "d-none"; //Paso 6
        private string panel_7 = "d-none"; //Paso 7
        private string panel_8 = "d-none"; //Paso 8
        private string panel_9 = "d-none"; //Paso 9

        private string TablaUsers = "d-none";
        private string TablaAdjuntos = "d-none";
        private string Radicado = "";
        private string IdDocumento = "";
        private string Anio = "";

        #endregion Environments(String)

        #region Environments(Numeric)

        private readonly int defaultConfiguration = 4;
        private int configuration { get; set; } = 0;
        private decimal contadorcarac = 0;

        #endregion Environments(Numeric)

        #region Environments(DateTime)

        private DateTime Min = new DateTime(2020, 1, 1, 19, 30, 45);
        private DateTime Max = DateTime.Now;

        #endregion Environments(DateTime)

        #region Environments(Bool)

        private bool multipleSelection { get; set; } = false;
        private bool EnabledDepartamento { get; set; } = true;
        private bool EnabledMunicipio { get; set; } = true;
        private bool DisableButtons { get; set; } = false;
        private bool NewFilig { get; set; } = false;

        private bool UserModalStatus = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<MetaDataRelationDtoRequest> lstMetaDataRelation { get; set; } = new();
        private List<VSystemParamDtoResponse>? lstReceptionCode { get; set; } = new();
        private List<VSystemParamDtoResponse>? lstPriorityCode { get; set; } = new();
        private List<VSystemParamDtoResponse>? lstNotificacion { get; set; } = new();
        private List<CountryDtoResponse>? lstCountryId { get; set; } = new();
        private List<StateDtoResponse>? lstStateId { get; set; } = new();
        private List<CityDtoResponse>? lstCityId { get; set; } = new();
        private List<RetunUserSearch> listOfRadication { get; set; } = new();
        private List<AttachmentsDtoRequest> listAttachment { get; set; } = new();
        private List<RetunUserSearch> listSender { get; set; } = new();
        private List<RetunUserSearch> listCopy { get; set; } = new();
        private List<RetunUserSearch> listRecipient { get; set; } = new();
        private List<AttachmentsDtoRequest> Attachments { get; set; } = new();

        // <--[ Diccionarios que determinan los valores que se van a validar para habilitar los diferentes pasos de la radicación ]-->

        private Dictionary<string, string> Panel1 = new Dictionary<string, string>();
        private Dictionary<string, string> Panel2 = new Dictionary<string, string>();
        private Dictionary<string, string> Panel4 = new Dictionary<string, string>();
        private Dictionary<string, string> Panel5 = new Dictionary<string, string>();
        private Dictionary<string, string> Panel6 = new Dictionary<string, string>();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            try
            {
                titleView = Translation["Comunications"] + " - " + Translation["FilingCommunicationSent"];
                await GetDefaultLocation();
                await GetDocumentTypeTCR();
                await GetPriority();
                await GetCountry();
                await GetNotification();
                await GetState();
                await GetCity();
                GeneratePanels("");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar la initialización: {ex.Message}");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private void HandleTRDSelectedChanged(MyEventArgs<VDocumentaryTypologyDtoResponse> trd)
        {
            DocTypologySearchModal.resetModal();
            DocTypologySearchModal.UpdateModalStatus(trd.ModalStatus);
            TRDSelected = trd.Data;
        }

        private void HandleMetaDataRelationSelected(MyEventArgs<MetaDataRelationDtoRequest> data)
        {
            metaDataValueModal.UpdateModalStatus(data.ModalStatus);
            metaDataRelationModal.UpdateModalStatus(!data.ModalStatus);
        }

        private void HandleSerachModalToUse(MyEventArgs<SearchConfigurationArgs> data)
        {
            configuration = data.Data.Configuration;
            title = data.Data.Title;
            multipleSelection = data.Data.MultipleSelection;
            //GenericSearchModal.UpdateModalStatus(data.ModalStatus);
            metaDataValueModal.UpdateModalStatus(!data.ModalStatus);
        }

        private void HandleUsersMetaDataSelected(MyEventArgs<List<object>> request)
        {
            metaDataValueModal.UserSelectionMetaData((List<VUserDtoResponse>)request.Data[0]);
            //GenericSearchModal.UpdateModalStatus(request.ModalStatus);
            metaDataValueModal.UpdateModalStatus(!request.ModalStatus);
        }

        private void HandleThirdPartyMetaDataSelected(MyEventArgs<ThirdPartyDtoResponse> request)
        {
            metaDataValueModal.ThirdPartySelectionMetaData(request.Data);
            //GenericSearchModal.UpdateModalStatus(request.ModalStatus);
            metaDataValueModal.UpdateModalStatus(!request.ModalStatus);
        }

        private async Task HandleAttachments(MyEventArgs<List<AttachmentsDtoRequest>> data)
        {
            Attachments = data.Data;

            if (Attachments?.Count > 0)
            {
                listAttachment = new();

                foreach (var item in Attachments)
                {
                    AttachmentsDtoRequest attachmentsData = new AttachmentsDtoRequest()
                    {
                        DataFile = item.DataFile,
                        ArchiveName = item.ArchiveName,
                        ArchiveExt = item.ArchiveExt.ToString().Replace(".", ""), //Es importante que no lleve punto
                        ExhibitCode = item.ExhibitCode,
                        AttCode = item.AttCode,
                        AttDescription = item.AttDescription,
                        IconPath = item.IconPath,
                        Size = item.Size,
                    };

                    listAttachment.Add(attachmentsData);
                }

                if (listAttachment.Count > 0)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["AttachmentUploadSuccessfulMessage"], true, Translation["Accept"], "", "", "");
                    TablaAdjuntos = "";
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
                    TablaAdjuntos = "d-none";
                }
            }
            else
            {
                listAttachment = new List<AttachmentsDtoRequest>();
            }

            await Task.Delay(1000);
        }

        private void HandleSelectedUserData(List<RetunUserSearch> selectedUsers)
        {
            listOfRadication = new();
            listOfRadication = selectedUsers;
            if (listOfRadication.Count > 0)
            {
                listRecipient = selectedUsers
               .Where(user => user.IsRecipient == true)
               .ToList();

                listSender = selectedUsers
                    .Where(user => user.IsSender == true)
                    .ToList();

                listCopy = selectedUsers
                    .Where(user => user.IsCopy == true)
                    .ToList();
                ActivarPanelAsync(listOfRadication.Count.ToString(), "DESTINATARIOS", 4);
                TablaUsers = "";
            }
            else
            {
                listRecipient = new();
                listSender = new();
                listCopy = new();
                ActivarPanelAsync("", "DESTINATARIOS", 4);
                TablaUsers = "d-none";
            }

        }

        private async Task HandleFormCreate()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                List<DocumentSignatory> listSignatory = new List<DocumentSignatory>();
                List<DocumentReceiver> listReceiver = new List<DocumentReceiver>();
                List<MetaData> _lstMetaDataRelation = LinkMetaDataToPetition(lstMetaDataRelation);

                foreach (var item in listSender)
                {
                    var obj = new DocumentSignatory() { SignatoryId = item.UserId, TypeSignatory = item.TypeOfUser };
                    listSignatory.Add(obj);
                }

                foreach (var item in listRecipient)
                {
                    var obj = new DocumentReceiver() { ReceiverId = item.UserId, TypeReceiver = item.TypeOfUser, ItsCopy = false };
                    listReceiver.Add(obj);
                }

                foreach (var item in listCopy)
                {

                    var obj = new DocumentReceiver() { ReceiverId = item.UserId, TypeReceiver = item.TypeOfUser, ItsCopy = item.IsCopy };
                    listReceiver.Add(obj);
                }

                FilingRequest = new FilingDtoRequest()
                {
                    DocumentaryTypologyBehaviorId = TRDSelected.DocumentaryTypologyBehaviorId,
                    RecordId = 0,
                    FileId = 0,
                    DocDescription = DocDescription,
                    Pages = Convert.ToInt32(Pages),
                    ReceptionCode = ReceptionCode,
                    PriorityCode = PriorityCode,
                    DocDate = DueDate.Value,
                    Parameters = null,
                    DocumentSignatory = listSignatory,
                    DocumentReceiver = listReceiver,
                    Attachment = listAttachment,//new List<Attachments>(),
                    metaData = _lstMetaDataRelation,// new List<MetaData>(),
                    AutomaticShipping = "NO",
                    EndManagement = "NO",
                    NameSignatory = "NO",
                    DueDays = 0,
                    DueHours = 0,
                    InternalDocument = InternalDocument,
                    GuideNumber = GuideNumber,
                    CountryId = CountryId,
                    StateId = StateId,
                    CityId = CityId,
                    User = "JulianDB",
                    UserId = 4072,
                    UserAssingId = listReceiver.First().ReceiverId
                };

                if (!string.IsNullOrWhiteSpace(ValueNotificacion))
                {
                    FilingRequest.Parameters = new(){
            { "NOTIFICACION", ValueNotificacion }
        };
                }

                var responseApi = await HttpClient.PostAsJsonAsync("documents/Filing/Filing", FilingRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<FilingDtoResponse>>();

                if (deserializeResponse.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, string.Format(Translation["CreateSuccessfullFilingMessage"], deserializeResponse.Data.FilingCode), true);
                    Radicado = deserializeResponse.Data.FilingCode;
                    IdDocumento = deserializeResponse.Data.ControlId.ToString();
                    Anio = DateTime.Now.Year.ToString();

                    #region FillOutFiling

                    FilingSC?.Parametros(false);
                    FilingSC.FilingNumber = Radicado;
                    FilingSC.DocumentId = IdDocumento;
                    FilingSC.Folios = Pages;
                    FilingSC.Annexes = listAttachment.Count.ToString();

                    if (listRecipient.Count > 1)
                    {
                        FilingSC.Recipients = listRecipient.First().UserName + " - " + listRecipient.First().UserPosition + ", Otro(s)";
                    }
                    else
                    {
                        FilingSC.Recipients = listRecipient.First().UserName + " - " + listRecipient.First().UserPosition;
                    }

                    #endregion FillOutFiling

                    SubsequentProcesses(1);
                    ActivarPanelAsync(deserializeResponse.Data.FilingCode, "RADICACION", 5);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["GeneratingFilingErrorMessage"], true, Translation["Accept"]);
                    FilingSC?.Parametros(false);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar el radicado: {ex.Message}");
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                throw;
            }
        }

        private void HandleMetaDataSelected(MyEventArgs<MetaDataRelationDtoRequest> data)
        {
            metaDataRelationModal.UpdateModalStatus(!data.ModalStatus);
            metaDataValueModal.MetaFieldSelected(data.Data);
            metaDataValueModal.UpdateModalStatus(data.ModalStatus);
        }

        private void HandleMetaDataUpdated(MyEventArgs<List<MetaDataRelationDtoRequest>> data)
        {
            lstMetaDataRelation = data.Data;

            if (lstMetaDataRelation == null || lstMetaDataRelation.Count == 0)
            {
                lstMetaDataRelation = new();
            }
        }

        private void HandleModalClosed()
        {
            LabelModal.UpdateModalStatus(false);
        }

        private void HandleUploadPDFModal()
        {
            if (FilingSC.Verify == true)
            {
                ActivarPanelAsync("true", "VERIFICAR-ARCHIVO", 6);
                NewFilig = false;
            }
        }

        #endregion HandleMethods

        #region GetMethods

        private async Task GetDocumentTypeTCR()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "MR");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    lstReceptionCode = deserializeResponse.Data;
                }
                else { lstReceptionCode = new(); }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
        }

        private async Task GetPriority()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "RPRI");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    lstPriorityCode = deserializeResponse.Data;
                }
                else { lstPriorityCode = new(); }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la prioridad: {ex.Message}");
            }
        }

        private async Task GetCountry()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<CountryDtoResponse>>>("location/Country/ByFilter");

                if (deserializeResponse!.Succeeded)
                {
                    lstCountryId = deserializeResponse.Data;

                    if (lstCountryId.Count > 0)
                    {
                        EnabledDepartamento = false;
                        EnabledMunicipio = false;
                    }
                    else
                    {
                        EnabledDepartamento = false;
                        EnabledMunicipio = false;
                    }
                }
                else
                {
                    lstCountryId = new();
                    EnabledDepartamento = false;
                    EnabledMunicipio = false;
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el país: {ex.Message}");
            }
        }

        private async Task GetState()
        {
            try
            {
                if (CountryId > 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("countryId");
                    HttpClient?.DefaultRequestHeaders.Add("countryId", CountryId.ToString());
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<StateDtoResponse>>>("location/State/ByFilter");
                    HttpClient?.DefaultRequestHeaders.Remove("countryId");

                    if (deserializeResponse!.Succeeded)
                    {
                        lstStateId = deserializeResponse.Data;

                        if (lstStateId.Count > 0)
                        {
                            //CityId = int.Parse(DefaultLocation.Value3.Trim());
                            Panel2["PAIS"] = CountryId.ToString();
                            Panel2["DEPARTAMENTO"] = "";
                            Panel2["MUNICIPIO"] = "";
                            EnabledDepartamento = true;
                            EnabledMunicipio = false;
                            lstCityId = new();
                        }
                        else
                        {
                            Panel2["PAIS"] = "";
                            Panel2["DEPARTAMENTO"] = "";
                            Panel2["MUNICIPIO"] = "";
                            EnabledDepartamento = false;
                            EnabledMunicipio = false;
                        }
                    }
                    else
                    {
                        lstStateId = new();
                        EnabledDepartamento = false;
                        EnabledMunicipio = false;
                    }
                }
                else
                {
                    StateId = 0;
                    CityId = 0;
                    EnabledDepartamento = false;
                    EnabledMunicipio = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el departamento: {ex.Message}");
            }
        }

        private async Task GetCity()
        {
            try
            {
                if (StateId > 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("stateId");
                    HttpClient?.DefaultRequestHeaders.Add("stateId", StateId.ToString());
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<CityDtoResponse>>>("location/City/ByFilter");
                    HttpClient?.DefaultRequestHeaders.Remove("stateId");

                    if (deserializeResponse!.Succeeded)
                    {
                        lstCityId = deserializeResponse.Data == null ? new List<CityDtoResponse>() : deserializeResponse.Data;

                        if (lstCityId.Count > 0)
                        {
                            Panel2["DEPARTAMENTO"] = StateId.ToString();
                            Panel2["MUNICIPIO"] = "";
                            EnabledMunicipio = true;
                        }
                        else
                        {
                            Panel2["DEPARTAMENTO"] = "";
                            Panel2["MUNICIPIO"] = "";
                            EnabledMunicipio = false;
                        }
                    }
                    else
                    {
                        lstStateId = new();
                        EnabledDepartamento = false;
                        EnabledMunicipio = false;
                    }
                }
                else
                {
                    CityId = 0;
                    EnabledMunicipio = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el municipio: {ex.Message}");
            }
        }

        private async Task GetNotification()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "RNOTI");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    lstNotificacion = deserializeResponse.Data;
                }
                else { lstNotificacion = new(); }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la notificación: {ex.Message}");
            }
        }

        public async Task GetDefaultLocation()
        {
            AppKeysFilterDtoRequest appKeysFilter = new();
            appKeysFilter.FunctionName = "FilingGeolocation";
            appKeysFilter.KeyName = "HierarchicalLocationID";
            var responseApi = await HttpClient.PostAsJsonAsync("params/AppKeys/ByFilter", appKeysFilter);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<AppKeysDtoResponse>>>();

            if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
            {
                DefaultLocation = deserializeResponse.Data[0];
                CountryId = int.Parse(DefaultLocation.Value1.Trim());
                StateId = int.Parse(DefaultLocation.Value2.Trim());
                CityId = int.Parse(DefaultLocation.Value3.Trim());
            }
        }

        #endregion GetMethods

        #region ShowModalsMethods

        private void showModal(bool value)
        {
            DocTypologySearchModal.UpdateModalStatus(true, "CL,E");
        }

        private void showModalAttachments()
        {
            ModalAttachments.UpdateModalStatus(true, listAttachment);
        }

        private async Task showModalMetadatos()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (!TRDSelected.DocumentaryTypologyBagId.HasValue)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["NoMetadataInBag"], true);
            }

            if (lstMetaDataRelation == null || lstMetaDataRelation.Count == 0)
            {
                await metaDataRelationModal.SearchByDocumentaryTypology((int)TRDSelected.DocumentaryTypologyBagId);
            }
            else
            {
                metaDataRelationModal.existingMetaDataRelations(lstMetaDataRelation);
            }

            metaDataRelationModal.UpdateModalStatus(true);
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private void showModalLabel()
        {
            LabelModal.UpdateModalStatus(true);
        }

        private void showUploadPDFModal()
        {
            UploadPDFModal.UpdateModalStatus(true);
        }

        private void showRecipient()
        {
            configuration = defaultConfiguration;
            title = titleView;
            UserModalStatus = true;
        }

        private void ShowDocRelationModal()
        {
            docRelationModal.getDocumentFillCode(Radicado);
            docRelationModal.UpdateModalStatus(true);
        }

        public void MetaDataValueClose(bool value)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            metaDataValueModal.UpdateModalStatus(!value);
            metaDataRelationModal.UpdateModalStatus(value);

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ShowModalsMethods

        #region ValidationMethods

        private void ContarCaracteres(ChangeEventArgs e)
        {
            String value = e.Value.ToString() ?? String.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                contadorcarac = value.Length;
                if (contadorcarac >= 10)
                {
                    ActivarPanelAsync(value, "ASUNTO", 2);
                }
                else
                {
                    ActivarPanelAsync("", "ASUNTO", 2);
                }
            }
            else
            {
                contadorcarac = 0;
                ActivarPanelAsync(value, "ASUNTO", 2);
            }
        }

        private void GeneratePanels(String valueDefault = "")
        {
            #region PANEL 1

            Panel1["TIPODOCUMENTO"] = valueDefault;
            Panel1["TRAMITEGESTOR"] = valueDefault;

            #endregion PANEL 1

            #region PANEL 2

            Panel2["PAIS"] = valueDefault;
            Panel2["DEPARTAMENTO"] = valueDefault;
            Panel2["MUNICIPIO"] = valueDefault;
            Panel2["FOLIOS"] = valueDefault;
            Panel2["ASUNTO"] = valueDefault;
            Panel2["FECHA"] = valueDefault;
            Panel2["NOTIFICATION"] = valueDefault;

            #endregion PANEL 2

            #region PANEL 4

            Panel4["DESTINATARIOS"] = valueDefault;

            #endregion PANEL 4

            #region PANEL 5

            Panel5["RADICACION"] = valueDefault;

            #endregion PANEL 5

            #region PANEL 7

            Panel6["VERIFICAR-ARCHIVO"] = valueDefault;

            #endregion PANEL 7
        }

        private async Task ActivarPanelAsync(string value, string componente, decimal panel)
        {
            if (panel == 2 && componente == "FECHA")
            {
                DateTime.TryParse(value, out var dueDate);
                DueDate = dueDate;
            }
            bool habPanel2 = false;
            bool habPanel3 = false;
            bool habPanel5 = false;
            bool habPanel6 = false;
            bool habPanel7 = false;

            if (Panel1.Count > 0 && Panel2.Count > 0 && Panel4.Count > 0) //validar correcto funcionamiento de estos paneles cuando se limpa la vista
            {
                switch (panel)
                {
                    case 1:
                        if (Panel1.ContainsKey(componente))
                        {
                            Panel1[componente] = value;
                            ReceptionCode = componente == "TIPODOCUMENTO" ? Panel1[componente] : ReceptionCode != "" ? ReceptionCode : "";
                        }

                        habPanel2 = Panel1.Values.All(x => !string.IsNullOrEmpty(x) && x != "0" && x != Translation["TypologyNotFound"]);
                        habPanel3 = Panel2.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");
                        habPanel5 = Panel4.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");
                        habPanel6 = Panel5.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");
                        habPanel7 = Panel6.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");

                        if (habPanel2) //Habilita el paso 2
                        {
                            CountryId = int.Parse(DefaultLocation.Value1.Trim());
                            StateId = int.Parse(DefaultLocation.Value2.Trim());
                            CityId = int.Parse(DefaultLocation.Value3.Trim());
                            Panel2["PAIS"] = CountryId.ToString();
                            Panel2["DEPARTAMENTO"] = StateId.ToString();
                            Panel2["MUNICIPIO"] = CityId.ToString();
                            panel_2 = "d-flex justify-content-center";

                            if (habPanel3) //Habilita el paso 3
                            {
                                panel_3 = panel_4 = "d-flex justify-content-center";

                                if (habPanel5) //Habilita el paso 5
                                {
                                    panel_5 = "d-flex justify-content-center";

                                    if (habPanel6) //Habilita el paso 6 solo si el paso 5 esta completo
                                    {
                                        panel_6 = panel_7 = "d-flex justify-content-center";

                                        if (habPanel7) //Habilita el paso 8 solo si el paso 7 esta completo
                                        {
                                            panel_8 = "d-flex justify-content-center";
                                        }
                                        else
                                        {
                                            panel_8 = "d-none";
                                        }
                                    }
                                    else
                                    {
                                        panel_6 = panel_7 = "d-none";
                                    }
                                }
                                else
                                {
                                    panel_5 = panel_6 = panel_7 = panel_8 = "d-none";
                                }
                            }
                            else
                            {
                                panel_3 = panel_4 = panel_5 = panel_6 = panel_7 = panel_8 = "d-none";
                            }
                        }
                        else
                        {
                            CountryId = 0;
                            StateId = 0;
                            CityId = 0;
                            Panel2["PAIS"] = "";
                            Panel2["DEPARTAMENTO"] = "";
                            Panel2["MUNICIPIO"] = "";
                            panel_2 = panel_3 = panel_4 = panel_5 = panel_6 = panel_7 = panel_8 = "d-none";
                        }
                        break;

                    case 2:
                        if (Panel2.ContainsKey(componente))
                        {
                            Panel2[componente] = value;
                            if (panel == 2)
                            {
                                switch (componente)
                                {
                                    case "PAIS":
                                        CountryId = int.Parse(value);
                                        if (CountryId > 0)
                                        {
                                            await GetState();
                                        }
                                        else
                                        {
                                            Panel2["PAIS"] = "";
                                            Panel2["DEPARTAMENTO"] = "";
                                            Panel2["MUNICIPIO"] = "";
                                            lstStateId = new();
                                            lstCityId = new();
                                            EnabledDepartamento = false;
                                            EnabledMunicipio = false;
                                        }
                                        break;

                                    case "DEPARTAMENTO":
                                        StateId = int.Parse(value);
                                        if (StateId > 0)
                                        {
                                            await GetCity();
                                        }
                                        else
                                        {
                                            Panel2["DEPARTAMENTO"] = "";
                                            Panel2["MUNICIPIO"] = "";
                                            EnabledMunicipio = false;
                                            lstCityId = new();
                                        }
                                        break;

                                    case "MUNICIPIO":
                                        CityId = int.Parse(value);
                                        if (CityId > 0)
                                        {
                                            Panel2["MUNICIPIO"] = CityId.ToString();
                                        }
                                        else
                                        {
                                            Panel2["MUNICIPIO"] = "";
                                        }
                                        break;
                                }
                            }
                            if (componente == "NOTIFICATION")
                            {
                                ValueNotificacion = value;
                            }
                        }

                        habPanel3 = Panel2.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");
                        habPanel5 = Panel4.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");
                        habPanel6 = Panel5.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");

                        if (habPanel3) //Habilita el paso 3
                        {
                            panel_3 = panel_4 = "d-flex justify-content-center";
                            FilingSC?.Parametros(true);

                            if (habPanel5) //Habilita el paso 5
                            {
                                panel_5 = "d-flex justify-content-center";

                                if (habPanel6) //Habilita el paso 6 solo si el paso 5 esta completo
                                {
                                    panel_6 = panel_7 = "d-flex justify-content-center";

                                    if (habPanel7) //Habilita el paso 8 solo si el paso 7 esta completo
                                    {
                                        panel_8 = "d-flex justify-content-center";
                                    }
                                    else
                                    {
                                        panel_8 = "d-none";
                                    }
                                }
                                else
                                {
                                    panel_6 = panel_7 = panel_8 = "d-none";
                                }
                            }
                            else
                            {
                                panel_5 = panel_6 = panel_7 = panel_8 = "d-none";
                            }
                        }
                        else
                        {
                            panel_3 = panel_4 = panel_5 = panel_6 = panel_7 = panel_8 = "d-none";
                            FilingSC?.Parametros(false);
                        }
                        break;

                    case 4:
                        if (Panel4.ContainsKey(componente))
                        {
                            Panel4[componente] = value;
                        }

                        habPanel5 = Panel4.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");
                        habPanel6 = Panel5.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");

                        if (habPanel5) //Habilita el paso 5
                        {
                            panel_5 = "d-flex justify-content-center";

                            if (habPanel6) //Habilita el paso 6 solo si el paso 5 esta completo
                            {
                                panel_6 = panel_7 = "d-flex justify-content-center";

                                if (habPanel7) //Habilita el paso 8 solo si el paso 7 esta completo
                                {
                                    panel_8 = "d-flex justify-content-center";
                                }
                                else
                                {
                                    panel_8 = "d-none";
                                }
                            }
                            else
                            {
                                panel_6 = panel_7 = panel_8 = "d-none";
                            }
                        }
                        else
                        {
                            panel_5 = panel_6 = panel_7 = panel_8 = "d-none";
                        }
                        break;

                    case 5:
                        if (Panel5.ContainsKey(componente))
                        {
                            Panel5[componente] = value;
                        }

                        habPanel6 = Panel5.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");
                        habPanel7 = Panel6.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");

                        if (habPanel6) //Habilita el paso 6 solo si el paso 5 esta completo
                        {
                            panel_6 = panel_7 = "d-flex justify-content-center";

                            if (habPanel7) //Habilita el paso 8 solo si el paso 7 esta completo
                            {
                                panel_8 = "d-flex justify-content-center";
                            }
                            else
                            {
                                panel_8 = "d-none";
                            }
                        }
                        else
                        {
                            panel_6 = panel_7 = panel_8 = "d-none";
                        }

                        break;

                    case 6:
                        if (Panel6.ContainsKey(componente))
                        {
                            Panel6[componente] = value;
                        }

                        habPanel7 = Panel6.Values.All(x => !string.IsNullOrEmpty(x) && x != "0");

                        if (habPanel7) //Habilita el paso 8 solo si el paso 7 esta completo
                        {
                            panel_8 = "d-flex justify-content-center";
                        }
                        else
                        {
                            panel_8 = "d-none";
                        }
                        break;
                }
            }
        }

        #endregion ValidationMethods

        #region OthersMethods

        private async Task RemoverFile(AttachmentsDtoRequest fileInfo)
        {
            RecordToDelete = fileInfo;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "RemoveFile");
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                if (args.ModalOrigin == "RemoveFile")
                {
                    listAttachment.Remove(RecordToDelete);

                    if (listAttachment.Count == 0)
                    {
                        TablaAdjuntos = "d-none";
                    }
                    else
                    {
                        TablaAdjuntos = "";
                    }
                }
            }
            else
            {
                Console.WriteLine("Registro No eliminado");
            }
        }

        private void RemoveUser(RetunUserSearch user)
        {
            if (user.IsSender == true)
            {
                listSender.RemoveAll(x => x.UserId == user.UserId);
            }
            if (user.IsRecipient == true)
            {
                listRecipient.RemoveAll(x => x.UserId == user.UserId);
            }
            if (user.IsCopy == true)
            {
                listCopy.RemoveAll(x => x.UserId == user.UserId);
            }

            listOfRadication.RemoveAll(x => x.UserId == user.UserId);
            genericUserSearch.RemoveUserSelectedById(user.UserId);
            if (listSender.Count() > 0 && listRecipient.Count() > 0)
            {
                ActivarPanelAsync(listOfRadication.Count.ToString(), "DESTINATARIOS", 4);
            }
            else
            {
                ActivarPanelAsync("", "DESTINATARIOS", 4);
            }
        }


        private void SubsequentProcesses(decimal processes)
        {
            switch (processes)
            {
                case 1: //Bloquear funcionamiento

                    #region DisablePanels

                    panel_1 += " deshabilitar-content";
                    panel_2 += " deshabilitar-content";
                    panel_3 += " deshabilitar-content";
                    panel_4 += " deshabilitar-content";
                    panel_5 += " deshabilitar-content";

                    #endregion DisablePanels

                    DisableButtons = true;

                    break;

                case 2: //Nueva radicación

                    #region WashComponents

                    ReceptionCode = "";
                    TRDSelected = new();
                    PriorityCode = "RPRI,SE";
                    panel_1 = "d-flex justify-content-center";

                    CountryId = 0;
                    StateId = 0;
                    EnabledDepartamento = false;
                    CityId = 0;
                    EnabledMunicipio = false;
                    GuideNumber = "";
                    Pages = "";
                    InternalDocument = "";
                    ValueNotificacion = "";
                    lstMetaDataRelation = new();
                    contadorcarac = 0;
                    DocDescription = "";
                    panel_2 = "d-none";

                    listAttachment = new();
                    TablaAdjuntos = "d-none";
                    listSender = new();
                    listRecipient = new();
                    listCopy = new();
                    TablaUsers = "d-none";
                    panel_3 = "d-none";
                    panel_4 = "d-none";

                    panel_5 = "d-none";
                    panel_6 = "d-none";
                    panel_7 = "d-none";
                    panel_8 = "d-none";
                    panel_9 = "d-none";

                    Panel4["DESTINATARIOS"] = "";
                    Panel5["RADICACION"] = "";
                    Panel6["VERIFICAR-ARCHIVO"] = "";

                    NewFilig = true;

                    #endregion WashComponents
                    Radicado = "";
                    IdDocumento = "";
                    DisableButtons = false;
                    GeneratePanels("");

                    break;

                case 3: // Mantener Datos

                    #region WashComponents

                    contadorcarac = 0;
                    DocDescription = "";
                    listAttachment = new();
                    TablaAdjuntos = "d-none";
                    listSender = new();
                    listRecipient = new();
                    listCopy = new();
                    TablaUsers = "d-none";
                    panel_1 = "";
                    panel_2 = "";
                    panel_3 = "d-none";
                    panel_4 = "d-none";
                    panel_5 = "d-none";
                    panel_6 = "d-none";
                    panel_7 = "d-none";
                    panel_8 = "d-none";
                    panel_9 = "d-none";
                    Panel4["DESTINATARIOS"] = "";
                    Panel5["RADICACION"] = "";
                    Panel6["VERIFICAR-ARCHIVO"] = "";
                    Radicado = "";
                    IdDocumento = "";

                    NewFilig = true;

                    #endregion WashComponents

                    DisableButtons = false;

                    break;
            }
        }

        private List<MetaData> LinkMetaDataToPetition(List<MetaDataRelationDtoRequest> dataToLink)
        {
            List<MetaData> metadataToLink = new();

            foreach (var item in dataToLink)
            {
                if (!String.IsNullOrEmpty(item.DataText))
                {
                    metadataToLink.Add(
                    new()
                    {
                        MetaFieldId = item.MetaFieldId,
                        ColorData = item.ColorData,
                        DataText = item.DataText,
                    }
                    );
                }
            }
            return metadataToLink;
        }

        #region RelatedDocument

        private async Task HandleValidateDocumentAsync(int controlId)
        {
            await validateDocumentInfo.GeneralInformation(controlId);
            validateDocumentInfo.UpdateModalStatus(true);
        }

        private async Task HandleDocumentAssociated(bool data)
        {
            docRelationModal.UpdateModalStatus(false);
        }

        #endregion RelatedDocument

        #region CloseModalSearchUser
        private void HandleModalUserClosed(bool status)
        {

            UserModalStatus = status;

        }

        #endregion
        #endregion OthersMethods

        #endregion Methods
    }
}