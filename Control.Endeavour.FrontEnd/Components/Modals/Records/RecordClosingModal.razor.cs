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
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Records
{
    public partial class RecordClosingModal
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
        private String? ClosedType { get; set; } = String.Empty;
        private String Justification { get; set; } = string.Empty;
        #endregion

        #region Modals
        private NotificationsComponentModal NotificationModal = new();
        #endregion

        #region Parameters
        [Parameter] public EventCallback<bool> OnRefreshData { get; set; }
        #endregion

        #region Models
        private RecordsClosedDtoRequest RecordsClosedRequest = new();
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
        private List<VSystemParamDtoResponse>? lstClosedType { get; set; } = new();
        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
            try
            {
                await GetClosedType();
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
                if(!String.IsNullOrEmpty(ClosedType) && !String.IsNullOrEmpty(Justification))
                {
                    SpinnerLoaderService.ShowSpinnerLoader(Js);

                    RecordsClosedRequest.RecordId = Convert.ToInt16(IdExpediente);
                    RecordsClosedRequest.ClosedType = ClosedType;
                    RecordsClosedRequest.Justification = Justification;

                    var responseApi = await HttpClient.PostAsJsonAsync("records/Records/RecordsClose", RecordsClosedRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        NotificationModal.UpdateModal(ModalType.Success,deserializeResponse.Data, true);
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
                    NotificationModal.UpdateModal(ModalType.Warning, String.IsNullOrEmpty(ClosedType) ? "¡Por favor escoja un tipo de cierre!":"¡Por favor digite una justificación!", true, Translation["Accept"], "", "", "");
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
        private async Task GetClosedType()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "TCEXP");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    lstClosedType = deserializeResponse.Data;
                }
                else { lstClosedType = new(); }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de cierre: {ex.Message}");
            }
        }
        #endregion

        public void GetValue(string closedType)
        {
            ClosedType = closedType;
        }

        public void UpdateModalStatus(bool status)
        {
            ModalStatus = status;
            StateHasChanged();
        }
        public void GetDataForClosing(int recordId, int recordNumber)
        {
            IdExpediente = recordId.ToString();
            RecordNumber = recordNumber.ToString();
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
