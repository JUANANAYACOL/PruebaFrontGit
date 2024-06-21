using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.Permission
{
    public partial class PermissionModal
    {
        #region Variables
        #region Inject 
        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Components
        private NotificationsComponentModal notificationModal;

        private TelerikDropDownList<FuncionalityDtoResponse, int> SelectDropdown;


        #endregion


        #region Parameters
        [Parameter] public bool ModalStatusPermisos { get; set; } = false;

        [Parameter] public EventCallback<bool> OnStatusChanged { get; set; }
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }
        private bool habilitarFuncionalidad { get; set; } = true;
        private string btnSaveOEdit = "Save";
        private string DefaultTextGrid= "SelectFunctionality";
        private PermissionDtoResponse _selectedRecord { get; set; }
        [Parameter] public int PerfilID { get; set; }
        private int FunctionalityId { get; set; }
        private int PermissionId { get; set; }
        private bool SelectAccessF { get; set; }
        private bool SelectCreateF { get; set; }
        private bool SelectModifyF { get; set; }
        private bool SelectDeleteF { get; set; }
        private bool SelectConsultF { get; set; }
        private bool SelectPrintF { get; set; }
        private bool SelectActivoState { get; set; }

        #endregion

        #region Models

        private List<FuncionalityDtoResponse> FuncionalidadList;
        CreatePermissionDtoRequest CreatePermissionDtoRequest = new CreatePermissionDtoRequest();
        EditPermissionDtoRequest editPermissionDtoRequest = new EditPermissionDtoRequest();

        #endregion



        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            await GetFuncionality();
            if (DefaultTextGrid == "SelectFunctionality")
                DefaultTextGrid = Translation[DefaultTextGrid];
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;

        }


        #endregion

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion


        #region UpdateModalStatus
        public void UpdateModalStatus(bool newValue)
        {
            DefaultTextGrid = "SelectFunctionality";
            ModalStatusPermisos = newValue;
            StateHasChanged();
        }
        #endregion

        #region HandleModalClosed
        private void HandleModalClosed(bool status)
        {
            ModalStatusPermisos = status;
            StateHasChanged();
        }
        #endregion


        #region HandleModalNotiClose
        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }
            if (notificationModal.Type == ModalType.Error)
            {
                UpdateModalStatus(args.ModalStatus);
            }

        }
        #endregion



        #region UpdateSelectedRecord
        public void UpdateSelectedRecord(PermissionDtoResponse? record)
        {
            try
            {
                _selectedRecord = record;
                Limpiar();
                PermissionId = _selectedRecord.PermissionId;
                FunctionalityId = _selectedRecord.FunctionalityId;
                SelectAccessF = _selectedRecord.AccessF;
                SelectCreateF = _selectedRecord.CreateF;
                SelectModifyF = _selectedRecord.ModifyF;
                SelectConsultF = _selectedRecord.ConsultF;
                SelectDeleteF = _selectedRecord.DeleteF;
                SelectPrintF = _selectedRecord.PrintF;
                SelectActivoState = _selectedRecord.ActiveState;
                habilitarFuncionalidad = false;
                btnSaveOEdit = "Edit";
                DefaultTextGrid = _selectedRecord.FunctionalityName;

                SelectDropdown.Refresh();
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al obtener los permisos: {ex.Message}");

            }

        }
        #endregion


        #region Limpiar
        public void Limpiar()
        {
            SelectAccessF = false;
            SelectCreateF = false;
            SelectModifyF = false;
            SelectConsultF = false;
            SelectPrintF = false;
            SelectDeleteF = false;
            SelectActivoState = false;
            StateHasChanged();
        }
        #endregion


        #region PostPermission

        private async Task PostPermission()
        {
            CreatePermissionDtoRequest.ProfileId = PerfilID;
            CreatePermissionDtoRequest.FunctionalityId = FunctionalityId;
            CreatePermissionDtoRequest.UserId = 0;
            CreatePermissionDtoRequest.AccessF = SelectAccessF;
            CreatePermissionDtoRequest.CreateF = SelectCreateF;
            CreatePermissionDtoRequest.ModifyF = SelectModifyF;
            CreatePermissionDtoRequest.ConsultF = SelectConsultF;
            CreatePermissionDtoRequest.DeleteF = SelectDeleteF;
            CreatePermissionDtoRequest.PrintF = SelectPrintF;
            CreatePermissionDtoRequest.ActiveState = SelectActivoState;
            CreatePermissionDtoRequest.User = "Admin";


            var responseApi = await HttpClient.PostAsJsonAsync("permission/Permission/CreatePermission", CreatePermissionDtoRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<CreatePermissionDtoRequest>>();
            if (deserializeResponse.Succeeded)
            {
                //Logica Exitosa

                Limpiar();

                SelectDropdown.Refresh();
                notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                await OnChangeData.InvokeAsync(true);
            }
            else
            {
                //Logica no Exitosa
                notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
            }



        }
        #endregion

        #region PutPermission


        private async Task PutPermission()
        {
            editPermissionDtoRequest.AccessF = SelectAccessF;
            editPermissionDtoRequest.CreateF = SelectCreateF;
            editPermissionDtoRequest.ModifyF = SelectModifyF;
            editPermissionDtoRequest.ConsultF = SelectConsultF;
            editPermissionDtoRequest.DeleteF = SelectDeleteF;
            editPermissionDtoRequest.PrintF = SelectPrintF;
            editPermissionDtoRequest.ActiveState = SelectActivoState;
            editPermissionDtoRequest.PermissionId = PermissionId;
            editPermissionDtoRequest.User = "admin";


            var responseApi = await HttpClient.PostAsJsonAsync("permission/Permission/UpdatePermission", editPermissionDtoRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<EditPermissionDtoRequest>>();
            if (deserializeResponse.Succeeded)
            {
                //Logica Exitosa

                Limpiar();
                notificationModal.UpdateModal(ModalType.Success, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                await OnChangeData.InvokeAsync(true);
            }
            else
            {
                //Logica no Exitosa
                notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
            }



        }

        #endregion

        #region Save
        private async Task Guardar()
        {
            try
            {
                if (FunctionalityId != null && FunctionalityId != 0)
                {
                    if (habilitarFuncionalidad == true)
                    {
                        PostPermission();
                    }
                    else
                    {
                        PutPermission();

                    }
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["PleaseSelectFeature"], true, Translation["Accept"]);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar un permiso {ex.Message}");
            }
        }
        #endregion

        #region GetFuncionality

        public async Task GetFuncionality()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("PerProfileid");
                HttpClient?.DefaultRequestHeaders.Add("PerProfileid", PerfilID.ToString());
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<FuncionalityDtoResponse>>>("permission/Functionality/ByFilterprofileId");
                HttpClient?.DefaultRequestHeaders.Remove("PerProfileid");

                if (deserializeResponse.Succeeded)
                {
                    //Logica Exitosa
                    FuncionalidadList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<FuncionalityDtoResponse>();
                    if (FuncionalidadList.Count > 0)
                    {
                        DefaultTextGrid = "SelectFunctionality";
                        habilitarFuncionalidad = true;

                    }

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las funcionalidades: {ex.Message}");
            }
        }
        #endregion

        #endregion
    }
}
