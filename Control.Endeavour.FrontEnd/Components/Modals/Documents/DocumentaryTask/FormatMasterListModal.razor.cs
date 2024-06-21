using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Interfaces.DriveService;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.XtraSpellChecker.Native;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Web;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.DocumentaryTask
{
    public partial class FormatMasterListModal
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
        private IGraphService graphService { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        [Inject]
        private NavigationManager navigation { get; set; }

        #endregion Inject

        #region Components

        private NotificationsComponentModal notificationModal;
        private NewPaginationComponent<TemplateDocumentDtoResponse, TemplateFilterDtoRequest> paginationComponent = new();

        #endregion Components

        #region Modals

        public InputModalComponent inputCode = new();
        public InputModalComponent inputName = new();
        public InputModalComponent inputVersion = new();
        public InputModalComponent inputProcess = new();

        #endregion Modals

        #region Parameters

        [Parameter] public EventCallback<MyEventArgs<TemplateModelDtoResponse>> OnStatusChanged { get; set; }

        #endregion Parameters

        #region Models

        private PaginationInfo paginationInfo = new();
        private TemplateDocumentDtoResponse templateDocumentDto = new();
        private TemplateFilterDtoRequest filter = new() { Type = "TFOR,DTXT" };
        private TemplateDocumentDtoResponse BasicTemplate = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string version = string.Empty;
        private string UriFilterTemplate = "documents/TemplateDocuments/ByFilters";

        #endregion Environments(String)

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool wordOnline = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<TemplateDocumentDtoResponse> templateDocumentList = new();
        private List<Value> docToGraphList = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            if (wordOnline)
            {
                await GetDriveTemplateDoc();
            }
            else
            {
                await GetTemplateDoc();
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

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
        }

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted && args.ModalOrigin.Equals("LoadFile") && templateDocumentDto != null)
            {
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", templateDocumentDto.FileId.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");

                TemplateModelDtoResponse sendModel = new() { TemplateId = templateDocumentDto.TemplateId, Archive = deserializeResponse.Data.DataFile };

                var eventArgs = new MyEventArgs<TemplateModelDtoResponse>
                {
                    Data = sendModel,
                    ModalStatus = false
                };

                await OnStatusChanged.InvokeAsync(eventArgs);
            }
        }

        #endregion HandleMethods

        #region OthersMethods

        private async Task GetTemplateDoc()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            filter.Code = string.IsNullOrEmpty(inputCode.InputValue) ? "" : inputCode.InputValue;
            filter.NameTemplate = string.IsNullOrEmpty(inputName.InputValue) ? "" : inputName.InputValue;
            filter.Version = string.IsNullOrEmpty(inputVersion.InputValue) ? 0 : int.Parse(inputVersion.InputValue);
            filter.Process = string.IsNullOrEmpty(inputProcess.InputValue) ? "" : inputProcess.InputValue;
            filter.ValidateActiveState = true;

            var responseApi = await HttpClient.PostAsJsonAsync(UriFilterTemplate, filter);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<TemplateDocumentDtoResponse>>>();

            if (deserializeResponse.Succeeded)
            {
                BasicTemplate = deserializeResponse.Data.Data.Where(x => x.TempCode.Equals("LPV")).FirstOrDefault();
                templateDocumentList = deserializeResponse.Data.Data.Where(x => !x.TempCode.Equals("LPV")).ToList() ?? new();
                paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                paginationComponent.ResetPagination(paginationInfo);
            }
            else
            {
                templateDocumentList = new();
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #region GetDriveTemplateDoc

        //private string object_id = "dc3bd1d6-4d8c-48ee-a8bb-b9af2e863cb8";
        string group_id = "97f33c08-21b4-48c6-9e7f-870a7e58dda4";

        private async Task GetDriveTemplateDoc()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            string token = await GetAccessToken();

            if (token != null)
            {
                //docToGraphList = await graphService.GetUserDocs(object_id, token);

                docToGraphList = await graphService.getGroupDocuments(group_id, token);

                if (docToGraphList.Any())
                {
                    Console.WriteLine("data correcta");
                }
                else { Console.WriteLine("ocurrio algo"); }
            }
            else { Console.WriteLine("no se pudo obtener el token"); }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #region GetAccessToken

        private async Task<string> GetAccessToken()
        {
            try
            {
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<string>>("security/Session/accessToken");

                if (deserializeResponse.Succeeded)
                {
                    var accessToken = deserializeResponse.Data;

                    return accessToken;
                }
                else { return null; }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener informacion de las compañias: {ex.Message}");
                return null;
            }
        }

        #endregion GetAccessToken

        #endregion GetDriveTemplateDoc

        private void HandlePaginationGrid(List<TemplateDocumentDtoResponse> newDataList)
        {
            templateDocumentList = newDataList;
        }

        private async Task ResetFormAsync()
        {
            filter = new() { Type = "TFOR,DTXT" };
            inputVersion.InputValue = string.Empty;
            version = string.Empty;
            inputName.InputValue = string.Empty;
            inputProcess.InputValue = string.Empty;
            inputCode.InputValue = string.Empty;
        }

        private void LoadFile(TemplateDocumentDtoResponse record)
        {
            templateDocumentDto = record;

            if (templateDocumentDto != null)
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["Name"] + ": " + templateDocumentDto.TempName, true, modalOrigin: "LoadFile");
            }
        }

        private async Task LoadFileWordAsync(Value record)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<string>>("documentarytasks/DocumentaryTask/EncryptPublicKey");
                if (deserializeResponse.Succeeded)
                {
                    var token = deserializeResponse.Data;
                    var documentUrl = BuildDocumentUrl(record.webUrl, token);
                    await Js.InvokeVoidAsync("openUrlInNewTab", documentUrl);
                }

                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener informacion de las compañias: {ex.Message}");
            }
        }

        public static string BuildDocumentUrl(string baseGraphUrl, string token)
        {
            string url = baseGraphUrl;

            url += $"?token={HttpUtility.UrlEncode(token)}";

            return url;
        }

        #endregion OthersMethods

        #endregion Methods
    }
}