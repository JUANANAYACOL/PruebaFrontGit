using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Web;

namespace Control.Endeavour.FrontEnd.Components.Components.Pagination
{
    /// <summary>
    /// Componente que realiza la funcionalidad de la paginación, permitiendo la personalización a través de tipos genéricos para adaptarse a diferentes tipos de datos.
    /// </summary>
    /// <typeparam name="T">El tipo de objeto que hace referencia a la entidad de respuesta .</typeparam>
    /// <typeparam name="M">El tipo de objeto que hace referencia a el request del filtro para la consulta de información.</typeparam>
    public partial class NewPaginationComponent<T, M> : ComponentBase where T : class where M : PaginationRequest
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

        #region Modals

        private NotificationsComponentModal notificationModal = new();

        #endregion Modals

        #region Parameters

        /// <summary>
        /// Objeto de la información de paginación para el componente.
        /// </summary>
        [Parameter] public PaginationInfo PaginationInfo { get; set; } = new();

        /// <summary>
        /// URI del recurso al que se va a consultar para traer la información.
        /// </summary>

        [Parameter] public string Uri { get; set; } = string.Empty;

        /// <summary>
        /// Filtro con el que se realiza la consulta, recordar que debe ser una clase que tenga la herencia de la clase PaginationRequest.
        /// </summary>

        [Parameter] public M? Filter { get; set; } = null;

        /// <summary>
        /// Evento que se dispara cuando es necesario refrescar la paginación, por ejemplo, después de cambiar la página o el tamaño de la página.
        /// </summary>

        [Parameter] public EventCallback<List<T>> OnPaginationRefresh { get; set; }

        #endregion Parameters

        #region Environments

        #region Environments(String)

        private string pageText = string.Empty;

        #endregion Environments(String)

        #region Environments(Numeric)

        public int selectedPage;
        private int totalPages;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool leftButtonEnabled = false;
        private bool rightButtonEnabled = true;
        private bool isRendered = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<DropDownOption> dropdownOptions = new();
        private List<T> DataObjectList = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override void OnInitialized()
        {
            pageText = Translation["Page"];
            SetDataListPages();
            UpdateButtonStates();
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                isRendered = true;
                // Opcionalmente, si necesitas llamar a ResetPagination aquí, puedes hacerlo de manera segura.
            }
        }

        #endregion HandleMethods

        #region OthersMethods

        #region ChangeButton

        private string GetLeftButtonImage()
        {
            return leftButtonEnabled ? "..\\img\\leftOn.svg" : "..\\img\\leftOff.svg";
        }

        private string GetRightButtonImage()
        {
            return rightButtonEnabled ? "..\\img\\rightOn.svg" : "..\\img\\rightOff.svg";
        }

        private void UpdateButtonStates()
        {
            leftButtonEnabled = totalPages > 1 && selectedPage > 1;
            rightButtonEnabled = totalPages > 1 && selectedPage < totalPages;
        }

        #endregion ChangeButton

        #region ChangePage

        private async Task GoToPreviousPage()
        {
            if (selectedPage > 1)
            {
                selectedPage -= 1;
                Filter.PageNumber = selectedPage;
                await OnPageSelectedAsync(selectedPage);
            }
        }

        public void ResetPagination(PaginationInfo Object)
        {
            PaginationInfo = Object;
            dropdownOptions = new List<DropDownOption>();
            selectedPage = 0;
            totalPages = 0;
            leftButtonEnabled = false;
            rightButtonEnabled = true;
            SetDataListPages();
            UpdateButtonStates();
            if (isRendered)
            {
                StateHasChanged();
            }
        }

        #endregion ChangePage

        #region SetDataListPages

        private void SetDataListPages()
        {
            for (int i = 1; i <= PaginationInfo.TotalPages; i++)
            {
                dropdownOptions.Add(new DropDownOption { PageNumber = i, PageName = $"{pageText} {i}" });
            }
            selectedPage = 1;
            totalPages = dropdownOptions.Count;
        }

        #endregion SetDataListPages

        #region PostPagination

        private async Task GoToNextPage()
        {
            if (selectedPage < totalPages)
            {
                selectedPage += 1;
                Filter.PageNumber = selectedPage;
                await OnPageSelectedAsync(selectedPage);
            }
        }

        private async Task OnPageSelectedAsync(int selectedPageNumber)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            selectedPage = selectedPageNumber;

            // Actualizar los botones
            leftButtonEnabled = selectedPage > 1;
            rightButtonEnabled = selectedPage < totalPages;
            Filter.PageNumber = selectedPage;
            var response = await HttpClient.PostAsJsonAsync(Uri, Filter);
            var deserializeResponse = await response.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<T>>>();
            if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
            {
                DataObjectList = deserializeResponse.Data.Data;
                Filter.PageNumber = null;
                await OnPaginationRefresh.InvokeAsync(DataObjectList);
            }
            else
            {
                DataObjectList = new List<T>();
                //notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion PostPagination

        #endregion OthersMethods

        #endregion Methods
    }
}