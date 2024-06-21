using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using DevExpress.Pdf;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.Ocsp;
using DevExpress.XtraPrinting.Native.LayoutAdjustment;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Security
{
    public partial class SecurityManagement
    {
        #region Variables

        #region Inject

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject

        #region Model

        private SecurityMangmemtParemeterDtoRequest request { get; set; } = new();

        #endregion Model

        #region Components

        private NotificationsComponentModal notificationModal { get; set; } = new();

        #endregion Components

        #region Environments

        #region Environments(String)

        private string MonthsQuantity { get; set; } = string.Empty;
        private string MinutesQuantity { get; set; } = string.Empty;

        private string PasswordQuantity { get; set; } = string.Empty;

        private string MonthsUser { get; set; } = string.Empty;

        private string MinutesUser { get; set; } = string.Empty;

        private string PasswordUser { get; set; } = string.Empty;

        private string MonthsDate { get; set; } = string.Empty;
        private string MinutesDate { get; set; } = string.Empty;

        private string PasswordDate { get; set; } = string.Empty;

        #endregion Environments(String)

        #region Environments(Numeric)

        private int MonthsId { get; set; } = new();
        private int MinutesId { get; set; } = new();

        private int PasswordId { get; set; } = new();

        private int CurrentTab { get; set; } = new();

        #endregion Environments(Numeric)

        #region Environments(bool)

        private bool MonthsActive { get; set; } = true;
        private bool MinutesActive { get; set; } = true;
        private bool PasswordActive { get; set; } = true;

        private bool MonthsSaveDisable { get; set; } = true;
        private bool MinutesSaveDisable { get; set; } = true;
        private bool PasswordSaveDisable { get; set; } = true;

        #endregion Environments(bool)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            await GetData();
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

        private async Task EditData(bool activeCase = false)
        {
            switch (CurrentTab)
            {
                case 0:
                    MonthsActive = activeCase;
                    break;

                case 1:
                    MinutesActive = activeCase;
                    break;

                case 2:
                    PasswordActive = activeCase;
                    break;
            }
            if (activeCase)
            {
                await GetData();
            }
            StateHasChanged();
        }

        private void EnableSaveButton()
        {
            int value = 0;

            switch (CurrentTab)
            {
                case 0:
                    MonthsSaveDisable = true;


                    MonthsQuantity= (!string.IsNullOrEmpty(MonthsQuantity) && int.TryParse(MonthsQuantity, out value))? value.ToString(): "0";




                    if (!string.IsNullOrEmpty(MonthsQuantity))
                    {
                        MonthsSaveDisable = false;
                    }

                    break;

                case 1:
                    MinutesSaveDisable = true;


                    MinutesQuantity = ( !string.IsNullOrEmpty(MinutesQuantity) && int.TryParse(MinutesQuantity, out value) ) ? value.ToString() : "0";
                    if (!string.IsNullOrEmpty(MinutesQuantity))
                    {
                        MinutesSaveDisable = false;
                    }
                    break;

                case 2:

                    PasswordSaveDisable = true;
                    PasswordQuantity = ( !string.IsNullOrEmpty(PasswordQuantity) && int.TryParse(PasswordQuantity, out value) ) ? value.ToString() : "0";

                    if (!string.IsNullOrEmpty(PasswordQuantity))
                    {
                        PasswordSaveDisable = false;
                    }
                    break;
            }

            StateHasChanged();
        }

        private async Task GetData()
        {
            var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<AppKeysDtoResponse>>>("security/System/ReadUsers");
            if (deserializeResponse.Succeeded)
            {
                var data = deserializeResponse.Data;
                var month = data.Where(apk => apk.KeyName.Equals("MonthsQuantity")).FirstOrDefault();
                var minute = data.Where(apk => apk.KeyName.Equals("MinutesQuantity")).FirstOrDefault();
                var password = data.Where(apk => apk.KeyName.Equals("PasswordQuantity")).FirstOrDefault();

                MonthsQuantity = month.Value1 ?? "0";
                MinutesQuantity = minute.Value1 ?? "0";
                PasswordQuantity = password.Value1 ?? "0";

                MonthsUser = month.UpdateUser ?? string.Empty;
                MinutesUser = minute.UpdateUser ?? string.Empty;
                PasswordUser = password.UpdateUser ?? string.Empty;

                MonthsId = month.AppKeyId;
                MinutesId = minute.AppKeyId;
                PasswordId = password.AppKeyId;

                MonthsDate = month.UpdateDate?.ToString();
                MinutesDate = minute.UpdateDate?.ToString();
                PasswordDate = password.UpdateDate?.ToString();
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true, Translation["Accept"], "");
            }
        }

        private async Task UpdateParemeter()
        {
            request = new();

            switch (CurrentTab)
            {
                case 0:
                    request = new()
                    {
                        AppKeyId = MonthsId,
                        Value1 = MonthsQuantity,
                    };
                    break;

                case 1:
                    request = new()
                    {
                        AppKeyId = MinutesId,
                        Value1 = MinutesQuantity
                    }
                    ;
                    break;

                case 2:
                    request = new()
                    {
                        AppKeyId = PasswordId,
                        Value1 = PasswordQuantity,
                    };
                    break;
            }

            var responseApi = await HttpClient.PostAsJsonAsync("security/System/UpdateSecurityParameter", request);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<AppKeysDtoResponse>>();
            if (deserializeResponse.Succeeded)
            {
                notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"], "");
                await GetData();
                await EditData(true);
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"], "");
            }
        }


        


        #endregion OtherMethods

        #endregion Methods
    }
}