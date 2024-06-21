using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Address;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.BranchOffice;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffice.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffices;
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

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class BranchOfficesPage
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

        private InputModalComponent BranchOfficeInput { get; set; } = new();

        private InputModalComponent CodeInput { get; set; } = new();

        private NewPaginationComponent<BranchOfficesDtoResponse, BranchOfficeFilterDtoRequest> PaginationComponent = new();

        #endregion Components

        #region Environments(String)

        private string BranchOfficeName { get; set; } = string.Empty;
        private string Code { get; set; } = string.Empty;
        private string UriFilterBranchOffice = "params/BranchOffice/ByFilter";

        #endregion Environments(String)

        #region Environments (bool)

        private bool dataChargue { get; set; } = new();

        #endregion Environments (bool)

        #region Modals

        private AddressModal modalAddress { get; set; } = new();
        private BranchOfficesModal modalbranchOffice { get; set; } = new();
        private NotificationsComponentModal notificationModal { get; set; } = new();

        #endregion Modals

        #region Models

        private PaginationInfo paginationInfo = new();
        private List<BranchOfficesDtoResponse> branchOfficesList { get; set; } = new();

        private MetaModel meta { get; set; } = new() { PageSize = 10 };
        private BranchOfficeFilterDtoRequest? FilterDtoRequest { get; set; } = new();

        private BranchOfficesDtoResponse recordToDelete { get; set; } = new();

        #endregion Models

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            ////EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            try
            {
                await GetBranchsOffices();
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
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

        #region GetBranchsOffices

        // Método para obtener la lista de sucursales.
        private async Task GetBranchsOffices()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                FilterDtoRequest = new()
                {
                    Code = this.Code,
                    NameOffice = this.BranchOfficeName
                };
                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterBranchOffice, FilterDtoRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<BranchOfficesDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data.Data.Any())
                {
                    branchOfficesList = deserializeResponse.Data.Data;
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    PaginationComponent.ResetPagination(paginationInfo);
                    dataChargue = true;
                }
                else
                {
                    branchOfficesList = new List<BranchOfficesDtoResponse>();
                    dataChargue = false;
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores en caso de fallo al obtener las sucursales.
                Console.WriteLine($"Error al obtener las sucursales: {ex.Message}");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetBranchsOffices

        #region HandleId

        private async Task HandleId(int id)
        {
            await modalAddress.UpdateModalIdAsync(id);
        }

        #endregion HandleId

        #region ShowModalEdit

        private async Task ShowModalEdit(BranchOfficesDtoResponse record)
        {
            modalbranchOffice.UpdateModalStatus(true);
            await modalbranchOffice.recieveBranchOfficeAsync(record);
        }

        #endregion ShowModalEdit

        #region ShowModalDelete

        private void ShowModalDelete(BranchOfficesDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "DeleteModal");
        }

        #endregion ShowModalDelete

        #region ShowModalCreate

        private void ShowModalCreate()
        {
            modalbranchOffice.UpdateModalStatus(true);
            modalAddress.ResetFormAsync();
        }

        #endregion ShowModalCreate

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            try
            {
                if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new();
                    request.Id = recordToDelete.BranchOfficeId;
                    request.User = "Front"; // Cambiar por la varibale de session del usuario
                    var responseApi = await HttpClient!.PostAsJsonAsync("params/BranchOffice/DeleteBranchOffice", request);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        await GetBranchsOffices();
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true);
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
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
        }

        #endregion HandleModalNotiClose

        private async Task HandleNewData(AddressDtoResponse newAddress)
        {
            await modalAddress.UpdateModalNewAddressAsync(newAddress);
        }

        #region HandleRefreshGridDataAsync

        private async Task HandleRefreshGridDataAsync(bool refresh)
        {
            await GetBranchsOffices();
        }

        #endregion HandleRefreshGridDataAsync

        #region HandleAddressModal

        private void HandleAddressModal(bool newValue)
        {
            modalAddress.UpdateModalStatus(newValue);
        }

        #endregion HandleAddressModal

        #region HandleUserSelectedChanged

        private void HandleAddressSelectedChanged(MyEventArgs<List<(string, AddressDtoRequest)>> address)
        {
            modalbranchOffice.updateAddressSelection(address.Data!);

            modalAddress.UpdateModalStatus(address.ModalStatus);
        }

        #endregion HandleUserSelectedChanged

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<BranchOfficesDtoResponse> newDataList)
        {
            branchOfficesList = newDataList;
        }

        #endregion HandlePaginationGrid

        #region OnClickReset

        private async Task OnClickReset()
        {
            Code = string.Empty;
            BranchOfficeName = string.Empty;
            await GetBranchsOffices();
            StateHasChanged();
        }

        #endregion OnClickReset

        #endregion OthersMethods

        #endregion Methods
    }
}