using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.TypologySearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VDocumentaryTypology.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray
{
    public partial class UpdateTypologyModal
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

        private VDocumentaryTypologyDtoResponse TRDSelected = new();

        #endregion

        #region Modals

        private GenericDocTypologySearchModal DocTypologySearchModal = new();
        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Parameters


        #endregion

        #region Models
        private ClassificationHistoryDtoResponse classificationHistoryDtoResponse = new();
        #endregion

        #region Environments

        #region Environments(String)

        private string docJustification = string.Empty;
        private string selectedTrd = string.Empty;
        private string classCodeValue = string.Empty;

        #endregion

        #region Environments(Numeric)
        private decimal characterCounter = 0;
        private int controlId = 0;
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool disabledButton = true;

        #endregion

        #region Environments(List & Dictionary)

        private List<ClassificationHistoryDtoResponse> classificationHistoryList = new();
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

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            StateHasChanged();
        }
        private async Task HandleModalNotiCloseAsync(ModalClosedEventArgs args)
        {
            try
            {
                if (args.IsAccepted && args.ModalOrigin.Equals("ValidateUpdateTypology"))
                {
                    SpinnerLoaderService.ShowSpinnerLoader(Js);
                    ClassificationHistoryDtoResponse request = new()
                    {
                        ControlId = controlId,
                        Justification = docJustification,
                        DocumentaryTypologyBehaviorLastId = TRDSelected.DocumentaryTypologyBehaviorId
                    };
                    if (ValidateFields(request))
                    {
                        var responseApi = await HttpClient!.PostAsJsonAsync("documentmanagement/ClassificationHistory/UpdateDocumentaryTypologyDocument", request);

                        var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ClassificationHistoryDtoResponse>>();
                        if (deserializeResponse.Succeeded)
                        {
                            await GetClassificationHistory();
                            ResetForm();
                            notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true);

                        }
                    }
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }

        }
        private void HandleTRDSelectedChanged(MyEventArgs<VDocumentaryTypologyDtoResponse> trd)
        {
            DocTypologySearchModal.resetModal();
            DocTypologySearchModal.UpdateModalStatus(trd.ModalStatus);
            TRDSelected = trd.Data;
            selectedTrd = TRDSelected.TypologyName;
        }

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #region FormMethods


        private async Task HandleValidSubmit()
        {
            try
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["UpdateTypologyQuestion"], true, modalOrigin: "ValidateUpdateTypology");


            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true, Translation["Accept"]);
            }
            // Lógica de envío del formulario

            StateHasChanged();
        }
        private void ResetForm()
        {
            docJustification = string.Empty;
            TRDSelected = new();
            selectedTrd = string.Empty;
            characterCounter = 0;
        }
        #endregion

        #endregion

        #region OthersMethods

        #region GetData

        async Task GetClassificationHistory()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                if (controlId > 0)
                {

                    HttpClient?.DefaultRequestHeaders.Remove("ControlId");
                    HttpClient?.DefaultRequestHeaders.Add("ControlId", $"{controlId}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ClassificationHistoryDtoResponse>>>("documentmanagement/ClassificationHistory/GetClassificationHistory").ConfigureAwait(false);
                    HttpClient?.DefaultRequestHeaders.Remove("ControlId");
                    if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                    {
                        classificationHistoryList = deserializeResponse.Data;
                    }
                    else
                    {
                        classificationHistoryList = new();
                    }
                }


                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Warning, Translation["ErrorProcessingRequest"] + ex.Message, true, Translation["Accept"], Translation["Cancel"]);
                
            }
        }

        #endregion

        #region ValidationMethods

        private void CountCharacters(ChangeEventArgs e)
        {
            String value = e.Value.ToString() ?? String.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                characterCounter = value.Length;
                if(characterCounter > 10)
                {
                    disabledButton = false;
                }
                else
                {
                    disabledButton = true;
                }

            }
            else
            {
                characterCounter = 0;

            }
        }

        private bool ValidateFields(ClassificationHistoryDtoResponse request)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(request.Justification))
            {
                errors.Add(Translation["Justification"]);
                errors.Add(Translation["Justification"]);
            }

            if (request.DocumentaryTypologyBehaviorLastId == 0 || request.DocumentaryTypologyBehaviorLastId == null)
            {
                errors.Add(Translation["DRT"]);
            }


            if (errors.Count > 0)
            {
                var message = string.Join(", ", errors);
                notificationModal.UpdateModal(ModalType.Error, Translation["FollowingFieldsRequired"] + ": " + message, true);
                return false;
            }

            return true;
        }

        #endregion

        #region ModalMethods

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        private void ShowModal(bool value)
        {
            DocTypologySearchModal.UpdateModalStatus(true, classCodeValue);
        }
        public async Task UpdateClassificationHistory(int Idcontrol,string classCode)
        {
            classCodeValue = classCode;
            controlId = Idcontrol;
            await GetClassificationHistory();
            StateHasChanged();
        }

        #endregion

        #endregion

        #endregion

    }
}
