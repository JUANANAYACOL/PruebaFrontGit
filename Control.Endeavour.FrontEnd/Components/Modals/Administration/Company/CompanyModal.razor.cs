using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.Company
{
    public partial class CompanyModal : ComponentBase
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

        private DragAndDropImageComponent logoDADComponent { get; set; } = new();
        private DragAndDropImageComponent bannerDADComponent { get; set; } = new();
        private DragAndDropImageComponent mobileBannerDADComponent { get; set; } = new();
        private InputModalComponent namebussiness = new();
        private InputModalComponent NIT = new();
        private InputModalComponent phone = new();
        private InputModalComponent web = new();
        private InputModalComponent email = new();
        private InputModalComponent identification = new();
        private InputModalComponent cellphone = new();
        private InputModalComponent nameAgentLegal = new();

        #endregion Components

        #region Modals

        private NotificationsComponentModal notificationModal = new();

        #endregion Modals

        #region Parameters

        [Parameter]
        public bool ModalStatus { get; set; }

        [Parameter]
        public EventCallback<bool> OnChangeData { get; set; }

        [Parameter]
        public EventCallback<bool> OnStatusChanged { get; set; }

        [Parameter]
        public EventCallback<AddressDtoResponse> OnDataSaved { get; set; }

        [Parameter]
        public EventCallback OnResetForm { get; set; }

        #endregion Parameters

        #region Models

        //Pictures
        private FileCompanyDtoRequest logoFile { get; set; } = new();

        private FileCompanyDtoRequest bannerFile { get; set; } = new();
        private FileCompanyDtoRequest mobileBannerFile { get; set; } = new();

        //Company
        private CompanyCreateDtoRequest CompaniesFormCreate = new CompanyCreateDtoRequest();

        private CompanyUpdateDtoRequest CompaniesFormUpdate = new CompanyUpdateDtoRequest();
        private CompanyDtoResponse CompaniesFormResponse = new CompanyDtoResponse();
        private CompanyDtoResponse _selectedRecord = new();

        //Address
        private AddressDtoRequest AddressformCompaniesRequest = new();

        private AddressDtoResponse AddressformCompaniesResponse = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        //pictures
        private List<FileInfoData> logoFileInfoData { get; set; } = new();

        private List<FileInfoData> bannerFileInfoData { get; set; } = new();
        private List<FileInfoData> mobileBannerFileInfoData { get; set; } = new();
        private string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png" };
        private string[] AllowedExtensionsLogo { get; set; } = { ".png" };

        //Address
        private string textAddress = string.Empty;

        //DocumentType
        private string legalAgentIdType { get; set; } = "";

        private string identificationType { get; set; } = "";

        #endregion Environments(String)

        #region Environments(Numeric)

        //pictures
        private int FileSize = 100;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool IsEditForm = false;
        private bool IsDisabledCode = false;
        private bool enableButton = true;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<VSystemParamDtoResponse>? documentTypeTDIJ;
        private List<VSystemParamDtoResponse>? documentTypeTDIN;

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            textAddress = Translation["EnterAddress"];
            ModalStatus = false;
            await GetDocumentTypeTDIJ();
            await GetDocumentTypeTDIN();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleMethods

        #region OtherMethods

        #region FormMethods

        #region SelectedMethod

        private async Task HandleValidSubmit()
        {
            try
            {
                if (IsEditForm)
                {
                    await Update();
                }
                else
                {
                    await Create();
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true, Translation["Accept"]);
            }

            StateHasChanged();
        }

        #endregion SelectedMethod

        #region Create

        private async Task Create()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                CompaniesFormCreate.CompanyData = new();
                CompaniesFormCreate.CompanyData.Address = new();

                CompaniesFormCreate.CompanyData.Address = AddressformCompaniesRequest;

                //Datos CompanyData
                CompaniesFormCreate.CompanyData.LegalAgentFullName = nameAgentLegal.InputValue;
                CompaniesFormCreate.CompanyData.CellPhoneNumber = cellphone.InputValue;
                CompaniesFormCreate.CompanyData.LegalAgentId = identification.InputValue;
                CompaniesFormCreate.CompanyData.PhoneNumber = phone.InputValue;
                CompaniesFormCreate.CompanyData.Email = email.InputValue;
                CompaniesFormCreate.CompanyData.WebAddress = web.InputValue;
                CompaniesFormCreate.CompanyData.LegalAgentIdType = legalAgentIdType;

                //Datos Companies
                CompaniesFormCreate.Identification = NIT.InputValue;
                CompaniesFormCreate.IdentificationType = identificationType;
                CompaniesFormCreate.BusinessName = namebussiness.InputValue;

                //Logo y Banner
                CompaniesFormCreate.CompanyData.LogoFile = logoFile.DataFile == null ? null : logoFile;
                CompaniesFormCreate.CompanyData.BannerFile = bannerFile.DataFile == null ? null : bannerFile;
                CompaniesFormCreate.CompanyData.MobileBannerFile = mobileBannerFile.DataFile == null ? null : mobileBannerFile;
                if (ValidateCompanyFields(CompaniesFormCreate))
                {
                    var responseApi = await HttpClient!.PostAsJsonAsync("companies/Company/CreateCompany", CompaniesFormCreate);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<CompanyDtoResponse>>();
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

                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
            }
        }

        #endregion Create

        #region Update

        public bool IsEmptyAddress(AddressDtoRequest address)
        {
            return address.CountryId == 0 &&
                   address.StateId == 0 &&
                   address.CityId == 0 &&
                   string.IsNullOrEmpty(address.StType) &&
                   string.IsNullOrEmpty(address.StNumber) &&
                   string.IsNullOrEmpty(address.StLetter) &&
                   (address.StBis == null) &&
                   string.IsNullOrEmpty(address.StComplement) &&
                   string.IsNullOrEmpty(address.StCardinality) &&
                   string.IsNullOrEmpty(address.CrType) &&
                   string.IsNullOrEmpty(address.CrNumber) &&
                   string.IsNullOrEmpty(address.CrLetter) &&
                   (address.CrBis == null) &&
                   string.IsNullOrEmpty(address.CrComplement) &&
                   string.IsNullOrEmpty(address.CrCardinality) &&
                   string.IsNullOrEmpty(address.HouseType) &&
                   string.IsNullOrEmpty(address.HouseClass) &&
                   string.IsNullOrEmpty(address.HouseNumber);
        }

        public async Task Update()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                CompaniesFormUpdate = new();
                CompaniesFormUpdate.CompanyData = new();
                var addressDtoRequestOld = fillAddress(AddressformCompaniesResponse);

                //CompanyData Response
                _selectedRecord.LegalAgentFullName = nameAgentLegal.InputValue;
                _selectedRecord.CellPhoneNumber = cellphone.InputValue;
                _selectedRecord.LegalAgentId = identification.InputValue;
                _selectedRecord.PhoneNumber = phone.InputValue;
                _selectedRecord.Email = email.InputValue;
                _selectedRecord.WebAddress = web.InputValue;
                _selectedRecord.LegalAgentIdType = legalAgentIdType;

                //Companies Response
                _selectedRecord.Identification = NIT.InputValue;
                _selectedRecord.IdentificationType = identificationType;
                _selectedRecord.BusinessName = namebussiness.InputValue;

                //Company Data Request
                CompaniesFormUpdate.CompanyData.LegalAgentIdType = _selectedRecord.LegalAgentIdType;
                CompaniesFormUpdate.CompanyData.LegalAgentId = _selectedRecord.LegalAgentId;
                CompaniesFormUpdate.CompanyData.LegalAgentFullName = _selectedRecord.LegalAgentFullName;
                CompaniesFormUpdate.CompanyData.CellPhoneNumber = _selectedRecord.CellPhoneNumber;

                CompaniesFormUpdate.CompanyData.PhoneNumber = _selectedRecord.PhoneNumber;
                CompaniesFormUpdate.CompanyData.Email = _selectedRecord.Email;
                CompaniesFormUpdate.CompanyData.WebAddress = _selectedRecord.WebAddress;
                CompaniesFormUpdate.CompanyData.Domain = _selectedRecord.Domain!;

                CompaniesFormUpdate.CompanyData.Address = (AddressformCompaniesRequest == null || IsEmptyAddress(AddressformCompaniesRequest)) ? addressDtoRequestOld : AddressformCompaniesRequest;

                //Companies Request
                CompaniesFormUpdate.Identification = _selectedRecord.Identification;
                CompaniesFormUpdate.IdentificationType = ((!identificationType.Contains("TDIJ")) ? $"{identificationType}" : CompaniesFormResponse.IdentificationType);
                CompaniesFormUpdate.BusinessName = _selectedRecord.BusinessName;

                FileCompanyDtoRequest? fileLogo = null;
                FileCompanyDtoRequest? fileBanner = null;
                FileCompanyDtoRequest? fileMobileBanner = null;
                if (CompaniesFormResponse.LogoFileId != null && logoFile.DataFile != null)
                {
                    fileLogo = new FileCompanyDtoRequest { FileExt = logoFile!.FileExt, FileName = logoFile.FileName, DataFile = logoFile.DataFile };
                }
                if (CompaniesFormResponse.BannerFileId != null && bannerFile.DataFile != null)
                {
                    fileBanner = new FileCompanyDtoRequest { FileExt = bannerFile!.FileExt, FileName = bannerFile.FileName, DataFile = bannerFile.DataFile };
                }
                if (CompaniesFormResponse.MobileBannerFileId != null && mobileBannerFile.DataFile != null)
                {
                    fileMobileBanner = new FileCompanyDtoRequest { FileExt = mobileBannerFile!.FileExt, FileName = mobileBannerFile.FileName, DataFile = mobileBannerFile.DataFile };
                }

                CompaniesFormUpdate.CompanyData.LogoFile = logoFile.DataFile == null ? fileLogo : logoFile;
                CompaniesFormUpdate.CompanyData.BannerFile = bannerFile.DataFile == null ? fileBanner : bannerFile;
                CompaniesFormUpdate.CompanyData.MobileBannerFile = mobileBannerFile.DataFile == null ? fileMobileBanner : mobileBannerFile;
                CompaniesFormUpdate.Id = _selectedRecord.CompanyId;
                if (ValidateCompanyFields(CompaniesFormUpdate))
                {
                    var responseApi = await HttpClient!.PostAsJsonAsync("companies/Company/UpdateCompany", CompaniesFormUpdate);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<CompanyDtoResponse>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                        await OnChangeData.InvokeAsync(true);
                        IsEditForm = false;
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
                        IsEditForm = true;
                    }
                }

                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
            }
        }

        #endregion Update

        #endregion FormMethods

        #region GetDocumentType

        #region GetDocumentTypeTDIN

        private async Task GetDocumentTypeTDIN()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "TDIN");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    documentTypeTDIN = deserializeResponse.Data!;
                }
                else
                {
                    documentTypeTDIN = null;
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de identificación: {ex.Message}");
            }
        }

        #endregion GetDocumentTypeTDIN

        #region GetDocumentTypeTDIJ

        private async Task GetDocumentTypeTDIJ()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "TDIJ");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                if (deserializeResponse!.Succeeded)
                {
                    documentTypeTDIJ = deserializeResponse.Data!;
                }
                else
                {
                    documentTypeTDIJ = null;
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de identificación: {ex.Message}");
            }
        }

        #endregion GetDocumentTypeTDIJ

        #endregion GetDocumentType

        #region GetPictures

        private async Task<FileDtoResponse?> GetPictures(int? id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                if (deserializeResponse!.Succeeded)
                {
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    return deserializeResponse.Data!;
                }
                else
                {
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    return null;
                }
            }
            catch
            {
                return new FileDtoResponse();
            }
        }

        #endregion GetPictures

        #region HandlePictures

        private void HandleMobileBannerPicture(List<FileInfoData> data)
        {
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                var pictureMobileBanner = data[0];
                mobileBannerFile = new FileCompanyDtoRequest();
                mobileBannerFile = new() { FileExt = pictureMobileBanner.Extension!.Replace(".", ""), DataFile = pictureMobileBanner.Base64Data!, FileName = pictureMobileBanner.Name };
                mobileBannerFileInfoData.Add(new FileInfoData { Name = pictureMobileBanner!.Name, Extension = pictureMobileBanner.Extension, Base64Data = pictureMobileBanner.Base64Data });
            }
            ValidateEnableButton();
        }

        private void HandleBannerPicture(List<FileInfoData> data)
        {
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                var pictureBanner = data[0];
                bannerFile = new FileCompanyDtoRequest();
                bannerFile = new() { FileExt = pictureBanner.Extension!.Replace(".", ""), DataFile = pictureBanner.Base64Data!, FileName = pictureBanner.Name };
                bannerFileInfoData.Add(new FileInfoData { Name = pictureBanner!.Name, Extension = pictureBanner.Extension, Base64Data = pictureBanner.Base64Data });
            }
            ValidateEnableButton();
        }

        private void HandleLogoPicture(List<FileInfoData> data)
        {
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                var pictureLogo = data[0];
                logoFile = new FileCompanyDtoRequest();
                logoFile = new() { FileExt = pictureLogo.Extension!.Replace(".", ""), DataFile = pictureLogo.Base64Data!, FileName = pictureLogo.Name };
                logoFileInfoData.Add(new FileInfoData { Name = pictureLogo!.Name, Extension = pictureLogo.Extension, Base64Data = pictureLogo.Base64Data });
            }
            ValidateEnableButton();
        }

        #endregion HandlePictures

        #region Record

        public async Task RecibirRegistro(CompanyDtoResponse response)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await GetDocumentTypeTDIN();
            await GetDocumentTypeTDIJ();
            _selectedRecord = response;
            IsDisabledCode = true;
            IsEditForm = true;

            CompaniesFormResponse.CompanyId = _selectedRecord.CompanyId;
            CompaniesFormResponse.AddressId = _selectedRecord.AddressId;
            CompaniesFormResponse.Identification = _selectedRecord.Identification;
            //Request
            CompaniesFormResponse.AddressId = _selectedRecord.AddressId;
            CompaniesFormResponse.Address = _selectedRecord.Address;
            textAddress = string.IsNullOrEmpty(_selectedRecord.Address.Trim()) ? Translation["EnterAddress"] : _selectedRecord.Address;
            CompaniesFormResponse.LegalAgentFullName = _selectedRecord.LegalAgentFullName;
            CompaniesFormResponse.CellPhoneNumber = _selectedRecord.CellPhoneNumber;
            CompaniesFormResponse.LegalAgentId = _selectedRecord.LegalAgentId;
            CompaniesFormResponse.PhoneNumber = _selectedRecord.PhoneNumber;
            CompaniesFormResponse.Email = _selectedRecord.Email;
            CompaniesFormResponse.WebAddress = _selectedRecord.WebAddress;
            CompaniesFormResponse.Domain = _selectedRecord.Domain;
            CompaniesFormResponse.LegalAgentIdType = _selectedRecord.LegalAgentIdType;

            //Companies Request
            CompaniesFormResponse.Identification = _selectedRecord.Identification;
            CompaniesFormResponse.IdentificationType = _selectedRecord.IdentificationType;
            CompaniesFormResponse.BusinessName = _selectedRecord.BusinessName;
            CompaniesFormResponse.LogoFileId = null;
            CompaniesFormResponse.BannerFileId = null;
            CompaniesFormResponse.MobileBannerFileId = null;

            if (_selectedRecord.LogoFileId != null && _selectedRecord.LogoFileId != 0)
            {
                var pictureLogo = await GetPictures(_selectedRecord.LogoFileId);
                logoFileInfoData.Add(new FileInfoData { Name = pictureLogo!.FileName, Extension = pictureLogo.FileExt, Base64Data = pictureLogo.DataFile });
                logoFile = new() { FileExt = pictureLogo.FileExt.Replace(".", ""), DataFile = pictureLogo.DataFile, FileName = pictureLogo.FileName };
                await logoDADComponent.NotifyFileListChanged();
                CompaniesFormResponse.LogoFileId = _selectedRecord.LogoFileId;
            }
            if (_selectedRecord.BannerFileId != null && _selectedRecord.BannerFileId != 0)
            {
                var pictureBanner = await GetPictures(_selectedRecord.BannerFileId);
                bannerFileInfoData.Add(new FileInfoData { Name = pictureBanner!.FileName, Extension = pictureBanner.FileExt, Base64Data = pictureBanner.DataFile });
                bannerFile = new() { FileExt = pictureBanner.FileExt.Replace(".", ""), DataFile = pictureBanner.DataFile, FileName = pictureBanner.FileName };
                await bannerDADComponent.NotifyFileListChanged();
                CompaniesFormResponse.BannerFileId = _selectedRecord.BannerFileId;
            }
            if (_selectedRecord.MobileBannerFileId != null && _selectedRecord.MobileBannerFileId != 0)
            {
                var pictureMobileBanner = await GetPictures(_selectedRecord.MobileBannerFileId);
                mobileBannerFileInfoData.Add(new FileInfoData { Name = pictureMobileBanner!.FileName, Extension = pictureMobileBanner.FileExt, Base64Data = pictureMobileBanner.DataFile });
                mobileBannerFile = new() { FileExt = pictureMobileBanner.FileExt.Replace(".", ""), DataFile = pictureMobileBanner.DataFile, FileName = pictureMobileBanner.FileName };
                await mobileBannerDADComponent.NotifyFileListChanged();
                CompaniesFormResponse.MobileBannerFileId = _selectedRecord.MobileBannerFileId;
            }

            identificationType = _selectedRecord.IdentificationType;
            legalAgentIdType = _selectedRecord.LegalAgentIdType!;
            await GetAddressAsync();
            ValidateEnableButton();
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion Record

        #region ModalAddressMethods

        #region UpdateAddress

        public void updateAddressSelection(List<(string, AddressDtoRequest)> address)
        {
            if (address != null && address.Count > 0)
            {
                (textAddress, AddressformCompaniesRequest) = address[0];
            }
            textAddress = string.IsNullOrEmpty(textAddress.Trim()) ? Translation["EnterAddress"] : textAddress;
            AddressformCompaniesResponse = fillAddressUpdate(address[0].Item2);
            ValidateEnableButton();
            StateHasChanged();
        }

        #endregion UpdateAddress

        #region FillAddress

        private AddressDtoRequest fillAddress(AddressDtoResponse address)
        {
            AddressDtoRequest? request = new AddressDtoRequest();
            request.CountryId = address.CountryId;
            request.StateId = address.StateId;
            request.CityId = address.CityId;
            request.StType = address.StType!;
            request.StNumber = address.StNumber!;
            request.StLetter = address.StLetter!;
            request.StBis = address.StBis != null ? (bool)address.StBis : false;
            request.StComplement = address.StComplement!;
            request.StCardinality = address.StCardinality!;
            request.CrType = address.CrType!;
            request.CrNumber = address.CrNumber!;
            request.CrLetter = address.CrLetter!;
            request.CrBis = address.CrBis != null ? (bool)address.CrBis : false;
            request.CrComplement = address.CrComplement!;
            request.CrCardinality = address.CrCardinality!;
            request.HouseType = address.HouseType!;
            request.HouseClass = address.HouseClass!;
            request.HouseNumber = address.HouseNumber!;
            return request;
        }

        private AddressDtoResponse fillAddressUpdate(AddressDtoRequest address)
        {
            AddressDtoResponse? request = new AddressDtoResponse();
            request.CountryId = address.CountryId;
            request.StateId = address.StateId;
            request.CityId = address.CityId;
            request.StType = address.StType!;
            request.StNumber = address.StNumber!;
            request.StLetter = address.StLetter!;
            request.StBis = address.StBis != null ? (bool)address.StBis : false;
            request.StComplement = address.StComplement!;
            request.StCardinality = address.StCardinality!;
            request.CrType = address.CrType!;
            request.CrNumber = address.CrNumber!;
            request.CrLetter = address.CrLetter!;
            request.CrBis = address.CrBis != null ? (bool)address.CrBis : false;
            request.CrComplement = address.CrComplement!;
            request.CrCardinality = address.CrCardinality!;
            request.HouseType = address.HouseType!;
            request.HouseClass = address.HouseClass!;
            request.HouseNumber = address.HouseNumber!;
            return request;
        }

        #endregion FillAddress

        #endregion ModalAddressMethods

        #region Modal

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
            enableButton = true;
            await OnResetForm.InvokeAsync();
            ResetFormAsync();
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                ResetFormAsync();
                await OnResetForm.InvokeAsync();
                UpdateModalStatus(args.ModalStatus);
            }
        }

        #endregion HandleModalNotiClose

        #region OpenNewModal

        #region GetAddress

        private async Task GetAddressAsync()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("IdAddress");
                HttpClient?.DefaultRequestHeaders.Add("IdAddress", $"{_selectedRecord.AddressId}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<AddressDtoResponse>>("administration/Address/ByFilterId");
                HttpClient?.DefaultRequestHeaders.Remove("IdAddress");

                if (deserializeResponse!.Succeeded)
                {
                    AddressformCompaniesResponse = deserializeResponse.Data!;
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessageS"], true, Translation["Accept"]);
            }
        }

        #endregion GetAddress

        private async Task OpenNewModal()
        {
            await OnStatusChanged.InvokeAsync(true);
            if (AddressformCompaniesResponse != null)
            {
                await OnDataSaved.InvokeAsync(AddressformCompaniesResponse);
            }
        }

        #endregion OpenNewModal

        #endregion Modal

        #region ResetFormAsync

        public void ResetFormAsync()
        {
            CompaniesFormResponse = new CompanyDtoResponse();
            AddressformCompaniesResponse = new();
            legalAgentIdType = "";
            identificationType = "";
            AddressformCompaniesRequest = new AddressDtoRequest();
            textAddress = Translation["EnterAddress"];
            logoFileInfoData = new();
            bannerFileInfoData = new();
            mobileBannerFileInfoData = new();
            mobileBannerFile = new();
            bannerFile = new();
            logoFile = new();
        }

        #endregion ResetFormAsync

        #region ValidateMethods

        private bool ValidateCompanyFields(dynamic companyDto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(companyDto.IdentificationType))
            {
                //errors.Add("Debe seleccionar un tipo de documento.");
                errors.Add(string.Format(Translation["CharacterSelectionValidation"], Translation["DocumentType"]));
            }

            if (string.IsNullOrWhiteSpace(companyDto.Identification))
            {
                errors.Add(string.Format(Translation["CharacterEmptyValidation"], Translation["TINCode"]));
            }
            else
            {
                if (!Regex.IsMatch(companyDto.Identification, @"^[a-zA-Z0-9]*$") || companyDto.Identification.Length < 8 || companyDto.Identification.Length > 100)
                {
                    //errors.Add("La identificación debe ser válida y tener entre 8 y 100 caracteres.");
                    errors.Add(string.Format(Translation["CharacterIntervalValidation"], Translation["TINCode"], 8, 100));
                }
            }
            if (string.IsNullOrWhiteSpace(companyDto.BusinessName))
            {
                errors.Add(string.Format(Translation["CharacterEmptyValidation"], Translation["BusinessName"]));
            }
            else
            {
                if (companyDto.BusinessName.Length < 3 || companyDto.BusinessName.Length > 200)
                {
                    errors.Add(string.Format(Translation["CharacterIntervalValidation"], Translation["BusinessName"], 3, 200));
                }
            }

            if (companyDto.CompanyData != null)
            {
                /*if (string.IsNullOrWhiteSpace(companyDto.CompanyData.LegalAgentIdType) || (companyDto.CompanyData.LegalAgentIdType.Length < 2 || companyDto.CompanyData.LegalAgentIdType.Length > 11))
                {
                    errors.Add(string.Format(Translation["CharacterSelectionValidation"], Translation["DocumentType"]));
                }
                if (string.IsNullOrWhiteSpace(companyDto.CompanyData.LegalAgentId) || (companyDto.CompanyData.LegalAgentId.Length < 5 || companyDto.CompanyData.LegalAgentId.Length > 100))
                {
                    errors.Add(string.Format(Translation["CharacterIntervalValidation"], Translation["IdentificationNumber"], 3, 100));
                }
                if (string.IsNullOrWhiteSpace(companyDto.CompanyData.LegalAgentFullName) || (companyDto.CompanyData.LegalAgentFullName.Length < 15 || companyDto.CompanyData.LegalAgentFullName.Length > 200))
                {
                    errors.Add(string.Format(Translation["CharacterIntervalValidation"], Translation["FullName"], 15, 200));
                }
                if (string.IsNullOrWhiteSpace(companyDto.CompanyData.CellPhoneNumber) || (companyDto.CompanyData.CellPhoneNumber.Length < 5 || companyDto.CompanyData.CellPhoneNumber.Length > 50))
                {
                    errors.Add(string.Format(Translation["CharacterIntervalValidation"], Translation["CellPhoneNumber"], 5, 50));
                }*/

                if (string.IsNullOrWhiteSpace(companyDto.CompanyData.PhoneNumber) || (companyDto.CompanyData.PhoneNumber.Length < 5 || companyDto.CompanyData.PhoneNumber.Length > 50))
                {
                    errors.Add(string.Format(Translation["CharacterIntervalValidation"], Translation["Phone"], 5, 50));
                }
                if (textAddress.Equals(Translation["EnterAddress"]))
                {
                    errors.Add(string.Format(Translation["CharacterSelectionValidation"], Translation["DocumentType"]));
                }
            }
            else
            {
                errors.Add(Translation["CompanyCannotEmpty"]);
            }

            if (errors.Count > 0)
            {
                var limitedErrors = errors.Take(2).ToList();
                var message = string.Join("\n", limitedErrors);
                notificationModal.UpdateModal(ModalType.Error, Translation["TheFollowingFieldsHaveErrors"] + "\n" + message, true);

                return false;
            }

            return true;
        }

        public void ValidateEnableButton()
        {
            var legalEnable = false;
            if (!string.IsNullOrEmpty(legalAgentIdType))
            {
                if (string.IsNullOrEmpty(CompaniesFormResponse?.LegalAgentId) || string.IsNullOrEmpty(CompaniesFormResponse?.LegalAgentFullName))
                {
                    legalEnable = true;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(CompaniesFormResponse?.LegalAgentId) || !string.IsNullOrEmpty(CompaniesFormResponse?.LegalAgentFullName))
                {
                    legalEnable = true;
                }
            }

            enableButton = string.IsNullOrWhiteSpace(identificationType) ||
                           string.IsNullOrWhiteSpace(CompaniesFormResponse?.Identification) ||
                           string.IsNullOrWhiteSpace(CompaniesFormResponse?.BusinessName) ||
                           string.IsNullOrWhiteSpace(CompaniesFormResponse?.PhoneNumber) ||
                           string.IsNullOrWhiteSpace(CompaniesFormResponse?.Email) ||
                           string.IsNullOrWhiteSpace(textAddress) || textAddress == Translation["EnterAddress"] ||
                           legalEnable;
        }

        #endregion ValidateMethods

        #endregion OtherMethods

        #endregion Methods
    }
}