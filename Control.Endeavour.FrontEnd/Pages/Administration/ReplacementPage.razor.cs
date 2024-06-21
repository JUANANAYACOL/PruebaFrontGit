using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Replacement;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VReplacement.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
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
using System.Text.RegularExpressions;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class ReplacementPage
    {
        #region Variables

        #region Inject

        /* [Inject]
         private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Components

        private InputModalComponent UserInput { get; set; } = new();
        private InputModalComponent ReplacemetInput { get; set; } = new();

        private NotificationsComponentModal notificationModal { get; set; } = new();

        private NewPaginationComponent<VReplacementDtoResponse, ReplacementFilterDtoRequest> paginationComponent { get; set; } = new();

        #endregion Components

        #region Modals

        private GenericSearchModal genericSearchModal { get; set; } = new();
        private ReplacementModal repalcementModal { get; set; } = new();

        #endregion Modals

        #region Models

        private NotificationService NotificationService { get; set; } = new();

        private VReplacementDtoResponse profileToUpdate { get; set; } = new();
        private DeleteGeneralDtoRequest deleteRequest { get; set; } = new();
        private PaginationInfo paginationInfo { get; set; } = new();

        private MetaModel meta { get; set; } = new() { PageSize = 10 };

        private ReplacementFilterDtoRequest replacementFilterDtoRequest { get; set; } = new();

        #endregion Models

        #region Environments bool

        private bool ExportAllPages { get; set; }
        public bool CancelExport { get; set; }

        private bool dataChargue { get; set; } = false;

        #endregion Environments bool

        #region Enviroments string

        private string patternNumeric { get; set; } = @"\d";
        private string UriFilterReplacement { get; set; } = "administration/Replacement/ByFilter";

        private string UserFullName { get; set; } = string.Empty;
        private string ReplacementFullName { get; set; } = string.Empty;

        #endregion Enviroments string

        #region Enviroments (DateTime)

        private DateTime? from { get; set; }
        private DateTime? to { get; set; }

        private DateTime minValueFrom { get; set; } = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime minValueTo { get; set; } = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime maxValueTo { get; set; } = new DateTime(3900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion Enviroments (DateTime)

        #region Enviroments (List & Dictionary)

        private List<VReplacementDtoResponse> ReplacementList { get; set; } = new();
        private List<VReplacementDtoResponse> allRecords { get; set; }

        #endregion Enviroments (List & Dictionary)

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            await GetReplacements();
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        private void ShowModal()
        {
            repalcementModal.ResetForm();
            repalcementModal.UpdateModalStatus(true);
        }

        private async Task HandleReplacementModalStatusChangedAsync(bool status)
        {
            genericSearchModal.UpdateModalStatus(status);

            if (!status)
            {
                repalcementModal.UpdateModalStatus(status);
                await GetReplacements();
            }

            StateHasChanged();
        }

        private void HandleGenericSearchStatusChanged(MyEventArgs<VUserDtoResponse> user)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            repalcementModal.updateUserSelection(user!.Data!);
            genericSearchModal.UpdateModalStatus(user!.ModalStatus);
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private void ShowModalEdit(VReplacementDtoResponse replacement)
        {
            repalcementModal.UpdateModalStatus(true);
            repalcementModal.UpdateSelectedRemplacement(replacement);
        }

        private async Task GetReplacements()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                replacementFilterDtoRequest = new()
                {
                    StartDate = from,
                    EndDate = to,
                    Name = UserFullName,
                    ReplacementName = ReplacementFullName
                };

                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterReplacement, replacementFilterDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<VReplacementDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data!.Data != null && deserializeResponse.Data.Data.Count > 0)
                {
                    ReplacementList = deserializeResponse.Data.Data ?? new();

                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                    dataChargue = true;
                }
                else
                {
                    ReplacementList = new();
                    paginationInfo = new();
                    paginationComponent.ResetPagination(paginationInfo);
                    dataChargue = false;
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los perfiles de usuario: {ex.Message}");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private void HandlePaginationGrid(List<VReplacementDtoResponse> newDataList)
        {
            ReplacementList = newDataList;
        }

        private void HandleRecordToDelete(VReplacementDtoResponse args)
        {
            profileToUpdate = args;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, Translation["Yes"], Translation["No"]);
        }

        private async Task HandleModalNotiCloseAsync(ModalClosedEventArgs args)
        {
            if (args.IsAccepted && notificationModal.Type.Equals(ModalType.Warning))
            {
                deleteRequest.Id = profileToUpdate.ReplacementId;

                var responseApi = await HttpClient!.PostAsJsonAsync("administration/Replacement/DeleteReplacement", deleteRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                var NotiResult = NotificationService.NotificationDecrypt(Translation, deserializeResponse);
                notificationModal.UpdateModal(NotiResult.ModalType, NotiResult.NotificationMessage, true, Translation["Accept"]);
                if (NotiResult.Succeed)
                {
                    await GetReplacements();
                }
            }
        }

        #region updateMinValue

        public void updateMinValue()
        {
            if (from != null)
            {
                minValueTo = (DateTime)from;
            }

            StateHasChanged();
        }

        #endregion updateMinValue

        #region updateMaxValue

        public void updateMaxValue()
        {
            if (to != null)
            {
                maxValueTo = (DateTime)to;
            }

            StateHasChanged();
        }

        #endregion updateMaxValue

        private async Task OnClickReset()
        {
            from = null;
            to = null;

            UserFullName = string.Empty;
            ReplacementFullName = string.Empty;
            minValueFrom = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            minValueTo = DateTime.Today;
            maxValueTo = new DateTime(3900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            await GetReplacements();
            StateHasChanged();
        }

        public void replaceNumbers()
        {
            UserFullName = string.IsNullOrEmpty(UserFullName) ? string.Empty : Regex.Replace(UserFullName, patternNumeric, string.Empty);
            ReplacementFullName = string.IsNullOrEmpty(ReplacementFullName) ? string.Empty : Regex.Replace(ReplacementFullName, patternNumeric, string.Empty);

            StateHasChanged();
        }

        private async Task GetAllRecords()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                replacementFilterDtoRequest = new()
                {
                    StartDate = from,
                    EndDate = to,
                    Name = UserFullName,
                    ReplacementName = ReplacementFullName
                };

                var responseApi = await HttpClient!.PostAsJsonAsync("administration/Replacement/ByFilterList", replacementFilterDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<VReplacementDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data.Count > 0)
                {
                    allRecords = deserializeResponse.Data;
                }
                else
                {
                    allRecords = new();
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los perfiles de usuario: {ex.Message}");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
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
            else { args.Data = ReplacementList; }

            args.IsCancelled = CancelExport;
        }

        #endregion Methods
    }
}