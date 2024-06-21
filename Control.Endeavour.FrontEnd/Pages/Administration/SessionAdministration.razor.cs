using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VReplacement.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Telerik.Blazor;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class SessionAdministration
    {

        #region Variables

        #region Inject 


        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }
        #endregion

        #region Components

        private NewPaginationComponent<BlockedUsersDtoResponse, BlockerUsersFilterDtoRequest> paginationComponent { get; set; } = new();
        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal { get; set; } = new();

        #endregion

        #region Parameters


        #endregion

        #region Models
        private BlockerUsersFilterDtoRequest Filter { get; set; } = new();
        private PaginationInfo paginationInfo = new();
        #endregion

        #region Environments

        #region Environments(String)
        private string patternNumeric { get; set; } = @"\d";
        private string UriFilterBlocked { get; set; } = "security/User/ByFilterUserBlocks";
        private string Firstname { get; set; } = string.Empty;
        private string Lastname { get; set; } = string.Empty;

        #endregion

        #region Environments(Numeric)
        public int UserNoveltyToUnlock { get; set; } = 0;
        #endregion

        #region Environments(DateTime)
        public DateTime? DateToSearch { get; set; } = null;

        private DateTime minValue { get; set; } = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        #endregion

        #region Environments(Bool)
        private bool dataChargue { get; set; } = false;
        #endregion

        #region Environments(List & Dictionary)
        private List<BlockedUsersDtoResponse> BlockedList { get; set; } = new();
        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            await GetBlockedUsers();

        }





        #endregion
        #region Methods
        #region HandleMethods



        private void HandlePaginationGrid(List<BlockedUsersDtoResponse> newDataList)
        {
            BlockedList = newDataList;
        }

        private async Task HandleModalNotiCloseAsync(ModalClosedEventArgs args)
        {
            if (notificationModal.Type.Equals(ModalType.Warning) && args.IsAccepted)
            {
                await unblockUser();

            }

        }




        #endregion

        #region OthersMethods



        private async Task OnClickReset()
        {

            Firstname = string.Empty;
            Lastname = string.Empty;
            DateToSearch = null;
            UserNoveltyToUnlock = 0;
            await GetBlockedUsers();
            StateHasChanged();
        }


        private void UnblockUserSelection(BlockedUsersDtoResponse UserToUnlock)
        {
            UserNoveltyToUnlock = UserToUnlock.UserNoveltyId;
            notificationModal.UpdateModal(ModalType.Warning, Translation["UnblockQuestion"], true, Translation["Accept"], Translation["Cancel"]);
        }

        private async Task GetBlockedUsers()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                Filter = new()
                {
                    FirstName = Firstname,
                    LastName = Lastname,
                    DateBlock=DateToSearch

                };

                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterBlocked, Filter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<BlockedUsersDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data!.Data.Any())
                {
                    BlockedList = deserializeResponse.Data.Data ?? new();

                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                    dataChargue = true;
                }
                else
                {
                    BlockedList = new();
                    paginationInfo = new();
                    paginationComponent.ResetPagination(paginationInfo);
                    dataChargue = false;
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error al obtener los perfiles de usuario: {ex.Message}");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task unblockUser()
        {


            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                SeriesUpdateDtoRequest seriesUpdateDto = new();


                HttpClient?.DefaultRequestHeaders.Remove("noveltyId");
                HttpClient?.DefaultRequestHeaders.Add("noveltyId", UserNoveltyToUnlock.ToString());
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<bool>>("security/User/UpdateBlockNovelty");
                HttpClient?.DefaultRequestHeaders.Remove("noveltyId");

                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                    await GetBlockedUsers();
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
                }


            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);

        }


        public void replaceNumbers()
        {
            Firstname = string.IsNullOrEmpty(Firstname) ? string.Empty : Regex.Replace(Firstname, patternNumeric, string.Empty);
            Lastname = string.IsNullOrEmpty(Lastname) ? string.Empty : Regex.Replace(Lastname, patternNumeric, string.Empty);

            StateHasChanged();
        }



        #endregion

        #endregion

    }
}
