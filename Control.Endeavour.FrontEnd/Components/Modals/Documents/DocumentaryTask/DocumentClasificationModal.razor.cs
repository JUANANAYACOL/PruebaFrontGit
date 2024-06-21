using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.TypologySearch;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VDocumentaryTypology.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.StateContainer.Documents;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.DocumentaryTask
{
    public partial class DocumentClasificationModal
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

        [Inject]
        private DocumentsStateContainer? documentsStateContainer { get; set; }

        #endregion Inject

        #region Modals

        private GenericDocTypologySearchModal genericDocTypologySearchModal = new();
        private GenericUserSearchModal genericUserSearch = new();

        #endregion Modals

        #region Parameters

        [Parameter]
        public EventCallback<DocumentClasificationDtoResponse> DocClasification { get; set; }

        #endregion Parameters

        #region Models

        private VDocumentaryTypologyDtoResponse TRDSelected = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string systemParamCL = "";
        private string systemParamMR = "";
        private string defaulTextCL = "";
        private string defaulTextMR = "";
        private string modalTitle = "";
        private string comunicationClass = "";
        public string descriptionInput { get; set; }

        #endregion Environments(String)

        #region Environments(Numeric)

        private int contadorcarac = 0;
        private int changeModal = 1;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool dropdownsEnable = true;
        private bool texAreaEnable = false;
        private bool isuser = false;
        private bool SeenSortDocs = false;
        private bool isEnableTRDButton = true;
        private bool isEnableReceiverButton = true;
        private bool validList = false;
        private bool dropdown = false;
        private bool enableButton = true;
        private bool UserModalStatus = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<UserClasification> UserList = new();
        private List<AdministrationUsers> ThirdList = new();
        private DocumentClasificationDtoResponse docClasification = new();
        private List<VSystemParamDtoResponse> systemFieldsCLList = new();
        private List<VSystemParamDtoResponse> systemFieldsMRList = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            defaulTextCL = Translation["SelectAnOption"];
            defaulTextMR = Translation["SelectAnOption"];
            modalTitle = Translation["UserSearch"];
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetClassCom();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
        }

        #endregion HandleMethods

        #region OthersMethods

        private void HandleSelectedModal(int changemodal)
        {
            try
            {
                changeModal = changemodal;

                if (changemodal == 2)
                {
                    modalTitle = Translation["UserSearch"] + " - " + Translation["NaturalLegalPersons"];
                }
                else
                {
                    modalTitle = Translation["UserSearch"];
                }

            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
            }
        }

        private void HandleSelectedUserData(List<RetunUserSearch> selectedUsers)
        {
            if (changeModal == 1)
            {
                var userRecords = selectedUsers
                .Where(user => user.TypeOfUser == "TDF,U")
                .Select(user => new UserClasification
                {
                    UserId = user.UserId,
                    Type = user.TypeOfUser,
                    FullName = user.UserName,
                    AdministrativeUnitName = user.UserAdministrativeUnitName,
                    ProductionOfficeName = user.UserProductionOfficeName,
                    Charge = user.UserPosition
                }).ToList();

                UserList.AddRange(userRecords);
                UserList = UserList.DistinctBy(x => x.UserId).ToList();

                if (docClasification.DestinationsUser == null)
                {
                    docClasification.DestinationsUser = new();
                }

                docClasification.DestinationsUser.AddRange(UserList);
                docClasification.DestinationsUser = docClasification.DestinationsUser.DistinctBy(x => x.UserId).ToList();
            }
            else
            {
                var thirdRecords = selectedUsers
                .Where(user => user.TypeOfUser == "TDF,T" || user.TypeOfUser == "TDF,TU")
                .Select(user => new AdministrationUsers
                {
                    ThirdPartyId = user.TypeOfUser == "TDF,T" ? user.UserId : (int?)null,
                    ThirdUserId = user.TypeOfUser == "TDF,TU" ? user.UserId : (int?)null,
                    FullName = user.UserName,
                    Type = user.TypeOfUser,
                    CompanyName = user.UserName,
                    IdentificationNumber = user.UserIdentificationNumber,
                    IdentificationTypeName = null,
                    Email = user.UserEmail
                }).ToList();

                ThirdList.AddRange(thirdRecords);
                ThirdList = ThirdList.DistinctBy(x => new { x.ThirdPartyId, x.ThirdUserId }).ToList();

                if (docClasification.DestinationsAdministration == null)
                {
                    docClasification.DestinationsAdministration = new();
                }

                docClasification.DestinationsAdministration.AddRange(ThirdList);
                docClasification.DestinationsAdministration = docClasification.DestinationsAdministration.DistinctBy(x => new { x.ThirdPartyId, x.ThirdUserId }).ToList();
            }

            validList = true;
        }

        private void HandleTRDSelectedChanged(MyEventArgs<VDocumentaryTypologyDtoResponse> trd)
        {
            genericDocTypologySearchModal.resetModal();
            genericDocTypologySearchModal.UpdateModalStatus(trd.ModalStatus);
            TRDSelected = trd.Data;

            if (TRDSelected != null)
            {
                GetTRDSelectedData(TRDSelected);
            }
        }

        #region GetComunicationClass

        public async Task GetClassCom()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "CL");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse.Data != null)
                {
                    systemFieldsCLList = deserializeResponse.Data.Where(x => !x.FieldCode.Equals("R")).Select(x => x).ToList() ?? new();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la clase de comunicacion: {ex.Message}");
            }
        }

        #endregion GetComunicationClass

        #region GetShippingMethod

        public async Task GetShippingM()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "MR");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse.Data != null)
                {
                    systemFieldsMRList = deserializeResponse.Data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el medio de envío: {ex.Message}");
            }
        }

        #endregion GetShippingMethod

        #region resetModal

        public void resetModal(bool value)
        {
            if (value)
            {
                docClasification.TypologyName = "";
                docClasification.AdministrativeUnitName = "";
                docClasification.ProductionOfficeName = "";
                docClasification.SeriesName = "";
                docClasification.SubSeriesName = "";
            }
            else
            {
                isEnableTRDButton = true;
                isEnableReceiverButton = true;
                docClasification.TypologyName = "";
                docClasification.AdministrativeUnitName = "";
                docClasification.ProductionOfficeName = "";
                docClasification.SeriesName = "";
                docClasification.SubSeriesName = "";
                descriptionInput = "";
                systemParamCL = "";
                systemParamMR = "";
                UserList = new();
                ThirdList = new();
                docClasification.DestinationsUser = new();
                docClasification.DestinationsAdministration = new();
                contadorcarac = 0;
            }
        }

        #endregion resetModal

        #region DropDown

        public async Task Dropdown(string value, bool value2)
        {
            if (value2)
            {
                resetModal(true);
                isEnableTRDButton = true;
                isEnableReceiverButton = true;
            }

            systemParamCL = value;

            if (value.Equals("CL,E"))
            {
                await GetShippingM();
                docClasification.DestinationsUser = new();
                dropdown = true;
                isuser = true;
                TRDSelected = new();
                UserList = new();
                ThirdList = new();
                comunicationClass = "Enviada";
            }
            else
            {
                docClasification.DestinationsAdministration = new();
                dropdown = false;
                isuser = false;
                isEnableTRDButton = false;
                systemParamMR = null;
                TRDSelected = new();
                UserList = new();
                ThirdList = new();
                comunicationClass = "Interna";
            }
            HandleSelectedModal(isuser ? 2 : 1);
            ValidateEnableButton();
        }

        public void Dropdown2(string value)
        {
            systemParamMR = value;

            if (!string.IsNullOrEmpty(systemParamMR))
            {
                isEnableTRDButton = false;
            }
            else
            {
                isEnableTRDButton = true;
            }
        }

        #endregion DropDown

        #region DeleteDestinations

        private async Task<bool> DeleteDestinations(int taskId, int id, string type)
        {
            UserDeleteDtoRequest Destinations = new UserDeleteDtoRequest()
            {
                TaskId = taskId,
                UserId = id,
                TypeUser = type
            };

            var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/DeleteDestinations", Destinations);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();

            if (deserializeResponse.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion DeleteDestinations

        #region DeleteUserResiver

        private async Task DeleteUserResiver(UserClasification user)
        {
            if (user.TaskId != null)
            {
                var deleteValidation = await DeleteDestinations(user.TaskId.Value, user.UserId, user.Type);

                if (deleteValidation)
                {
                    UserList.Remove(user);
                    docClasification.DestinationsUser.Remove(user);
                    genericUserSearch.RemoveUserSelectedById(user.UserId);
                }
            }
            else
            {
                UserList.Remove(user);
                docClasification.DestinationsUser.Remove(user);
                genericUserSearch.RemoveUserSelectedById(user.UserId);
            }
        }

        #endregion DeleteUserResiver

        #region DeleteThirdResiver

        private async Task DeleteThirdResiver(AdministrationUsers user)
        {
            if (user.TaskId != null)
            {
                var deleteValidation = await DeleteDestinations(user.TaskId.Value, (int)(user.ThirdPartyId ?? user.ThirdUserId), user.Type);

                if (deleteValidation)
                {
                    ThirdList.Remove(user);
                    docClasification.DestinationsAdministration.Remove(user);
                    var userId = user.ThirdUserId ?? user.ThirdUserId;
                    genericUserSearch.RemoveUserSelectedById(userId.Value);

                }
            }
            else
            {
                ThirdList.Remove(user);
                docClasification.DestinationsAdministration.Remove(user);
                var userId = user.ThirdUserId ?? user.ThirdUserId;
                genericUserSearch.RemoveUserSelectedById(userId.Value);
            }
        }

        #endregion DeleteThirdResiver

        #region CountChar

        private void ContarCaracteres(ChangeEventArgs e)
        {
            String value = e.Value.ToString() ?? String.Empty;
            descriptionInput = value;

            if (!string.IsNullOrEmpty(e.Value.ToString()))
            {
                contadorcarac = e.Value.ToString().Length;
            }
            else
            {
                contadorcarac = 0;
            }

            ValidateEnableButton();
        }

        #endregion CountChar

        #region SendDocumentClasification

        private async Task SelectSortDocAsync()
        {
            docClasification.Description = descriptionInput;
            docClasification.ClassCode = systemParamCL;
            docClasification.ComunicationClass = comunicationClass;

            if (systemParamMR != null)
            {
                docClasification.ShipingMethod = systemParamMR;
            }

            docClasification.ShipingMethod = systemParamMR;

            if (documentsStateContainer.documentResponseComentaryClosed.ControlId == 0)
            {
                docClasification.IdTypology = TRDSelected.DocumentaryTypologyBehaviorId;
            }

            docClasification.DestinationsUser = UserList;
            docClasification.DestinationsAdministration = ThirdList;

            await DocClasification.InvokeAsync(docClasification);
        }

        #endregion SendDocumentClasification

        #region UpdateDocumentClasification

        public async Task UpdateDocClasification(int id, bool value, DocumentClasificationDtoResponse doc)
        {
            try
            {
                SeenSortDocs = !value;
                dropdownsEnable = !SeenSortDocs;
                texAreaEnable = SeenSortDocs;

                HttpClient?.DefaultRequestHeaders.Remove("TaskId");
                HttpClient?.DefaultRequestHeaders.Add("TaskId", $"{id}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<DocumentClasificationDtoResponse>>("documentarytasks/DocumentaryTask/GetClasificationTask");
                HttpClient?.DefaultRequestHeaders.Remove("TaskId");

                docClasification = deserializeResponse.Data;
                docClasification.DestinationsUser = docClasification.DestinationsUser ?? new();
                docClasification.DestinationsAdministration = docClasification.DestinationsAdministration ?? new();

                if (SeenSortDocs)
                {
                    defaulTextCL = docClasification.ComunicationClass;
                    systemParamCL = docClasification.ClassCode;
                    await Dropdown(systemParamCL, false);
                    defaulTextMR = docClasification.ReceptionCode;
                    isEnableTRDButton = true;
                    systemParamMR = docClasification.ShipingMethod;
                    descriptionInput = docClasification.Description;
                    contadorcarac = docClasification.Description.Count();
                }
                else
                {
                    isEnableReceiverButton = false;

                    if (doc != null)
                    {
                        systemParamCL = doc.ClassCode;
                        await Dropdown(systemParamCL, false);
                        systemParamMR = doc.ShipingMethod;
                        docClasification.DestinationsAdministration.AddRange(doc.DestinationsAdministration);
                        docClasification.DestinationsUser.AddRange(doc.DestinationsUser);
                        descriptionInput = doc.Description;
                        contadorcarac = doc.Description.Count();
                        docClasification.TypologyName = doc.TypologyName;
                        docClasification.AdministrativeUnitName = doc.AdministrativeUnitName;
                        docClasification.ProductionOfficeName = doc.ProductionOfficeName;
                        docClasification.SeriesName = doc.SeriesName;
                        docClasification.SubSeriesName = doc.SubSeriesName;
                    }
                    else
                    {
                        systemParamCL = docClasification.ClassCode;
                        await Dropdown(systemParamCL, false);
                        systemParamMR = docClasification.ShipingMethod;
                        descriptionInput = docClasification.Description;
                        contadorcarac = docClasification.Description.Count();
                    }

                    ValidateEnableButton();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar la clasificacion del documento: {ex.Message}");
            }
        }

        #endregion UpdateDocumentClasification

        #region UpdateData

        public async Task DocClasificationMT(DocumentResponseComentaryClosedDtoResponse record)
        {
            systemParamCL = record.ClassCode;
            await Dropdown(systemParamCL, true);
            systemParamMR = record.ShipingMethod;
            docClasification.DestinationsAdministration = new();
            docClasification.DestinationsAdministration.AddRange(record.DestinationsAdministration);
            docClasification.TypologyName = record.TypologyName;
            docClasification.AdministrativeUnitName = record.AdministrativeUnitName;
            docClasification.ProductionOfficeName = record.ProductionOfficeName;
            docClasification.SeriesName = record.SeriesName;
            docClasification.SubSeriesName = record.SubSeriesName;
            docClasification.ShipingMethod = systemParamMR;
            docClasification.IdTypology = record.DocumentaryTypologiesBehaviorId;
            ThirdList.AddRange(record.DestinationsAdministration);
            ThirdList.ForEach(x => x.Type = "TDF,T");

            dropdownsEnable = false;
            isEnableReceiverButton = false;
            isEnableTRDButton = true;
        }

        #endregion UpdateData

        #region Data Modals

        //public void GetReceiverUsersData(List<VUserDtoResponse> receiver)
        //{
        //    UserList.AddRange(receiver.Select(x => new UserClasification
        //    {
        //        UserId = x.UserId,
        //        type = "TDF,U",
        //        FullName = x.FullName,
        //        AdministrativeUnitName = x.AdministrativeUnitName,
        //        ProductionOfficeName = x.ProductionOfficeName,
        //        Charge = x.Charge
        //    }).ToList());

        //    UserList = UserList.DistinctBy(x => x.UserId).ToList();

        //    if (docClasification.DestinationsUser == null)
        //    {
        //        docClasification.DestinationsUser = new();
        //    }

        //    docClasification.DestinationsUser.AddRange(UserList);
        //    docClasification.DestinationsUser = docClasification.DestinationsUser.DistinctBy(x => x.UserId).ToList();
        //    validList = true;
        //}

        //public void GetReceiverThirdData(List<ThirdPartyDtoResponse> receiver1, List<ThirdUserDtoResponse> receiver2)
        //{
        //    ThirdList.AddRange(receiver1.Select(x => new AdministrationUsers
        //    {
        //        ThirdPartyId = x.ThirdPartyId,
        //        FullName = string.IsNullOrEmpty(x.FullName.Trim()) ? x.Names : x.FullName,
        //        type = "TDF,T",
        //        CompanyName = x.Names,
        //        IdentificationNumber = x.IdentificationNumber,
        //        IdentificationTypeName = x.IdentificationTypeName,
        //        Email = (string.IsNullOrEmpty(x.Email1) ? x.Email2 : x.Email1)
        //    }).ToList());

        //    ThirdList.AddRange(receiver2.Select(x => new AdministrationUsers
        //    {
        //        ThirdUserId = x.ThirdUserId,
        //        FullName = x.Names,
        //        type = "TDF,TU",
        //        CompanyName = x.CompanyName,
        //        IdentificationNumber = x.IdentificationNumber,
        //        IdentificationTypeName = x.IdentificationTypeName,
        //        Email = x.Email
        //    }).ToList());

        //    ThirdList = ThirdList.DistinctBy(x => new { x.ThirdPartyId, x.ThirdUserId }).ToList();

        //    if (docClasification.DestinationsAdministration == null)
        //    {
        //        docClasification.DestinationsAdministration = new();
        //    }

        //    docClasification.DestinationsAdministration.AddRange(ThirdList);
        //    docClasification.DestinationsAdministration = docClasification.DestinationsAdministration.DistinctBy(x => new { x.ThirdPartyId, x.ThirdUserId }).ToList();
        //    validList = false;
        //}

        public void GetTRDSelectedData(VDocumentaryTypologyDtoResponse trd)
        {
            TRDSelected = trd;
            isEnableReceiverButton = false;
            docClasification.TypologyName = TRDSelected.TypologyName;
            docClasification.AdministrativeUnitName = TRDSelected.AdministrativeUnitName;
            docClasification.ProductionOfficeName = TRDSelected.ProductionOfficeName;
            docClasification.SeriesName = TRDSelected.SeriesName;
            docClasification.SubSeriesName = TRDSelected.SubSeriesName;
        }

        #endregion Data Modals

        #region Call Modals

        private void OpenModalTRD()
        {
            genericDocTypologySearchModal.UpdateModalStatus(true, systemParamCL);
        }

        private void OpenNewModalUser()
        {
            //genericSearchModal.UpdateModalStatus(true);
            UserModalStatus = true;
        }

        #endregion Call Modals

        #region ValidationMethods

        private void ValidateEnableButton()
        {
            enableButton = string.IsNullOrWhiteSpace(descriptionInput) ||
                   string.IsNullOrWhiteSpace(systemParamCL);
        }

        #endregion

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