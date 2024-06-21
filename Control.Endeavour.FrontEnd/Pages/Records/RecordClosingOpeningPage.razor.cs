using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Records;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Models.Models.Records.Request;
using Control.Endeavour.FrontEnd.Models.Models.Records.Response;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.Blazor.Primitives.Internal;
using DevExpress.XtraPrinting;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Pages.Records
{
    public partial class RecordClosingOpeningPage
    {

		#region Variables

		#region Inject 

		[Inject]
		private HttpClient? HttpClient { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Components
        NewPaginationComponent<VRecordsDtoResponse, RecordFilterDtoRequest> PaginationComponentCloseRecords = new();
        NewPaginationComponent<VRecordsDtoResponse, RecordFilterDtoRequest> PaginationComponentReOpenRecords = new();
        #endregion

        #region Modals
        private RecordClosingModal RecordClosingModal = new();
        private RecordOpeningModal RecordOpeningModal = new();
        #endregion

        #region Parameters


        #endregion

        #region Models
        private MetaModel Meta { get; set; } = new() { PageSize = 10 };
        private NotificationsComponentModal NotificationModal = new();
        private RecordFilterDtoRequest RecordFilter = new();
        private PaginationInfo PaginationInfoCloseRecords = new();
        private PaginationInfo PaginationInfoReOpenRecords = new();
        #endregion

        #region Environments

        #region Environments(String)
        private String IdExpediente { get; set; } = "";
        private String IdExpedienteOpening { get; set; } = "";
        private string UriFilterRecords = "records/Records/Filter";

        #endregion

        #region Environments(Numeric)
        private int IdDocVersion { get; set; } = new();
        private int IdAdminUnit { get; set; } = new();
        private int IdProOffice { get; set; } = new();
        private int IdDocVersionOpening { get; set; } = new();
        private int IdAdminUnitOpening { get; set; } = new();
        private int IdProOfficeOpening { get; set; } = new();
        private int IdExpedienteGrid { get; set; } = 0;
        private int RecordNumber { get; set; } = 0;
        private int IdSubSeriesGrid { get; set; }
        public int ActiveTabIndex { get; set; } = 0;
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)
        private bool IsEnableAdminUnit = false;
        private bool IsEnableProOffice = false;
        private bool IsEnableAdminUnitOpening = false;
        private bool IsEnableProOfficeOpening = false;
        #endregion

        #region Environments(List & Dictionary)
        private List<DocumentalVersionDtoResponse> DocVersionList { get; set; } = new();
        private List<AdministrativeUnitsDtoResponse> AdminUnitList { get; set; } = new();
        private List<ProductionOfficesDtoResponse> ProOfficesList { get; set; } = new();
        private List<DocumentalVersionDtoResponse> DocVersionOpeningList { get; set; } = new();
        private List<AdministrativeUnitsDtoResponse> AdminUnitOpeningList { get; set; } = new();
        private List<ProductionOfficesDtoResponse> ProOfficesOpeningList { get; set; } = new();

        private List<VRecordsDtoResponse> RecordsInfoList = new();
        private List<VRecordsDtoResponse> RecordsOpenList = new();
        private List<VSystemParamDtoResponse> RecordTypeList = new();
        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await GetDocumentalVersions();
            await GetRecords(filtro: "Opening");
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }


		#endregion

		#region Methods

		#region HandleMethods

		private async Task HandleLanguageChanged()
		{
			StateHasChanged();
		}
        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                if(args.ModalOrigin == "Closing")
                {
                    RecordOpeningModal.GetDataForOpening(IdExpedienteGrid, RecordNumber, IdSubSeriesGrid);
                    RecordOpeningModal.UpdateModalStatus(true);
                }
            }
            else
            {
                Console.WriteLine("Registro No eliminado");
            }
        }
        private async Task HandleCleanFilter()
        {
            IsEnableAdminUnit = false;
            IsEnableProOffice = false;
            IdDocVersion = 0;
            IdAdminUnit = 0;
            IdProOffice = 0;
            IdExpediente = String.Empty;
            RecordFilter.recordNumber = String.Empty;
            RecordFilter = new();
            await GetRecords(filtro:"Opening");
        }
        private async Task HandleOpeningCleanFilter()
        {
            IsEnableAdminUnitOpening = false;
            IsEnableProOfficeOpening = false;
            IdDocVersionOpening = 0;
            IdAdminUnitOpening = 0;
            IdProOfficeOpening = 0;
            IdExpedienteOpening = String.Empty;
            RecordFilter.recordNumber = String.Empty;
            RecordFilter = new();
            await GetRecords(filtro:"Closing");
        }

        private async Task HandleSearchFilter()
        {
            RecordFilter.RecordId = RecordFilter.RecordId == null ? IdExpediente : RecordFilter.RecordId;
            if (IdExpediente == "0")
            {
                NotificationModal.UpdateModal(ModalType.Information, "El RecordId ingresado no es válido. Por favor, ingrese un valor positivo mayor que cero.", true);
            }
            else
            {
                await GetRecords(IdExpediente, "Opening");
            }

        }
        private async Task HandleOpeningSearchFilter()
        {
            RecordFilter.RecordId = RecordFilter.RecordId == null ? IdExpedienteOpening : RecordFilter.RecordId;
            if (IdExpedienteOpening == "0")
            {
                NotificationModal.UpdateModal(ModalType.Information, "El RecordId ingresado no es válido. Por favor, ingrese un valor positivo mayor que cero.", true);
            }
            else
            {
                await GetRecords(IdExpedienteOpening, "Closing");
            }

        }

        private void HandlePaginationGrid(List<VRecordsDtoResponse> newDataList)
        {
            RecordsInfoList = newDataList;
        }
        private void HandlePaginationGrid2(List<VRecordsDtoResponse> newDataList)
        {
            RecordsOpenList = newDataList;
        }
        private async Task HandleRefreshOpening()
        {
            await GetRecords(filtro: "Opening");
            RecordClosingModal.UpdateModalStatus(false);
        }
        private async Task HandleRefreshClosing()
        {
            await GetRecords(filtro: "Closing");
            await GetRecords(filtro: "Opening");
            RecordOpeningModal.UpdateModalStatus(false);
        }

        private void HandleTabClick(int tabIndex)
        {
            Console.WriteLine($"Se hizo clic en la pestaña número {tabIndex}");
        }

        #endregion

        #region OthersMethods

        #region GetMethods
        public async Task GetDocumentalVersions()
        {
            try
            {
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<DocumentalVersionDtoResponse>>>("paramstrd/DocumentalVersions/ByDocumentalVersions");
                if (deserializeResponse!.Succeeded)
                {
                    DocVersionList = deserializeResponse.Data!;
                    DocVersionList = DocVersionList.Where(x => x.EndDate > DateTime.Now || x.EndDate == null).ToList();
                    DocVersionList.ForEach(x =>
                    {
                        x.Code = $"{x.StartDate!.Value.Year} a Actual - Version Documental {x.Code}";
                    });
                    DocVersionOpeningList = DocVersionList;
                    Meta = deserializeResponse.Meta ?? new() { PageSize = 10 };
                }
            }
            catch (Exception ex)
            {
                NotificationModal.UpdateModal(ModalType.Error, $"Error al obtener Versiones Documentales: {ex.Message}", true, Translation["Accept"]);
            }
        }
        public async Task GetAdministrativeUnits(int id, string action)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                if (action == "Opening")
                {
                    IdDocVersionOpening= id;
                    AdminUnitOpeningList = new();
                    IdAdminUnitOpening = 0;

                    HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
                    HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", $"{IdDocVersionOpening}");
                }
                else
                {
                    IdDocVersion = id;
                    AdminUnitList = new();
                    IdAdminUnit = 0;

                    HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
                    HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", $"{IdDocVersion}");
                }

                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
                HttpClient?.DefaultRequestHeaders.Remove("key");

                if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                {
                    if (action == "Opening") 
                    { 
                        AdminUnitOpeningList = deserializeResponse.Data!;
                    }
                    else
                    {
                        AdminUnitList = deserializeResponse.Data!;
                    }

                    EnableField(true, false, action);
                    //RecordFilter.AdministrativeUnitId = id;
                    Meta = deserializeResponse.Meta!;
                }
                else
                {
                    EnableField(false, false, action);
                    //RecordFilter.AdministrativeUnitId = 0;
                    RecordFilter.ProductionOfficeId = 0;
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                NotificationModal.UpdateModal(ModalType.Error, $"Error al obtener Unidades Administrativas: {ex.Message}", true, Translation["Accept"]);
            }
        }
        public async Task GetProducOffice(int id, string action)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                if (action == "Opening")
                {
                    IdAdminUnitOpening = id;
                    ProOfficesOpeningList = new();
                    IdProOfficeOpening = 0;

                    HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                    HttpClient?.DefaultRequestHeaders.Add("AdministrativeUnitId", $"{IdAdminUnitOpening}");
                }
                else
                {
                    IdAdminUnit = id;
                    ProOfficesList = new();
                    IdProOffice = 0;

                    HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                    HttpClient?.DefaultRequestHeaders.Add("AdministrativeUnitId", $"{IdAdminUnit}");
                }

                RecordFilter.AdministrativeUnitId = id;

                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>("paramstrd/ProductionOffice/ByProductionOffices");
                HttpClient?.DefaultRequestHeaders.Remove("key");

                if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                {
                    if (action == "Opening")
                    {
                        ProOfficesOpeningList = deserializeResponse.Data!;
                    }
                    else
                    {
                        ProOfficesList = deserializeResponse.Data!;
                    }

                    EnableField(true, true, action);
                    Meta = deserializeResponse.Meta!;
                }
                else
                {
                    EnableField(true, false, action);
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                NotificationModal.UpdateModal(ModalType.Error, $"Error al obtener Officinas Productoras: {ex.Message}", true, Translation["Accept"]);
            }
        }
        public void GetValue(int id, string action)
        {
            try
            {
                if (action == "Opening")
                {
                    RecordFilter.ProductionOfficeId = id;
                    IdProOfficeOpening = id;
                }
                else
                {
                    RecordFilter.ProductionOfficeId = id;
                    IdProOffice = id;
                }
            }
            catch (Exception ex)
            {
                if (action == "Opening")
                {
                    RecordFilter.ProductionOfficeId = 0;
                    IdProOfficeOpening = 0;
                }
                else
                {
                    RecordFilter.ProductionOfficeId = 0;
                    IdProOffice = 0;
                }

                NotificationModal.UpdateModal(ModalType.Error, $"Error al asignarle valor a la Officina Productora: {ex.Message}", true, Translation["Accept"]);
            }
        }
        //public async Task GetRecords(string recordNumber = "", string filtro = "")
        //{
        //    try
        //    {
        //        SpinnerLoaderService.ShowSpinnerLoader(Js);
        //        RecordFilter.OrderAsc = false;
        //        RecordFilter.OrderBy = "RecordId";
        //        if (!String.IsNullOrEmpty(recordNumber)) { RecordFilter.recordNumber = recordNumber; }
                
        //        var responseApi = await HttpClient.PostAsJsonAsync(UriFilterRecords, RecordFilter);

        //        var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<VRecordsDtoResponse>>>();

        //        if (deserializeResponse.Succeeded && deserializeResponse.Data.Data != null)
        //        {
        //            RecordsInfoList = deserializeResponse.Data.Data;

        //            if (String.IsNullOrEmpty(filtro))
        //            {
        //                RecordsOpenList = RecordsInfoList.Where(x => x.ActiveState == false).ToList();
        //                RecordsInfoList = RecordsInfoList.Where(x => x.ActiveState == true).ToList();
        //            }
        //            else
        //            {
        //                if(filtro == "Opening")
        //                {
        //                    RecordsOpenList = RecordsInfoList.Where(x => x.ActiveState == false).ToList();
        //                    RecordsInfoList = new();
        //                }
        //                else
        //                {
        //                    RecordsInfoList = RecordsInfoList.Where(x => x.ActiveState == true).ToList();
        //                    RecordsOpenList = new();
        //                }
        //            }
        //            PaginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
        //            PaginationComponent.ResetPagination(PaginationInfo);
        //            SpinnerLoaderService.HideSpinnerLoader(Js);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        SpinnerLoaderService.HideSpinnerLoader(Js);
        //        NotificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
        //    }
        //}

        public async Task GetRecords(string recordNumber = "", string filtro = "")
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                RecordFilter.OrderAsc = false;
                RecordFilter.OrderBy = "RecordId";

                if (!String.IsNullOrEmpty(recordNumber))
                {
                    RecordFilter.recordNumber = recordNumber;
                }

                switch (filtro)
                {
                    case "Opening":
                        RecordFilter.ActiveState = true;
                        break;
                    case "Closing":
                        RecordFilter.ActiveState = false;
                        break;
                    default:
                        RecordFilter.ActiveState = null;
                        break;
                }

                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterRecords, RecordFilter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<VRecordsDtoResponse>>>();

                if (deserializeResponse.Succeeded && deserializeResponse.Data.Data != null)
                {
                    if (filtro == "Opening")
                    {
                        RecordsInfoList = deserializeResponse.Data.Data;
                        PaginationInfoCloseRecords = deserializeResponse.Data.ExtractPaginationInfo();
                        PaginationComponentCloseRecords.ResetPagination(PaginationInfoCloseRecords);
                    }
                    else
                    {
                        RecordsOpenList = deserializeResponse.Data.Data;
                        PaginationInfoReOpenRecords = deserializeResponse.Data.ExtractPaginationInfo();
                        PaginationComponentReOpenRecords.ResetPagination(PaginationInfoReOpenRecords);

                    }
                }

                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                NotificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }



        #endregion

        public void EnableField(bool a, bool b, string action)
        {
            if (action == "Closing")
            {
                IsEnableAdminUnit = a;
                IsEnableProOffice = b;

                IdAdminUnit = a ? IdAdminUnit : 0;
                IdProOffice = b ? IdProOffice : 0;
            }
            else
            {
                IsEnableAdminUnitOpening = a;
                IsEnableProOfficeOpening = b;

                IdAdminUnitOpening = a ? IdAdminUnitOpening : 0;
                IdProOfficeOpening = b ? IdProOfficeOpening : 0;
            }
        }

        private void RecordClosing(VRecordsDtoResponse record)
        {
            RecordClosingModal.GetDataForClosing(record.RecordId, record.RecordNumber);
            RecordClosingModal.UpdateModalStatus(true);
        }

        private void RecordOpening(VRecordsDtoResponse record)
        {
            IdExpedienteGrid = record.RecordId;
            RecordNumber = record.RecordNumber;
            IdSubSeriesGrid = (int)record.SubSeriesId;

            NotificationModal.UpdateModal(ModalType.Warning, Translation["MessageReopen"], true, Translation["Accept"], Translation["Cancel"], modalOrigin: "Closing");
        }
        private async Task TabChangedHandler(int newIndex)
        {
            ActiveTabIndex = newIndex;
            if (ActiveTabIndex == 1)
            {
                await GetRecords(filtro:"Closing");
            }
        }

        #endregion

        #endregion

    }
}
