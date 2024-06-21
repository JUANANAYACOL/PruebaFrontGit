using Control.Endeavour.FrontEnd.Components.Components.Modals;
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

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray
{
    public partial class ValidateDocumentGeneralInfoModal
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

        #endregion

        #region Components

        //public TelerikPdfViewer PdfViewerRef = new();

        #endregion

        #region Modals

        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Parameters


        #endregion

        #region Models

        private GeneralInformationDtoResponse? document = new();

        #endregion

        #region Environments

        #region Environments(String)

        private string DisplayPdfViewerClass = "d-none";
        private string DisplayTableClass = string.Empty;
        private string ColTableData = "col-md-12";
        public string FileBase64Data = string.Empty;
        private string btnExpandPdfText = string.Empty;
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
        private bool isExpandPdfView = false;
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
        private void HandleModalClosed(bool status)
        {
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

                    await GetImagePdf();
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

        public async Task GeneralInformation(int Idcontrol)
        {
            controlId = Idcontrol;
            await GetGeneralInfo();
            StateHasChanged();
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
            {Translation["Filing"], document?.DocumentInformation?.ExternalFiling ?? ""},
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
            {Translation["DocumentDate"], $"{document?.DocumentInformation?.DocDate?.Date.ToString("dd/MM/yyyy") ?? "N/A"}"},
            {Translation["FilingUser"], document?.DocumentInformation ?.CreateUser ?? "N/A"},
            {Translation["FilingUserBranch"], document?.DocumentInformation?.BrachOfficeUser ?? "N/A"},
            {Translation["Justification"], "-"},
            {Translation["UserDigitized"], document?.DocumentInformation ?.CreateUser ?? "N/A"},
            {Translation["DateDigitized"], $"{document?.DocumentInformation?.CreateDate!:dd/MM/yyyy HH:mm:ss}" ?? "N/A"},
            {Translation["ClosingComment"], document?.DocumentInformation?.CommentaryUserClosedProcess ?? "N/A"},
            {Translation["UserClosed"], document?.DocumentInformation?.UserClosedProcess ?? "N/A"},
            {Translation["JustificationReactivation"], "N/A"},
        };
        }

        

        private void ShowPdfViewer()
        {
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
}
