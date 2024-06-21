using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD
{
    public partial class DocumentalVersionModal : ComponentBase
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

        #endregion Inject

        #region Components

        private InputModalComponent code = new();
        private InputModalComponent name = new();

        #endregion Components

        #region Modals

        private NotificationsComponentModal notificationModal { get; set; } = new();

        #endregion Modals

        #region Parameters

        [Parameter]
        public string Id { get; set; } = "";

        [Parameter]
        public EventCallback<bool> OnStatusChanged { get; set; }

        [Parameter]
        public EventCallback<bool> OnStatusChangedUpdate { get; set; }

        [Parameter]
        public EventCallback<bool> OnChangeData { get; set; }

        #endregion Parameters

        #region Models

        private DocumentalVersionDtoResponse docVersionResponse = new();
        private DocumentalVersionDtoResponse _selectedRecord = new();
        private DocumentalVersionDtoRequest docVersionDtoRequest = new();
        private DocumentalVersionUpdateDtoRequest docVersionDtoRequestUpdate = new DocumentalVersionUpdateDtoRequest();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string description { get; set; } = "";
        private string textType = "SelectAType";
        private string? versionType { get; set; } = "";
        private string extensionMessage { get; set; } = string.Empty;

        #endregion Environments(String)

        #region Environments(Numeric)

        private int FileSize = 100;

        private decimal characterCounter { get; set; } = 0;
        private int companyId { get; set; } = 0;

        #endregion Environments(Numeric)

        #region Environments(DateTime)

        private DateTime? from { get; set; }
        private DateTime? to { get; set; }
        private DateTime minValueTo { get; set; } = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime maxValueTo { get; set; } = DateTime.Now;

        #endregion Environments(DateTime)

        #region Environments(Bool)

        private bool IsEditForm { get; set; } = false;
        private bool modalStatus { get; set; } = false;
        private bool activeState { get; set; } = true;
        private bool currentDocumentalVersion { get; set; } = false;
        private bool saveIsDisable { get; set; } = true;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<FileInfoData> organizationFiles { get; set; } = new();
        private List<FileInfoData> adminActs { get; set; } = new();
        private string[] AllowedExtensions { get; set; } = { ".pdf" };
        private string[] documentalType = { "TVD", "TRD" };
        private List<CompanyDtoResponse> companiesList { get; set; } = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            extensionMessage = GetUploadMessage();
            await GetCompanies();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #region FormMethods

        #region SelectedMethod

        private async Task HandleValidSubmit()
        {
            if (IsEditForm)
            {
                await Update();
            }
            else
            {
                await Create();
            }
            StateHasChanged();
        }

        #endregion SelectedMethod

        #region CreateDocumentalVersion

        private async Task Create()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                docVersionDtoRequest.CompanyId = companyId;
                docVersionDtoRequest.VersionType = versionType;
                docVersionDtoRequest.Code = code.InputValue;
                docVersionDtoRequest.Name = name.InputValue;
                docVersionDtoRequest.Description = description;
                docVersionDtoRequest.StartDate = from;
                docVersionDtoRequest.EndDate = !currentDocumentalVersion ? to : null;

                //Organigrama
                docVersionDtoRequest.FileName = organizationFiles[0].Name!;
                docVersionDtoRequest.FileExt = organizationFiles[0].Extension!.Replace(".", "");
                docVersionDtoRequest.DataFile = organizationFiles[0].Base64Data!;

                docVersionDtoRequest.ActiveState = activeState;

                var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/DocumentalVersions/CreateDocumentalVersion", docVersionDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<DocumentalVersionDtoResponse>>();

                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                    await OnChangeData.InvokeAsync(true);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion CreateDocumentalVersion

        #region UpdateDocumentalVersion

        private async Task Update()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                docVersionDtoRequestUpdate.CompanyId = companyId;
                docVersionDtoRequestUpdate.VersionType = versionType;
                docVersionDtoRequestUpdate.Name = name.InputValue;
                docVersionDtoRequestUpdate.Code = code.InputValue;
                docVersionDtoRequestUpdate.Description = description;
                docVersionDtoRequestUpdate.StartDate = from;
                docVersionDtoRequestUpdate.EndDate = !currentDocumentalVersion ? to : null;
                docVersionDtoRequestUpdate.ActiveState = activeState;

                var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/DocumentalVersions/UpdateDocumentalVersion", docVersionDtoRequestUpdate);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<DocumentalVersionDtoResponse>>();
                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                    await OnChangeData.InvokeAsync(true);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion UpdateDocumentalVersion

        #endregion FormMethods

        #endregion HandleMethods

        #region OthersMethods

        #region ReceiveDocumentalVersion

        public void ReceiveDocumentalVersion(DocumentalVersionDtoResponse response)
        {
            _selectedRecord = response;

            companyId = _selectedRecord.CompanyId;
            docVersionDtoRequestUpdate.DocumentalVersionId = _selectedRecord.DocumentalVersionId;
            docVersionDtoRequest.VersionType = _selectedRecord.VersionType;
            docVersionDtoRequest.Name = _selectedRecord.Name;
            docVersionDtoRequest.Description = _selectedRecord.Description;
            docVersionDtoRequest.Code = _selectedRecord.Code;
            versionType = _selectedRecord.VersionType;
            from = _selectedRecord.StartDate;
            to = _selectedRecord.EndDate;
            currentDocumentalVersion = (to == null || to.Value > DateTime.Now);
            description = string.IsNullOrEmpty(_selectedRecord.Description) ? "" : _selectedRecord.Description;
            characterCounter = description.Length;
            activeState = (bool)_selectedRecord.ActiveState!;

            updateMaxValue();
            updateMinValue();
            IsEditForm = true;
            EnableSaveButton();
        }

        #endregion ReceiveDocumentalVersion

        #region GetCompanies

        private async Task GetCompanies()
        {
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<CompanyDtoResponse>>>("companies/Company/ByCompanies");
            if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
            {
                companiesList = deserializeResponse.Data;
            }
        }

        #endregion GetCompanies

        #region ResetFormAsync

        public async Task ResetFormAsync()
        {
            if (!IsEditForm)
            {
                docVersionDtoRequest = new();
            }

            activeState = true;
            organizationFiles = new();
            adminActs = new();
            docVersionDtoRequest.Name = "";
            docVersionDtoRequest.Code = string.Empty;
            docVersionDtoRequest.ActiveState = true;
            versionType = "";
            description = "";
            minValueTo = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            maxValueTo = DateTime.Now;
            from = null;
            to = null;
            currentDocumentalVersion = false;
            characterCounter = 0;
            companyId = 0;
            companiesList = new();
            await GetCompanies();
            EnableSaveButton();
            StateHasChanged();
        }

        #endregion ResetFormAsync

        #region HandleModal

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region HandleModalClosed

        private async Task HandleModalClosed(bool status)
        {
            modalStatus = status;
            IsEditForm = false;
            await ResetFormAsync();
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
                IsEditForm = false;
                await ResetFormAsync();
            }

            StateHasChanged();
        }

        #endregion HandleModalNotiClose

        #endregion HandleModal

        #region HandleDatePicker

        #region updateMinValue

        public void updateMinValue()
        {
            if (from != null)
            {
                minValueTo = (DateTime)from;
                minValueTo = minValueTo.AddDays(1);
            }
            EnableSaveButton();
            StateHasChanged();
        }

        #endregion updateMinValue

        #region updateMaxValue

        public void updateMaxValue()
        {
            if (to != null)
            {
                maxValueTo = (DateTime)to;
                maxValueTo = maxValueTo.AddDays(-1);
            }
            EnableSaveButton();
            StateHasChanged();
        }

        #endregion updateMaxValue

        #endregion HandleDatePicker

        #region CountCharacters

        private void CountCharacters(ChangeEventArgs e)
        {
            string value = e.Value!.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                characterCounter = value.Length;
            }
            else
            {
                characterCounter = 0;
            }
        }

        #endregion CountCharacters

        #region Acts

        #region HandleNewActsToCreate

        private void HandleNewActsToCreate(List<FileInfoData> data)
        {
            if (docVersionDtoRequest.AdministrativeActs.Count < 10)
            {
                var file = data.LastOrDefault();
                if (file != null)
                {
                    AdministrativeActDVDtoRequest act = new()
                    {
                        FileName = file.Name!,
                        FileExt = file.Extension!.Replace(".", ""),
                        DataFile = file.Base64Data!
                    };

                    if (docVersionDtoRequest.AdministrativeActs.Find(x => x.DataFile.SequenceEqual(act.DataFile)) == null)
                    {
                        adminActs.Clear();
                        docVersionDtoRequest.AdministrativeActs.Add(act);

                        StateHasChanged();
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, $"{Translation["FileAlreadyAttachedError"]}", true, Translation["Yes"], null);
                    }
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["NumberFilesExceededMessage"]}", true, Translation["Yes"], null);
            }

            EnableSaveButton();
        }

        #endregion HandleNewActsToCreate

        #region RemoveAct

        private void RemoveAdministrativeAct(AdministrativeActDVDtoRequest act)
        {
            docVersionDtoRequest.AdministrativeActs.Remove(act);
            EnableSaveButton();
        }

        #endregion RemoveAct

        #endregion Acts

        #region EnableSaveButton

        private async Task CheckCurrentDocumentalVersionInfo(bool check)
        {
            currentDocumentalVersion = check;
            if (check == true && versionType!.Contains("TRD"))
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["VerifyCurrentDocumentalVersion"], true, Translation["Accept"], "");
                if (companiesList.Count > 1)
                {
                    companiesList = companiesList.Skip(companiesList.Count - 1).ToList();
                    companyId = companiesList.LastOrDefault()!.CompanyId;
                }
                else if (companiesList.Count == 1)
                {
                    companyId = companiesList.FirstOrDefault()!.CompanyId;
                }
                else
                {
                    await GetCompanies();
                }
            }
            else
            {
                await GetCompanies();
            }

            EnableSaveButton();
        }

        public async Task ChangeVersionTypeAsync()
        {
            if ((currentDocumentalVersion && versionType!.Contains("TRD")) && companiesList.Count > 1)
            {
                companiesList = companiesList.Skip(companiesList.Count - 1).ToList();
                companyId = companiesList.LastOrDefault()!.CompanyId;
            }
            else if ((currentDocumentalVersion && versionType!.Contains("TRD")) && companiesList.Count == 1)
            {
                companyId = companiesList.FirstOrDefault()!.CompanyId;
            }
            else
            {
                await GetCompanies();
            }
        }

        public void EnableSaveButton()
        {
            var everyoneHasDateAndName = false;
            docVersionDtoRequest.AdministrativeActs.ToList().ForEach(x =>
            {
                if (x.AdminActDate == null || string.IsNullOrEmpty(x.ActName))
                {
                    everyoneHasDateAndName = true;
                }
            });

            var validFrom = string.IsNullOrEmpty(versionType) || (!currentDocumentalVersion || versionType.Contains("TVD"));
            if (validFrom)
            {
                if ((organizationFiles.Count == 0 && !IsEditForm) || from == null || to == null || string.IsNullOrEmpty(versionType) || string.IsNullOrEmpty(docVersionDtoRequest.Code) || string.IsNullOrEmpty(docVersionDtoRequest.Name) || companyId <= 0 || everyoneHasDateAndName)
                {
                    saveIsDisable = true;
                }
                else
                {
                    saveIsDisable = false;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(versionType) || (organizationFiles.Count == 0 && !IsEditForm) || from == null || string.IsNullOrEmpty(docVersionDtoRequest.Code) || string.IsNullOrEmpty(docVersionDtoRequest.Name) || (companyId <= 0 || (companiesList != null && companiesList.Count > 0 && companiesList.LastOrDefault()!.CompanyId != companyId)) || everyoneHasDateAndName)
                {
                    saveIsDisable = true;
                }
                else
                {
                    saveIsDisable = false;
                }
            }
            StateHasChanged();
        }

        #endregion EnableSaveButton

        #region GetUploadMessage

        public string GetUploadMessage()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            string extensions = string.Join(" " + Translation["Or"] + " ", AllowedExtensions).Replace(".", "").ToUpper();
            SpinnerLoaderService.HideSpinnerLoader(Js);

            return string.Format(Translation["AllowedFormatFiles"], extensions, FileSize);
        }

        #endregion GetUploadMessage

        #endregion OthersMethods

        #endregion Methods
    }
}