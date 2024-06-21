using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VReplacement.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.Pdf;
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Modals.Security.ManagementValidity
{
    public partial class ManagementValidityModal
    {
        #region Variables

        #region Inject

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject

        #region Parameter

        [Parameter] public EventCallback<bool> OnStatusChanged { get; set; }

        #endregion Parameter

        #region Modal

        private NotificationsComponentModal notificationModal { get; set; } = new();

        #endregion Modal

        #region Models

        private ProdOfficetParameterCreateDtoRequest request { get; set; } = new();

        #endregion Models

        #region Environments

        #region Enviroments(string)

        private string MonthsQuantity { get; set; } = string.Empty;

        #endregion Enviroments(string)

        #region Environments(Numeric)

        private int idDocVersion { get; set; } = new();
        private int idAdminUnit { get; set; } = new();
        private int idProOffice { get; set; } = new();

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool SaveDisable { get; set; } = true;
        private bool ModalStatus { get; set; } = new();
        private bool IsEditForm { get; set; } = new();

        private bool isEnableAdminUnit { get; set; } = false;
        private bool isEnableProOffice { get; set; } = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<DocumentalVersionDtoResponse> docVersionList { get; set; } = new();
        private List<AdministrativeUnitsDtoResponse> adminUnitList { get; set; } = new();
        private List<ProductionOfficesDtoResponse> proOfficesList { get; set; } = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            await GetDocumentalVersions();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private void HandleModalClosed(bool status)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            ModalStatus = status;
            StateHasChanged();

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success && args.IsAccepted)
            {
                await OnStatusChanged.InvokeAsync(false);
            }

            StateHasChanged();
        }

        private async Task HandleValidSubmit()
        {
            await HandleFormCreate();
        }

        private async Task HandleFormCreate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                request = new()
                {
                    ProductionOfficeId = idProOffice,
                    PasswordExpirationTime = !string.IsNullOrEmpty(MonthsQuantity) ? int.Parse(MonthsQuantity) : 0,
                    IsCreate = IsEditForm ? false : true
                };

                var responseApi = await HttpClient!.PostAsJsonAsync("security/System/UpdateProdOfficeParameter", request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ProdOfficeManagmentDtoResponse>>();
                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, ( IsEditForm ? Translation["UpdateSuccesfulMessage"] : Translation["CreateSuccessfulMessage"] ), true, Translation["Accept"], "");
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, ( IsEditForm ? Translation["CannotUpdate"] : Translation["CreateErrorMessage"] ), true, Translation["Accept"], "");
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, ( IsEditForm ? Translation["UpdateErrorMessage"] : Translation["CreateErrorMessage"] ), true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleMethods

        #region OtherMethods

        public async Task UpdateModalStatus(bool value)
        {
            ModalStatus = value;
            IsEditForm = false;
            await ResetForm();
            EnableSaveButton();
            StateHasChanged();
        }

        #region Reset

        public async Task ResetForm()
        {
            if (!IsEditForm)
            {
                docVersionList = new();
                adminUnitList = new();
                proOfficesList = new();
                idDocVersion = new();
                idAdminUnit = new();
                idProOffice = new();
                await GetDocumentalVersions();
                EnableSaveButton();
            }
            MonthsQuantity = string.Empty;
        }

        #endregion Reset

        #region GetDocumentalVersions

        public async Task GetDocumentalVersions()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<DocumentalVersionDtoResponse>>>("paramstrd/DocumentalVersions/ByDocumentalVersions");
                if (deserializeResponse!.Succeeded)
                {
                    docVersionList = deserializeResponse.Data!;
                    string toCurrent = Translation["ToCurrent"];
                    string documentaryVersion = Translation["DocumentaryVersion"];
                    docVersionList.ForEach(x =>
                    {
                        x.Code = $"{( ( x.EndDate == null || x.EndDate.Value > DateTime.Now ) ? $"{x.StartDate!.Value.Year} {toCurrent} - {documentaryVersion} {x.Code}" : $"{x.StartDate!.Value.Year} a {x.EndDate.Value.Year} - {documentaryVersion} {x.Code} {x.VersionType}" )}";
                    });
                    docVersionList = docVersionList.OrderBy(x => x.DocumentalVersionId).Reverse().ToList();
                    docVersionList = docVersionList.Where(x => x.EndDate == null).ToList();

                    EnableField(false, false);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["LoadDocumentalVersionErrorMessage"]}: {ex.Message}", true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetDocumentalVersions

        #region GetAdministrativeUnits

        public async Task GetAdministrativeUnits(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                idDocVersion = id;
                adminUnitList = new();
                idAdminUnit = 0;

                HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
                HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", $"{idDocVersion}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
                HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");

                if (deserializeResponse!.Succeeded && ( deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null ))
                {
                    adminUnitList = deserializeResponse.Data!;
                    EnableField(true, false);
                }
                else
                {
                    EnableField(false, false);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["ErrorGettingRecords"]}: {ex.Message}", true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetAdministrativeUnits

        #region GetProducOffice

        public async Task GetProducOffice(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                idAdminUnit = id;
                proOfficesList = new();
                idProOffice = 0;

                HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                HttpClient?.DefaultRequestHeaders.Add("AdministrativeUnitId", $"{idAdminUnit}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>("paramstrd/ProductionOffice/ByProductionOffices");
                HttpClient?.DefaultRequestHeaders.Remove("key");

                if (deserializeResponse!.Succeeded && ( deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null ))
                {
                    proOfficesList = deserializeResponse.Data!;
                    EnableField(true, true);
                }
                else
                {
                    EnableField(true, false);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["ErrorGettingRecords"]}", true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetProducOffice

        public async Task<AdministrativeUnitsDtoResponse?> GetAdministrtativeUnitById(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                HttpClient?.DefaultRequestHeaders.Remove("adminUnitId");
                HttpClient?.DefaultRequestHeaders.Add("adminUnitId", $"{id}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<AdministrativeUnitsDtoResponse>>("paramstrd/AdministrativeUnit/ByFilterId");
                HttpClient?.DefaultRequestHeaders.Remove("adminUnitId");

                if (deserializeResponse!.Succeeded && ( deserializeResponse.Data != null ))
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
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["ErrorGettingRecords"]}", true, Translation["Accept"]);
                return null;
            }
        }

        #region EnableField

        public void EnableField(bool a, bool b)
        {
            isEnableAdminUnit = a;
            isEnableProOffice = b;

            idAdminUnit = a ? idAdminUnit : 0;
            idProOffice = b ? idProOffice : 0;
        }

        #endregion EnableField

        private void EnableSaveButton()
        {
            SaveDisable = true;

            int value = 0;
            MonthsQuantity = ( !string.IsNullOrEmpty(MonthsQuantity) && int.TryParse(MonthsQuantity, out value) ) ? value.ToString() : "0";

            if (!string.IsNullOrEmpty(MonthsQuantity) && idProOffice > 0)
            {
                SaveDisable = false;
            }
        }

        public async Task updateRecord(ProdOfficeManagmentDtoResponse record)
        {
            IsEditForm = true;

            await GetDocumentalVersions();
            var adminisUnitSearch = await GetAdministrtativeUnitById(record.AdministrativeUnitId);

            if (adminisUnitSearch != null)
            {
                if (docVersionList.Select(item => item.DocumentalVersionId).Contains(adminisUnitSearch.DocumentalVersionId))
                {
                    idDocVersion = adminisUnitSearch.DocumentalVersionId;
                    await GetAdministrativeUnits(idDocVersion);
                    if (adminUnitList.Select(item => item.AdministrativeUnitId).Contains(record.AdministrativeUnitId))
                    {
                        await GetProducOffice(record.AdministrativeUnitId);

                        if (proOfficesList.Select(item => item.ProductionOfficeId).Contains(record.ProductionOfficeId))
                        {
                            idProOffice = record.ProductionOfficeId;
                        }
                    }
                }
            }

            isEnableAdminUnit = false;
            isEnableProOffice = false;

            MonthsQuantity = record.PasswordExpirationTime?.ToString();
            EnableSaveButton();
            StateHasChanged();
        }

        #endregion OtherMethods

        #endregion Methods
    }
}