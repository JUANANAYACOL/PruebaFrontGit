using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using DevExpress.Blazor.Internal.Editors;
using DevExpress.Blazor.Primitives.Internal;
using DevExpress.XtraSpellChecker;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Globalization;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Modals.SystemConfiguration
{
    public partial class FilingsFormatsModal
    {

        #region Variables

        #region Inject 

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }
        [Inject]
        private IJSRuntime Js { get; set; }
        [Inject]
        private HttpClient? HttpClient { get; set; }
        #endregion

        #region Components


        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Parameters

        [Parameter] public EventCallback<bool> OnChangeData { get; set; }


        #endregion

        #region Models

        private DocCodeFormatDtoResponse DocCodeFormatDtoRequest = new();
        private DocCodeFormatDtoResponse DocCodeFormatUpdateDtoRequest = new();

        #endregion

        #region Environments

        #region Environments(String)

        private string FillCodeTest = string.Empty;
        private string SuffixMaxLength = string.Empty;
        private string PrefixMaxLength = string.Empty;

        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool IsEditForm = false;
        private bool modalStatus = false;
        private bool IsEnablenButtom = true;
        private bool IsEnablenSuffix = true;
        private bool IsEnablenPrefix = true;

        #endregion

        #region Environments(List & Dictionary)
        private List<DateDtoResponse> Year = new();
        private List<VSystemParamDtoResponse> classCodeList = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            await GetClasses();
            FillList();
        }


        #endregion

        #region Methods

        #region HandleMethods

        private void HandleModalClosed(bool status)
        {
            DocCodeFormatDtoRequest = new();
            DocCodeFormatUpdateDtoRequest = new();
            FillCodeTest = string.Empty;
            modalStatus = status;
            IsEditForm = false;
            IsEnablenButtom = true;
            SuffixMaxLength = string.Empty;
            PrefixMaxLength = string.Empty;
            StateHasChanged();
        }
        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {



        }

        #region FormMethods
        private async Task HandleValidSubmit()
        {

            if (IsEditForm)
            {
                await HandleFormUpdate();
            }
            else
            {
                await HandleFormCreate();
            }

            StateHasChanged();

        }
        private async Task HandleFormCreate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (ValidateFormFields())
                {
                    var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/DocCodeFormat/CreateDocCodeFormat", DocCodeFormatDtoRequest);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != "0")
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                        await OnChangeData.InvokeAsync(true);
                        IsEditForm = false;
                        ResetFormAsync();
                    }
                    else
                    {
                        if (deserializeResponse.Data.Equals("0"))
                        {
                            notificationModal.UpdateModal(ModalType.Information, Translation["FillingFormat"], true, Translation["Accept"], buttonTextCancel: "");
                        }
                        else
                        {
                            notificationModal.UpdateModal(ModalType.Information, Translation["CreateErrorMessage"], true, Translation["Accept"], buttonTextCancel: "");
                        }
                        
                    }
                }

            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        private async Task HandleFormUpdate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (ValidateFormFields())
                {

                    var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/DocCodeFormat/UpdateDocCodeFormat", DocCodeFormatDtoRequest);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != "0")
                    {
                        IsEditForm = false;
                        ResetFormAsync();
                        notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                        await OnChangeData.InvokeAsync(true);
                        UpdateModalStatus(false);

                    }
                    else
                    {
                        if (deserializeResponse.Data.Equals("0"))
                        {
                            notificationModal.UpdateModal(ModalType.Information, Translation["FillingFormat"], true, Translation["Accept"], buttonTextCancel: "");
                        }
                        else
                        {
                            notificationModal.UpdateModal(ModalType.Information, Translation["CannotUpdate"], true, Translation["Accept"], buttonTextCancel: "");
                        }
                        
                    }

                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);

        }

        private void ResetFormAsync()
        {
            if (!IsEditForm)
            {
                DocCodeFormatDtoRequest = new();
                SuffixMaxLength = string.Empty;
                PrefixMaxLength = string.Empty;
            }
            else
            {

                DocCodeFormatDtoRequest = new() { 
                    DocCodeFormatId = DocCodeFormatUpdateDtoRequest.DocCodeFormatId,
                    ClassCode = DocCodeFormatUpdateDtoRequest.ClassCode,
                    ClassCodeName = DocCodeFormatUpdateDtoRequest.ClassCodeName,
                    Prefix = DocCodeFormatUpdateDtoRequest.Prefix,
                    PrefixLength = DocCodeFormatUpdateDtoRequest.PrefixLength,
                    Data = DocCodeFormatUpdateDtoRequest.Data,
                    DataLength = DocCodeFormatUpdateDtoRequest.DataLength,
                    Suffix = DocCodeFormatUpdateDtoRequest.Suffix,
                    SuffixLength = DocCodeFormatUpdateDtoRequest.SuffixLength,
                    Separator = DocCodeFormatUpdateDtoRequest.Separator,
                    Active = DocCodeFormatUpdateDtoRequest.Active,
                    ActiveState = DocCodeFormatUpdateDtoRequest.ActiveState,
                    UpdateUser = DocCodeFormatUpdateDtoRequest.UpdateUser,
                    UpdateDate = DocCodeFormatUpdateDtoRequest.UpdateDate
                
                };

                SuffixMaxLength = DocCodeFormatUpdateDtoRequest.SuffixLength.ToString();
                PrefixMaxLength = DocCodeFormatUpdateDtoRequest.PrefixLength.ToString();
            }
            FillCodeTest = string.Empty;
            
        }

        #endregion




        #endregion

        #region OthersMethods

        #region ModalMethods

        private void SetTestFillingCode()
        {
            var controlId = "XXXX";
            var prefix = DocCodeFormatDtoRequest.Prefix.PadRight(DocCodeFormatDtoRequest.PrefixLength, '0');
            var suffix = DocCodeFormatDtoRequest.Suffix.PadRight(DocCodeFormatDtoRequest.SuffixLength, '0');
            var data = DocCodeFormatDtoRequest.Data.PadRight(DocCodeFormatDtoRequest.DataLength, '0');

            FillCodeTest = $"{prefix}{DocCodeFormatDtoRequest.Separator}{data}{DocCodeFormatDtoRequest.Separator}{controlId}{suffix}";
        }


        public void ReceiveRecord(DocCodeFormatDtoResponse response)
        {
            
            DocCodeFormatDtoRequest = new()
            {
                DocCodeFormatId = response.DocCodeFormatId,
                ClassCode = response.ClassCode,
                ClassCodeName = response.ClassCodeName,
                Prefix = response.Prefix,
                PrefixLength = response.PrefixLength,
                Data = response.Data,
                DataLength = response.DataLength,
                Suffix = response.Suffix,
                SuffixLength = response.SuffixLength,
                Separator = response.Separator,
                Active = response.Active,
                ActiveState = response.ActiveState,
                UpdateUser = response.UpdateUser,
                UpdateDate = response.UpdateDate
            };

            DocCodeFormatUpdateDtoRequest = new()
            {
                DocCodeFormatId = response.DocCodeFormatId,
                ClassCode = response.ClassCode,
                ClassCodeName = response.ClassCodeName,
                Prefix = response.Prefix,
                PrefixLength = response.PrefixLength,
                Data = response.Data,
                DataLength = response.DataLength,
                Suffix = response.Suffix,
                SuffixLength = response.SuffixLength,
                Separator = response.Separator,
                Active = response.Active,
                ActiveState = response.ActiveState,
                UpdateUser = response.UpdateUser,
                UpdateDate = response.UpdateDate
            };

            SuffixMaxLength = DocCodeFormatUpdateDtoRequest.SuffixLength.ToString();
            PrefixMaxLength = DocCodeFormatUpdateDtoRequest.PrefixLength.ToString();
            IsEditForm = true;
        }

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        #endregion

        #region GetDataMethods

        private void FillList()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            for (int i = 2021; i < DateTime.Now.Year + 1; i++)
            {
                Year.Add(new DateDtoResponse { nombre = $"{(i + 1)}", valor = (i + 1) });
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
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

                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion

        #region ValidateMethods
        public void EnableSaveButton()
        {
            
            if (!string.IsNullOrEmpty(SuffixMaxLength) && int.TryParse(SuffixMaxLength, out int suffixLength))
            {
                if(DocCodeFormatDtoRequest.SuffixLength != suffixLength)
                {
                    DocCodeFormatDtoRequest.Suffix = string.Empty;
                }
                DocCodeFormatDtoRequest.SuffixLength = suffixLength;
                IsEnablenSuffix = false;
                
            }
            else
            {
                IsEnablenSuffix = true;
            }

            if (!string.IsNullOrEmpty(PrefixMaxLength) && int.TryParse(PrefixMaxLength, out int prefixLength))
            {
                if (DocCodeFormatDtoRequest.PrefixLength != prefixLength)
                {
                    DocCodeFormatDtoRequest.Prefix = string.Empty;
                }
                DocCodeFormatDtoRequest.PrefixLength = prefixLength;
                IsEnablenPrefix = false;
                
            }
            else
            {
                IsEnablenPrefix = true;
            }


            if (AreAllFieldsValid())
            {

                IsEnablenButtom = false;
            }
            else
            {
                IsEnablenButtom = true;
            }
            
        }
        private bool AreAllFieldsValid()
        {
            
            bool isClassCodeValid = !string.IsNullOrEmpty(DocCodeFormatDtoRequest.ClassCode);
            bool isDataValid = !string.IsNullOrEmpty(DocCodeFormatDtoRequest.Data);
            bool isPrefixValid = !string.IsNullOrEmpty(DocCodeFormatDtoRequest.Prefix);
            bool isSuffixValid = !string.IsNullOrEmpty(DocCodeFormatDtoRequest.Suffix);
            

            return isClassCodeValid && isDataValid && isPrefixValid && isSuffixValid ;
        }
        private bool ValidateFormFields()
        {
            var errors = new List<string>();
            if (string.IsNullOrEmpty(DocCodeFormatDtoRequest.ClassCode))
            {
                errors.Add(string.Format(Translation["CharacterEmptyValidation"], Translation["ComunicationClass"]));
            }
            if (string.IsNullOrEmpty(DocCodeFormatDtoRequest.Data))
            {
                errors.Add(string.Format(Translation["CharacterEmptyValidation"], Translation["Gantt_Year"]));
            }
            if (string.IsNullOrEmpty(DocCodeFormatDtoRequest.Prefix))
            {
                errors.Add(string.Format(Translation["CharacterEmptyValidation"], Translation["Prefix"]));
            }

            if (string.IsNullOrEmpty(DocCodeFormatDtoRequest.Suffix))
            {
                errors.Add(string.Format(Translation["CharacterEmptyValidation"], Translation["Suffix"]));
            }

            if (errors.Count > 0)
            {
                var message = string.Join(" ", errors);
                notificationModal.UpdateModal(ModalType.Error, Translation["TheFollowingFieldsHaveErrors"] + "\n" + message, true);
                return false;
            }

            return true;
        }

        #endregion

        #endregion

        #endregion

    }
}
