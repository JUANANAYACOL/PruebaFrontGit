using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Address;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.ThirdParty;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.ThirdUser;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Profile.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdParty.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdParty.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.Data.Filtering.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class ThirdPartyPage
    {
        #region Variables

        #region Inject

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject] private NavigationManager NavigationManager { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Components

        private InputModalComponent namesInput { get; set; } = new();
        private InputModalComponent emailInput { get; set; } = new();
        private InputModalComponent identificationInput { get; set; } = new();

        #endregion Components

        #region Modals

        private ThirdPartyModal modalThirdParty { get; set; } = new();
        private AddressModal modalAddress { get; set; } = new();
        private NotificationsComponentModal notificationModal { get; set; } = new();
        private ThirdUserModal ThirdUserModalRef { get; set; } = new();

        #endregion Modals

        #region Models

        public FunctionalityToJson? Permissions { get; set; } = new();
        private NotificationService NotificationService { get; set; } = new();
        private ThirdPartyFilterDtoRequest FilterDtoRequestPN { get; set; } = new() { PersonType = "PN" };
        private ThirdPartyFilterDtoRequest FilterDtoRequestPJ { get; set; } = new() { PersonType = "PJ" };
        private List<ThirdPartyDtoResponse> ThirdPartyListPN { get; set; } = new();
        private List<ThirdPartyDtoResponse> ThirdPartyListPJ { get; set; } = new();
        private List<ThirdPartyDtoResponse> allRecords { get; set; }
        private ThirdPartyDtoResponse thirdPartyToDelete { get; set; } = new();

        #endregion Models

        #region Environments

        //inputs

        private string FirstName { get; set; } = string.Empty;

        private string LastName { get; set; } = string.Empty;
        private string names { get; set; } = string.Empty;

        private string email { get; set; } = string.Empty;
        private string identification { get; set; } = string.Empty;
        
        private string UriFilterThirdParty { get; set; } = "administration/ThirdParty/ByFilter";
        private string patternNumeric { get; set; } = @"\d";

        //Tabs
        private int currentTab { get; set; } = 0;

        private bool dataChargue { get; set; } = false;

        private bool ExportAllPages { get; set; }
        public bool CancelExport { get; set; }

        private NewPaginationComponent<ThirdPartyDtoResponse, ThirdPartyFilterDtoRequest> paginationComponentPN { get; set; } = new();
        private NewPaginationComponent<ThirdPartyDtoResponse, ThirdPartyFilterDtoRequest> paginationComponentPJ { get; set; } = new();

        private PaginationInfo paginationInfoPN { get; set; } = new();
        private PaginationInfo paginationInfoPJ { get; set; } = new();

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await GetPermissions();
            //EventAggregator!.LanguageChangedEvent += HandleLanguageChanged;
            await GetThirdPartyAll();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region OtherMethods

        #region Gets

        #region GetThirdParty

        private async Task GetThirdPartyAll()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await ApplyFiltersAsync();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetThirdParty

        #region GetPNAndPJ

        //public async Task GetThirdPartyPN()
        //{
        //    try
        //    {
        //        FilterDtoRequestPN = new() { PersonType = "PN", ActiveState = false };
        //        var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterThirdParty, FilterDtoRequestPN);
        //        var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<ThirdPartyDtoResponse>>>();
        //        if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
        //        {
        //            ThirdPartyListPN = deserializeResponse.Data.Data ?? new();
        //            paginationInfoPN = deserializeResponse.Data.ExtractPaginationInfo();
        //            paginationComponentPN.ResetPagination(paginationInfoPN);
        //            StateHasChanged();
        //        }
        //        else
        //        {
        //            notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error al obtener personas naturales y juridicas: {ex.Message}");
        //    }
        //}

        //public async Task GetThirdPartyPJ()
        //{
        //    try
        //    {
        //        FilterDtoRequestPJ = new() { PersonType = "PJ", ActiveState = false };
        //        var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterThirdParty, FilterDtoRequestPJ);
        //        var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<ThirdPartyDtoResponse>>>();
        //        if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
        //        {
        //            ThirdPartyListPJ = deserializeResponse.Data.Data ?? new();
        //            paginationInfoPJ = deserializeResponse.Data.ExtractPaginationInfo();
        //            paginationComponentPJ.ResetPagination(paginationInfoPJ);
        //            StateHasChanged();
        //        }
        //        else
        //        {
        //            notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error al obtener personas naturales y juridicas: {ex.Message}");
        //    }
        //}

        #endregion GetPNAndPJ

        #endregion Gets

        #region RefreshGrid

        private async Task HandleRefreshGridData(bool refresh)
        {
            await GetThirdPartyAll();
        }

        #endregion RefreshGrid

        #region Filters

        #region ApplyFilters

        private async Task ApplyFiltersAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            try
            {
                ThirdPartyFilterDtoRequest request = new()
                {
                    PersonType = ( currentTab == 0 ? "PN" : "PJ" ),
                    Names = names,
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = email,
                    IdentificationNumber = identification,
                    ActiveState = false
                };

                if (request.PersonType.Equals("PN"))
                {
                    FilterDtoRequestPN = request;
                }
                else
                {
                    FilterDtoRequestPJ = request;
                }

                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterThirdParty, request.PersonType.Equals("PN") ? FilterDtoRequestPN : FilterDtoRequestPJ);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<ThirdPartyDtoResponse>>>();

                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data.TotalCount > 0)
                {
                    if (currentTab == 0)
                    {
                        ThirdPartyListPN = deserializeResponse.Data.Data ?? new();
                        paginationInfoPN = deserializeResponse.Data.ExtractPaginationInfo();
                        paginationComponentPN.ResetPagination(paginationInfoPN);
                    }
                    else
                    {
                        ThirdPartyListPJ = deserializeResponse.Data.Data ?? new();
                        paginationInfoPJ = deserializeResponse.Data.ExtractPaginationInfo();
                        paginationComponentPJ.ResetPagination(paginationInfoPJ);
                    }

                    dataChargue = true;
                }
                else
                {
                    if (currentTab == 0)
                    {
                        ThirdPartyListPN = new();
                        paginationInfoPN = new();
                        paginationComponentPN.ResetPagination(paginationInfoPN);
                    }
                    else
                    {
                        ThirdPartyListPJ = new();
                        paginationInfoPJ = new();
                        paginationComponentPJ.ResetPagination(paginationInfoPJ);
                    }
                    dataChargue = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener personas naturales y juridicas: {ex.Message}");
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
            StateHasChanged();
        }

        #endregion ApplyFilters

        #region ResetFilter

        public async Task ResetFiltersAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            FirstName = string.Empty;
            LastName = string.Empty;
            names = string.Empty;
            email = string.Empty;
            identification = string.Empty;
            ThirdPartyListPN.Clear();
            ThirdPartyListPJ.Clear();
            await GetThirdPartyAll();

            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ResetFilter

        #endregion Filters

        #region ModalThirdParty

        private async Task TabChangedHandler(int newIndex)
        {
            currentTab = newIndex;
            await ResetFiltersAsync();
        }

        private async Task ShowModal()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await modalThirdParty.PersonTypeSelectedModal(currentTab);
            modalThirdParty.UpdateModalStatus(true);
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private void HandleStatusChanged(bool status)
        {
            modalAddress.UpdateModalStatus(status);
        }

        private void HandleForm()
        {
            modalAddress.ResetForm();
        }

        #region HandleNewData

        private async Task HandleNewData(AddressDtoResponse newAddress)
        {
            await modalAddress.UpdateModalNewAddressAsync(newAddress);
        }

        #endregion HandleNewData

        private async Task ShowModalEdit(ThirdPartyDtoResponse record)
        {
            await modalThirdParty.PersonTypeSelectedModal(currentTab);
            modalThirdParty.UpdateModalStatus(true);
            await modalThirdParty.ReceiveThirdPartyAsync(record);
        }

        private void HandleAddressChanged(MyEventArgs<List<(string, AddressDtoRequest)>> address)
        {
            modalAddress.UpdateModalStatus(address.ModalStatus);
            modalThirdParty.updateAddressSelection(address.Data!);
        }

        #endregion ModalThirdParty

        #region HandleRecordToDelete

        private void HandleRecordToDelete(ThirdPartyDtoResponse record)
        {
            thirdPartyToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "DeleteModal");
        }

        #endregion HandleRecordToDelete

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            try
            {
                if (args.IsAccepted && args.ModalOrigin!.Equals("DeleteModal") && notificationModal.Type.Equals(ModalType.Warning))
                {
                    DeleteGeneralDtoRequest request = new() { Id = thirdPartyToDelete.ThirdPartyId, User = "user" };
                    var responseApi = await HttpClient!.PostAsJsonAsync("administration/ThirdParty/DeleteThirdParty", request);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    var NotiResult = NotificationService.NotificationDecrypt(Translation, deserializeResponse);
                    notificationModal.UpdateModal(NotiResult.ModalType, NotiResult.NotificationMessage, true, Translation["Accept"]);
                    if (NotiResult.Succeed)
                    {
                        await HandleRefreshGridData(true);
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
        }

        private async Task HandleThirdUsers(ThirdPartyDtoResponse record)
        {
            await ThirdUserModalRef.UpdateModalStatus(true);
            await ThirdUserModalRef.UpdateThirdUserList(record.ThirdPartyId);
        }

        #region HandlePaginations

        #region HandlePaginationPN

        private void HandlePaginationGridPN(List<ThirdPartyDtoResponse> newDataList)
        {
            ThirdPartyListPN = newDataList;
        }

        #endregion HandlePaginationPN

        #region HandlePaginationPJ

        private void HandlePaginationGridPJ(List<ThirdPartyDtoResponse> newDataList)
        {
            ThirdPartyListPJ = newDataList;
        }

        #endregion HandlePaginationPJ

        #endregion HandlePaginations

        public void replaceNumbers()
        {
            if (currentTab == 0)
            {
                names = string.IsNullOrEmpty(names) ? string.Empty : Regex.Replace(names, patternNumeric, string.Empty);
            }

            StateHasChanged();
        }

        #endregion OtherMethods

        #region GetPermissions

        private async Task GetPermissions()
        {
            try
            {
                string nameView = NavigationManager.Uri.Remove(0, NavigationManager.BaseUri.Length);

                if (!nameView.Equals("Home") && !nameView.Equals("UserProfile"))
                {
                    HttpClient?.DefaultRequestHeaders.Remove("viewName");
                    HttpClient?.DefaultRequestHeaders.Add("viewName", nameView);
                    var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FunctionalityToJson>>("access/ViewsFuncionality/FuncionalityPerViewName");
                    HttpClient?.DefaultRequestHeaders.Remove("viewName");

                    if (deserializeResponse!.Succeeded)
                    {
                        Permissions = deserializeResponse.Data ?? new();
                    }
                }
            }
            catch
            {
                Permissions = new();
            }

            StateHasChanged();
        }

        private async Task GetAllRecords()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            try
            {
                ThirdPartyFilterDtoRequest request = new()
                {
                    PersonType = ( currentTab == 0 ? "PN" : "PJ" ),
                    Names = names,
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = email,
                    IdentificationNumber = identification,
                    ActiveState = false
                };

                if (request.PersonType.Equals("PN"))
                {
                    FilterDtoRequestPN = request;
                }
                else
                {
                    FilterDtoRequestPJ = request;
                }

                var responseApi = await HttpClient!.PostAsJsonAsync("administration/ThirdParty/ByFilterList", request.PersonType.Equals("PN") ? FilterDtoRequestPN : FilterDtoRequestPJ);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ThirdPartyDtoResponse>>>();

                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data.Count > 0)
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
                Console.WriteLine($"Error al obtener personas naturales y juridicas: {ex.Message}");
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
            StateHasChanged();
        }

        public async Task OnBeforeExcelExport(GridBeforeExcelExportEventArgs args)
        {
            if (ExportAllPages)
            {
                await GetAllRecords(); //aquí lo cambian por su método de consumir todos los registros

                if (allRecords.Any())
                {
                    args.Data = allRecords;
                }
            }
            else
            {
                if (currentTab == 0)
                {
                    args.Data = ThirdPartyListPN;
                    paginationComponentPN.ResetPagination(paginationInfoPN);
                }
                else
                {
                    args.Data = ThirdPartyListPJ;
                }
            }

            args.IsCancelled = CancelExport;
        }

        #endregion GetPermissions

        #endregion Methods
    }
}