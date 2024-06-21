using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.FormatMasterList;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.ViewDocuments;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.Blazor;
using DevExpress.Blazor.Primitives.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.IO;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;
using static Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response.SystemFieldsDtoResponse;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class FormatMasterListPage
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

        private InputModalComponent CodeInput { get; set; } = new();
        private InputModalComponent VersionInput { get; set; } = new();
        private InputModalComponent NametemplateInput { get; set; } = new();
        private InputModalComponent ProcessInput { get; set; } = new();
        public object JsRuntime { get; private set; }

        private NewPaginationComponent<TemplateDocumentDtoResponse, TemplateFilterDtoRequest> paginationComponent = new();

        #endregion Components

        #region Modals

        private NotificationsComponentModal notificationModal = new();
        private NotificationsComponentModal notificationModalSucces = new();
        private TemplateModal templateModal = new();
        private ViewDocumentModal viewDocumentModal = new();

        #endregion Modals

        #region Models

        private TemplateFilterDtoRequest filtro { get; set; } = new();
        private TemplateDocumentDtoResponse TemplateForm = new();
        private PaginationInfo paginationInfo = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string ValueFormatDoc { get; set; } = "";
        private string Code { get; set; } = "";
        private string Version { get; set; } = "";
        private string Name { get; set; } = "";
        private string Process { get; set; } = "";

        private string textFD = "";



        private string UriFilterTemplate = "documents/TemplateDocuments/ByFilters";

        #endregion Environments(String)

        #region Environments(Bool)

        public bool SecondGrid { get; private set; }

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private byte[] FileData = Array.Empty<byte>();
        private List<VSystemParamDtoResponse> FormatDoc = new();
        private List<TemplateDocumentDtoResponse> DocTemplatelist = new();
        private List<TemplateDocumentDtoResponse> DocTemplatelistCode = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            textFD = Translation["SelectAnOption"];
            await GetFormatDoc();
            await GetByFilter();
            ////EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #region ShowModal

        private void ShowTemplateModal()
        {
            templateModal.UpdateModalStatus(true);
        }

        private async Task ShowModalEdit(TemplateDocumentDtoResponse record)
        {
            await templateModal.RecibirRegistro(record);
            templateModal.UpdateModalStatus(true);
        }

        #endregion ShowModal

        #endregion HandleMethods

        #region OthersMethods

        #region GetFormatDoc

        public async Task GetFormatDoc()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");
                HttpClient?.DefaultRequestHeaders.Add("ParamCode", "TFOR");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");

                if (deserializeResponse.Data != null)
                {
                    FormatDoc = deserializeResponse.Data;
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las acciones: {ex.Message}");
            }
        }

        #endregion GetFormatDoc

        #region GetByFilter

        public async Task GetByFilter()
        {
            try
            {
                string code = string.IsNullOrEmpty(Code) ? "" : CodeInput.InputValue;
                int version = string.IsNullOrEmpty(Version) ? 0 : int.Parse(VersionInput.InputValue);
                string name = string.IsNullOrEmpty(Name) ? "" : NametemplateInput.InputValue;
                string process = string.IsNullOrEmpty(Process) ? "" : ProcessInput.InputValue;
                string valueFormatDoc = string.IsNullOrEmpty(ValueFormatDoc) ? "" : ValueFormatDoc;

                filtro = new()
                {
                    Code = code,
                    Version = version,
                    NameTemplate = name,
                    Process = process,
                    Type = valueFormatDoc
                };

                filtro.OrderBy = "CreateDate";
                filtro.OrderAsc = false;

                SpinnerLoaderService.ShowSpinnerLoader(Js);
                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterTemplate, filtro);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<TemplateDocumentDtoResponse>>>();

                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    DocTemplatelist = deserializeResponse.Data.Data;
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                }
                else
                {
                    DocTemplatelist = new();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el listado maestro de formatos: {ex.Message}");
            }
        }

        #endregion GetByFilter

        #region ResetFilters

        public async Task ResetFiltersAsync()
        {
            Code = "";
            Version = "";
            Name = "";
            Process = "";
            ValueFormatDoc = "";
            await GetByFilter();

            StateHasChanged();
        }

        #endregion ResetFilters

        #region OnRowExpandHandler

        private async Task OnRowExpandHandler(GridRowExpandEventArgs args)
        {
            try
            {
                TemplateDocumentDtoResponse? record = args.Item as TemplateDocumentDtoResponse;

                HttpClient?.DefaultRequestHeaders.Remove("Code");
                HttpClient?.DefaultRequestHeaders.Add("Code", $"{record.TempCode}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<TemplateDocumentDtoResponse>>>("documents/TemplateDocuments/ByFilterCode");
                HttpClient?.DefaultRequestHeaders.Remove("Code");

                if (deserializeResponse.Succeeded)
                {
                    DocTemplatelistCode = deserializeResponse.Data;
                    SecondGrid = true;
                }
                else { Console.Write(deserializeResponse.Message); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el listado maestro de formatos: {ex.Message}");
            }
        }

        #endregion OnRowExpandHandler

        #region HandleRefreshGridDataAsync

        private async Task HandleRefreshGridDataAsync(bool refresh)
        {
            await GetByFilter();
            await GetFormatDoc();
        }

        #endregion HandleRefreshGridDataAsync

        #region ViewAttachment

        private async Task HandleViewAttachment(TemplateDocumentDtoResponse record)
        {
            await viewDocumentModal.DownloadFile(record.FileId);
        }

        #endregion ViewAttachment

        #region DeleteTemplate

        private void ShowModalDelete(TemplateDocumentDtoResponse record)
        {
            TemplateForm = record;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "DeleteModal");
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
            {
                DeleteGeneralDtoRequest record = new DeleteGeneralDtoRequest()
                {
                    Id = TemplateForm.TemplateId,
                };

                var responseApi = await HttpClient.PostAsJsonAsync("documents/TemplateDocuments/DeleteTemplate", record);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                if (deserializeResponse.Succeeded)
                {
                    notificationModalSucces.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true);
                    await GetByFilter();
                }
            }
        }

        #endregion DeleteTemplate

        #region PaginationGrid

        private void HandlePaginationGrid(List<TemplateDocumentDtoResponse> newDataList)
        {
            DocTemplatelist = newDataList;
        }

        #endregion PaginationGrid

        #region ExportToExcel

        public bool CancelExport { get; set; }

        private List<TemplateDocumentDtoResponse> allRecords { get; set; }

        bool ExportAllPages { get; set; }

        public async Task GetAllRecords()
        {
            try
            {
                string code = string.IsNullOrEmpty(Code) ? "" : CodeInput.InputValue;
                int version = string.IsNullOrEmpty(Version) ? 0 : int.Parse(VersionInput.InputValue);
                string name = string.IsNullOrEmpty(Name) ? "" : NametemplateInput.InputValue;
                string process = string.IsNullOrEmpty(Process) ? "" : ProcessInput.InputValue;
                string valueFormatDoc = string.IsNullOrEmpty(ValueFormatDoc) ? "" : ValueFormatDoc;

                filtro = new()
                {
                    Code = code,
                    Version = version,
                    NameTemplate = name,
                    Process = process,
                    Type = valueFormatDoc
                };

                filtro.OrderBy = "CreateDate";
                filtro.OrderAsc = false;

                var responseApi = await HttpClient.PostAsJsonAsync("documents/TemplateDocuments/ByNoPaginationFilters", filtro);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<TemplateDocumentDtoResponse>>>();

                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    allRecords = deserializeResponse.Data;
                }
                else
                {
                    allRecords = new();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el listado maestro de formatos: {ex.Message}");
            }
        }

        public async Task OnBeforeExcelExport(GridBeforeExcelExportEventArgs args)
        {

            if (ExportAllPages)
            {
                await GetAllRecords();

                if (allRecords.Any())
                {
                    args.Data = allRecords;
                }
            }
            else { args.Data = DocTemplatelist; }

            args.IsCancelled = CancelExport;
        }

        #endregion

        #endregion OthersMethods

        #endregion Methods
    }
}