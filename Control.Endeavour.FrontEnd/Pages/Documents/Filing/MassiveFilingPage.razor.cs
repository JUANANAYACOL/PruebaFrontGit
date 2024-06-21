using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components.Editor;
using static Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response.SystemFieldsDtoResponse;
using Telerik.SvgIcons;
using static Telerik.Blazor.ThemeConstants;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Response;
using DevExpress.DocumentView;
using Microsoft.VisualBasic;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Microsoft.Extensions.Localization;

namespace Control.Endeavour.FrontEnd.Pages.Documents.Filing
{
    public partial class MassiveFilingPage
    {
		#region Variables

		#region Inject 
		//[Inject] private EventAggregatorService? EventAggregator { get; set; }

		[Inject] private HttpClient? HttpClient { get; set; }

        [Inject] private IJSRuntime Js { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }


        #endregion

        #region Components
        private PaginationComponent<FilingDtoResponse, FilingFilterDtoRequest> PaginationComponet { get; set; } = new();
        #endregion

        #region Modals

        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Models

        private BulkFilingsDtoRequest BulkFilingsDtoRequest = new ();
        private MetaModel Meta { get; set; } = new() { PageSize = 3 };
        private FilingFilterDtoRequest? FilingFilterDtoRequest { get; set; } = new();

        #endregion

        #region Environments

        #region Environments(String)

        private string AlertMessage = "";
        private string SystemParamCL = "";
        private string DefaulTextCL = "";
        private string BtnVisible = "d-none";
        private string panel_1 = ""; //Paso 1
        private string panel_2 = "d-none"; //Paso 2
        private string panel_3 = "d-none"; //Paso 3
        private string panel_4 = "d-none"; //Paso 4
        MarkupString formatMessage;

        #endregion

        #region Environments(Numeric)

        private int FileSize = 10;

        #endregion

        #region Environments(Bool)

        private bool readOnlyComu = false; 
        private bool DisableButtons { get; set; } = true;

        #endregion

        #region Environments(List & Dictionary)

        private List<VSystemParamDtoResponse> SystemFieldsCLList = new();
        private List<FilingDtoResponse> ListFiling { get; set; } = new();
        private string[] AllowedExtensions { get; set; } = { ".xlsx", ".xls" };

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            DefaulTextCL = Translation["SelectAnOption"];
            formatMessage = new MarkupString(Translation["ChooseCommunicationMessage"]);
            AlertMessage = GetUploadMessage();
            await GetClassCom();

        }

		#endregion

		#region Methods

		#region HandleMethods

		private async Task HandleLanguageChanged()
		{
			StateHasChanged();
		}

        private async Task HandleFileFDP12(List<FileInfoData> data)
        {

            if (data != null && data.Count > 0)
            {
                BulkFilingsDtoRequest.ClassCode = SystemParamCL;
                BulkFilingsDtoRequest.FileExt = data[0].Extension!.ToString().Replace(".", ""); //Es importante que no lleve punto
                BulkFilingsDtoRequest.Archive = data[0].Base64Data;
                BulkFilingsDtoRequest.UserId = 4072;
                readOnlyComu = true;
                BtnVisible = "";
                panel_3 = "";
            }
            else
            {
                BulkFilingsDtoRequest = new BulkFilingsDtoRequest();
                readOnlyComu = false;
                BtnVisible = "d-none";
                panel_3 = "d-none";
            }
        }

        private void HandleDropdownListChange(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                SystemParamCL = value;
                DisableButtons = false;
                panel_2 = "";
            }
            else
            {
                SystemParamCL = DefaulTextCL;
                DisableButtons = true;
                panel_2 = "d-none";
            }
        }
        private void HandlePaginationGrid(List<FilingDtoResponse> newDataList)
        {
            ListFiling = newDataList;
        }

        private async Task HandleFormMassiveFiling()
        {

            if(BulkFilingsDtoRequest.UserId > 0) {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                var responseApi = await HttpClient.PostAsJsonAsync("documents/Filing/BulkFilings", BulkFilingsDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<BulkFilingsDtoResponse>>();

                if (deserializeResponse.Succeeded)
                {
                    int cuantos = deserializeResponse.Data!.Filing.Count;
                    DisableView();
                    notificationModal.UpdateModal(ModalType.Success, string.Format(Translation["#FiledDocuments"], cuantos), true);
                    ListFiling = deserializeResponse.Data.Filing;
                    Meta = deserializeResponse.Meta!;
                    panel_4 = "";
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["FilingBatchErrorMessage"], true, Translation["Accept"], "", "", "");
                    panel_4 = "d-none";
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["FilingBatchErrorMessage"], true, Translation["Accept"], "", "", "");
                panel_4 = "d-none";
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
        }

        #endregion

        #region GetMethods

        public async Task GetClassCom()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "CL");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse.Data != null)
                {
                    SystemFieldsCLList = deserializeResponse.Data.ToList() ?? new();
                    Meta = deserializeResponse.Meta;
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la clase de comunicacion: {ex.Message}");
            }
        }

        private async Task<FileDtoResponse?> GetFileBase64(int? id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
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
            catch(Exception ex)
            {
                Console.WriteLine($"Error al obtener el archivo: {ex.Message}");
                return new FileDtoResponse();
            }

        }

        #endregion

        #region OthersMethods
        public string GetUploadMessage()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            string extensions = string.Join(" ó ", AllowedExtensions).Replace(".", "").ToUpper();
            SpinnerLoaderService.HideSpinnerLoader(Js);
            return string.Format(Translation["AllowedFormatFiles"], extensions, FileSize);
        }

        private async Task DownloadTemplate()
        {
            if (!string.IsNullOrEmpty(SystemParamCL))
            {
                Int32 fileId = 0;
                String NombrePlantilla = Translation["Of"] + " ";
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                Dictionary<string, Int32> plantillasId = new() //Se debe validar que este dicionario venga de base y no este quemado - JR
                {
                     { "CL,I", 7940 },
                     { "CL,R", 7941 },
                     { "CL,E", 7942 }
                };

                if (plantillasId.ContainsKey(SystemParamCL))
                {
                    fileId = plantillasId[SystemParamCL];
                    NombrePlantilla = SystemParamCL == "CL,I" ? NombrePlantilla += Translation["InternalFiling"] : SystemParamCL == "CL,R" ? NombrePlantilla += Translation["FilingReceived"] : NombrePlantilla += Translation["FilingSent"];
                }

                FileDtoResponse objFile = await GetFileBase64(fileId);
                string nombreArchivo = objFile.FileName + "." + objFile.FileExt;

                bool download = await Js.InvokeAsync<bool>("DescargarArchivoBase64", nombreArchivo, Convert.ToBase64String(objFile.DataFile));
                if (download)
                {
                    readOnlyComu = true;
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    notificationModal.UpdateModal(ModalType.Success, string.Format(Translation["Template#DownloadSuccessfully"], NombrePlantilla), true, Translation["Accept"], title: "¡Descarga exitosa!");
                }
                else
                {
                    readOnlyComu = false;
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    notificationModal.UpdateModal(ModalType.Error, Translation["DownloadTemplateErrorMessage"], true, Translation["Accept"], "", "", "");
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ChooseCommunication4Download"], true, Translation["Accept"], "", "", "");
            }
        }
        private void ChangeClass()
        {
            panel_1 = " ";
            panel_2 = "d-none";
            panel_3 = "d-none";
            panel_4 = "d-none";
            DisableButtons = true;
            SystemParamCL = "";
            readOnlyComu = false;
            ListFiling = new();
            BulkFilingsDtoRequest = new BulkFilingsDtoRequest();
        }
        private void DisableView()
        {
            panel_1 += " deshabilitar-content";
            panel_2 += " deshabilitar-content";
            panel_3 += " deshabilitar-content";
            BtnVisible = "d-none";
        }


        #endregion

        #endregion
    }
}
