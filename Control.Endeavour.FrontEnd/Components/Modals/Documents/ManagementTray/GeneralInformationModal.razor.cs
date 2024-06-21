using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.DocumentaryTask;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray;

public partial class GeneralInformationModal
{

    #region Variables

    #region Inject 
    [Inject]
    private IJSRuntime Js { get; set; }
    [Inject]
    private HttpClient? HttpClient { get; set; }
    /*[Inject]
    private EventAggregatorService? EventAggregator { get; set; }*/
    [Inject]
    private IStringLocalizer<Translation>? Translation { get; set; }
    [Parameter]
    public EventCallback<bool> OnModalClosed { get; set; }

    #endregion

    #region Components

    //public TelerikPdfViewer PdfViewerRef = new();

    #endregion

    #region Modals


    private NotificationsComponentModal notificationModal = new();
    private WorKFlowModal worKFlowModal = new();
    private UpdateTypologyModal updateTypologyModal = new();
    private MetaDataDocumentModal metaDataDocumentModal = new();
    private CopiesDocumentModal copiesDocumentModal = new();
    private RelatedDocumentsModal relatedDocumentsModal = new();

    #endregion

    #region Parameters

    
    private DocTaskWorkFlowModal docTaskWorkFlowModal = new();

    #endregion Components       

    #region Models
    
    private GeneralInformationDtoResponse? document = new();
    #endregion Models



    #region Parameters

    [Parameter] public bool docTask { get; set; } = true;

    #endregion



    #region Environments

    #region Environments(String)

    private string DisplayPdfViewerClass = "d-none";
    private string DisplayTableClass = string.Empty;
    private string ColTableData = "col-md-12";
    public string FileBase64Data = string.Empty;
    private string DropdownMenuRecords = "dropdown-menu-show-no";
    private string DropdownMenuLoans = "dropdown-menu-show-no";
    private string DropdownMenuDocuments = "dropdown-menu-show-no";
    private string classCode = string.Empty;
    private string btnExpandPdfText = "";
    private string btnExpandPdfIcon = "fa-solid fa-maximize";
    private string pdfViewerClass = "col-md-6";
    
    #endregion

    #region Environments(Numeric)

    public int controlId = 0;


    #endregion

    #region Environments(DateTime)

    #endregion

    #region Environments(Bool)

    private bool modalStatus = false;
    private bool isDropdownOpen = false;
    private bool isDropdownLoans = false;
    private bool isExpandPdfView = false;
    private bool isDropdownMenuDocuments = false;
    private bool isImagePdf = true;
    private bool btnsVisiblePdfActions = false;

    #endregion

    #region Environments(List & Dictionary)

    private Dictionary<string, string> dataInfoDocument = new();
    private Dictionary<string, string> dataInfoTrd = new();
    public byte[] FileData { get; set; } = Array.Empty<byte>();

    #endregion

    #endregion

    #endregion


    #region OnInitializedAsync
    protected override async Task OnInitializedAsync()
    {
        try
        {
            btnExpandPdfText = Translation["ExpandPDF"];
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            modalStatus = false;
           
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al realizar la initialización: {ex.Message}");
        }
    }
    #endregion 



    #region Methods

    #region HandleMethods

    private async Task HandleLanguageChanged()
    {
        StateHasChanged();
    }
    #region HandleModalClose
    private async Task HandleModalClosedAsync(bool status)
    {
        await OnModalClosed.InvokeAsync(false);
        modalStatus = status;
        isExpandPdfView = false;
        isImagePdf = true;
        DisplayTableClass = string.Empty;
        pdfViewerClass = "col-md-6";
        btnsVisiblePdfActions = false;
        StateHasChanged();
    }

    private void HandleModalNotiClose(ModalClosedEventArgs args)
    {

    }

    #endregion
    #endregion

    #region OthersMethods

    #region GetData
    public async Task GetGeneralInfo()
    {
        try
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            HidePdfViewer();
            if (controlId > 0)
            {
                HttpClient?.DefaultRequestHeaders.Remove("controlId");
                HttpClient?.DefaultRequestHeaders.Add("controlId", $"{controlId}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<GeneralInformationDtoResponse>>("documents/Document/GeneralInfo").ConfigureAwait(false);
                HttpClient?.DefaultRequestHeaders.Remove("controlId");
                if (deserializeResponse!.Succeeded)
                {
                    document = deserializeResponse.Data;
                }            
            }

            InsertData();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        catch (Exception ex)
        {
            notificationModal.UpdateModal(ModalType.Warning, Translation["ErrorProcessingRequest"], true, Translation["Accept"], Translation["Cancel"]);
            Console.WriteLine(Translation["Accept"] + $": {ex.Message}");
        }
    }
    public async Task GetImagePdf()
    {
        try
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            FileBase64Data = "";
            if (controlId > 0)
            {
                HttpClient?.DefaultRequestHeaders.Remove("controlId");
                HttpClient?.DefaultRequestHeaders.Add("controlId", $"{controlId}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<ImagePdfDtoResponse>>("documents/Document/GetImageDocument").ConfigureAwait(false);
                HttpClient?.DefaultRequestHeaders.Remove("controlId");
                if (deserializeResponse!.Succeeded)
                {
                    FileBase64Data = deserializeResponse.Data.DataFile;
                    FileData = Convert.FromBase64String(FileBase64Data);
                }
                else
                {
                    if (!deserializeResponse!.Succeeded && deserializeResponse.Data.DataFile.Equals("No hay imagen subida"))
                    {
                        isImagePdf = false;
                    }
                }
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        catch (Exception ex)
        {
            notificationModal.UpdateModal(ModalType.Warning, Translation["ErrorGettingPdfImage"], true, Translation["Accept"], Translation["Cancel"]);

        }
    }


    #endregion

    #region OpenModal


    public async Task GeneralInformation(int Idcontrol, string ClassCode = "")
    {
        controlId = Idcontrol;
        classCode = ClassCode;
        await GetGeneralInfo();
        StateHasChanged();
    }
    async Task ShowWorKFlow()
    {
        if (controlId > 0)
        {
            worKFlowModal.UpdateModalStatus(true);
            await worKFlowModal.WorKFlowAsync(controlId);
        }
    }
    private async Task ShowModalUpdateTypology()
    {
        
        await updateTypologyModal.UpdateClassificationHistory(controlId,classCode);
        updateTypologyModal.UpdateModalStatus(true);
    }
    private async Task ShowModalMetaDataDocument()
    {
        await metaDataDocumentModal.GetMetaDataDocumentHistory(controlId, document?.DocumentInformation?.ExternalFiling, classCode);
        metaDataDocumentModal.UpdateModalStatus(true);
    }
    private async Task ShowModalDocumentCopies()
    {
        await copiesDocumentModal.LoadDocumentcopies(controlId, document?.DocumentInformation?.ExternalFiling);
        copiesDocumentModal.UpdateModalStatus(true);
    }

    private async Task ShowModalDocumentRelated()
    {
        await relatedDocumentsModal.LoadDocumentRelated(controlId, document?.DocumentInformation?.ExternalFiling);
        relatedDocumentsModal.UpdateModalStatus(true);
    }

    #endregion

    #region GeneralMethods

    public void UpdateModalStatus(bool newValue)
    {
        modalStatus = newValue;
        StateHasChanged();
    }

    #region ActionMethods

    private void InsertData()
    {
        dataInfoTrd = new Dictionary<string, string>
        {
            {Translation["DocumentClassification"], document?.DocumentClasification?.DocumentalVersionName ?? ""},
            {Translation["AdministrativeUnit"], document?.DocumentClasification?.AdministrativeUnitName ?? ""},
            {Translation["ProductionOffice"], document?.DocumentClasification?.ProductionOfficeName ?? ""},
            {Translation["Serie"], document?.DocumentClasification?.SeriesName ?? ""},
            {Translation["SubSerie"], document?.DocumentClasification?.SubseriesName ?? ""},
            {Translation["DocumentaryTypology"], document?.DocumentClasification?.DocumentaryTypologiesName ?? ""},
            {Translation["CorrespondenceType"], document?.DocumentClasification?.CorrespondenceType ?? ""}

        };
        dataInfoDocument = new Dictionary<string, string>
        {
            {Translation["Class"], $"{document?.DocumentInformation?.ClassCode  ?? ""}"},
            {Translation["ControlID"], $"{document?.DocumentInformation?.ControlId ?? 0}"},
            {Translation["FilingUpper"], document?.DocumentInformation?.ExternalFiling ?? ""},
            {Translation["ExternalFiling"], $"{document?.DocumentInformation?.DocumentId ?? 0}"},
            {Translation["Gantt_Year"], document?.DocumentInformation?.Year ?? ""},
            {Translation["Priority"], document?.DocumentInformation?.Priority ?? ""},
            {Translation["GuideNumber"] +" / "+Translation["ZIPCode"], document?.DocumentInformation?.NRoGuia ?? "N/A"},
            {Translation["ReceptionMeans"] +" / "+Translation["Shipment"], document?.DocumentInformation?.ReceptionCode?? ""},
            {Translation["Subject"], document?.DocumentInformation?.DocDescription ?? ""},
            {Translation["Notification"], document?.DocumentInformation?.Notificacion ?? ""},
            {Translation["Signatory"], document?.DocumentInformation?.Firmantes ?? ""},
            {Translation["Recipient(s)"], document?.DocumentInformation?.Destinatarios ?? ""},
            {Translation["File(s)Id"], ""},
            {Translation["DueDate"], $"{document?.DocumentInformation?.DueDate!:dd/MM/yyyy HH:mm:ss}" ?? "N/A"},
            {Translation["Days"] +" / "+Translation["HoursDueDate"], document?.DocumentInformation?.DaysHoursDueDate ?? "N/A"},
            {Translation["ActiveInSystem"], document?.DocumentInformation?.Active == true ? "SI" : "NO"},
            {Translation["FilingReceived"], $"{document?.DocumentInformation?.CreateDate!:dd/MM/yyyy HH:mm:ss}" ?? "N/A"},
            {Translation["DocumentDate"], $"{document ?.DocumentInformation ?.DocDate !:dd/MM/yyyy HH:mm:ss}" ?? "N/A"},
            {Translation["FilingUser"], document?.DocumentInformation ?.CreateUser ?? "N/A"},
            {Translation["FilingUserBranch"], document?.DocumentInformation?.BrachOfficeUser ?? "N/A"},
            {Translation["Justification"], "-"},
            {Translation["UserDigitized"], document?.DocumentInformation ?.UserDigitalizedName ?? "N/A"},
            {Translation["DateDigitized"], $"{document ?.DocumentInformation ?.DigitalizedDate !:dd/MM/yyyy HH:mm:ss}" ?? "N/A"},
            {Translation["ClosingComment"], document?.DocumentInformation?.CommentaryUserClosedProcess ?? "N/A"},
            {Translation["UserClosed"], document?.DocumentInformation?.UserClosedProcess ?? "N/A"},
            {Translation["JustificationReactivation"], "N/A"},
        };
    }

    void ToggleDropdown(ref bool dropDownOpenRef, ref string classdropdown)
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

    private async Task ShowPdfViewerAsync()
    {
        await GetImagePdf();
        if (isImagePdf)
        {
            DisplayPdfViewerClass = "";
            ColTableData = "col-md-6";
            btnsVisiblePdfActions = true;
        }
        else
        {
            notificationModal.UpdateModal(ModalType.Information, Translation["NoImage4Document"], true, buttonTextCancel: string.Empty);
            btnsVisiblePdfActions = false;
        }

    }
    private void ActionsPdfDisplayView()
    {
        switch (isExpandPdfView)
        {
            case true:
                btnExpandPdfText = Translation["ExpandPDF"];
                btnExpandPdfIcon = "fa-solid fa-maximize";
                isExpandPdfView = false;
                DisplayTableClass = string.Empty;
                pdfViewerClass = "col-md-6";
                break;

            case false:
                btnExpandPdfText = Translation["MinimizePDF"];
                btnExpandPdfIcon = "fa-solid fa-minimize";
                isExpandPdfView = true;

                DisplayTableClass = "d-none";
                pdfViewerClass = "col-md-12";

                break;
        }
    }
    private void HidePdfViewer()
    {
        DisplayPdfViewerClass = "d-none";
        ColTableData = "col-md-12";
        btnsVisiblePdfActions = false;
    }

    #endregion

    #endregion

    #endregion

    #endregion

}
