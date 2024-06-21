using Control.Endeavour.FrontEnd.Models.Models.Administration.ImporterTrdTvd.Response;
using Control.Endeavour.FrontEnd.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.ImporterTrdTvd
{
    public partial class ImportResultModal : ComponentBase
    {
        #region Variables

        #region Inject

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject



        #region Parameters

        [Parameter]
        public EventCallback<bool> OnStatusChanged { get; set; }

        [Parameter]
        public EventCallback<bool> CloseModals { get; set; }

        [Parameter]
        public bool modalStatus { get; set; } = false;

        #endregion Parameters

        #region Models

        private ImporterDtoResponse importerResponse { get; set; } = new();

        #endregion Models

        #region Environments

        #region Environments(Numeric)

        private int total = 0;

        #endregion Environments(Numeric)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            // //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
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

        #region TotalRecords

        public void TotalRecords()
        {
            total = importerResponse.contUnity + importerResponse.contOffice + importerResponse.contSeries +
            importerResponse.contSubseries + importerResponse.contTypologies + importerResponse.contRetentions +
            importerResponse.contTRD + importerResponse.contTRDC;
        }

        #endregion TotalRecords

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region HandleModalClosed

        private async Task HandleModalClosed(bool status)
        {
            modalStatus = status;
            await CloseModals.InvokeAsync(false);
            ResetFormAsync();
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #region ResetFormAsync

        private void ResetFormAsync()
        {
            total = 0;
            importerResponse = new();
        }

        #endregion ResetFormAsync

        #region GetData

        public void GetData(ImporterDtoResponse data)
        {
            importerResponse = data;
            TotalRecords();
            StateHasChanged();
        }

        #endregion GetData

        #endregion OthersMethods

        #endregion Methods
    }
}