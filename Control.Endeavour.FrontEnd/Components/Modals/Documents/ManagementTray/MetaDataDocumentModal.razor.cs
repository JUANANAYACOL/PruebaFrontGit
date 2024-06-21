using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
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
    public partial class MetaDataDocumentModal
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

        #endregion Inject

        #region Modals

        private NotificationsComponentModal notificationModal = new();
        private GenericUserSearchModal genericUserSearch = new();

        #endregion Modals

        #region Models

        private EditMetadaDocumentDtoRequest MetaDataDocumentRequest = new();
        private List<MetadataListHistoryDtoResponse> MetaDataDocumentChanges = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string controlId = string.Empty;
        private string filingCode = string.Empty;
        private string classCodeMeta = string.Empty;
        private string classVisible = string.Empty;
        private string nameOfSignatory = string.Empty;
        private string editClassVisible = "d-none";
        private string gridClassVisible = "d-none";
        private string DocDescription = string.Empty;
        private string DocumentId = string.Empty;
        private string NRoGuia = string.Empty;
        private string PriorityCode = string.Empty;
        private string ReceptionCode = string.Empty;
        private string Notification = string.Empty;

        #endregion Environments(String)

        #region Environments(Numeric)

        private decimal CharacterCounterDescription = 0;
        private decimal CharacterCounterJustification = 0;
        private int ModalUserType = 1;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool visibleFields = false;
        private bool enableFieldsEdit = true;
        private bool isEditForm = false;
        private bool enableButtomDocument = true;
        private bool UserModalStatus = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private Dictionary<string, object> attributes = new Dictionary<string, object>
        {
            { "disabled", "disabled" }
        };

        private List<VSystemParamDtoResponse>? LstNotificacion = new();
        private List<VSystemParamDtoResponse>? LstReceptionCode = new();
        private List<VSystemParamDtoResponse>? LstPriorityCode = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        


        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetDocumentReceptionType();
            await GetNotification();
            await GetPriority();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleUpdateMetaDataDocument()
        {
            try
            {

                var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Document/UpdateMetadataDocument", MetaDataDocumentRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                    CancelEditMetaData();
                    await GetMetaDataDocumentHistory(int.Parse(controlId), filingCode, classCodeMeta);
                }
                else
                {
                    Console.WriteLine(deserializeResponse.Errors.ToString());
                    notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear: {ex.Message}");
            }
        }

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private void HandleModalClosed(bool value)
        {
            UpdateModalStatus(value);
            editClassVisible = string.Empty;
            attributes = new Dictionary<string, object>
            {
                { "disabled", "disabled" }
            };
            MetaDataDocumentRequest = new();
            isEditForm = false;
            enableFieldsEdit = true;
            classVisible = "d-none";
            editClassVisible = "d-none";
        }

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            try
            {
                if (!string.IsNullOrEmpty(args.ModalOrigin))
                {
                    switch (args.ModalOrigin)
                    {
                        case "EditMetaDataDocument":
                            HandleModalEdit(args);
                            break;

                        default:
                            notificationModal.UpdateModal(ModalType.Error, "ModalOrigin no reconocido", true);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
        }

        private async Task HandleModalEdit(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                gridClassVisible = "d-none";
                editClassVisible = string.Empty;
                isEditForm = true;
                enableFieldsEdit = false;
                attributes.Remove("disabled");
                MetaDataDocumentRequest.ControlId = int.Parse(controlId);
                CharacterCounterDescription = string.IsNullOrEmpty(MetaDataDocumentRequest.DocDescription) ? 0 : MetaDataDocumentRequest.DocDescription.Length; 
            }
        }

        private void CancelEditMetaData()
        {
            MetaDataDocumentRequest.DocDescription = DocDescription;
            MetaDataDocumentRequest.DocumentId = DocumentId;
            MetaDataDocumentRequest.NRoGuia = NRoGuia;
            MetaDataDocumentRequest.PriorityCode = PriorityCode;
            MetaDataDocumentRequest.ReceptionCode = ReceptionCode;
            enableFieldsEdit = true;
            isEditForm = false;
            editClassVisible = "d-none";
            gridClassVisible = "d-none";
            attributes = new Dictionary<string, object>
            {
                { "disabled", "disabled" }
            };
        }


        private void HandleSelectedUserData(List<RetunUserSearch> selectedUsers)
        {
            MetaDataDocumentRequest.SignatoryId = selectedUsers[0].UserId;
            MetaDataDocumentRequest.SignatoryType = selectedUsers[0].TypeOfUser;
            nameOfSignatory = selectedUsers[0].UserName;
        }

        #endregion HandleMethods

        #region OthersMethods

        #region ValidationMethods

        private void CountCharacters(ChangeEventArgs e, ref decimal CharacterCounter, string type = "")
        {
            string value = e.Value.ToString() ?? string.Empty;
            if(type == "Justification")
            {
                MetaDataDocumentRequest.Justification = value;
            }
            else

            if (!string.IsNullOrEmpty(value))
            {
                CharacterCounter = value.Length;
                if (CharacterCounter < 10)
                {
                    enableButtomDocument = false;
                }
            }
            else
            {
                CharacterCounter = 0;
            }
        }

        #endregion ValidationMethods

        #region ModalMethods

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        private void UserModalSearch()
        {
            if (isEditForm)
            {
                UserModalStatus = true;
            }
        }

        private void EditMetaDataDocument()
        {
            notificationModal.UpdateModal(ModalType.Information, Translation["UpdateMetaDataQuestion"], true, modalOrigin: "EditMetaDataDocument");
        }

        #endregion ModalMethods

        #region GetData

        private async Task GetDocumentReceptionType()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "MR");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    LstReceptionCode = deserializeResponse.Data;
                }
                else { LstReceptionCode = new(); }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
        }

        private async Task GetPriority()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "RPRI");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    LstPriorityCode = deserializeResponse.Data;
                }
                else { LstPriorityCode = new(); }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la prioridad: {ex.Message}");
            }
        }

        private async Task GetNotification()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "RNOTI");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    LstNotificacion = deserializeResponse.Data;
                }
                else { LstNotificacion = new(); }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la notificación: {ex.Message}");
            }
        }

        public async Task GetMetaDataDocumentHistory(int controlIdDocument, string filingDocumentCode, string classCode)
        {
            try
            {
                SetControlData(controlIdDocument, filingDocumentCode, classCode);

                HttpClient.DefaultRequestHeaders.Remove("ControlId");
                HttpClient.DefaultRequestHeaders.Add("ControlId", controlIdDocument.ToString());

                var documentHistory = await FetchDocumentHistory();
                if (documentHistory != null)
                {
                    MetaDataDocumentChanges = documentHistory;
                }

                var documentMetadata = await FetchDocumentMetadata();
                if (documentMetadata != null)
                {
                    SetDocumentMetadata(documentMetadata);
                }

                
            }
            catch (Exception ex)
            {
                // Handle exception or log it
            }
        }

        private void SetControlData(int controlIdDocument, string filingDocumentCode, string classCode)
        {
            filingCode = filingDocumentCode;
            controlId = controlIdDocument.ToString();
            classCodeMeta = classCode;
            switch (classCode)
            {
                case "CL,R":
                    visibleFields = true;
                    classVisible = string.Empty;
                    ModalUserType = 2;
                    break;

                case "CL,E":
                    visibleFields = false;
                    classVisible = "d-none";
                    ModalUserType = 1;
                    break;

                case "CL,I":
                    visibleFields = false;
                    classVisible = "d-none";
                    ModalUserType = 1;
                    break;
            }
        }

        private async Task<List<MetadataListHistoryDtoResponse>> FetchDocumentHistory()
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<MetadataListHistoryDtoResponse>>>("documentmanagement/Document/SearchMetadataDocumentList");
                if (response?.Succeeded ?? false)
                {
                    return response.Data;
                }
            }
            catch (Exception ex)
            {
                
            }
            return new List<MetadataListHistoryDtoResponse>();
        }

        private async Task<EditMetadaDocumentDtoRequest> FetchDocumentMetadata()
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<EditMetadaDocumentDtoRequest>>("documentmanagement/Document/SearchMetadataDocument");
                if (response?.Succeeded ?? false)
                {
                    return response.Data;
                }
            }
            catch (Exception ex)
            {
                
            }
            return null;
        }

        private void SetDocumentMetadata(EditMetadaDocumentDtoRequest metadata)
        {
            MetaDataDocumentRequest = metadata;
            DocDescription = metadata.DocDescription;
            DocumentId = metadata.DocumentId;
            NRoGuia = metadata.NRoGuia;
            PriorityCode = metadata.PriorityCode;
            ReceptionCode = metadata.ReceptionCode;
            nameOfSignatory = metadata.SignatoryName;
            gridClassVisible = string.Empty;
        }


        #endregion GetData

        #region CloseModalSearchUser
        private void HandleModalUserClosed(bool status)
        {

            UserModalStatus = status;

        }

        #endregion

        #endregion OthersMethods

        #endregion Methods
    }
}