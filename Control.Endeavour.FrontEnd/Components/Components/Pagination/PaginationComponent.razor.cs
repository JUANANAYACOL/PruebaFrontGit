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
    public partial class PaginationComponent<T, M> : ComponentBase where T : class where M : class
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


        #endregion

        #region Modals

        private NotificationsComponentModal notificationModal;

        #endregion

        #region Parameters
        [Parameter] public MetaModel? ObjectMeta { get; set; }
        [Parameter] public List<T>? DataObjectList { get; set; }
        [Parameter] public M? Filter { get; set; } = null;
        [Parameter] public Dictionary<string, dynamic> Headers { get; set; } = new Dictionary<string, dynamic>();
        [Parameter] public EventCallback<List<T>> OnGetPaginationRefresh { get; set; }
        [Parameter] public EventCallback<List<T>> OnPaginationRefresh { get; set; }
        [Parameter] public bool Valid { get; set; } = false;
        #endregion

        #region Models

        #endregion

        #region Environments

        #region Environments(String)

        #endregion

        #region Environments(Numeric)

        public int selectedPage;
        private int totalPages;

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool leftButtonEnabled = false;
        private bool rightButtonEnabled = true;

        #endregion

        #region Environments(List & Dictionary)

        List<DropDownOption> dropdownOptions = new List<DropDownOption>();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync

        protected override void OnInitialized()
        {
            SetDataListPages();
            UpdateButtonStates();
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

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

        #endregion

        #region ChangePage

        private async Task GoToPreviousPage()
        {
            if (selectedPage > 1)
            {
                await OnPageSelectedAsync(selectedPage - 1);
            }
        }

        private async Task GoToPreviousPageGet()
        {
            if (selectedPage > 1)
            {
                await OnPageSelectedGetAsync(selectedPage - 1);
            }
        }

        public void ResetPagination(MetaModel Object)
        {
            ObjectMeta = Object;
            dropdownOptions = new List<DropDownOption>();
            selectedPage = 0;
            totalPages = 0;

            leftButtonEnabled = false;
            rightButtonEnabled = true;
            SetDataListPages();
            UpdateButtonStates();
        }

        #endregion

        #region SetDataListPages

        private void SetDataListPages()
        {
            for (int i = 1; i <= ObjectMeta.TotalPages; i++)
            {
                dropdownOptions.Add(new DropDownOption { PageNumber = i, PageName = $"{Translation["Page"]} {i}" });
            }
            selectedPage = 1;
            totalPages = dropdownOptions.Count;
        }

        #endregion

        #region ModifyUrl

        public static string ModifyUrl(string originalUrl, int pageNumber, int pageSize)
        {
            var uriBuilder = new UriBuilder(originalUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            // Actualizar los parámetros pageNumber y pageSize
            query["pageNumber"] = pageNumber.ToString();
            query["pageSize"] = pageSize.ToString();

            uriBuilder.Query = query.ToString();
            string url = uriBuilder.ToString();
            Uri uri = new Uri(url);

            string baseUrl = uri.GetLeftPart(UriPartial.Authority);
            string remainingPart = url.Substring(baseUrl.Length);
            if (remainingPart.StartsWith("/"))
            {
                remainingPart = remainingPart.Substring(1);
            }


            return remainingPart;
        }

        #endregion

        #region PostPagination

        private async Task GoToNextPage()
        {
            if (selectedPage < totalPages)
            {
                await OnPageSelectedAsync(selectedPage + 1);
            }
        }

        private async Task OnPageSelectedAsync(int selectedPageNumber)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            selectedPage = selectedPageNumber;
            var numeros = ObjectMeta.TotalPages;

            // Actualizar los botones
            leftButtonEnabled = selectedPage > 1;
            rightButtonEnabled = selectedPage < totalPages;

            string MicroServicesUrl = ModifyUrl(ObjectMeta.FirstPageUrl, selectedPage, ObjectMeta.PageSize);

            var response = await HttpClient.PostAsJsonAsync(MicroServicesUrl, Filter); /*await CallService.Post<List<T>, M>(MicroServicesUrl, Filter);*/
            var deserializeResponse = await response.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<T>>>();
            if (deserializeResponse.Succeeded)
            {
                DataObjectList = deserializeResponse.Data;
                await OnPaginationRefresh.InvokeAsync(DataObjectList);
            }
            else
            {
                DataObjectList = new List<T>();
                notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion

        #region GetPagination

        private async Task GoToNextPageGet()
        {
            if (selectedPage < totalPages)
            {
                await OnPageSelectedGetAsync(selectedPage + 1);
            }
        }

        private async Task OnPageSelectedGetAsync(int selectedPageNumber)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            selectedPage = selectedPageNumber;

            // Actualizar los botones
            leftButtonEnabled = selectedPage > 1;
            rightButtonEnabled = selectedPage < totalPages;

            string MicroServicesUrl = ModifyUrl(ObjectMeta.FirstPageUrl, selectedPage, ObjectMeta.PageSize);
            var requestHeaders = Headers.Any() ? Headers : null;

            HttpClient?.DefaultRequestHeaders.Remove($"{requestHeaders.Keys.FirstOrDefault()}");
            HttpClient?.DefaultRequestHeaders.Add($"{requestHeaders.Keys.FirstOrDefault()}", $"{requestHeaders.Values.FirstOrDefault()}");
            var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<T>>>($"{MicroServicesUrl}");
            HttpClient?.DefaultRequestHeaders.Remove($"{requestHeaders.Keys.FirstOrDefault()}");
            if (deserializeResponse.Succeeded)
            {
                DataObjectList = deserializeResponse.Data;
                await OnGetPaginationRefresh.InvokeAsync(DataObjectList);
            }
            else
            {
                DataObjectList = new List<T>();
                notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion

        #endregion

        #endregion

    }
}