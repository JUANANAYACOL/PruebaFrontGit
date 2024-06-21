using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Profile.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Profile.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Security.Core.DTOs.Response;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.Profile
{
    public partial class ProfileModal : ComponentBase
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

        private NotificationsComponentModal notificationModal { get; set; } = new();
        private InputModalComponent profileInput { get; set; } = new();
        private InputModalComponent profileCodeInput { get; set; } = new();

        #endregion Components

        #region Parameters

        [Parameter] public bool modalStatus { get; set; }
        [Parameter] public EventCallback<bool> OnStatusChanged { get; set; }

        #endregion Parameters

        #region Models

        private CreateProfileDtoRequest profileCreateDtoRequest { get; set; } = new();
        private UpdateProfileDtoRequest profileUpdateDtoRequest { get; set; } = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string pattern { get; set; } = @"\d";
        public string profile { get; set; } = "";
        private bool activeState { get; set; } = true;
        public string profileCode { get; set; } = "";
        public string description { get; set; } = "";
        private bool isEditForm { get; set; }

        #endregion Environments(String)

        #region Environments(Numeric)

        private decimal CharacterCounter { get; set; } = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool saveIsDisable { get; set; } = true;

        private bool SelectAllCreateEnabled { get; set; }
        private bool SelectAllUpdateEnabled { get; set; }
        private bool SelectAllDeleteEnabled { get; set; }
        private bool SelectAllReadEnabled { get; set; }
        private bool SelectAllPrintEnabled { get; set; }
        private bool SelectAllAppendEnabled { get; set; }

        private bool SelectAllCreate { get; set; }

        private bool SelectAllUpdate { get; set; }

        private bool SelectAllRead { get; set; }

        private bool SelectAllDelete { get; set; }

        private bool SelectAllPrint { get; set; }

        private bool SelectAllAppend { get; set; }

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<FunctionalityToJson> functionalityList { get; set; } = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await GetFunctionalities();
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        #region HandleFormCreate

        private async Task HandleFormCreate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                profileCreateDtoRequest.Profile1 = profile;
                profileCreateDtoRequest.Description = description;
                profileCreateDtoRequest.ActiveState = activeState;
                profileCreateDtoRequest.ProfileCode = profileCode;
                profileCreateDtoRequest.PermissionJsonClass = functionalityList;

                var responseApi = await HttpClient!.PostAsJsonAsync("security/Profile/CreateProfile", profileCreateDtoRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ProfilesDtoResponse>>();

                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["CannotCreate"], true, Translation["Accept"]);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleFormCreate

        #region HandleFormUpdate

        private async Task HandleFormUpdate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                profileUpdateDtoRequest.Profile1 = profile;
                profileUpdateDtoRequest.Description = description;
                profileUpdateDtoRequest.ActiveState = activeState;
                profileUpdateDtoRequest.ProfileCode = profileCode;
                profileUpdateDtoRequest.PermissionJsonClass = functionalityList;

                var responseApi = await HttpClient!.PostAsJsonAsync("security/Profile/UpdateProfile", profileUpdateDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ProfilesDtoResponse>>();
                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["CannotUpdate"], true, Translation["Accept"]);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleFormUpdate

        #region GetFunctionalities

        private async Task GetFunctionalities()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                //replacementFilterDtoRequest = new();

                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<FunctionalityToJson>>>("access/View/ByFilter");

                //var responseApi = await HttpClient!.PostAsJsonAsync("administration/Profile/ByFilter", replacementFilterDtoRequest);
                //var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<VReplacementDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    functionalityList = deserializeResponse.Data ?? new();
                    functionalityList.ForEach(item => { item.Name = Translation[item.Name.Trim()]; });
                }
                else { functionalityList = new(); }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los perfiles de usuario: {ex.Message}");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetFunctionalities

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            isEditForm = false;

            modalStatus = newValue;
            ResetFormAsync();
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion UpdateModalStatus

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #endregion HandleMethods

        #region OthersMethods

        #region ResetFormAsync

        private void ResetFormAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            profile = string.Empty;
            description = string.Empty;
            profileCode = string.Empty;
            activeState = true;
            functionalityList.ForEach(item =>
            {
                item.Create = false;
                item.Read = false;
                item.Update = false;
                item.Delete = false;
                item.Append = false;
                item.Print = false;
            });
            SelectAllCreate = false;
            SelectAllUpdate = false;
            SelectAllDelete = false;
            SelectAllRead = false;
            SelectAllAppend = false;
            SelectAllPrint = false;
            CharacterCounter = 0;
            EnableSaveButton();
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ResetFormAsync

        #region HandleModalClosed

        private void HandleModalClosed(bool status)
        {
            ResetFormAsync();
            modalStatus = status;
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                await OnStatusChanged.InvokeAsync(false);
            }

            StateHasChanged();
        }

        #endregion HandleModalNotiClose

        #region HandleValidSubmit

        private async Task HandleValidSubmit()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            if (Regex.IsMatch(profile, pattern))
            {
                notificationModal.UpdateModal(ModalType.Information, string.Format(Translation["NotValidParameter"], $"{Translation["Profile"]}", $"{Translation["Number(s)"]}"), true, Translation["Accept"], "");
            }
            else if (string.IsNullOrEmpty(profile))
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["EnterName"], true, Translation["Accept"], "");
            }
            else if (string.IsNullOrEmpty(profileCode))
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["EnterCode"], true, Translation["Accept"], "");
            }
            else if (isEditForm)
            {
                await HandleFormUpdate();
            }
            else
            {
                await HandleFormCreate();
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleValidSubmit

        #region CountCharacters

        private void CountCharacters(ChangeEventArgs e)
        {
            string value = e.Value.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                CharacterCounter = value.Length;
            }
            else
            {
                CharacterCounter = 0;
            }
        }

        #endregion CountCharacters

        #region GridFunctionalities

        private bool IsAnySelected(IEnumerable<FunctionalityDtoResponse> selectedItems)
        {
            return selectedItems.Count() > 0 && selectedItems.Count() < functionalityList.Count();
        }

        private bool IsAllSelected(IEnumerable<FunctionalityDtoResponse> selectedItems)
        {
            return selectedItems.Count() == functionalityList.Count();
        }

        private bool GridHasData()
        {
            return functionalityList.Count() > 0;
        }

        #endregion GridFunctionalities

        #region selectAllItems

        private void selectAllItems(bool value, string nameColumn)
        {
            switch (nameColumn.ToLower())
            {
                case "create":
                    SelectAllCreate = value;
                    functionalityList.ForEach(item =>
                    {
                        item.Create = item.Jsonability.CreateAbility ? value : false;
                    });
                    break;

                case "update":
                    SelectAllUpdate = value;
                    functionalityList.ForEach(item => { item.Update = item.Jsonability.UpdateAbility ? value : false; });
                    break;

                case "delete":
                    SelectAllDelete = value;
                    functionalityList.ForEach(item => { item.Delete = item.Jsonability.DeleteAbility ? value : false; });
                    break;

                case "read":
                    SelectAllRead = value;
                    functionalityList.ForEach(item => { item.Read = item.Jsonability.ReadAbility ? value : false; ; });
                    break;

                case "append":
                    SelectAllAppend = value;
                    functionalityList.ForEach(item => { item.Append = item.Jsonability.AppendAbility ? value : false; });
                    break;

                case "print":
                    SelectAllPrint = value;
                    functionalityList.ForEach(item => { item.Print = item.Jsonability.PrintAbility ? value : false; });
                    break;

                default:
                    throw
                    new
                    ArgumentException(
                    "Nombre de propiedad inválido", nameof(nameColumn));
            }
        }

        #endregion selectAllItems

        #region fillSelectedItems

        private void fillSelectedItems(List<FunctionalityToJson> selectedFunctionalities)
        {
            selectedFunctionalities.ForEach(funcionality =>
            {
                var index = functionalityList.FindIndex(func => func.ViewId == funcionality.ViewId);
                if (index != null || index != -1)
                {
                    var name = functionalityList[index].Name;

                    name = Translation[name.Trim()];
                    functionalityList[index] = new()
                    {
                        Name = name,
                        Append = funcionality.Jsonability.AppendAbility ? funcionality.Append : false,
                        Create = funcionality.Jsonability.CreateAbility ? funcionality.Create : false,
                        Delete = funcionality.Jsonability.DeleteAbility ? funcionality.Delete : false,
                        Print = funcionality.Jsonability.PrintAbility ? funcionality.Print : false,
                        Read = funcionality.Jsonability.ReadAbility ? funcionality.Read : false,
                        Update = funcionality.Jsonability.UpdateAbility ? funcionality.Update : false,
                        ViewId = funcionality.ViewId,
                        Jsonability = funcionality.Jsonability
                    };
                }
            });
        }

        #endregion fillSelectedItems

        #region UpdateSelectedProfile

        public void UpdateSelectedProfile(ProfileDtoResponse profile)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            this.isEditForm = true;

            this.profile = profile.Profile1;
            this.activeState = profile.ActiveState;
            this.description = profile.Description;
            this.profileCode = profile.ProfileCode;
            profileUpdateDtoRequest.ProfileId = profile.ProfileId;
            profileCreateDtoRequest.Profile1 = profile.Profile1;
            profileCreateDtoRequest.ActiveState = profile.ActiveState;
            profileCreateDtoRequest.Description = profile.Description;
            profileCreateDtoRequest.ProfileCode = profile.ProfileCode;
            CharacterCounter = profileCreateDtoRequest.Description.Length;
            profileCreateDtoRequest.PermissionJsonClass = profile.PermissionJsonClass ?? new();

            fillSelectedItems(profile.PermissionJsonClass);
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion UpdateSelectedProfile

        #region EnableSaveButton

        public void EnableSaveButton()
        {
            profile = string.IsNullOrEmpty(profile) ? string.Empty : Regex.Replace(profile, pattern, string.Empty);

            if (string.IsNullOrEmpty(profile) || string.IsNullOrEmpty(profileCode))
            {
                saveIsDisable = true;
            }
            else
            {
                saveIsDisable = false;
            }
            StateHasChanged();
        }

        #endregion EnableSaveButton

        #endregion OthersMethods

        #endregion Methods
    }
}