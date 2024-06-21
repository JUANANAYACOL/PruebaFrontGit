using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Permission;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffice.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffices;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Security;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class PermissionPage
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

        #endregion

        #region Components
        private NotificationsComponentModal notificationModalSucces;
        private NotificationsComponentModal notificationModal;

        #endregion

        #region Modals
        private PermissionModal ModalPermission = new();
        private bool CrearEditar { get; set; } = true;
        #endregion

        #region Parameters
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }
        #endregion

        #region Models
        private PermissionDtoResponse recordToDelete { get; set; }
        #endregion

        #region Environments
        private PaginationComponent<PermissionDtoResponse, CreatePermissionDtoRequest> PaginationComponet { get; set; } = new();


        #region Environments(String)

        #endregion

        #region Environments(Numeric)
        private int IdPerfil { get; set; }
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)
        private bool Habilitar { get; set; } = true;
        #endregion

        #region Environments(List & Dictionary)
        private List<ProfilesDtoResponse> PerfilesList { get; set; }
        private List<PermissionDtoResponse> PermisosList = new List<PermissionDtoResponse>();
        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetProfile();
        }


        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

        #region ShowModalEdit
        private async Task ShowModalEdit(PermissionDtoResponse args)
        {
            CrearEditar = false;
            ModalPermission.UpdateModalStatus(true);
            ModalPermission.UpdateSelectedRecord(args);

        }
        #endregion

        #region ShowModalDelete
        private void ShowModalDelete(PermissionDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "DeleteModal");
        }
        #endregion


        #region ShowModal
        private async Task ShowModal()
        {
            ModalPermission.UpdateModalStatus(true);
            ModalPermission.PerfilID = IdPerfil;
            ModalPermission.Limpiar();
            await ModalPermission.GetFuncionality();

        }
        #endregion

        #region GetProfile

        private async Task GetProfile()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                ProfilesDtoResponse profilesDtoResponse = new ProfilesDtoResponse();
                var responseApi = await HttpClient.PostAsJsonAsync("permission/Profile/ByFilter", profilesDtoResponse);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ProfilesDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    //Logica Exitosa
                    PerfilesList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<ProfilesDtoResponse>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los perfiles: {ex.Message}");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);


        }
        #endregion

        #region GetPermission
        private async Task GetPermission()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("PerProfileid");
                HttpClient?.DefaultRequestHeaders.Add("PerProfileid", IdPerfil.ToString());
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<PermissionDtoResponse>>>("permission/Permission/ByFilterProfileId");
                HttpClient?.DefaultRequestHeaders.Remove("PerProfileid");
                if (deserializeResponse.Succeeded)
                {
                    //Logica Exitosa
                    Habilitar = false;
                    PermisosList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<PermissionDtoResponse>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los permisos: {ex.Message}");
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }


        #endregion

        #region HandleModalNotiClose
        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                if (recordToDelete != null && args.ModalOrigin.Equals("DeleteModal"))
                {
                    DeletePermissionDtoRequest deletePermissionDtoRequest = new();
                    deletePermissionDtoRequest.Id = recordToDelete.PermissionId;
                    deletePermissionDtoRequest.User = "Admin";

                    var responseApi = await HttpClient.PostAsJsonAsync("permission/Permission/DeletePermission", deletePermissionDtoRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse.Succeeded)
                    {
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true, Translation["Accept"]);
                        }
                    }
                    else
                    {
                        //Logica no Exitosa
                        notificationModalSucces.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"]);
                    }
                    await HandleRefreshGridDataAsync(true);

                }
            }
            else
            {
                Console.WriteLine("Registro No eliminado");
            }


        }
        #endregion

        #region HandleRefreshGridDataAsync
        public async Task HandleRefreshGridDataAsync(bool refresh)
        {
            await GetPermission();
        }
        #endregion

        #endregion

    }
}

