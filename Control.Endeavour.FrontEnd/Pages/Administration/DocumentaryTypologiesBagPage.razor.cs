using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.DocumentaryTypologiesBag;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Profile;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Replacement;
using Control.Endeavour.FrontEnd.Models.Models.Administration.DocumentaryTypologiesBag.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.DocumentaryTypologiesBag.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VReplacement.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.Blazor.Popup.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Security.AccessControl;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class DocumentaryTypologiesBagPage
    {
        #region Variables

        #region Inject

        [Inject]
        private IJSRuntime Js { get; set; }

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Components

        private InputModalComponent DTBInput { get; set; } = new();
        private NewPaginationComponent<DocumentaryTypologiesBagDtoResponse, DocumentaryTypologiesBagFitlerDtoRequest> paginationComponent { get; set; } = new();

        #endregion Components

        #region Modals

        private DocumentaryTypologiesBagModal documentaryTypologiesBagModal { get; set; } = new();
        private DocumentaryTypologiesBagMetaDataModal documentaryTypologiesBagMetaDataModal { get; set; } = new();

        #endregion Modals

        #region Models

        private PaginationInfo paginationInfo = new();
        private DocumentaryTypologiesBagFitlerDtoRequest documentaryTypologiesBagFitlerDtoRequest { get; set; } = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string UriFilterDocumentarytypologies { get; set; } = "documentarytypologies/DocumentaryTypologiesBag/ByFilter";
        private string metaDataSelectedModalTitle { get; set; } = string.Empty;
        private string DTBName { get; set; } = string.Empty;
        private string dtbModalTitle { get; set; } = string.Empty;

        private bool dataChargue { get; set; } = false;

        #endregion Environments(String)

        #region Environments(List & Dictionary)

        private List<DocumentaryTypologiesBagDtoResponse> documentaryTypologiesBagList { get; set; } = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            await GetDocumentaryTypology();
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private async Task HandleStatusChanged(bool status)
        {
            documentaryTypologiesBagModal.UpdateModalStatus(status);
            await GetDocumentaryTypology();
        }

        private async Task HandleStatusMetaDataChanged(bool status)
        {
            documentaryTypologiesBagMetaDataModal.UpdateModalStatus(status);
            await GetDocumentaryTypology();
        }

        private void HandlePaginationGrid(List<DocumentaryTypologiesBagDtoResponse> newDataList)
        {
            documentaryTypologiesBagList = newDataList;
        }

        #endregion HandleMethods

        private void ShowModal()
        {
            dtbModalTitle = $"{Translation["Bags"]} - {Translation["Create"]} - {Translation["DocumentaryTypologiesBag"]}";
            documentaryTypologiesBagModal.UpdateModalStatus(true);
        }

        private async Task GetDocumentaryTypology()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                documentaryTypologiesBagFitlerDtoRequest = new()
                {
                    TypologyName = DTBName
                };

                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterDocumentarytypologies, documentaryTypologiesBagFitlerDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<DocumentaryTypologiesBagDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    documentaryTypologiesBagList = deserializeResponse.Data.Data ?? new();

                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                    dataChargue = true;
                }
                else
                {
                    documentaryTypologiesBagList = new();
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

        private void ShowModalEdit(DocumentaryTypologiesBagDtoResponse dtb)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            dtbModalTitle = $"{Translation["Bags"]} - {Translation["Edit"]} - {Translation["DocumentaryTypologiesBag"]}";
            documentaryTypologiesBagModal.UpdateModalStatus(true);
            documentaryTypologiesBagModal.UpdateSelectedDTB(dtb);
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task ShowModalMetaData(DocumentaryTypologiesBagDtoResponse dtb)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await documentaryTypologiesBagMetaDataModal.GetmetaFieldsOfBag(( dtb ?? new() ).DocumentaryTypologyBagId, false);
            await documentaryTypologiesBagMetaDataModal.UpdateModalStatus(true);
      
            metaDataSelectedModalTitle = dtb.TypologyName;

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #region OnClickReset

        private async Task OnClickReset()
        {
            DTBName = string.Empty;

            await GetDocumentaryTypology();
            StateHasChanged();
        }

        #endregion OnClickReset

        #endregion Methods
    }
}