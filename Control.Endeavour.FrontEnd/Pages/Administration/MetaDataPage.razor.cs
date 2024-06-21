using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.MetaData;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaField.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaField.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VMetaField.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Microsoft.Extensions.Localization;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class MetaDataPage
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

        private InputModalComponent nameInput { get; set; } = new();
        private InputModalComponent codeInput { get; set; } = new();
        private NotificationsComponentModal modalNotification { get; set; } = new();
        private NotificationsComponentModal notificationModalSucces { get; set; } = new();
        private NewPaginationComponent<MetaFieldsDtoResponse, MetaFieldsFilterDtoRequest> paginationComponent { get; set; } = new();
        #endregion Components

        #region Modals
        private NotificationService NotificationService { get; set; } = new();
        private MetaModel meta { get; set; } = new() { PageSize = 10 };
        private List<MetaFieldsDtoResponse> MetaFields { get; set; } = new();
        private List<VSystemParamDtoResponse> systemParamList { get; set; } = new();
        private MetaFieldsFilterDtoRequest metaFieldByFilter { get; set; } = new();
        private MetaDataModal modalMetaFields { get; set; } = new();
        private PaginationInfo paginationInfo = new();
        #endregion Modals

        #region Models

        private DeleteGeneralDtoRequest deleteRequest { get; set; } = new();

        #endregion Models

        #region Environments

        private int metaTitleId { get; set; } = new();
        private string fieldTypeCode { get; set; } = "";
        private string name { get; set; } = "";
        private string code { get; set; } = "";

        private bool dataChargue { get; set; } = false;
        private string UriFilterMetaFiled { get; set; } = "paramsdocumentary/MetaFields/ByFilter";

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await GetFieldType();
            await GetMetaFields();
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region GetFieldType

        private async Task GetFieldType()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "FTY");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded && ( deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null ))
                {
                    systemParamList = deserializeResponse.Data ?? new();
                }
                else
                {
                    systemParamList = new();
                }
            }
            catch
            {
                systemParamList = new();
            }
        }

        #endregion GetFieldType

        #region GetMetaFields

        private async Task GetMetaFields()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                metaFieldByFilter = new()
                {
                    Code = code,
                    NameMetaField = name,
                    FieldType = fieldTypeCode
                };

                var responseApi = await HttpClient!.PostAsJsonAsync("paramsdocumentary/MetaFields/ByFilter", metaFieldByFilter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<MetaFieldsDtoResponse>>>();
                if (deserializeResponse!.Succeeded && ( deserializeResponse.Data?.Data.Count != 0 || deserializeResponse.Data.Data != null ))
                {
                    MetaFields = deserializeResponse.Data!.Data ?? new();
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                    dataChargue = true;
                }
                else
                {
                    MetaFields = new();
                    paginationInfo = new();
                    paginationComponent.ResetPagination(paginationInfo);
                    dataChargue = false;
                }
                StateHasChanged();
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch
            {
                MetaFields = new();
                meta = new() { PageSize = 10 };
            }
        }

        #endregion GetMetaFields

        #region GetFieldTypeCode

        public void GetFieldTypeCode(string code)
        {
            try
            {
                fieldTypeCode = code;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener Officinas Productoras: {ex.Message}");
            }
        }

        #endregion GetFieldTypeCode

        #region OnClickReset

        private async Task OnClickReset()
        {
            name = "";
            code = "";
            fieldTypeCode = "";

            metaFieldByFilter = new();
            await GetMetaFields();
            StateHasChanged();
        }

        #endregion OnClickReset

        #region OnClickSearch

        private async Task OnClickSearch()
        {
            await GetMetaFields();
        }

        #endregion OnClickSearch

        #region

        private async Task HandleStatusChanged(bool status)

        {
            /*        PageLoadService.MostrarSpinnerReadLoad(Js);*/
            await GetMetaFields();
            /*            PageLoadService.OcultarSpinnerReadLoad(Js);*/
        }

        #endregion Methods

        #region ShowModal

        private void ShowModal()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            modalMetaFields.UpdateModalStatus(true);
            modalMetaFields.ResetForm();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ShowModal

        #region ShowModalEdit

        private async Task ShowModalEdit(MetaFieldsDtoResponse value)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            modalMetaFields.UpdateModalStatus(true);
            modalMetaFields.UpdateMetaData(value);

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ShowModalEdit

        #region ShowModalDelete

        private void ShowModalDelete(MetaFieldsDtoResponse value)
        {
            metaTitleId = value.MetaFieldId;
            modalNotification.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"] + $": {value.NameMetaField}", true, Translation["Yes"], Translation["No"]);
        }

        #endregion ShowModalDelete

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                await DeleteMetatitle();
                await GetFieldType();
                await GetMetaFields();
            }
        }

        #endregion HandleModalNotiClose

        #region HandleModalClose

        private async Task HandleModalClose(bool newValue)
        {
            modalMetaFields.UpdateModalStatus(newValue);

            if (!newValue)
            {
                await GetFieldType();
                await GetMetaFields();
            }
        }

        #endregion HandleModalClose

        #region DeleteMetatitle

        public async Task DeleteMetatitle()
        {
            deleteRequest = new() { Id = metaTitleId, User = "" };

            var responseApi = await HttpClient!.PostAsJsonAsync("paramsdocumentary/MetaFields/DeleteMetaFields", deleteRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();

            var NotiResult = NotificationService.NotificationDecrypt(Translation, deserializeResponse);
            notificationModalSucces.UpdateModal(NotiResult.ModalType, NotiResult.NotificationMessage, true, Translation["Accept"], "");
        }

        #endregion DeleteMetatitle

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<MetaFieldsDtoResponse> newDataList)
        {
            MetaFields = newDataList;
        }

        #endregion HandlePaginationGrid

        #endregion Methods
    }
}