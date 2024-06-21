using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Records.Response;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.Filing;
using Newtonsoft.Json.Linq;
using Control.Endeavour.FrontEnd.Resources;
using Microsoft.Extensions.Localization;

namespace Control.Endeavour.FrontEnd.Components.Modals.Records
{
    public partial class GetRecordsVolumesModal
    {

        #region Variables

        #region Inject 
        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Components


        #endregion

        #region Modals

        private NotificationsComponentModal NotificationModal = new();
        private UploadPDFModal UploadPDFModal = new();
        private CreateVolumeRecordModal CreateVolumeRecordModal = new();

        #endregion

        #region Parameters


        #endregion

        #region Models

        #endregion

        #region Environments

        #region Environments(String)
        private string recordNumber = string.Empty;
        #endregion

        #region Environments(Numeric)
        private int recordSelectedId = 0;
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)
        private bool modalStatus = false;
        #endregion

        #region Environments(List & Dictionary)

        private List<VolumeDtoResponse> recordVolumens = new();
        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;

            
        }


        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #region ModalClose

        private async Task HandleModalClosedAsync(bool status)
        {
            modalStatus = status;
            StateHasChanged();
        }
        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {


        }
        private void HandleUploadPDFModal()
        {
            UploadPDFModal.UpdateModalStatus(false);
        }
        private async Task HandleRefreshGrid()
        {
            await GetRecordsVolumens(recordSelectedId, Convert.ToInt32(recordNumber));
            CreateVolumeRecordModal.UpdateModalStatus(false);
        }

        #endregion
        #endregion

        #region OthersMethods

        #region Getvolumens

        public async Task GetRecordsVolumens(int recordId, int recordNumberData)
        {
            try
            {
                recordSelectedId = recordId;
                recordNumber = recordNumberData.ToString();
                HttpClient?.DefaultRequestHeaders.Remove("recordId");
                HttpClient?.DefaultRequestHeaders.Add("recordId", $"{recordId}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VolumeDtoResponse>>>("records/Volume/VolumeByRecordId");
                if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                {
                    recordVolumens = deserializeResponse.Data;
                }
                else
                {
                    NotificationModal.UpdateModal(ModalType.Information, Translation["NoVolume4File"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "NoVolumenNotification");
                }
            }
            catch (Exception ex)
            {
                NotificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }

        #endregion

        #region ModalMethods
        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        #endregion

        #region OpenModals

        private void ShowModalGeneralInformation(VolumeDtoResponse volumeRecord)
        {
            UploadPDFModal.GetVolumenId(volumeRecord.VolumeId);
            UploadPDFModal.UpdateModalStatus(true);
        }

        private void CreateNewVolume()
        {
            CreateVolumeRecordModal.GetDataForCreateVolumen(recordSelectedId, Convert.ToInt32(recordNumber), recordVolumens.Count + 1);
            CreateVolumeRecordModal.UpdateModalStatus(true);
        }

        #endregion

        #endregion

        #endregion

    }
}
