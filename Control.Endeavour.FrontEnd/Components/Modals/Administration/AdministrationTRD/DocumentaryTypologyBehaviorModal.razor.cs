using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VDocumentaryTypology.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD
{
    public partial class DocumentaryTypologyBehaviorModal : ComponentBase
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

        private TRDCBehaviourModal trdcBehaviourModal { get; set; }
        private NotificationsComponentModal notificationModal = new();

        #endregion Modals

        #region Parameters

        [Parameter] public EventCallback<MyEventArgs<VDocumentaryTypologyDtoResponse>> OnStatusChanged { get; set; }
        [Parameter] public string title { get; set; } = "";

        [Parameter]
        public EventCallback<int> OnIdSaved { get; set; }

        [Parameter]
        public EventCallback OnResetForm { get; set; }

        #endregion Parameters

        #region Models

        private NewPaginationComponent<DocumentaryTypologiesBehaviorsDtoResponse, DocumentaryTypologiesBehaviorsFilterDtoRequest> paginationComponent = new();
        private PaginationInfo paginationInfo = new();
        private DocumentaryTypologiesBehaviorsFilterDtoRequest? filterDtoRequest { get; set; } = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string UriFilterDocumentaryTypologiesBehaviors = "documentarytypologies/DocumentaryTypologiesBehaviors/ByFilter";

        private string UriFilterDocTypology = "documentarytypologies/DocumentaryTypologies/ByFilter";

        #endregion Environments(String)

        #region Environments(Numeric)

        public int documentaryTypologyId = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool modalStatus { get; set; } = false;
        private bool dataChargue = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        public List<DocumentaryTypologiesBehaviorsDtoResponse> documentaryTypologiesBehaviorList { get; set; } = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
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

        #region HandleModalClosed

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            //UpdateForm = true;
            /*IsEditForm = false;
            IsDisabledCode = false;
            adminUnitDtoResponse = new();
            CharacterCounter = 0;
            ResetFormAsync();*/
        }

        #endregion HandleModalClosed

        #region Modal

        private async Task ShowModal()
        {
            UpdateModalStatus(false);
            trdcBehaviourModal.HandleDocumentaryTypologyId(documentaryTypologyId);
            await trdcBehaviourModal.UpdateModalStatus(true);

            /*modalDocumentalVersion.UpdateModalStatus(true);*/
        }

        private async Task ShowModalEditAsync(DocumentaryTypologiesBehaviorsDtoResponse record)
        {
            UpdateModalStatus(false);
            trdcBehaviourModal.HandleDocumentaryTypologyId(documentaryTypologyId);
            await trdcBehaviourModal.UpdateModalStatus(true);
            await trdcBehaviourModal.UpdateSelectedRecordAsync(record);
            /*modalDocumentalVersion.UpdateModalStatus(true);
            modalDocumentalVersion.ReceiveDocumentalVersion(record);*/
        }

        #endregion Modal

        #region HandlePagination

        private void HandlePaginationGrid(List<DocumentaryTypologiesBehaviorsDtoResponse> newDataList)
        {
            documentaryTypologiesBehaviorList = newDataList;
        }

        #endregion HandlePagination

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                await OnResetForm.InvokeAsync();
                UpdateModalStatus(args.ModalStatus);
            }
        }

        #endregion HandleModalNotiClose

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        public async Task UpdateModalIdAsync(int id)
        {
            documentaryTypologyId = id;
            await GetDocT();
            StateHasChanged();
        }

        #region GetDocTypologies

        public async Task GetDocT()
        {
            try
            {
                var filterDocTypology = new DocumentaryTypologiesBehaviorsFilterDtoRequest();
                filterDocTypology = new()
                {
                    DocumentaryTypologyId = documentaryTypologyId
                };
                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterDocumentaryTypologiesBehaviors, filterDocTypology);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<DocumentaryTypologiesBehaviorsDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    documentaryTypologiesBehaviorList = deserializeResponse.Data!.Data!;
                    dataChargue = true;
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                }
                else
                {
                    documentaryTypologiesBehaviorList = new();
                    dataChargue = false;
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["LoadDocumentalVersionErrorMessage"]}: {ex.Message}", true, Translation["Accept"]);
            }
        }

        #endregion GetDocTypologies

        public async Task TRDCBehaviourOnModalStatus(bool response)
        {
            trdcBehaviourModal.HandleDocumentaryTypologyId(documentaryTypologyId);
            UpdateModalStatus(!response);
            await trdcBehaviourModal.UpdateModalStatus(response);
        }

        #region RefreshGrid

        private async Task HandleRefreshGridData(bool refresh)
        {
            await GetDocT();
        }

        #endregion RefreshGrid

        #endregion OthersMethods

        #endregion Methods
    }
}