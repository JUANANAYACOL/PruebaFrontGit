using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.Label;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.TypologySearch;
using Control.Endeavour.FrontEnd.Components.Modals.Records;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VDocumentaryTypology.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Request;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Models.Models.Records.Request;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.SearchEngines
{
    public partial class DocumentSearchEnginePage
    {

        #region Variables

        #region Inject 


        [Inject]
        private HttpClient? HttpClient { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }
        [Inject] private IJSRuntime Js { get; set; }
        #endregion

        #region Components

        private NewPaginationComponent<SearchDtoResponse, SearchDtoRequest> paginationComponent = new();
        #endregion

        #region Modals

        private GenericUserSearchModal genericUserSearch = new();
        private NotificationsComponentModal notificationModal = new();
        private GenericDocTypologySearchModal DocTypologySearchModal = new();
        private GeneralInformationModal generalInformation = new();
        private MetaDataRecordsFilter metaDataFilterModal = new();

        private LabelModal LabelModal = new();

        #endregion

        #region Parameters


        #endregion

        #region Models

        private SearchDtoRequest DocumentSearchDtoRequest = new();
        private VDocumentaryTypologyDtoResponse TRDSelected = new();
        private RetunUserSearch UserSelected = new();
        private PaginationInfo paginationInfo = new();

        #endregion

        #region Environments

        #region Environments(String)

        private string titleView = "Document search engine";
        private string ControlId = string.Empty;
        private string ReceptionCode = string.Empty;
        private string SearchEngineUserSearchPanel = "d-none";
        private string ResponseFilter = "Expedientes";
        private string HideFilterSearchPanel = string.Empty;
        private string buttonClassIcon = "fa-solid fa-eye-slash";
        private string buttonTextHide = string.Empty;
        private string UriSearchEngine = "documents/Document/SearchEngineDocument";
        private string SearchMetaDatosClass ="d-none";

        #endregion

        #region Environments(Numeric)
        public int UserSearchModalType = 3;
        #endregion

        #region Environments(DateTime)
        private DateTime Min = new DateTime(2019, 12, 31);
        private DateTime Max = new DateTime(2080, 12, 31);
        #endregion

        #region Environments(Bool)
        private bool modalUserStatus = false;

        #endregion

        #region Environments(List & Dictionary)
        private List<VSystemParamDtoResponse>? lstReceptionCode { get; set; } = new();
        private List<VSystemParamDtoResponse> classCodeList = new();
        private List<SearchDtoResponse> DocumentsList = new();
        private List<RetunUserSearch> listSender { get; set; } = new();
        private List<RetunUserSearch> listRecipient { get; set; } = new();
        private List<string> optionsFilter = new();
        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            titleView = Translation["DocumentSearchEngine"];
            optionsFilter = new List<string>() { Translation["Radicator"], Translation["Manager"] };
            ResponseFilter = Translation["Records"];
            buttonTextHide = Translation["HideParameters"];
            await GetTypeReception();
            await GetClasses();

        }


        #endregion

        #region Methods

        #region HandleMethods
        private void HandlePaginationGrid(List<SearchDtoResponse> newDataList)
        {
            DocumentsList = newDataList;
        }
        #region ModalDataReturnMethods
        private void HandleModalClosedLabel()
        {
            LabelModal.UpdateModalStatus(false);
        }
        private void HandleTRDSelectedChanged(MyEventArgs<VDocumentaryTypologyDtoResponse> trd)
        {
            DocTypologySearchModal.resetModal();
            DocTypologySearchModal.UpdateModalStatus(trd.ModalStatus);
            TRDSelected = trd.Data;
            SearchMetaDatosClass = string.Empty;
            DocumentSearchDtoRequest.DocumentaryTypologyBehaviorId = TRDSelected.DocumentaryTypologyBehaviorId;
        }

        private async Task HandleMetaDataFilter(List<MetaDataFilter> newDataList)
        {
            DocumentSearchDtoRequest.MetaData = newDataList;
        }


        #endregion

        #region HandleModalUserMethods


        private void HandleSelectedUserData(List<RetunUserSearch> selectedUsers)
        {
            if (UserSearchModalType == 1)
            {
                UserSelected = selectedUsers[0];
                DocumentSearchDtoRequest.UserId = UserSelected.UserId;

            }
            else if (UserSearchModalType == 3 || UserSearchModalType == 4 || UserSearchModalType == 5)
            {
                if (selectedUsers.Count > 0)
                {
                    listRecipient = selectedUsers
                   .Where(user => user.IsRecipient == true)
                   .ToList();

                    listSender = selectedUsers
                        .Where(user => user.IsSender == true)
                        .ToList();

                    DocumentSearchDtoRequest.Receivers = listRecipient
                       .Select(user => new ReceiversDtoRequest
                       {
                           IdUser = user.UserId,
                           TypeUser = user.TypeOfUser
                       })
                       .ToList();

                    DocumentSearchDtoRequest.Signatories = listSender
                        .Select(user => new SignatoryDtoRequest
                        {
                            IdUser = user.UserId,
                            TypeUser = user.TypeOfUser
                        })
                        .ToList();
                }
                else
                {
                    listRecipient = new();
                    listSender = new();
                    DocumentSearchDtoRequest.Signatories = new();
                    DocumentSearchDtoRequest.Receivers = new();
                }
            }




        }
        private void HandleModalClosed(bool status)
        {
            modalUserStatus = status;

        }

        #endregion

        #endregion

        #region OthersMethods

        #region OpenModalMethods
        private void showRecipient()
        {
            modalUserStatus = true;
        }
        private void showModalTRD()
        {
            DocTypologySearchModal.UpdateModalStatus(true);
        }
        private async Task showModalMetaDataAsync()
        {
            await metaDataFilterModal.LoadMetaDataBydocumentaryTypologyBagId(TRDSelected.DocumentaryTypologyBagId.Value);
            await metaDataFilterModal.UpdateModalStatusAsync(true);
            
        }
        private void showUserSerch()
        {
            UserSearchModalType = 1;
            modalUserStatus = true;
        }
        private async Task ShowModalLabelAsync(SearchDtoResponse record)
        {
            var receivers = record.DocumentReceivers.Split(',');
            var reciversLabel = string.Empty;
            if (receivers.Length > 1)
            {
                reciversLabel = receivers.First() + ", Otro(s)";
            }
            else
            {
                reciversLabel = receivers.First();
            }
            LabelModal.simpleUpdateModalStatus(true);
            await LabelModal.AssignVariablesAsync(record.FilingCode, record.ControlId.ToString() , reciversLabel,"","");
            
        }
        private async Task ShowModalGeneralInformation(SearchDtoResponse model)
        {
            await generalInformation.GeneralInformation(model.ControlId, model.ClassCodeName);
            generalInformation.UpdateModalStatus(true);
        }
        #endregion

        #region SearchMethods

        private async Task CleanFilter()
        {
            DocumentSearchDtoRequest = new();
            ControlId = string.Empty;
            SearchEngineUserSearchPanel = "d-none";
            DocumentsList = new();
            buttonClassIcon = "fa-solid fa-eye-slash";
            HideFilterSearchPanel = string.Empty;
            buttonTextHide = Translation["HideParameters"];


        }


        private async Task SearchDocuemntData()
        {

            var responseApi = await HttpClient.PostAsJsonAsync(UriSearchEngine, DocumentSearchDtoRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<SearchDtoResponse>>>();

            if (deserializeResponse.Data.Data.Count > 0 && deserializeResponse.Data != null)
            {
                DocumentsList = deserializeResponse.Data.Data;
                buttonTextHide = Translation["ShowParameters"];
                HideFilterSearchPanel = "d-none";

            }
            else
            {
                notificationModal.UpdateModal(Models.Enums.Components.Modals.ModalType.Information, Translation["CouldntFindInfByParameters"], true, buttonTextCancel: "");
            }
        }

        #endregion

        #region GetDataMethods
        private async Task GetClasses()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "CL");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    classCodeList = deserializeResponse.Data;
                }
                else { classCodeList = new(); }
            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(Models.Enums.Components.Modals.ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        private async Task GetTypeReception()
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
                    lstReceptionCode = deserializeResponse.Data;
                }
                else { lstReceptionCode = new(); }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(Models.Enums.Components.Modals.ModalType.Error, Translation["LoadErrorMessage"], true);
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

        #region ActionMethods
        private void OnValueChangedFilter(string newValue)
        {
            ResponseFilter = newValue;
            if (newValue == Translation["Radicator"])
            {

            }
            else if (newValue == Translation["Manager"])
            {
                DocumentSearchDtoRequest.SearchManagementUser = true;
            }
        }

        private void HidePanelSearch()
        {
            if (buttonClassIcon == "fa-solid fa-eye-slash")
            {
                buttonClassIcon = "fa-solid fa-eye";
                HideFilterSearchPanel = "d-none";
                buttonTextHide = Translation["ShowParameters"];

            }
            else
            {
                buttonClassIcon = "fa-solid fa-eye-slash";
                HideFilterSearchPanel = string.Empty;
                buttonTextHide = Translation["HideParameters"];
            }
        }

        private void DeleteTypologySelected()
        {
            TRDSelected = new();
            DocumentSearchDtoRequest.DocumentaryTypologyBehaviorId = 0;
            SearchMetaDatosClass = "d-none";
        }
        private void DeleteSenderSelected()
        {
            UserSelected = new();
            DocumentSearchDtoRequest.UserId = 0;
        }

        private void EnableUserSearch(string classCode)
        {
            DocumentSearchDtoRequest.ClassCode = classCode;
            if (!string.IsNullOrEmpty(DocumentSearchDtoRequest.ClassCode))
            {
                SearchEngineUserSearchPanel = string.Empty;
                switch (DocumentSearchDtoRequest.ClassCode)
                {
                    case "CL,E":
                        UserSearchModalType = 3;
                        break;
                    case "CL,I":
                        UserSearchModalType = 4;
                        break;
                    case "CL,R":
                        UserSearchModalType = 5;
                        break;
                }

            }
            else
            {
                DocumentSearchDtoRequest.Signatories = new();
                DocumentSearchDtoRequest.Receivers = new();
                SearchEngineUserSearchPanel = "d-none";
            }
        }
        private void RemoveUser(RetunUserSearch user)
        {
            if (user.IsSender == true)
            {
                listSender.RemoveAll(x => x.UserId == user.UserId);
            }
            if (user.IsRecipient == true)
            {
                listRecipient.RemoveAll(x => x.UserId == user.UserId);
            }
            genericUserSearch.RemoveUserSelectedById(user.UserId);
        }

        #endregion



        #endregion

        #endregion

    }
}
