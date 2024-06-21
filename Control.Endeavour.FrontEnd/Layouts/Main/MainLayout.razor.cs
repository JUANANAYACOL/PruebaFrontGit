using Control.Endeavour.Frontend.Client.Models.ComponentViews.Menu.Request;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Views.Menu;
using Control.Endeavour.FrontEnd.Models.Enums.Generic;
using Control.Endeavour.FrontEnd.Components.Components.User;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.CodeRecovery.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Interfaces.Authentication;
using Control.Endeavour.FrontEnd.Services.Interfaces.Storage;
using Control.Endeavour.FrontEnd.Services.Services.Authentication;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.SvgIcons;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Captcha;
using Control.Endeavour.FrontEnd.StateContainer.Filing;
using Control.Endeavour.FrontEnd.Models.Models.Menu.Request;
using System;
using Control.Endeavour.FrontEnd.Resources;
using Microsoft.Extensions.Localization;
using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Response;
using Control.Endeavour.FrontEnd.StateContainer.Documents;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Profile.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Components.Components.Inactivity;
using Control.Endeavour.FrontEnd.Components.Modals.SearchEngine;

namespace Control.Endeavour.FrontEnd.Layouts.Main
{
    public partial class MainLayout
    {
        #region Variables

        #region Inject

        //[Inject] private EventAggregatorService? EventAggregator { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        [Inject] private IJSRuntime Js { get; set; }
        [Inject] private RenewTokenService RenewToken { get; set; }
        [Inject] private ISessionStorage SessionStorage { get; set; }
        [Inject] private HttpClient? HttpClient { get; set; }
        [Inject] private IAuthenticationJWT? AuthenticationJWT { get; set; }
        [Inject] private ProfileStateContainer authenticationStateContainer { get; set; }
        [Inject] private FilingStateContainer? FilingSC { get; set; }
        [Inject] private IConfiguration? Configuration { get; set; }
        [Inject] private IStringLocalizer<Translation>? Translation { get; set; }

        [Inject]
        private DocumentsStateContainer? documentsStateContainer { get; set; }

        #endregion Inject

        #region Parameters

        [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }

        #endregion Parameters

        #region Models

        private InactivityComponent inactivityComponent { get; set; } = new();
        private SubMenu subMenuInstance = new();
        private NotificationsComponentModal notificationModal = new();
        private SearchEngineModal searchEngineModal = new();

        //public SpinnerCargandoComponent spinerLoader = new();
        private UserDtoResponse userDataToShow = new();

        private FileDtoResponse fileData = new();
        public FunctionalityToJson? PermissionsList { get; set; } = null;

        #endregion Models

        #region Environments

        #region Environments(String)

        private string navbarCollapseCss => isNavbarCollapsed ? "collapse navbar-collapse" : "navbar-collapse";
        private string DropdownMenuCss => isDropdownOpen ? "dropdown-menu show" : "dropdown-menu";

        private string subDefault = "";
        private string subNavFiling = "d-none";
        private string subNavManagement = "d-none";
        private string subNavDocumentaryTasks = "d-none";
        private string subNavRecord = "d-none";
        private string subNavSearchers = "d-none";
        private string version { get; set; } = string.Empty;

        private string SourceUserPicture { get; set; } = string.Empty;

        #region QuickAccessMenu

        private string HomePage = "Home";
        private string FilingPage = "Filing";
        private string FilingInternalPage = "FilingInternal";
        private string FilingSentPage = "FilingSent";
        private string MassiveFiling = "MassiveFiling";
        private string[] validRoutes = new[] { "Filing", "FilingInternal", "FilingSent", "MassiveFiling" };
        private string CreateDocumentaryTaskPage = "CreateDocumentaryTask";
        private string DocumentaryTaskTrayPage = "DocumentaryTaskTray";
        private string ManagementTrayPage = "ManagementTray";
        private string RecordConsultPage = "RecordsConsult";
        private string RecordClosingOpeningPage = "RecordClosingOpening";
        private string DocumentSearchEnginePage = "DocumentSearchEngine";

        // <--[Iconos del Menu]-->

        private string _default = "";
        private string body_container = "bc";
        private string IconMenu = "../img/menu/iconMenu.svg";
        private string HomeImage = "../img/menu/inicio.svg";

        #region Acciones Radicacion

        private string FilingImage = "../img/menu/radicacion.svg";
        private string FilingReceived = "../img/menu/ventanillaRecibida.svg";
        private string FilingInternal = "../img/menu/ventanillaInterna.svg";
        private string Filingsent = "../img/menu/ventanillaEnviada.svg";
        private string FilingUnofficial = "../img/menu/ventanillaNoOficial.svg";
        private string FilingFaster = "../img/menu/ventanillaRapida.svg";
        private string MassiveRadiation = "../img/menu/ventanillaRapida.svg";

        #endregion Acciones Radicacion

        #region Gestion

        private string ManagementImage = "../img/menu/gestion.svg";
        private string ManagementTray = "../img/menu/gestionsubItem.svg";
        private string ManagementBoard = "../img/menu/tableroControl.svg";

        #endregion Gestion

        #region Tareas Documentales

        private string DocumentaryTasksImage = "../img/menu/tareasDocumentales.svg";
        private string DocumentaryTasksEditor = "../img/menu/editorTexto.svg";
        private string DocumentaryTasksSpreadsheet = "../img/menu/hojaCalculo.svg";
        private string DocumentaryTasksTray = "../img/menu/bandejaTareasSubItem.svg";

        #endregion Tareas Documentales

        #region Expedientes

        private string RecordImage = "../img/menu/expedientes.svg";
        private string RecordConsult = "../img/menu/consultaExpediente.svg";
        private string RecordAdministration = "../img/menu/administraacionExp.svg";
        private string RecordRequest = "../img/menu/solicitudPrestamo.svg";
        private string RecordClosingOpening = "../img/menu/consultaExpediente.svg";

        #endregion Expedientes

        #region Buscadores

        private string SearchersImage = "../img/menu/buscadores.svg";
        private string SearchersConsult = "../img/menu/consultaDocumentos.svg";
        private string SearchersSearch = "../img/menu/busquedaRapida.svg";

        #endregion Buscadores

        private string BpmImage = "../img/menu/bpm.svg";
        private string EnvironmentalImpactImage = "../img/menu/impactoAmbiental.svg";
        private string LogoutImage = "../img/menu/cerrarSesion.svg";

        #endregion QuickAccessMenu

        #endregion Environments(String)

        #region Environments(Numeric)

        private int activeIndex = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        public bool ValidAccess { get; set; } = false;
        public bool radicacipnActiva { get; set; } = false;
        private bool isNavbarCollapsed = true;
        private bool isDropdownOpen = false;
        private bool darkMode = false;
        private bool showNewMenu = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        // Lista de Menus de la BD
        private List<MenuModels> MenusModels { get; set; } = new List<MenuModels>();

        private List<string> ShortCutsToDeploy { get; set; } = new();

        // Diccionario que agrupa los diccionarios anteriores por nivel.
        private Dictionary<int, Dictionary<int, bool>> expandedItemsByLevel = new Dictionary<int, Dictionary<int, bool>>
        {
            { 1, new Dictionary<int, bool>() },
            { 2, new Dictionary<int, bool>() },
            { 3, new Dictionary<int, bool>() },
            { 4, new Dictionary<int, bool>() },
        };

        private List<AppKeysDtoResponse> LstAppKeys = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                version = Configuration["version"].ToString();

                var authState = await AuthenticationState;
                if (!authState.User.Identity.IsAuthenticated)
                {
                    NavigationManager.NavigateTo("");
                }
                else
                {
                    await GetShortCuts();

                    try
                    {
                        string nameView = NavigationManager.Uri.Remove(0, NavigationManager.BaseUri.Length);

                        ValidAccess = true;

                        if (!nameView.Equals("Home") && !nameView.Equals("UserProfile"))
                        {
                            HttpClient?.DefaultRequestHeaders.Remove("viewName");
                            HttpClient?.DefaultRequestHeaders.Add("viewName", nameView);
                            var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FunctionalityToJson>>("access/ViewsFuncionality/FuncionalityPerViewName");
                            HttpClient?.DefaultRequestHeaders.Remove("viewName");

                            if (!deserializeResponse!.Succeeded)
                            {
                                ValidAccess = false;
                                PermissionsList = new();

                                NavigationManager?.NavigateTo("404");
                            }

                            PermissionsList = deserializeResponse.Data ?? new();
                        }
                    }
                    catch (Exception)
                    {
                        NavigationManager?.NavigateTo("404");
                    }
                    await GetUserInfo();
                    await GetMenus();
                    var timeExpiration = await SessionStorage.GetValue<string>(ValuesKeysEnum.TimeExpiration);
                    var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(timeExpiration));
                    var timeExpirationMinute = (DateTime.Parse(dateTimeOffset.LocalDateTime.ToString()) - DateTime.Now).Minutes;
                    RenewToken.Start(timeExpirationMinute);
                    darkMode = await Js.InvokeAsync<bool>("checkTheme"); // Verificar Theme almacenado en el localStorage
                    changeImage(darkMode);
                    StateHasChanged();
                }
            }
            catch
            {
                NavigationManager.NavigateTo("");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.ModalOrigin == "CerrarSesion")
            {
                if (args.IsAccepted)
                {
                    var response = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<bool>>("security/Session/UpdateLogout");

                    if (response.Succeeded)
                    {
                        await AuthenticationJWT.LogoutToken();
                        NavigationManager?.NavigateTo("");
                        authenticationStateContainer.SelectedComponentChanged("Login");
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["SigningOutError"], true, Translation["Accept"], "", "", "");
                    }
                }
            }
            else if (args.ModalOrigin.Contains("~"))
            {
                if (args.IsAccepted)
                {
                    var valores = args.ModalOrigin.Split('~');

                    if (valores[2] == "task")
                    {
                        NavigationManager.NavigateTo("/" + valores[0]);
                        ModificarSubnavItem(ref valores[1]);
                        documentsStateContainer.documentResponseComentaryClosed = new();
                        documentsStateContainer.ActiveTask = false;
                    }
                    else
                    {
                        NavigationManager.NavigateTo("/" + valores[0]);
                        ModificarSubnavItem(ref valores[1]);
                    }
                }
            }
        }

        private void ModificarSubnavItem(ref string subnavItem)
        {
            subnavItem = "d-none";
        }

        /// <summary>
        /// Verifica si un menu tiene submenus/opciones de nivel 1 o si esta activo.
        /// </summary>
        /// <param name="menu">Objeto de tipo Menu</param>
        /// <returns>True si el MenuItems1s del menu no esta vacio</returns>
        private bool HasSubMenuItems(MenuModels menu)
        {
            return menu.MenuItems1s != null;
        }

        /// <summary>
        /// Verifica si un submenú de nivel 1 tiene una vista, si no es asi significa que tiene opciones.
        /// </summary>
        /// <param name="menuItem1">Objeto de tipo menuItem1</param>
        /// <returns>True si el menuItem1 tiene una vista null</returns>
        private bool HasSubMenuItems(MenuItems1 menuItem1)
        {
            return menuItem1.View == null;
        }

        /// <summary>
        /// Verifica si un submenu de nivel 2 tiene una vista, si no es asi significa que tiene opciones.
        /// </summary>
        /// <param name="menuItem2">Objeto de tipo menuItem2</param>
        /// <returns>True si el menuItem2 tiene una vista null</returns>
        private bool HasSubMenuItems(MenuItems2 menuItem2)
        {
            return menuItem2.View == null;
        }

        #endregion HandleMethods

        #region GetMethods

        private async Task GetMenus()
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<MenuModels>>>("access/ViewsFuncionality/ByFilterToken");

                if (response.Succeeded)
                {
                    MenusModels = response.Data;
                }
                else
                {
                    Console.WriteLine("No hay menus disponibles");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los perfiles de usuario: {ex.Message}");
            }
        }

        private async Task GetUserInfo()
        {
            var response = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<UserDtoResponse>>("security/User/ByFilterToken");

            if (response.Succeeded)
            {
                userDataToShow = response.Data;
                await GetProfilePicture(userDataToShow.PictureFileId.ToString());
            }
            else
            {
                Console.WriteLine("No hay menus disponibles");
            }
        }

        #endregion GetMethods

        #region PostMethods

        private async Task SignOff()
        {
            try
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["CloseSessionMessage"], true, Translation["Yes"], Translation["No"], modalOrigin: "CerrarSesion");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cerrar sesión: {ex.Message}");
            }
        }

        #endregion PostMethods

        #region ToggleMethods

        private void ToggleNavbar()
        {
            isNavbarCollapsed = !isNavbarCollapsed;
        }

        private void ToggleDropdown()
        {
            isDropdownOpen = !isDropdownOpen;
        }

        private void ToggleNewMenu()
        {
            showNewMenu = !showNewMenu;
            CloseAllMenus();
        }

        /// <summary>
        /// Verifica si esta expandido y cierra otros niveles
        /// </summary>
        /// <param name="id">El id del menu o submenu</param>
        /// <param name="level">El nivel en el que se desea verificar la expansion</param>
        private void ToggleSubMenu(int id, int level, string NameView, List<ViewParameters> parameters = null)
        {
            if (!string.IsNullOrEmpty(NameView))
            {
                if (parameters != null && parameters.Count > 0)
                {
                    string _parameters = parameters.First().Value;
                    NavigationManager.NavigateTo("/" + NameView + "/{" + _parameters + "}");
                    showNewMenu = false;
                }
                else
                {
                    NavigationManager.NavigateTo("/" + NameView);
                    showNewMenu = false;
                }
            }
            // Antes de abrir un nuevo menu, cierra los menus previamente abiertos.
            CloseMenusInSameOrLowerLevel(id, level);
            // Cambia el estado de expansion del menú seleccionado.
            ToggleExpansion(id, level);
        }

        /// <summary>
        /// Cambia el estado de expansion del menu en el nivel especificado.
        /// </summary>
        /// <param name="id">El id del menu o submenu</param>
        /// <param name="level">El nivel en el que se desea verificar la expansion</param>
        private void ToggleExpansion(int id, int level)
        {
            for (int i = 1; i <= level; i++)
            {
                if (!expandedItemsByLevel.ContainsKey(i))
                {
                    expandedItemsByLevel[i] = new Dictionary<int, bool>();
                }
            }
            if (expandedItemsByLevel.TryGetValue(level, out var expandedItems))
            {
                if (expandedItems.ContainsKey(id))
                {
                    expandedItems[id] = !expandedItems[id];
                }
                else
                {
                    expandedItems[id] = true;
                }
            }
        }

        private async Task ToggleTheme()
        {
            darkMode = await Js.InvokeAsync<bool>("toggleTheme");
            changeImage(darkMode);
        }

        #endregion ToggleMethods

        #region OthersMethods

        private void SetActive(int index)
        {
            activeIndex = index;
        }

        /// <summary>
        /// Verifica si un menu esta expandido en un nivel determinado (menu principal o subniveles de este)
        /// </summary>
        /// <param name="id">El id del menu o submenu</param>
        /// <param name="level">El nivel en el que se desea verificar la expansion</param>
        /// <returns>Si el menu o submenu está expandido en el nivel especificado sera True; de lo contrario, false.</returns>
        private bool IsExpanded(int id, int level)
        {
            if (expandedItemsByLevel.TryGetValue(level, out var expandedItems))
            {
                return expandedItems.ContainsKey(id) && expandedItems[id];
            }

            return false;
        }

        /// <summary>
        /// Cierra los menús y niveles anteriormente abiertos.
        /// </summary>
        /// <param name="id">El id del menu o submenu</param>
        /// <param name="level">El nivel en el que se desea verificar la expansion</param>
        public void CloseMenusInSameOrLowerLevel(int id, int level)
        {
            // Cerrar todos los menús en niveles iguales o inferiores.
            foreach (var kvp in expandedItemsByLevel)
            {
                var menuLevel = kvp.Key;
                var expandedItems = kvp.Value;

                if (menuLevel >= level)
                {
                    foreach (var key in expandedItems.Keys.ToList())
                    {
                        if (key != id && expandedItems[key])
                        {
                            expandedItems[key] = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cierra todos los menus.
        /// </summary>
        public void CloseAllMenus()
        {
            foreach (var expandedItems in expandedItemsByLevel.Values)
            {
                foreach (var key in expandedItems.Keys.ToList())
                {
                    if (expandedItems[key])
                    {
                        expandedItems[key] = false;
                    }
                }
            }
        }

        private void OnNavitigionMenu(ref string subnavItem, string Page, string Parameters = "")
        {
            if (showNewMenu)
            {
                ToggleNewMenu();
            }

            Uri uri = new Uri(NavigationManager.Uri);
            string[] pathSegments = uri.Segments;
            string lastSegment = pathSegments.Last().Trim('/');

            if (validRoutes.Contains(lastSegment))
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["FilingCloseQuestion"], true, Translation["Yes"], Translation["No"], modalOrigin: Page + "~" + subnavItem + "~ActiveFiling");
                subnavItem = "d-none";
            }
            else if (lastSegment.Equals(CreateDocumentaryTaskPage))
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["DocumentTaskCloseQuestion"], true, Translation["Yes"], Translation["No"], modalOrigin: Page + "~" + subnavItem + "~task");
                subnavItem = "d-none";
            }
            else
            {
                if (!string.IsNullOrEmpty(Parameters))
                {
                    NavigationManager.NavigateTo("/" + Page + "/{" + Parameters + "}");
                    subnavItem = "d-none";
                    StateHasChanged();
                }
                else
                {
                    NavigationManager.NavigateTo("/" + Page);
                    subnavItem = "d-none";
                    StateHasChanged();
                }
            }
        }

        private void OnCloseImageBox(ref string image, ref string subnavItem)
        {
            image = image.Contains("Hover.svg") ? image.Replace("Hover.svg", ".svg") : image;
            subnavItem = "d-none";
        }

        private void changeImage(bool darkMode)
        {
            if (darkMode)
            {
                #region Iconos SubMenu

                IconMenu = "../img/menu/iconMenuWhite.svg";
                HomeImage = "../img/menu/inicioWhite.svg";

                #region Acciones Radicacion

                FilingImage = "../img/menu/radicacionWhite.svg";
                FilingReceived = "../img/menu/ventanillaRecibidaWhite.svg";
                FilingInternal = "../img/menu/ventanillaInternaWhite.svg";
                Filingsent = "../img/menu/ventanillaEnviadaWhite.svg";
                FilingUnofficial = "../img/menu/ventanillaNoOficialWhite.svg";
                FilingFaster = "../img/menu/ventanillaRapidaWhite.svg";
                MassiveRadiation = "../img/menu/ventanillaRapidaWhite.svg";

                #endregion Acciones Radicacion

                #region Gestion

                ManagementImage = "../img/menu/gestionWhite.svg";
                ManagementTray = "../img/menu/gestionsubItemWhite.svg";
                ManagementBoard = "../img/menu/tableroControlWhite.svg";

                #endregion Gestion

                #region Tareas Documentales

                DocumentaryTasksImage = "../img/menu/tareasDocumentalesWhite.svg";
                DocumentaryTasksEditor = "../img/menu/editorTextoWhite.svg";
                DocumentaryTasksSpreadsheet = "../img/menu/hojaCalculoWhite.svg";
                DocumentaryTasksTray = "../img/menu/bandejaTareasSubItemWhite.svg";

                #endregion Tareas Documentales

                #region Expedientes

                RecordImage = "../img/menu/expedientesWhite.svg";
                RecordConsult = "../img/menu/consultaExpedienteWhite.svg";
                RecordAdministration = "../img/menu/administraacionExpWhite.svg";
                RecordRequest = "../img/menu/solicitudPrestamoWhite.svg";

                #endregion Expedientes

                #region Buscadores

                SearchersImage = "../img/menu/buscadoresWhite.svg";
                SearchersConsult = "../img/menu/consultaDocumentosWhite.svg";
                SearchersSearch = "../img/menu/busquedaRapidaWhite.svg";

                #endregion Buscadores

                BpmImage = "../img/menu/bpmWhite.svg";
                EnvironmentalImpactImage = "../img/menu/impactoAmbientalWhite.svg";
                LogoutImage = "../img/menu/cerrarSesionWhite.svg";

                #endregion Iconos SubMenu
            }
            else
            {
                #region Iconos SubMenu

                IconMenu = "../img/menu/iconMenu.svg";
                HomeImage = "../img/menu/inicio.svg";

                #region Acciones Radicacion

                FilingImage = "../img/menu/radicacion.svg";
                FilingReceived = "../img/menu/ventanillaRecibida.svg";
                FilingInternal = "../img/menu/ventanillaInterna.svg";
                Filingsent = "../img/menu/ventanillaEnviada.svg";
                FilingUnofficial = "../img/menu/ventanillaNoOficial.svg";
                FilingFaster = "../img/menu/ventanillaRapida.svg";
                MassiveRadiation = "../img/menu/ventanillaRapida.svg";

                #endregion Acciones Radicacion

                #region Gestion

                ManagementImage = "../img/menu/gestion.svg";
                ManagementTray = "../img/menu/gestionsubItem.svg";
                ManagementBoard = "../img/menu/tableroControl.svg";

                #endregion Gestion

                #region Tareas Documentales

                DocumentaryTasksImage = "../img/menu/tareasDocumentales.svg";
                DocumentaryTasksEditor = "../img/menu/editorTexto.svg";
                DocumentaryTasksSpreadsheet = "../img/menu/hojaCalculo.svg";
                DocumentaryTasksTray = "../img/menu/bandejaTareasSubItem.svg";

                #endregion Tareas Documentales

                #region Expedientes

                RecordImage = "../img/menu/expedientes.svg";
                RecordConsult = "../img/menu/consultaExpediente.svg";
                RecordAdministration = "../img/menu/administraacionExp.svg";
                RecordRequest = "../img/menu/solicitudPrestamo.svg";

                #endregion Expedientes

                #region Buscadores

                SearchersImage = "../img/menu/buscadores.svg";
                SearchersConsult = "../img/menu/consultaDocumentos.svg";
                SearchersSearch = "../img/menu/busquedaRapida.svg";

                #endregion Buscadores

                BpmImage = "../img/menu/bpm.svg";
                EnvironmentalImpactImage = "../img/menu/impactoAmbiental.svg";
                LogoutImage = "../img/menu/cerrarSesion.svg";

                #endregion Iconos SubMenu
            }
        }

        #endregion OthersMethods

        #region FXMethods

        private void OnMouseOverWithSubItem(ref string image, ref string subnavItem, int activeIndex)
        {
            isDropdownOpen = false;
            if (!string.IsNullOrEmpty(image))
            {
                if (subnavItem == "bc")
                {
                    showNewMenu = showNewMenu ? !showNewMenu : showNewMenu;
                }
                else
                {
                    image = !image.Contains("White.svg") ? !image.Contains("Hover.svg") ? image.Replace(".svg", "Hover.svg") : image : image.Replace("White.svg", "Hover.svg");
                    SetActive(activeIndex);
                }
            }

            switch (activeIndex)
            {
                case 0:

                    subNavFiling = "d-none";
                    subNavManagement = "d-none";
                    subNavDocumentaryTasks = "d-none";
                    subNavRecord = "d-none";
                    subNavSearchers = "d-none";

                    break;

                case 1:

                    subNavFiling = "";
                    subNavManagement = "d-none";
                    subNavDocumentaryTasks = "d-none";
                    subNavRecord = "d-none";
                    subNavSearchers = "d-none";

                    break;

                case 2:

                    subNavFiling = "d-none";
                    subNavManagement = "";
                    subNavDocumentaryTasks = "d-none";
                    subNavRecord = "d-none";
                    subNavSearchers = "d-none";

                    break;

                case 3:

                    subNavFiling = "d-none";
                    subNavManagement = "d-none";
                    subNavDocumentaryTasks = "";
                    subNavRecord = "d-none";
                    subNavSearchers = "d-none";

                    break;

                case 4:

                    subNavFiling = "d-none";
                    subNavManagement = "d-none";
                    subNavDocumentaryTasks = "d-none";
                    subNavRecord = "";
                    subNavSearchers = "d-none";

                    break;

                case 5:

                    subNavFiling = "d-none";
                    subNavManagement = "d-none";
                    subNavDocumentaryTasks = "d-none";
                    subNavRecord = "d-none";
                    subNavSearchers = "";

                    break;

                case 6:

                    subNavFiling = "d-none";
                    subNavManagement = "d-none";
                    subNavDocumentaryTasks = "d-none";
                    subNavRecord = "d-none";
                    subNavSearchers = "d-none";

                    break;

                case 7:

                    subNavFiling = "d-none";
                    subNavManagement = "d-none";
                    subNavDocumentaryTasks = "d-none";
                    subNavRecord = "d-none";
                    subNavSearchers = "d-none";

                    break;

                case 8:

                    subNavFiling = "d-none";
                    subNavManagement = "d-none";
                    subNavDocumentaryTasks = "d-none";
                    subNavRecord = "d-none";
                    subNavSearchers = "d-none";

                    break;
            }
        }

        private void OnMouseOutWithSubItem(ref string image, ref string subnavItem)
        {
            if (darkMode)
            {
                image = image.Replace("Hover.svg", "White.svg");
            }
            else
            {
                image = image.Replace("Hover.svg", ".svg");
            }
        }

        private void OnMouseOver(ref string image, int activeIndex)
        {
            if (darkMode)
            {
                image = image.Contains("White.svg") ? image.Replace("White.svg", "Hover.svg") : image.Replace(".svg", "Hover.svg");
            }
            else
            {
                image = image.Replace(".svg", "Hover.svg");
            }
            //SetActive(activeIndex);
        }

        private void OnMouseOut(ref string image)
        {
            if (darkMode)
            {
                image = image.Replace("Hover.svg", "White.svg");
            }
            else
            {
                image = image.Replace("Hover.svg", ".svg");
            }
        }

        #endregion FXMethods

        private async Task GetShortCuts()
        {
            try
            {
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<string>>>("access/View/ByFilterToken");
                if (deserializeResponse.Succeeded)
                {
                    ShortCutsToDeploy = deserializeResponse.Data;
                }
                else
                {
                    ShortCutsToDeploy = new();
                }
            }
            catch (Exception)
            {
                ShortCutsToDeploy = new();
            }

            StateHasChanged();
        }

        #region Terms&ConditionsActions

        private async Task<FileDtoResponse?> GetFile(string id)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                if (deserializeResponse!.Succeeded)
                {
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    return deserializeResponse.Data!;
                }
                else
                {
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    return null;
                }
            }
            catch
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                return null;
            }
        }

        private async Task GetAppKeys(string keyName)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                AppKeysFilterDtoRequest appKeysFilter = new();
                appKeysFilter.FunctionName = "LayoutFunction";
                appKeysFilter.KeyName = keyName;
                var responseApi = await HttpClient.PostAsJsonAsync("params/AppKeys/ByFilter", appKeysFilter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<AppKeysDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data.Any())
                {
                    LstAppKeys = deserializeResponse.Data;
                }
                else
                {
                    LstAppKeys = new();
                    notificationModal.UpdateModal(ModalType.Error, string.Format(Translation["KeyHasNoValues"], "LayoutFunction"), true);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task OpenPdfTermsCondition(string codeKeyLog)
        {
            string filename = string.Empty;
            LstAppKeys = new();
            switch (codeKeyLog)
            {
                case "Terms&Conditions":
                    await GetAppKeys("Terms&Conditions");
                    filename = "Terms&Conditions.pdf";
                    break;

                case "PrivacyPolicy":
                    await GetAppKeys("PrivacyPolicy");
                    filename = "PrivacyPolicy";
                    break;

                default:
                    notificationModal.UpdateModal(ModalType.Error, "ModalOrigin no reconocido", true);
                    break;
            }

            if (LstAppKeys != null && LstAppKeys.Any())
            {
                FileDtoResponse objFile = await GetFile(LstAppKeys[0].Value1);
                await Js.InvokeVoidAsync("openPdfFromBytes", objFile.DataFile);
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }

        #endregion Terms&ConditionsActions

        #region GetProfilePicture

        private async Task GetProfilePicture(string fileId)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                fileId = string.IsNullOrEmpty(fileId) ? "6791" : fileId;

                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", fileId);
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    fileData = deserializeResponse.Data;
                    SourceUserPicture = fileData != null ?
                    $"data:image/{fileData.FileExt.ToLowerInvariant()};base64,{Convert.ToBase64String(fileData.DataFile)}" :
                    "../img/usuario.png";
                }
                else
                {
                    SourceUserPicture = "../img/usuario.png";
                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true, Translation["Accept"], "");
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true, Translation["Accept"], "");
            }
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetProfilePicture

        #region SearchEngine

        private void OpenSearchEngine()
        {
            searchEngineModal.UpdateModalStatus(true);
        }

        #endregion

        #endregion Methods
    }
}