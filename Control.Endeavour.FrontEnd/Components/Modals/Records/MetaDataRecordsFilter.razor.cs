using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Records.Request;
using Control.Endeavour.FrontEnd.Models.Models.Records.Response;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Drawing.Text;
using System.Net.Http.Json;
using System.Reflection;


namespace Control.Endeavour.FrontEnd.Components.Modals.Records
{
    public partial class MetaDataRecordsFilter
    {

        #region Variables

        #region Inject 
        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Components


        #endregion

        #region Modals

        private NotificationsComponentModal notificationModal = new();
        private GenericUserSearchModal genericUserSearch = new();

        #endregion

        #region Parameters

        [Parameter] public EventCallback<List<MetaDataFilter>> OnChangeMetaDataFilter { get; set; }


        #endregion

        #region Models

        private List<MetaDataFilter> metaDataFilters = new();
        private List<VMetaDataDtoResponse> metaDataList = new();
        private MetaDataFilterDtoRequest filterRequest = new();
        private RetunUserSearch UserSelected = new();
        #endregion

        #region Environments

        #region Environments(String)

        private string UriMetaDataFilter = "records/MetaData/MetaDataByFilter";
        private string Tittle = "";

        #endregion

        #region Environments(Numeric)
        public int UserSearchModalType = 1;
        #endregion

        #region Environments(DateTime)
        private DateTime Min = new(2020, 1, 1, 19, 30, 45);
        private DateTime Max = DateTime.Now;

        #endregion

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool modalUserStatus = false;

        #endregion

        #region Environments(List & Dictionary)

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            Tittle = Translation["Records"] + " - " + Translation["FiltersByMetadata"];

        }


        #endregion

        #region Methods

        #region HandleMethods

        #region HandleSendData

        private void SendMetaDataFilter()
        {
            notificationModal.UpdateModal(ModalType.Information, Translation["FilterMetaDataQuestion"], true, buttonTextAceptar: Translation["Yes"], buttonTextCancel: Translation["No"], modalOrigin: "FilterOption");

        }
        private void ResetMetaDataFilter()
        {
            metaDataList.ForEach(metaData =>
            {
                metaData.MetaValue = string.Empty;
                metaData.MetaDateValue = null;
                metaData.MetaBoolValue = null;
            });
        }

        #endregion

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            switch (args.ModalOrigin)
            {
                case "FilterOption":
                    if (args.IsAccepted)
                    {
                        metaDataFilters = metaDataList
                            .Where(metaData => !string.IsNullOrEmpty(metaData.MetaValue) ||
                                               metaData.MetaDateValue != null ||
                                               metaData.MetaBoolValue != null)
                            .Select(metaData => new MetaDataFilter
                            {
                                MetaFieldId = metaData.MetaFieldId ?? 0,
                                DataText = metaData.FieldTypeCode switch
                                {
                                    "FTY,13" => metaData.MetaValue,
                                    "FTY,14" => metaData.MetaDateValue?.ToString("dd MMMM yyyy"),
                                    "FTY,15" => metaData.MetaBoolValue?.ToString(),
                                    "FTY,16" => metaData.MetaValue,
                                    "FTY,17" => UserSelected.UserName,
                                    "FTY,18" => UserSelected.UserName,
                                    _ => null
                                }
                            }).ToList()
                            .Where(filter => !string.IsNullOrWhiteSpace(filter.DataText))
                            .ToList();
                        await OnChangeMetaDataFilter.InvokeAsync(metaDataFilters);
                        await UpdateModalStatusAsync(args.ModalStatus);
                    }
                    break;
                case "NotFoundDataModal":

                    await UpdateModalStatusAsync(args.ModalStatus);
                    break;


            }


        }
        private void HandleSelectedUserData(List<RetunUserSearch> selectedUsers)
        {
            UserSelected = selectedUsers[0];




        }
        private void HandleModalClosed(bool status)
        {
            modalUserStatus = status;

        }
        private async Task HandleModalClosedAsync(bool status)
        {
            modalStatus = status;
            StateHasChanged();
        }

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

        #region OthersMethods

        #region ModalMethods
        public async Task UpdateModalStatusAsync(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();

        }

        public async Task LoadMetaDataInfo(bool isSeries, int id)
        {
            filterRequest = new();
            try
            {
                string nameObject = string.Empty;
                switch (isSeries)
                {
                    case true: //MetaDatos a partir de la serie
                        filterRequest.SeriesId = id;
                        nameObject = "Serie";
                        break;
                    case false: //Metadatos a partir de subseries
                        filterRequest.SubSeriesId = id;
                        nameObject = "Sub-Serie";
                        break;

                }
                var responseApi = await HttpClient.PostAsJsonAsync(UriMetaDataFilter, filterRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<VMetaDataDtoResponse>>>();
                if (!deserializeResponse.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Information, string.Format(Translation["NotFoundMetaData"], nameObject), true, Translation["Accept"], buttonTextCancel: "", modalOrigin: "NotFoundDataModal");
                    metaDataList = new();
                    SpinnerLoaderService.HideSpinnerLoader(Js);

                }
                else
                {
                    metaDataList = deserializeResponse.Data;
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            catch (Exception ex)
            {

                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }

        public async Task LoadMetaDataBydocumentaryTypologyBagId(int documentaryTypologyBagId)
        {
            Tittle = Translation["DocumentSearchEngine"];
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("documentaryTypologyBagId");
                HttpClient?.DefaultRequestHeaders.Add("documentaryTypologyBagId", documentaryTypologyBagId.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VMetaDataDtoResponse>>>("documentmanagement/MetaData/MetaDataByDocumentaryTypologyBag");
                HttpClient?.DefaultRequestHeaders.Remove("documentaryTypologyBagId");

                if (deserializeResponse!.Succeeded)
                {
                    metaDataList = deserializeResponse.Data;
                }
                else { metaDataList = new(); }

            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #region ShowModalActions
        private void showUserSerch(int typeUser)
        {
            if(typeUser == 1)
            {
                UserSearchModalType = 1;
            }
            else
            {
                UserSearchModalType = 2;
            }

            modalUserStatus = true;
        }
        private void DeleteSenderSelected()
        {
            UserSelected = new();
        }

        #endregion

        #endregion

        #endregion

        #endregion

    }
}
