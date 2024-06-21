using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.StateContainer.ManagementTray;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Pages.Security
{
    public partial class ManagementTrayPerUser
    {

        #region Variables

        #region Inject 
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }
        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
        [Inject]
        private ManagementTrayStateContainer? managementTrayStateContainer { get; set; }
        [Inject]
        private NavigationManager navigation { get; set; }
        #endregion

        #region Components
        private NewPaginationComponent<VUserDtoResponse, VUserDtoRequest> paginationComponetPost { get; set; } = new();


        #endregion

        #region Modals

        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Parameters

        [CascadingParameter]
        public UserDtoResponse UserDataToShow { get; set; }
        #endregion

        #region Models

        private VUserDtoRequest userFilterRequest = new();
        private PaginationInfo paginationInfo = new();

        #endregion

        #region Environments

        #region Environments(String)
        private string UriFilterVUser { get; set; } = "generalviews/VUser/ByFilter";

        #endregion

        #region Environments(Numeric)
        private int currentDocumentalVersionID = 0;

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool isEnableProOffice = false;

        #endregion

        #region Environments(List & Dictionary)
        private List<DocumentalVersionDtoResponse> docVersionList = new();
        private List<AdministrativeUnitsDtoResponse> adminUnitList = new();
        private List<ProductionOfficesDtoResponse> proOfficesList = new();
        private List<VUserDtoResponse> userListData { get; set; } = new();
        private List<VSystemParamDtoResponse>? chargeList { get; set; }


        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            await GetDataUserCharge();
            await GetDocumentalVersions();
            await GetUsersData();
            

        }


        #endregion

        #region Methods

        #region HandleMethods

        private void HandlePaginationGrid(List<VUserDtoResponse> newDataList)
        {
            userListData = newDataList;
        }
        #endregion

        #region OthersMethods
        #region FilterMethods

        private async Task CleanFilter()
        {
            userFilterRequest = new();
            await GetUsersData();
        }
        private async Task SearchByFilter()
        {

            if (!string.IsNullOrEmpty(userFilterRequest.FirstName) ||
                !string.IsNullOrEmpty(userFilterRequest.LastName) ||
                !string.IsNullOrEmpty(userFilterRequest.ChargeCode) ||
                userFilterRequest.AdministrativeUnitId != 0 ||
                userFilterRequest.ProductionOfficeId != 0)
            {
                await GetUsersData();

            }
            else
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["EnterParameterSearch"], true, buttonTextCancel: "");
            }
        }



        #endregion

        #region GetData
        private async Task GetUsersData()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {

                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterVUser, userFilterRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<VUserDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data.Data.Any())
                {
                    userListData = deserializeResponse.Data.Data;
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponetPost.ResetPagination(paginationInfo);
                }
                else
                {
                    userListData = new();
                    paginationInfo = new();
                    paginationComponetPost.ResetPagination(paginationInfo);
                    
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        private async Task GetDataUserCharge()
        {
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            HttpClient?.DefaultRequestHeaders.Add("paramCode", "CAR");
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            if (deserializeResponse!.Succeeded)
            {
                chargeList = deserializeResponse.Data;
            }
            else { chargeList = new(); }
        }
        public async Task GetDocumentalVersions()
        {
            try
            {
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<DocumentalVersionDtoResponse>>>("paramstrd/DocumentalVersions/ByDocumentalVersions");
                if (deserializeResponse!.Succeeded)
                {
                    docVersionList = deserializeResponse.Data!;
                    string toCurrent = Translation["ToCurrent"];
                    string documentaryVersion = Translation["DocumentaryVersion"];
                    docVersionList.ForEach(x =>
                    {
                        x.Code = $"{((x.EndDate == null || x.EndDate.Value > DateTime.Now) ? $"{x.StartDate!.Value.Year} {toCurrent} - {documentaryVersion} {x.Code}" : $"{x.StartDate!.Value.Year} a {x.EndDate.Value.Year} - {documentaryVersion} {x.Code} {x.VersionType}")}";
                    });
                    docVersionList = docVersionList.OrderBy(x => x.DocumentalVersionId).Reverse().ToList();
                    currentDocumentalVersionID = docVersionList.FirstOrDefault(x => x.EndDate == null || x.EndDate.Value > DateTime.Now).DocumentalVersionId;
                    await GetAdministrativeUnits(currentDocumentalVersionID);
                }
            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }
        private async Task GetAdministrativeUnits(int id)
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
                HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", $"{id}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
                HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");

                if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                {
                    adminUnitList = deserializeResponse.Data;
                }

                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }



        private async Task GetProducOffice(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);


                HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                HttpClient?.DefaultRequestHeaders.Add("AdministrativeUnitId", $"{id}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>("paramstrd/ProductionOffice/ByProductionOffices");
                if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                {
                    proOfficesList = deserializeResponse.Data;
                    userFilterRequest.AdministrativeUnitId = id;
                }
                HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                SpinnerLoaderService.HideSpinnerLoader(Js);

            }
            catch (Exception)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }

        #endregion

        #region ActionsMethods
        private async Task ShowUserManagamentTray(VUserDtoResponse record)
        {
            if(record.UserId == UserDataToShow.UserId)
            {
                navigation.NavigateTo("/ManagementTray");
            }
            else
            {
                var FilterRequestDocumentManagement = new
                {
                    AssingUserId = record.UserId,
                    IsAnotherUser = true
                };


                managementTrayStateContainer.ManagementTrayParameters(record.UserId, true, record.FullName);
                EncriptService.Encrypt(HttpClient, Js, FilterRequestDocumentManagement, "FilterRequestDocumentManagement");
                navigation.NavigateTo("/ManagementTray");
            }
            
        }
        

        #endregion

        #endregion

        #endregion

    }
}
