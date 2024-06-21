using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VReplacement.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.Data.Browsing.Design;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.ThirdUser
{
    public partial class ThirdUserModal
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Modal

        private NotificationsComponentModal notificationModal = new();

        #endregion Modal

        #region Models
        private NotificationService NotificationService { get; set; } = new();
        private PaginationInfo paginationInfo = new();
        private ThirdUserDtoRequest? ThirdUserToEdit { get; set; } = null;
        private ThirdUserFilterDtoRequest filterDtoRequest { get; set; } = new();

        #endregion Models

        #region Components

        private NewPaginationComponent<ThirdUserDtoRequest, ThirdUserFilterDtoRequest> paginationComponent { get; set; } = new();
        private InputModalComponent IdentificationNumberInput { get; set; } = new();
        private InputModalComponent NamesInput { get; set; } = new();
        private InputModalComponent ChargueInput { get; set; } = new();
        private InputModalComponent OfficeInput { get; set; } = new();
        private InputModalComponent EmailInput { get; set; } = new();
        private InputModalComponent CellphoneInput { get; set; } = new();

        #endregion Components

        #region Environments

        #region Environments(Bool)
        private bool ExportAllPages { get; set; }
        public bool CancelExport { get; set; }
        private bool dataChargue { get; set; } = false;
        private bool ModalStatus { get; set; } = false;
        private bool IsEditForm { get; set; } = false;
        private bool saveIsDisable { get; set; } = true;
        private bool activeState { get; set; } = true;

        #endregion Environments(Bool)

        #region Enviroments(string)

        private string IdentificationNumber { get; set; } = string.Empty;
        private string identificationTypeCode { get; set; } = string.Empty;
        private string Names { get; set; } = string.Empty;
        private string Chargue { get; set; } = string.Empty;
        private string Office { get; set; } = string.Empty;
        private string Email { get; set; } = string.Empty;
        private string Cellphone { get; set; } = string.Empty;
        private string patternNumeric { get; set; } = @"\d";

        private string UriFilterThirdUser { get; set; } = "administration/ThirdUser/ByFilter";
        private string pattern { get; set; } = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        #region Enviroments(Numeric)

        private int ThirdPartyId { get; set; } = new();
        private int ThirdUserIdToDelete { get; set; } = new();

        #endregion Enviroments(Numeric)

        #endregion Enviroments(string)

        private List<ThirdUserDtoRequest> allRecords { get; set; }
        private List<VSystemParamDtoResponse> IdentificationTypeList { get; set; } = new();
        private List<ThirdUserDtoRequest> ThirdUserList { get; set; } = new();

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            await GetIdentificationType();

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleValidSubmit()
        {

            if (Regex.IsMatch(Names, patternNumeric))
            {
                notificationModal.UpdateModal(ModalType.Information, string.Format(Translation["NotValidParameter"], $"{Translation["NamesSurnames"]}", $"{Translation["Number(s)"]}"), true, Translation["Accept"], "");
            }




            else if (!Regex.IsMatch(Email, pattern))
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["VerifyEmail"], true, Translation["Accept"], "");
            }
            else
            {
                if (ThirdUserToEdit != null)
                {
                    await HandleUpdate();
                }
                else
                {
                    await HandleCreate();
                }
             
            }

            StateHasChanged();
        }

        private async Task HandleCreate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                ThirdUserDtoRequest newRecord = new()
                {
                    IdentificationType = identificationTypeCode,
                    IdentificationNumber = IdentificationNumber,
                    Names = Names,
                    Charge = Chargue,
                    Area = Office,
                    Email = Email,
                    CellPhone = Cellphone,
                    ActiveState = activeState,
                    ThirdPartyId = this.ThirdPartyId
                };

                var responseApi = await HttpClient.PostAsJsonAsync("administration/ThirdUser/CreateThirdUser", newRecord);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ThirdUserDtoResponse>>();

                var NotiResult = NotificationService.NotificationDecrypt(Translation, deserializeResponse);
                notificationModal.UpdateModal(NotiResult.ModalType, NotiResult.NotificationMessage, true, Translation["Accept"], "");

               
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task HandleUpdate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                ThirdUserDtoRequest updateThirdUSer = new()
                {
                    IdentificationType = identificationTypeCode,
                    IdentificationNumber = IdentificationNumber,
                    Names = Names,
                    Charge = Chargue,
                    Area = Office,
                    Email = Email,
                    CellPhone = Cellphone,
                    ActiveState = activeState,
                    ThirdUserId = ThirdUserToEdit.ThirdUserId,
                    Login = ThirdUserToEdit.Login,
                    Password = ThirdUserToEdit.Password,
                    ThirdPartyId = ThirdUserToEdit.ThirdPartyId,
                };

                var responseApi = await HttpClient.PostAsJsonAsync("administration/ThirdUser/UpdateThirdUser", updateThirdUSer);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ThirdUserDtoResponse>>();
                var NotiResult = NotificationService.NotificationDecrypt(Translation, deserializeResponse);
                notificationModal.UpdateModal(NotiResult.ModalType, NotiResult.NotificationMessage, true, Translation["Accept"],"");
              
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private void HandleModalClosed(bool status)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            ModalStatus = status;
            IsEditForm = false;

            ResetFormAsync();
            StateHasChanged();

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (notificationModal.Type == ModalType.Success && args.IsAccepted)
            {
                await UpdateThirdUserList(ThirdPartyId);
                ResetFormAsync();
            }
            else if (notificationModal.Type == ModalType.Warning && args.IsAccepted)
            {
                DeleteGeneralDtoRequest request = new() { Id = ThirdUserIdToDelete };
                var responseApi = await HttpClient!.PostAsJsonAsync("administration/ThirdUser/DeleteThirdUser", request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();

                var NotiResult = NotificationService.NotificationDecrypt(Translation, deserializeResponse);
 
                if (deserializeResponse!.Succeeded)
                {
                    if (args.ModalOrigin.Equals("DeleteModal"))
                    {
                        notificationModal.UpdateModal(NotiResult.ModalType, NotiResult.NotificationMessage, true, Translation["Accept"], "");
                    }
                }
                else
                {
                    notificationModal.UpdateModal(NotiResult.ModalType, NotiResult.NotificationMessage, true, Translation["Accept"], "");
                }
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private void HandleThirdUserUpdate(ThirdUserDtoRequest record)
        {
            ThirdUserToEdit = record;
            identificationTypeCode = record.IdentificationType;
            IdentificationNumber = record.IdentificationNumber;
            Names = record.Names;
            Chargue = record.Charge;
            Office = record.Area;
            Email = record.Email;
            Cellphone = record.CellPhone;
            activeState = record.ActiveState;
        }

        private void HandleToDelete(int id)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            ThirdUserIdToDelete = id;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "DeleteModal");
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private void HandlePaginationGrid(List<ThirdUserDtoRequest> newDataList)
        {
            ThirdUserList = newDataList;
        }

        #endregion HandleMethods

        #region OtherMethods

        public void ResetFormAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            ThirdUserIdToDelete = 0;
            identificationTypeCode = string.Empty;
            IdentificationNumber = string.Empty;
            Names = string.Empty;
            Chargue = string.Empty;
            Office = string.Empty;
            Email = string.Empty;
            Cellphone = string.Empty;
            activeState = true;
            ThirdUserToEdit = null;

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        public async Task UpdateModalStatus(bool newValue)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            ModalStatus = newValue;
            await GetIdentificationType();
            EnableSaveButton();

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        public async Task UpdateThirdUserList(int thirdUserId)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            ThirdPartyId = thirdUserId;

            filterDtoRequest = new()
            {
                ThirdPartyId = thirdUserId
            };

            var responseApi = await HttpClient.PostAsJsonAsync(UriFilterThirdUser, filterDtoRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<ThirdUserDtoRequest>>>();
            if (( deserializeResponse.Data != null && deserializeResponse.Data.Data != null && deserializeResponse.Data.Data.Count != 0 ) && deserializeResponse.Succeeded)
            {
                ThirdUserList = deserializeResponse.Data.Data ?? new();
                paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                paginationComponent.ResetPagination(paginationInfo);
                dataChargue = true;
            }
            else
            {
                ThirdUserList = new();
                paginationInfo = new();
                paginationComponent.ResetPagination(paginationInfo);
                dataChargue = false;
            }

            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task GetIdentificationType()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "TDIN");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                if (deserializeResponse!.Succeeded && ( deserializeResponse.Data != null && deserializeResponse.Data.Any() ))
                {
                    IdentificationTypeList = deserializeResponse.Data;
                }
                else
                {
                    IdentificationTypeList = new();
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, "BeServerErrorException", true, Translation["Accept"], "");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        public void EnableSaveButton()
        {

            Names= string.IsNullOrEmpty(Names)? string.Empty:Regex.Replace(Names, patternNumeric, string.Empty);


            if (!string.IsNullOrEmpty(Names) && !string.IsNullOrEmpty(IdentificationNumber) && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(identificationTypeCode))
            {
                saveIsDisable = false;
            }
            else
            {
                saveIsDisable = true;
            }
            StateHasChanged();
        }


        public  async Task GetAllRecords()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);


            filterDtoRequest = new()
            {
                ThirdPartyId = ThirdPartyId
            };

            var responseApi = await HttpClient.PostAsJsonAsync("administration/ThirdUser/ByFilterList", filterDtoRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ThirdUserDtoRequest>>>();
            if (( deserializeResponse.Data != null && deserializeResponse.Data != null && deserializeResponse.Data.Count != 0 ) && deserializeResponse.Succeeded)
            {
                allRecords = deserializeResponse.Data;
            }
            else
            {
                allRecords = new();
            }

            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }



        public async Task OnBeforeExcelExport(GridBeforeExcelExportEventArgs args)
        {

            if (ExportAllPages)
            {
                await GetAllRecords(); //aquí lo cambian por su método de consumir todos los registros

                if (allRecords.Any())
                {
                    args.Data = allRecords;
                }
            }
            else { args.Data = ThirdUserList; }

            args.IsCancelled = CancelExport;
        }
        #endregion OtherMethods

        #endregion Methods
    }
}