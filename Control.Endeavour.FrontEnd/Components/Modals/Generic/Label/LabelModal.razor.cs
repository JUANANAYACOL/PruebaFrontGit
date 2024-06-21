using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Filing;
using DevExpress.XtraPrinting;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Net.ConnectCode.Barcode;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Modals.Generic.Label
{
    public partial class LabelModal
    {

        #region Variables

        #region Inject 
        //[Inject] private EventAggregatorService? EventAggregator { get; set; }
        [Inject] private HttpClient? HttpClient { get; set; }
        [Inject] private FilingStateContainer? FilingSC { get; set; }
        [Inject] private IJSRuntime? Js { get; set; }
        [Inject] private IStringLocalizer<Translation>? Translation { get; set; }
        #endregion

        #region Components


        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Parameters
        [Parameter] public string Width { get; set; } = "20%";
        [Parameter] public bool ModalStatus { get; set; } = false;
        [Parameter] public EventCallback<bool> OnModalClosed { get; set; }
        #endregion

        #region Models

        #endregion

        #region Environments

        #region Environments(String)
        private string RadicadoTitle = "";
        private string CodigoBarras = "";

        private string FilingNumber = "";
        private string DocumentId = "";
        private string Recipients = "";
        private string NumberOfRecipientes = "1";
        private string Folios = "";
        private string Annexes = "";
        private string PhysicalDescription = "";
        #endregion

        #region Environments(Numeric)
        private bool isQuickFiling = false;
        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(List & Dictionary)

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            
            
        }
        #endregion

        #region Methods
        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private void HandleModalClosed(bool status)
        {
            ModalStatus = status;
            StateHasChanged();
        }
        #endregion

        #region OthersMethods

        #region ModalActionsMethods

        public async void UpdateModalStatus(bool newValue)
        {
            ModalStatus = newValue;
            if (!string.IsNullOrEmpty(FilingSC?.FilingNumber) && ModalStatus)
            {
                FilingNumber = FilingSC.FilingNumber;
                DocumentId = FilingSC.DocumentId;
                Recipients = FilingSC.Recipients;
                Folios = FilingSC.Folios;
                Annexes = FilingSC.Annexes;
                RadicadoTitle = Translation["FilingLabel"]+": " + FilingNumber;
                await GetLabelFilling();
            }
            StateHasChanged();
        }

        public async Task AssignVariablesAsync(
        string filingNumber, string documentId, string recipients, string folios, string annexes)
        {
            FilingNumber = filingNumber;
            DocumentId = documentId;
            Recipients = recipients;
            Folios = folios;
            Annexes = annexes;
            await GetLabelFilling();
        }
        public void simpleUpdateModalStatus(bool newValue)
        {
            ModalStatus = newValue;
            StateHasChanged();
        }
        private async Task CloseModal()
        {
            await OnModalClosed.InvokeAsync(false);
        }
        #endregion

        #region GenerateBarcode


        private async Task GetLabelFilling()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("controlId");
                HttpClient?.DefaultRequestHeaders.Add("controlId", DocumentId);
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<byte[]>>("documents/Document/GetLabelModel");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    var byteArray = deserializeResponse.Data;
                    string base64Data = Convert.ToBase64String(byteArray);
                    await Js.InvokeVoidAsync("updateBarcodeImageFromBase64", base64Data);
                }
                
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
            finally
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
        }

        #region QuickFillingData

        public void GetDataQuickFiling(string numberOfRecipientes, string physicalDescription)
        {
            NumberOfRecipientes = numberOfRecipientes;
            PhysicalDescription = physicalDescription;
            isQuickFiling = true;
        }
        
        #endregion

        #endregion

        private async Task PrintDiv()
        {
            await Js.InvokeVoidAsync("printDiv", "contentToPrint");
        }

        #endregion

        #endregion

    }
}
