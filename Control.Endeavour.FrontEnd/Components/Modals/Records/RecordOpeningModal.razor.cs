using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Records.Request;
using Control.Endeavour.FrontEnd.Pages.Records;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System;
using System.Net.Http.Json;


namespace Control.Endeavour.FrontEnd.Components.Modals.Records
{
    public partial class RecordOpeningModal
    {

        #region Variables

        #region Inject 
        [Inject] private HttpClient? HttpClient { get; set; }
        [Inject] private IJSRuntime? Js { get; set; }
        [Inject] private IStringLocalizer<Translation>? Translation { get; set; }
        #endregion

        #region Components
        private String? IdExpediente { get; set; }
        private String? RecordNumber { get; set; }
        private String Justification { get; set; } = string.Empty;
        private String? IdSubSeries { get; set; }
        #endregion

        #region Modals
        private NotificationsComponentModal NotificationModal = new();
        #endregion

        #region Parameters
        [Parameter] public EventCallback<bool> OnRefreshData { get; set; }
        #endregion

        #region Models
        private RecordsOpeningDtoRequest RecordsOpeningDtoRequest = new();
        #endregion

        #region Environments

        #region Environments(String)
        #endregion

        #region Environments(Numeric)
        private decimal contadorcarac = 0;
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)
        private bool ModalStatus = false;
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
                Console.WriteLine($"Error al realizar la initialización: {ex.Message}");
            }
        }
        #endregion

        #region Methods

        #region HandleMethods
        private async Task HandleModalClosedAsync(bool status)
        {
            ModalStatus = status;
            StateHasChanged();
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                if (args.ModalOrigin == "RemoveFile")
                {

                }
            }
            else
            {
                Console.WriteLine("Registro No eliminado");
            }
        }

        private async Task HandleGenerateClosure()
        {
            try
            {
                if (!String.IsNullOrEmpty(Justification))
                {
                    SpinnerLoaderService.ShowSpinnerLoader(Js);

                    RecordsOpeningDtoRequest.RecordId = Convert.ToInt16(IdExpediente);
                    RecordsOpeningDtoRequest.Justification = Justification;
                    RecordsOpeningDtoRequest.SubSeriesId = Convert.ToInt16(IdSubSeries);

                    var responseApi = await HttpClient.PostAsJsonAsync("records/Records/ReOpenRecords", RecordsOpeningDtoRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        NotificationModal.UpdateModal(ModalType.Success, deserializeResponse.Data, true);
                        UpdateModalStatus(false);
                        await OnRefreshData.InvokeAsync(true);
                        SpinnerLoaderService.HideSpinnerLoader(Js);
                    }
                    else
                    {
                        NotificationModal.UpdateModal(ModalType.Error, Translation["GeneratingFilingErrorMessage"], true, "Aceptar");
                        SpinnerLoaderService.HideSpinnerLoader(Js);
                    }

                }
                else
                {
                    NotificationModal.UpdateModal(ModalType.Warning, "¡Por favor digite una justificación!", true, Translation["Accept"], "", "", "");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar el cierre de expediente: {ex.Message}");
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                throw;
            }
        }

        #endregion

        #region OthersMethods

        #region GetMethods
        #endregion

        public void UpdateModalStatus(bool status)
        {
            ModalStatus = status;
            StateHasChanged();
        }
        public void GetDataForOpening(int recordId, int recordNumber, int subSeriesId)
        {
            IdExpediente = recordId.ToString();
            RecordNumber = recordNumber.ToString();
            IdSubSeries = subSeriesId.ToString();
        }

        private void ContarCaracteres(ChangeEventArgs e)
        {
            String value = e.Value.ToString() ?? String.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                contadorcarac = value.Length;
            }
            else
            {
                contadorcarac = 0;
            }
        }

        #endregion

        #endregion

    }
}
