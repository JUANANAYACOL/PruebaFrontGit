using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.Attachments;
using Control.Endeavour.FrontEnd.Models.Enums.Attachments;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Documents;
using Control.Endeavour.FrontEnd.Models.Enums.Generic;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.Documents;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Models.Models.Records.Response;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Documents;
using Control.Endeavour.FrontEnd.StateContainer.ManagementTray;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Globalization;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Documents.ManagementTray;

public partial class ManagementTrayPage : IDisposable
{
    #region Variables

    #region Inject

    //[Inject] private EventAggregatorService? EventAggregator { get; set; }
    [Inject] private HttpClient? HttpClient { get; set; }

    [Inject] private ManagementTrayStateContainer managementTrayStateContainer { get; set; }
    [Inject] private IJSRuntime Js { get; set; }
    [Inject]
    protected NavigationManager NavigationManager { get; set; }

    [Inject]
    private IStringLocalizer<Translation>? Translation { get; set; }

    #endregion Inject

    #region Components

    private int mounthValue { get; set; }
    private int yearValue { get; set; }
    private int daysValue { get; set; }
    public bool enabledMes { get; set; } = false;
    public bool enabledDia { get; set; } = false;
    private string idControl { get; set; }
    private string numRadicado { get; set; }
    private string? classCodeValue { get; set; }
    private string? prioridadValue { get; set; }
    private InputModalComponent IdcontrolInput { get; set; }
    private InputModalComponent NumRadicaInput { get; set; }
    private DocumentStatusEnum statusPage = DocumentStatusEnum.WithoutProcessingWord;

    #endregion Components

    #region Input

    private InputModalComponent idControlInput { get; set; }
    private InputModalComponent numRadicaInput { get; set; }

    #endregion Input

    #region Models

    private MetaModel Meta = new();
    private MetaModel MetaGrid = new();
    private DataCardDtoResponse DataCards = new();
    private DocumentDtoResponse InfoGridCard = new();
    private NotificationsComponentModal notificationModal = new();
    private ManagementOfProceduresModal managementOfProcedures = new();
    private GeneralInformationModal generalInformation = new();
    private PaginationInfo paginationInfo = new();
    private DocumentManagementCountDtoRequest filterDocumentManagamentById = new();

    private AttachmentTrayModal attachmentTrayModal = new();
    private PaginationComponent<DocumentDtoResponse, ManagementTrayFylterDtoRequest> PaginationComponet = new();
    private NewPaginationComponent<DocumentDtoResponse, ManagementTrayFylterDtoRequest> paginationComponent = new();
    private ManagementTrayFylterDtoRequest FilterDtoRequest = new();
    private List<DocumentDtoResponse>? managementTrayToReturn { get; set; } = new();
    private List<int> SelectedControlIds = new();
    private List<FileInfoData> lstFileInfoData { get; set; } = new();
    private List<AttachmentsDtoRequest> listAttachment { get; set; } = new();
    private AttachmentsType Filing = AttachmentsType.Filing;

    #endregion Models

    #region Environments(String)

    private string textYear = string.Empty;
    private string textMes = string.Empty;
    private string textDia = string.Empty;
    private string textClase = string.Empty;
    private string textPrio = string.Empty;
    private string Gex = "";
    private string Enp = "";
    private string Etr = "";
    private string Cop = "";
    private string GexP = "";
    private string EnpP = "";
    private string EtrP = "";
    private string CopP = "";
    private string Estado = "";
    private string StatusCode = "";
    private string TablaAdjuntos = "d-none";
    private string UriFilterDocuments = "documentmanagement/Document/FilterPaged";
    private string UriFilterCardsDocuemnts = "documentmanagement/Document/ByAssingUserId";
    private string ResponseRecordClosed = "No";
    private string HeaderTitle = string.Empty;

    #endregion Environments(String)

    #region Environments(Numeric)

    private int companyId = 17;
    private int Total = 0;
    private int controlId = 0;

    #endregion Environments(Numeric)

    #region Environments(Bool)

    private bool activeState = false;
    private bool clickPendiente = false;
    private bool dueDateValue = false;
    private bool activateProcedure = false;
    private bool istCopy = false;
    private bool isMasive = false;
    private bool isSubscribed = false;

    #endregion Environments(Bool)

    #region Environments(DateTime)

    private DateTime? SelectedDate { get; set; }
    private DateTime Min = DateTime.Now;
    private DateTime Max = new DateTime(2025, 1, 1, 19, 30, 45);

    #endregion Environments(DateTime)

    #region Environments(List & Dictionary)

    private List<VSystemParamDtoResponse>? FormatBG { get; set; } = new();
    private List<VSystemParamDtoResponse>? FormatCL { get; set; } = new();
    private List<DocumentDtoResponse>? GeneralList { get; set; } = new();
    private List<DateDtoResponse> Year = new();
    private List<DateDtoResponse> Mounth = new();
    private List<DateDtoResponse> Days = new();
    private List<DataCardDtoRequest> Data = new();
    private List<AttachmentDocumentDtoRequest> AttachmentRequest = new();
    private List<string> optionsRecordClosed = new List<string>() { "Sí", "No" };

    #endregion Environments(List & Dictionary)

    #endregion Variables

    #region OnInitializedAsync

    protected override async Task OnInitializedAsync()
    {
        try
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            optionsRecordClosed = new List<string>() { Translation["Yes"], Translation["No"] };
            textYear = Translation["SelectAnOption"];

            textMes = Translation["SelectAnOption"];

            textDia = Translation["SelectAnOption"];

            textClase = Translation["SelectAnOption"];

            textPrio = Translation["SelectAnOption"];

            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            
            managementTrayToReturn = new();
            FillList();
            
            await GetPriority();
            await GetNotiComuni();
            //managementTrayStateContainer = await EncriptService.DecryptData<ManagementTrayStateContainer>(HttpClient, Js, "FilterRequestDocumentManagement");
            
            if (managementTrayStateContainer.UserId != 0 && managementTrayStateContainer.AnotherUser)
            {
                filterDocumentManagamentById.AssingUserId = managementTrayStateContainer.UserId;
                filterDocumentManagamentById.IsAnotherUser = managementTrayStateContainer.AnotherUser;
                HeaderTitle = $"{Translation["Management"]} - {Translation["ManagementTray"]} - {managementTrayStateContainer.UserFullName}";


                await GetDataCards();
                await GetData(DocumentStatusEnum.WithoutProcessingWord);

            }
            else
            {
                HeaderTitle = $"{Translation["Management"]} - {Translation["ManagementTray"]}";
                filterDocumentManagamentById = new();
                await GetDataCards();

                if (managementTrayStateContainer.Status > 0)
                {
                    await GetData(managementTrayStateContainer.Status);
                }
                else
                {
                    await GetData(DocumentStatusEnum.WithoutProcessingWord);
                }
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al realizar la initialización: {ex.Message}");
        }
    }

    public void Dispose()
    {
        // Código de limpieza aquí
        Console.WriteLine(managementTrayStateContainer.UserId);
        managementTrayStateContainer.ManagementTrayParameters(0, false,string.Empty);
        Console.WriteLine(managementTrayStateContainer.UserId);
        
    }


    #endregion OnInitializedAsync

    #region Methods

    #region GetFillList

    private void FillList()
    {
        SpinnerLoaderService.ShowSpinnerLoader(Js);
        // llenar listado de meses
        var cultura = CultureInfo.CreateSpecificCulture("es-CO");
        var qry = from m in cultura.DateTimeFormat.MonthNames
                  select cultura.TextInfo.ToTitleCase(m);

        var item = 1;
        foreach (var mes in qry)
        {
            Mounth.Add(new DateDtoResponse { nombre = mes, valor = item });
            item++;
        }

        // llenar listado de dias
        for (int i = 0; i < 31; i++)
        {
            Days.Add(new DateDtoResponse { nombre = $"{(i + 1)}", valor = (i + 1) });
        }

        // llenar listado de años
        for (int i = 2021; i < DateTime.Now.Year + 1; i++)
        {
            Year.Add(new DateDtoResponse { nombre = $"{(i + 1)}", valor = (i + 1) });
        }
        SpinnerLoaderService.HideSpinnerLoader(Js);
    }

    #endregion GetFillList

    #region DataCards

    private async Task GetDataCards()
    {
        try
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            Data = new();


            var responseApi = await HttpClient.PostAsJsonAsync(UriFilterCardsDocuemnts, filterDocumentManagamentById);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<DataCardDtoResponse>> ();
            
            //var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<DataCardDtoResponse>>("documentmanagement/Document/ByAssingUserId");
            
            DataCards = deserializeResponse.Data;

            if (DataCards != null)
            {
                int total = DataCards.withoutProcessing + DataCards.inProgress + DataCards.SuccessfullManagement;
                double porcent = (Convert.ToDouble(DataCards.withoutProcessing) * 100) / total;
                double porcent2 = (Convert.ToDouble(DataCards.inProgress) * 100) / total;
                double porcent3 = (Convert.ToDouble(DataCards.SuccessfullManagement) * 100) / total;
                double porcent4 = (0 * 100) / total;
                Total = total;
                EnpP = porcent != 0 ? porcent.ToString("N2") + "%" : "0";
                EtrP = porcent2 != 0 ? porcent2.ToString("N2") + "%" : "0";
                GexP = porcent3 != 0 ? porcent3.ToString("N2") + "%" : "0";
                CopP = porcent4 != 0 ? porcent4.ToString("N2") + "%" : "0";
                CopP = "";

                DataCardDtoRequest dato1 = new DataCardDtoRequest()
                {
                    Category = Translation[DataCards.InProgressWord],
                    Value = porcent2,
                    color = "#EAD519"
                };

                DataCardDtoRequest dato2 = new DataCardDtoRequest()
                {
                    Category = Translation[DataCards.WithoutProcessingWord],
                    Value = porcent,
                    color = "#AB2222"
                };

                DataCardDtoRequest dato3 = new DataCardDtoRequest()
                {
                    Category = Translation[DataCards.SuccessfullManagementWord],
                    Value = porcent3,
                    color = "#82A738"
                };

                Data.Add(dato1);
                Data.Add(dato2);
                Data.Add(dato3);

                Enp = DataCards.withoutProcessing.ToString();
                Etr = DataCards.inProgress.ToString();
                Gex = DataCards.SuccessfullManagement.ToString();
                Cop = DataCards.copies.ToString();
            }
            else
            {
                Enp = "0";
                Etr = "0";
                Gex = "0";
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
        }
    }

    #endregion DataCards

    #region Dropdownlist

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
                FormatBG = deserializeResponse.Data;
                Meta = deserializeResponse.Meta ?? new();
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        catch (Exception ex)
        {
            SpinnerLoaderService.HideSpinnerLoader(Js);
            Console.WriteLine($"Error al obtener la prioridad: {ex.Message}");
        }
    }

    private async Task GetNotiComuni()
    {
        try
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            HttpClient?.DefaultRequestHeaders.Add("paramCode", "CL");
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            if (deserializeResponse!.Succeeded)
            {
                FormatCL = deserializeResponse.Data;
                Meta = deserializeResponse.Meta ?? new();
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        catch (Exception ex)
        {
            SpinnerLoaderService.HideSpinnerLoader(Js);
            Console.WriteLine($"Error al obtener la prioridad: {ex.Message}");
        }
    }

    private async Task cascadingMes()
    {
        enabledMes = true;
    }

    private async Task cascadingDia()
    {
        enabledDia = true;
    }

    #endregion Dropdownlist

    #region Data Grilla

    private async Task GetData(DocumentStatusEnum status)
    {
        try
        {
            statusPage = status;
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            istCopy = false;
            GeneralList = new();
            Meta = new();
            activateProcedure = false;
            if (status == DocumentStatusEnum.WithoutProcessingWord)
            {
                Estado = Translation[DocumentStatusEnum.WithoutProcessingWord.GetDisplayValue()];
                StatusCode = DocumentStatusEnum.WithoutProcessingWord.GetCoreValue();
                activateProcedure = true;
            }
            if (status == DocumentStatusEnum.InProgressWord)
            {
                Estado = Translation[DocumentStatusEnum.InProgressWord.GetDisplayValue()];
                StatusCode = DocumentStatusEnum.InProgressWord.GetCoreValue();
            }
            if (status == DocumentStatusEnum.SuccessfullManagementWord)
            {
                Estado = Translation[DocumentStatusEnum.SuccessfullManagementWord.GetDisplayValue()];

                StatusCode = DocumentStatusEnum.SuccessfullManagementWord.GetCoreValue();
            }

            FilterDtoRequest = new()
            {
                FlowStateCode = StatusCode,
                ClassCode = classCodeValue,
                ControlId = string.IsNullOrWhiteSpace(idControl) ? 0 : Convert.ToInt32(idControl),
                FilingCode = numRadicado,
                PriorityCode = !string.IsNullOrEmpty(prioridadValue) ? prioridadValue : "",
                Year = yearValue,
                Month = mounthValue,
                Days = daysValue,
                DueDate = dueDateValue,
                ItsCopy = istCopy,
            };

            if (status == DocumentStatusEnum.Copy)
            {
                istCopy = true;
                FilterDtoRequest = new()
                {
                    ItsCopy = istCopy
                };
            }
            FilterDtoRequest.OrderAsc = false;
            FilterDtoRequest.OrderBy = "CreateDate";
            FilterDtoRequest.IsAnotherUser = filterDocumentManagamentById.IsAnotherUser;
            FilterDtoRequest.AssingUserId = filterDocumentManagamentById.AssingUserId;
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            var responseApi = await HttpClient.PostAsJsonAsync(UriFilterDocuments, FilterDtoRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<DocumentDtoResponse>>>();
            if (!deserializeResponse.Succeeded)
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["CouldntFindInfByParameters"], true, Translation["Accept"], buttonTextCancel: "");
                GeneralList = new();
                paginationComponent.ResetPagination(paginationInfo);
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            else
            {
                GeneralList = deserializeResponse.Data.Data;
                GeneralList = GeneralList
                .DistinctBy(data => data.ControlId)
                .ToList();
                paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                paginationComponent.ResetPagination(paginationInfo);
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        catch (Exception ex)
        {
            SpinnerLoaderService.HideSpinnerLoader(Js);
            Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
        }
    }

    private void HandlePaginationGrid(List<DocumentDtoResponse> newDataList)
    {
        newDataList.ForEach(data => data.Selected = SelectedControlIds.Contains(data.ControlId));

        GeneralList = newDataList
        .DistinctBy(data => data.ControlId)
        .ToList();
    }

    #endregion Data Grilla

    #region Metodos Generales

    public async Task Search()
    {
        if (!string.IsNullOrEmpty(idControl) || !string.IsNullOrEmpty(numRadicado) ||
            !string.IsNullOrEmpty(classCodeValue) || !string.IsNullOrEmpty(prioridadValue) ||
            mounthValue > 0 || yearValue > 0 || daysValue > 0)
        {
            await GetData(statusPage);
        }
        else
        {
            notificationModal.UpdateModal(ModalType.Warning, Translation["EnterParameterSearch"], true);
        }
    }

    private async Task Refresh()
    {
        idControl = "";
        numRadicado = "";
        classCodeValue = "";
        prioridadValue = "";
        yearValue = 0;
        mounthValue = 0;
        daysValue = 0;
        await GetData(DocumentStatusEnum.WithoutProcessingWord);
    }

    #endregion Metodos Generales

    #region Modals

    private async Task ShowModalManagementProcedure(DocumentDtoResponse model)
    {
        await managementOfProcedures.UpdateModalStatus(true, model.ClassCode);
        managementOfProcedures.ResetFormAsync();
        await managementOfProcedures.ManagementOfProcedures(model);
    }

    private async Task ShowModalGeneralInformation(DocumentDtoResponse model)
    {
        await generalInformation.GeneralInformation(model.ControlId, model.ClassCode);
        generalInformation.UpdateModalStatus(true);
    }

    #region GetFile

    private async Task<FileDtoResponse?> GetFile(int? id)
    {
        SpinnerLoaderService.ShowSpinnerLoader(Js);
        try
        {
            HttpClient?.DefaultRequestHeaders.Remove("FileId");
            HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
            HttpClient?.DefaultRequestHeaders.Remove("FileId");
            if (deserializeResponse!.Succeeded)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                return deserializeResponse.Data!;
            }
            else
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                return null;
            }
        }
        catch
        {
            SpinnerLoaderService.HideSpinnerLoader(Js);
            return null;
        }
    }

    private async Task DownloadPdfFile(DocumentDtoResponse model)
    {
        if (model.FileId != 0 && model.FileId != null)
        {
            FileDtoResponse objFile = await GetFile(model.FileId);
            string nameFile = "";
            if (!objFile!.FileName.Contains($".{objFile.FileExt}"))
            {
                nameFile = $"{objFile!.FileName}.{objFile.FileExt}";
            }
            else
            {
                nameFile = objFile!.FileName;
            }

            bool download = await Js.InvokeAsync<bool>("DescargarArchivoBase64", nameFile, Convert.ToBase64String(objFile.DataFile));
            if (download)
            {
                notificationModal.UpdateModal(ModalType.Success, Translation["FileDownloadSuccessfully"], true, Translation["Accept"], title: Translation["DownloadSuccessfullyMessage"]);
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["DownloadFileErrorMessage"], true);
            }
        }
        else
        {
            notificationModal.UpdateModal(ModalType.Error, Translation["DownloadFileErrorMessage"], true);
        }
    }

    #endregion GetFile

    public async Task ShowModalAttachments(DocumentDtoResponse model)
    {
        controlId = model.ControlId;
        await attachmentTrayModal.UpdateAttachmentDocument(model.ControlId);
        attachmentTrayModal.UpdateModalStatus(true);
    }

    private async Task OpenModalManagementProceduresTM()
    {
        isMasive = true;
        if (string.IsNullOrEmpty(classCodeValue))
        {
            notificationModal.UpdateModal(ModalType.Warning, Translation["ChooseCommunication"], true, buttonTextCancel: "");
        }
        else
        {
            if (managementTrayToReturn.Count == 0)
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["ChooseDocumments"], true, buttonTextCancel: "");
            }
            else
            {
                managementOfProcedures.PrepareModal(SelectedControlIds);
                await managementOfProcedures.UpdateModalStatus(true, classCodeValue);
            }
        }
    }

    #endregion Modals

    #region HandleMethods

    private async Task HandleModalNotiClose(ModalClosedEventArgs args)
    {
    }

    private async Task HandleRefreshData(bool refresh)
    {
        await GetDataCards();
        await GetData(DocumentStatusEnum.WithoutProcessingWord);
        classCodeValue = "";
        isMasive = false;
    }

    #endregion HandleMethods

    #region changeManagementTray

    public void ChangeStateManagementTray(DocumentDtoResponse model)
    {
        if (!model.Selected && SelectedControlIds.Contains(model.ControlId))
        {
            SelectedControlIds.Remove(model.ControlId);
            managementTrayToReturn.RemoveAll(x => x.ControlId == model.ControlId);
        }
        else if (model.Selected && !SelectedControlIds.Contains(model.ControlId))
        {
            SelectedControlIds.Add(model.ControlId);
            managementTrayToReturn.Add(model);
        }
    }

    #endregion changeManagementTray

    #region ValueChangedMethods

    private void OnValueChangedRecordClosed(string newValue)
    {
        ResponseRecordClosed = newValue;
        if (newValue == Translation["Yes"])
        {
            dueDateValue = true;
        }
        else if (newValue == Translation["No"])
        {
            dueDateValue = true;
        }
    }

    private async Task ChangeClassCodeAction(string value)
    {
        classCodeValue = value;
        if (isMasive)
        {
            SelectedControlIds = new();
            managementTrayToReturn = new();
            await GetData(DocumentStatusEnum.WithoutProcessingWord);
        }
    }

    #endregion ValueChangedMethods

    #endregion Methods
}