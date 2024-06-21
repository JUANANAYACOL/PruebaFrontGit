using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Replacement;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Components.Modals.Security.ManagementValidity;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Security
{
    public partial class ManagementValidityPage
    {
        #region Variables

        #region Inject

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject


  

        #region Components

        private NewPaginationComponent<ProdOfficeManagmentDtoResponse, ProdOfficeManagmentDtoRequest> paginationComponent { get; set; } = new();
        private PaginationInfo paginationInfo { get; set; } = new();

        #endregion Components

        #region Modals

        private ManagementValidityModal managementValidityModal { get; set; } = new();
        private NotificationsComponentModal notificationModal { get; set; } = new();

        #endregion Modals

        #region Models

        private ProdOfficeManagmentDtoRequest FilterSearch { get; set; } = new();

        #endregion Models

        #region Enviroment

        #region Enviroment (bool)

        private bool dataChargue { get; set; } = false;

        #endregion Enviroment (bool)

        #region Enviroment(String)

        private string ProductionOfficeName { get; set; } = string.Empty;
        private string Code { get; set; } = string.Empty;

        private string UriFilterProdParameter { get; set; } = "security/System/ReadProdOfficeParameter";

        #endregion Enviroment(String)

        #region List(Numeric)

        private int ProdOffId { get; set; } = new();

        #endregion List(Numeric)

        #region Enviroment(List && Dictionary)

        private List<ProdOfficeManagmentDtoResponse> ListProdParameter { get; set; } = new();

        #endregion Enviroment(List && Dictionary)

        #endregion Enviroment

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            await SearchData();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private void HandleDeleteData(ProdOfficeManagmentDtoResponse item)
        {
            ProdOffId = item.ProductionOfficeId;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, Translation["Yes"], Translation["No"]);
        }

        private async Task HandleModalNotiCloseAsync(ModalClosedEventArgs args)
        {
            if (args.IsAccepted && notificationModal.Type == ModalType.Warning)
            {
                await DeleteRecord();
            }
         
        }

        private void HandlePaginationGrid(List<ProdOfficeManagmentDtoResponse> newDataList)
        {
            ListProdParameter = newDataList;
        }

        #endregion HandleMethods

        private async Task ShowModal()
        {
            await managementValidityModal.UpdateModalStatus(true);

            StateHasChanged();
        }

        private async Task ClearData()
        {
            ProductionOfficeName = string.Empty;
            Code = string.Empty;
            await SearchData();
        }

        private async Task SearchData()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                FilterSearch = new()
                {
                    Code = Code,

                    Name = ProductionOfficeName
                };

                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterProdParameter, FilterSearch);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<ProdOfficeManagmentDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data.Data.Any())
                {
                    ListProdParameter = deserializeResponse.Data.Data;
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                    dataChargue = true;
                }
                else
                {
                    ListProdParameter = new();
                    paginationInfo = new();
                    paginationComponent.ResetPagination(paginationInfo);
                    dataChargue = false;
                }

                StateHasChanged();
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task DeleteRecord()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("productionOffId");
                HttpClient?.DefaultRequestHeaders.Add("productionOffId", $"{ProdOffId}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<int>>("security/System/DeleteProdOfficeParameter");
                HttpClient?.DefaultRequestHeaders.Remove("productionOffId");

                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true, Translation["Accept"]);
                    await SearchData();
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"]);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task ShowModalEdit(ProdOfficeManagmentDtoResponse request)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            await managementValidityModal.UpdateModalStatus(true);
            await managementValidityModal.updateRecord(request);
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }


        private async Task HandleModalStatusChangedAsync(bool status)
        {
            await managementValidityModal.UpdateModalStatus(false);

            await SearchData();
            StateHasChanged();
        }



        #endregion Methods
    }
}