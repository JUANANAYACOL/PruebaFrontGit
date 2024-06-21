using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.ClipboardSource.SpreadsheetML;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using Telerik.Windows.Documents;

namespace Control.Endeavour.FrontEnd.Pages.Administration.AdministracionTRD
{
    public partial class CcdPage
    {
        #region Variables

        #region Inject

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject



        #region Modals
        private NotificationsComponentModal notificationModal { get; set; } = new();

        #endregion Modals



        #region Models

        public TelerikPdfViewer PdfViewerRef { get; set; } = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        public string FileBase64Data;
        private string displayTableHide = "";
        private string displayPdfViewerHide = "";
        private string displayTable = "col-lg-4";
        private string displayDocument = "col-lg-8";
        private string docIcon = "fa-solid fa-eye";
        private string tableIcon = "fa-solid fa-eye";
        private string tableText = "ShowInformation";
        private string docText = "ShowDocument";

        #endregion Environments(String)

        #region Environments(Numeric)

        private int IdDocumental { get; set; } = 0;
        private int idAdUnit { get; set; } = 0;
        private int IdproOffice { get; set; } = 0;

        #endregion Environments(Numeric)



        #region Environments(Bool)

        private bool administrativeUnitEnable { get; set; } = false;
        private bool productionOfficeEnable { get; set; } = false;
        private bool saveIsDisable { get; set; } = true;

        private bool validDisplayDocument = false;
        private bool validDisplayTable = false;
        private bool showTableActive = false;
        private bool showDocumentActive = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<DocumentalVersionDtoResponse> documentalVersionsList = new();
        private List<AdministrativeUnitsDtoResponse> administrativeUnitsList { get; set; } = new();
        private List<ProductionOfficesDtoResponse> productionOfficesList = new();
        private List<FileInfoData> listNewAdminActs { get; set; } = new();

        private byte[]? SelectedPDF { get; set; }
        private string[] AllowedExtensions { get; set; } = { ".pdf" };
        private byte[]? ccdReport { get; set; }

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetDocumentalVersions();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #region GenerateReport

        public async Task GenerateReport()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                var request = new CcdDtoRequest
                {
                    DocumentalVersionId = IdDocumental,
                    AdministrativeUnitId = idAdUnit,
                    ProductionOfficeId = IdproOffice
                };

                var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/Ccd/CreateCcd", request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();
                if (deserializeResponse!.Succeeded)
                {
                    ccdReport = Convert.FromBase64String(deserializeResponse.Data!);
                    SelectedPDF = ccdReport;

                    notificationModal.UpdateModal(ModalType.Success, Translation["CcdGenerated"], true, Translation["Accept"]);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["ErrorProcessingRequest"], true, Translation["Accept"]);
                }

                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ErrorProcessingRequest"], true, Translation["Accept"]);
            }
        }

        #endregion GenerateReport

        #endregion HandleMethods

        #region OthersMethods

        #region GetDataMethods

        #region GetDocumentalVersions

        private async Task GetDocumentalVersions()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                var responseApi = await HttpClient.GetAsync("paramstrd/DocumentalVersions/ByDocumentalVersions");
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<DocumentalVersionDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    string toCurrent = Translation["ToCurrent"];
                    string documentaryVersion = Translation["DocumentaryVersion"];
                    documentalVersionsList = deserializeResponse.Data;
                    documentalVersionsList.ForEach(x =>
                    {
                        x.Code = $"{((x.EndDate == null || x.EndDate.Value > DateTime.Now) ? $"{x.StartDate!.Value.Year} {toCurrent} - {documentaryVersion} {x.Code}" : $"{x.StartDate!.Value.Year} a {x.EndDate.Value.Year} - {documentaryVersion} {x.Code} {x.VersionType}")}";
                    });

                    documentalVersionsList = documentalVersionsList.OrderBy(x => x.StartDate).Reverse().ToList();
                }
                else
                {
                    documentalVersionsList = new();
                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetDocumentalVersions

        #region GetAdministrativeUnits

        private async Task GetAdministrativeUnits(int value)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                IdDocumental = value;
                idAdUnit = 0;
                administrativeUnitEnable = false;
                productionOfficeEnable = false;
                administrativeUnitsList = new();
                productionOfficesList = new();

                if (IdDocumental != 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
                    HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", $"{IdDocumental}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
                    HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");

                    if (deserializeResponse!.Succeeded && deserializeResponse.Data?.Count != 0)
                    {
                        administrativeUnitsList = deserializeResponse.Data ?? new();
                        administrativeUnitEnable = true;
                    }
                    else
                    {
                        administrativeUnitsList = new();
                        productionOfficesList = new();
                        administrativeUnitEnable = false;
                        productionOfficeEnable = false;
                    }

                    ccdReport = null;
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetAdministrativeUnits

        #region GetProductionOffices

        private async Task GetProductionOffices(int value)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                idAdUnit = value;
                IdproOffice = 0;
                productionOfficeEnable = false;
                productionOfficesList = new();

                if (idAdUnit != 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                    HttpClient?.DefaultRequestHeaders.Add("AdministrativeUnitId", $"{idAdUnit}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>("paramstrd/ProductionOffice/ByProductionOffices");
                    HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");

                    if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                    {
                        productionOfficesList = deserializeResponse.Data ?? new();
                        productionOfficeEnable = true;
                    }
                    else
                    {
                        productionOfficesList = new();
                        productionOfficeEnable = false;
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetProductionOffices

        #endregion GetDataMethods

        #region TableAndPdf

        #region HandleVisibilityButtons

        #region HandleVisibilityTable

        private void ShowTable()
        {
            validDisplayTable = !validDisplayTable;

            if (validDisplayTable && displayDocument.Equals("col-lg-6") || !validDisplayTable && displayDocument.Equals("col-lg-12") || validDisplayTable && displayDocument.Equals("col-lg-12"))
            {
                displayTable = "col-lg-12";
                displayDocument = "d-none";
                tableIcon = "fa-solid fa-eye-slash";
                tableText = Translation["HideInformation"];
                docIcon = "fa-solid fa-eye";
                docText = Translation["ShowDocument"];
            }
            else if (!validDisplayTable && displayDocument.Equals("col-lg-6"))
            {
                docText = Translation["ShowDocument"];
                displayTable = "col-lg-12";
                displayDocument = "d-none";
                tableIcon = "fa-solid fa-eye-slash";
                tableText = Translation["HideInformation"];
                docIcon = "fa-solid fa-eye";
            }
            else
            {
                tableText = Translation["ShowInformation"];
                displayTable = "col-lg-6";
                displayDocument = "col-lg-6";
                tableIcon = "fa-solid fa-eye";
            }
        }

        #endregion HandleVisibilityTable

        #region HandleVisibilityPdf

        private void ShowDocument()
        {
            validDisplayDocument = !validDisplayDocument;

            if (validDisplayDocument && displayTable.Equals("col-lg-6") || !validDisplayDocument && displayTable.Equals("col-lg-12") || validDisplayDocument && displayTable.Equals("col-lg-12"))
            {
                displayTable = "d-none";
                displayDocument = "col-lg-12";
                docIcon = "fa-solid fa-eye-slash";
                docText = Translation["HideInformation"];
                tableIcon = "fa-solid fa-eye";
                tableText = "Mostrar información";
            }
            else if (!validDisplayDocument && displayTable.Equals("col-lg-6"))
            {
                tableText = Translation["ShowInformation"];
                docText = Translation["HideInformation"];
                displayTable = "d-none";
                displayDocument = "col-lg-12";
                docIcon = "fa-solid fa-eye-slash";
                tableIcon = "fa-solid fa-eye";
            }
            else
            {
                docText = Translation["ShowDocument"];
                displayTable = "col-lg-6";
                displayDocument = "col-lg-6";
                docIcon = "fa-solid fa-eye";
            }
        }

        #endregion HandleVisibilityPdf

        #endregion HandleVisibilityButtons

        #endregion TableAndPdf

        #region Filters

        #region ResetFilter

        public void ResetFiltersAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (IdDocumental != 0 || idAdUnit != 0 || IdproOffice != 0)
            {
                IdDocumental = 0;
                idAdUnit = 0;
                IdproOffice = 0;
                administrativeUnitEnable = false;
                productionOfficeEnable = false;
                administrativeUnitsList = new();
                productionOfficesList = new();
                ccdReport = null;
                SelectedPDF = null;
                //dataChargue = false;
                //await GetSeries();
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["AtLeastOne4Clean"], true, Translation["Accept"]);
            }
            EnableSaveButton();
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ResetFilter

        #endregion Filters

        #region EnableSaveButton

        public void EnableSaveButton()
        {
            if (IdDocumental <= 0)
            {
                saveIsDisable = true;
            }
            else
            {
                saveIsDisable = false;
            }

            StateHasChanged();
        }

        #endregion EnableSaveButton

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            try
            {
                /*if (args.IsAccepted)
                {
                    ResetFiltersAsync();
                }*/
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
        }

        #endregion HandleModalNotiClose

        #region DownloadFile

        private async Task DownloadFile()
        {
            var Data = Convert.ToBase64String(ccdReport);

            bool download = await Js.InvokeAsync<bool>("DescargarArchivoBase64", "CcdReport.xlsx", Data);
            if (download)
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["FileDownloadSuccessfully"], true, Translation["Accept"], "");
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["DownloadFileErrorMessage"], true, Translation["Accept"], "");
            }
        }

        #endregion DownloadFile

        #endregion OthersMethods

        #endregion Methods
    }
}