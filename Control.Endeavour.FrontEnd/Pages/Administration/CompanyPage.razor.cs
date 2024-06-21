using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Address;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Company;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class CompanyPage
    {
        #region Variables

        #region Inject

        /* [Inject]
         private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Components

        private NewPaginationComponent<CompanyDtoResponse, CompanyFilterDtoRequest> PaginationComponent = new();

        #endregion Components

        #region Modals

        private CompanyModal modalCompanies = new();
        private AddressModal modalAddress = new();
        private NotificationsComponentModal notificationModal = new();

        #endregion Modals

        #region Models
        private PaginationInfo paginationInfo = new();
        private AddressDtoRequest addressrequest = new();
        private CompanyFilterDtoRequest companyDtoRequest { get; set; } = new();
        private CompanyDtoResponse companyUpdate = new();
        private CompanyDtoResponse companyToDelete = new();
        private CompanyDtoResponse _selectedRecord = new();


        #endregion Models

        #region Environments

        #region Environments(String)

        private string UriFilterCompany = "companies/Company/ByFilter";

        #endregion Environments(String)



        #region Environments(Bool)
        private bool modalStatus = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<CompanyDtoResponse> CompaniesList = new();
        private List<(string, AddressDtoRequest)> addressDtoRequest = null;

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            ////EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetCompanies();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleMethods

        #region OtherMethods

        #region GetCompany

        private async Task GetCompanies()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                
                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterCompany, companyDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<CompanyDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    CompaniesList = deserializeResponse.Data.Data ?? new();
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    PaginationComponent.ResetPagination(paginationInfo);
                }
                else
                {
                    CompaniesList =  new();
                    paginationInfo = new();
                    PaginationComponent.ResetPagination(paginationInfo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetCompany

        #region DeleteCompany

        private void HandleRecordToDelete(CompanyDtoResponse record)
        {
            companyToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "DeleteModal");
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (args.IsAccepted && args.ModalOrigin!.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new() { Id = companyToDelete.CompanyId, User = "user" };

                    var responseApi = await HttpClient!.PostAsJsonAsync("companies/Company/DeleteCompany", request);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        await HandleRefreshGridData(true);
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true, Translation["Accept"]);
                        }
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"]);
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["RegistrationDeleteErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion DeleteCompany

        #region RefreshGrid

        private async Task HandleRefreshGridData(bool refresh)
        {
            await GetCompanies();
        }

        #endregion RefreshGrid

        #region ModalCompanies

        private async Task ShowModalEdit(CompanyDtoResponse record)
        {
            modalCompanies.UpdateModalStatus(true);
            await modalCompanies.RecibirRegistro(record);
        }

        #endregion ModalCompanies

        #region ModalAddress

        private void ShowModal()
        {
            modalCompanies.UpdateModalStatus(true);
            modalCompanies.ResetFormAsync();
        }

        private void HandleStatusChanged(bool status)
        {
            modalAddress.UpdateModalStatus(status);
        }

        private async Task HandleNewData(AddressDtoResponse newAddress)
        {
            await modalAddress.UpdateModalNewAddressAsync(newAddress);
        }

        private void HandleForm()
        {
            modalAddress.ResetForm();
        }

        private void HandleUserSelectedChanged(MyEventArgs<List<(string, AddressDtoRequest)>> address)
        {
            modalAddress.UpdateModalStatus(address.ModalStatus);
            modalCompanies.updateAddressSelection(address.Data!);
        }

        #endregion ModalAddress

        #region PaginationGrid

        private void HandlePaginationGrid(List<CompanyDtoResponse> newDataList)
        {
            CompaniesList = newDataList;
        }

        #endregion PaginationGrid

        #region FilterMethods

        private async Task CleanFilter()
        {
            companyDtoRequest = new();
            await GetCompanies();

        }
        private async Task SearchByFilter()
        {
            if (!string.IsNullOrEmpty(companyDtoRequest.BusinessName) || !string.IsNullOrEmpty(companyDtoRequest.Identification))
            {

                await GetCompanies();

            }
            else
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["EnterParameterSearch"], true, buttonTextCancel: "");
            }
        }

        #endregion

        #endregion OtherMethods

        #endregion Methods
    }
}