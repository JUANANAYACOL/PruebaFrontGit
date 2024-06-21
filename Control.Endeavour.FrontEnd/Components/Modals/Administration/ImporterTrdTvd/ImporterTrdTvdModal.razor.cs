using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ImporterTrdTvd.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ImporterTrdTvd.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.ImporterTrdTvd
{
    public partial class ImporterTrdTvdModal : ComponentBase
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

        #region Modals

        private NotificationsComponentModal notificationModal = new();

        #endregion Modals

        #region Parameters

        [Parameter]
        public EventCallback<bool> OnChangeData { get; set; }

        [Parameter]
        public EventCallback<bool> OnStatusChanged { get; set; }

        [Parameter]
        public EventCallback<ImporterDtoResponse> OnInfoChange { get; set; }

        #endregion Parameters

        #region Models

        private ImporterDtoRequest importDtoRequest = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string description { get; set; } = "";
        private string textDocumentalVersion { get; set; } = "SelectDocumentaryVersion";
        private string dataError = "";

        #endregion Environments(String)

        #region Environments(Numeric)

        private int FileSize = 10;
        private decimal characterCounter { get; set; } = 0;
        private int idDocumentalVersion { get; set; }

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool modalStatus { get; set; } = false;
        private bool enableDownloadReport = false;
        private bool saveIsDisable { get; set; } = true;
        private bool readyToCheck { get; set; } = true;
        private bool readyToImport { get; set; } = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<FileInfoData> template { get; set; } = new();
        private List<DocumentalVersionDtoResponse> documentalVersionList = new();
        private string[] AllowedExtensions { get; set; } = { ".xlsx" };

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

        #endregion HandleMethods

        #region OthersMethods

        #region GetDocumentalVersions

        private async Task GetDocumentalVersions()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<DocumentalVersionDtoResponse>>>("paramstrd/DocumentalVersions/ByDocumentalVersions");
                if (deserializeResponse!.Succeeded)
                {
                    string toCurrent = Translation["ToCurrent"];
                    string documentaryVersion = Translation["DocumentaryVersion"];
                    documentalVersionList = deserializeResponse.Data ?? new();
                    documentalVersionList.ForEach(x =>
                    {
                        x.Code = $"{((x.EndDate == null || x.EndDate.Value > DateTime.Now) ? $"{x.StartDate!.Value.Year} {toCurrent} - {documentaryVersion} {x.Code}" : $"{x.StartDate!.Value.Year} a {x.EndDate.Value.Year} - {documentaryVersion} {x.Code} {x.VersionType}")}";
                    });
                    documentalVersionList = documentalVersionList.OrderBy(x => x.StartDate).Reverse().ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las versiones documentales: {ex.Message}");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetDocumentalVersions

        #region Modal

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region HandleModalNotiClose

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }
        }

        #endregion HandleModalNotiClose

        #region HandleModalClosed

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            ResetFormAsync();
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #endregion Modal

        #region ResetFormAsync

        public void ResetFormAsync()
        {
            template = new();
            description = "";
            idDocumentalVersion = 0;
            enableDownloadReport = false;
            dataError = string.Empty;
            characterCounter = 0;
            readyToCheck = true;
            StateHasChanged();
        }

        #endregion ResetFormAsync

        #region CountCharacters

        private void CountCharacters(ChangeEventArgs e)
        {
            string value = e.Value.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                characterCounter = value.Length;
            }
            else
            {
                characterCounter = 0;
            }
        }

        #endregion CountCharacters

        #region CreateImport

        private async Task CreateImport()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                importDtoRequest.DocumentalVersionId = idDocumentalVersion;
                importDtoRequest.DescriptionHistory = description;
                importDtoRequest.FileName = template[0].Name!;
                importDtoRequest.FileExt = template[0].Extension!.Replace(".", "");
                importDtoRequest.DataFile = template[0].Base64Data!;

                var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/Importer/CreateImport", importDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ImporterDtoResponse>>();

                if (deserializeResponse!.Succeeded)
                {
                    await OnInfoChange.InvokeAsync(deserializeResponse!.Data);
                    await OnChangeData.InvokeAsync(true);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion CreateImport

        #region ValidateImport

        private async Task ValidateImport()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                var request = new ImporterErrorReportDtoRequest();
                request.DocumentalVersionId = idDocumentalVersion;
                request.DataFile = template[0].Base64Data!;
                request.Identifier = null;
                request.LanguageCode = null;

                var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/Importer/ValidateImport", request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();

                if (deserializeResponse!.Succeeded)
                {
                    if (deserializeResponse.Data == null)
                    {
                        notificationModal.UpdateModal(ModalType.Information, Translation["ReadyToImport"], true, Translation["Accept"], "");
                        enableDownloadReport = false;
                        dataError = string.Empty;
                        readyToImport = true;
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Information, Translation["ImporterErrorReport"], true, Translation["Accept"], "");
                        enableDownloadReport = true;
                        dataError = deserializeResponse.Data;
                    }
                }
                else
                {
                    enableDownloadReport = false;
                    dataError = "Error";
                    notificationModal.UpdateModal(ModalType.Error, Translation["CheckTemplateErrorMessage"], true, Translation["Accept"], "");
                }

                EnableSaveButton();
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["CheckTemplateErrorMessage"], true, Translation["Accept"], "");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ValidateImport

        #region DownloadFile

        private async Task DownloadFile()
        {
            var dataFile = Convert.FromBase64String(dataError);
            var Data = Convert.ToBase64String(dataFile);

            bool download = await Js.InvokeAsync<bool>("DescargarArchivoBase64", "ImporterErrorReport.xlsx", Data);
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

        #region EnableSaveButton

        public void EnableSaveButton()
        {
            if (template != null && template.Count > 0 && template[0].Base64Data != null && idDocumentalVersion > 0)
            {
                readyToCheck = false;
            }
            else
            {
                readyToCheck = true;
            }

            var ok = true;
            if (enableDownloadReport == false && string.IsNullOrEmpty(dataError) && readyToImport == true)
            {
                ok = false;
            }

            if (readyToCheck || ok || idDocumentalVersion <= 0 || (!string.IsNullOrEmpty(dataError) || dataError.Equals("Error")) || template == null || template.Count == 0 || template[0].Base64Data == null)
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

        #region Checking

        public void Checking()
        {
            if (template != null && template.Count > 0 && template[0].Base64Data != null && idDocumentalVersion > 0)
            {
                readyToCheck = false;
            }
            else
            {
                readyToCheck = true;
            }

            EnableSaveButton();
        }

        public void CheckingFile2()
        {
            readyToCheck = false;
            if (template == null || template.Count == 0 || template[0].Base64Data == null)
            {
                readyToCheck = true;
            }
            saveIsDisable = true;
            enableDownloadReport = false;
        }

        #endregion Checking

        #endregion OthersMethods

        #endregion Methods
    }
}