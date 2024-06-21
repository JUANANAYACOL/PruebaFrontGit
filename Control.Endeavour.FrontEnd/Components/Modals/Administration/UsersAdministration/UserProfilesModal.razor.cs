using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Drawing.Text;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.UsersAdministration
{
    public partial class UserProfilesModal
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

        private PaginationComponent<ProfilesDtoResponse, ProfileByFilterDtoRequest> paginationComponetPost = new();

        #endregion Components

        #region Modals

        private NotificationsComponentModal notificationModal = new();

        #endregion Modals

        #region Parameters

        [Parameter] public EventCallback<IEnumerable<ProfilesDtoResponse>> OnProfilesSelected { get; set; }
        [Parameter] public int CompanyID { get; set; }
        [Parameter] public bool IsEditForm { get; set; }

        #endregion Parameters

        #region Models

        private MetaModel ProfilesMeta = new();
        private ProfileByFilterDtoRequest profileByFilterDto = new();

        #endregion Models

        #region Environments



        #region Environments(Bool)
        private bool modalStatus = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<ProfilesDtoResponse>? lstProfilesByCompanyID { get; set; } = new List<ProfilesDtoResponse>();
        private IEnumerable<ProfilesDtoResponse> SelectedProfiles { get; set; } = Enumerable.Empty<ProfilesDtoResponse>();

        private List<int> SelectdProfilesId { get; set; } = new List<int>();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            try
            {
                //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
                lstProfilesByCompanyID = new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar la initialización: {ex.Message}");
            }
            //PageLoadService.OcultarSpinnerReadLoad(Js);
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private void HandlePaginationGrid(List<ProfilesDtoResponse> newDataList)
        {
            lstProfilesByCompanyID = newDataList;
        }

        private void HandleModalClosed(bool status)
        {
            SelectedProfiles = Enumerable.Empty<ProfilesDtoResponse>();

            modalStatus = status;
            StateHasChanged();
        }

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                modalStatus = args.ModalStatus;
            }
        }

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleMethods

        #region OthersMethods

        #region Get Data Methods

        private async Task GetProfileByCompany(int companyID)
        {
            try
            {
                lstProfilesByCompanyID = new();
                profileByFilterDto.CompanyId = companyID;

                var responseApi = await HttpClient.PostAsJsonAsync("permission/Profile/ByFilter", profileByFilterDto);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ProfilesDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    lstProfilesByCompanyID = deserializeResponse.Data;
                    ProfilesMeta = deserializeResponse.Meta;
                    paginationComponetPost.ResetPagination(ProfilesMeta);
                }
                else
                {
                    lstProfilesByCompanyID = new();
                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los perfiles del usuario: {ex.Message}");
            }
        }

        #endregion Get Data Methods

        #region ModalMethods

        public async Task UpdateModalStatusAsync(bool newValue, int companyId = 17)
        {
            await GetProfileByCompany(companyId);
            modalStatus = newValue;
            StateHasChanged();
        }

        protected void OnSelect(IEnumerable<ProfilesDtoResponse> profiles)
        {
            SelectedProfiles = profiles;
        }

        private async Task SendProfilesId()
        {
            await OnProfilesSelected.InvokeAsync(SelectedProfiles);
        }

        #endregion ModalMethods

        #endregion OthersMethods

        #endregion Methods
    }
}