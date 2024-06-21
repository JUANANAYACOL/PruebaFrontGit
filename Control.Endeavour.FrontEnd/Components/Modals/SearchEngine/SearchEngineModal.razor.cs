using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using OpenAI_API.Models;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.SearchEngine
{
    public partial class SearchEngineModal
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
        private NotificationsComponentModal notificationModal = new();
        private GeneralInformationModal generalInformation = new();

        #endregion

        #region Parameters


        #endregion

        #region Models
        private SearchDtoRequest DocumentSearchDtoRequest = new();
        private SearchDtoResponse Document = new();


        #endregion

        #region Environments

        #region Environments(String)

        private string ControlId = string.Empty;
        private string UriSearchEngine = "documents/Document/SearchEngineDocument";
        #endregion

        #region Environments(Numeric)


        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)


        private bool modalStatus = false;
        private bool ShowGeneralInfo = false;


        #endregion

        #region Environments(List & Dictionary)



        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar la inicialización: {ex.Message}");
            }

        }


        #endregion

        #region Methods

        #region HandleMethods
        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
        }




        #endregion

        #region OthersMethods

        #region SearchMethods
        private async Task HandleKeyPressAsync(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                if (!string.IsNullOrWhiteSpace(ControlId))
                {
                    var responseApi = await HttpClient.PostAsJsonAsync(UriSearchEngine, DocumentSearchDtoRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<SearchDtoResponse>>>();

                    if (deserializeResponse.Data.Data.Count > 0 && deserializeResponse.Data != null)
                    {
                        Document = deserializeResponse.Data.Data[0];
                        ShowGeneralInfo = true;
                        if(ShowGeneralInfo)
                        {
                            await generalInformation.GeneralInformation(Document.ControlId, Document.ClassCode);
                            generalInformation.UpdateModalStatus(true);
                        }

                        //await generalInformation.GeneralInformation(Document.ControlId, Document.ClassCodeName);
                        //generalInformation.UpdateModalStatus(true);
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Information, Translation["CouldntFindInfByParameters"], true, buttonTextCancel: "");
                    }

                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Warning, Translation["EnterParameterSearch"], true, buttonTextCancel: "");
                }
            }
        }
        #endregion

        #region ValidationMethods
        private void UpdateControlId(string value)
        {
            ControlId = new string(value.Where(char.IsDigit).ToArray());
            if (!string.IsNullOrEmpty(ControlId))
            {
                DocumentSearchDtoRequest.ControlId = int.Parse(ControlId);
            }
        }

        #endregion
        #region ModalMethods

        private void CloseSearchModal(bool value)
        {
            UpdateModalStatus(value);
        }
        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        #endregion

        #endregion

        #endregion

    }
}
