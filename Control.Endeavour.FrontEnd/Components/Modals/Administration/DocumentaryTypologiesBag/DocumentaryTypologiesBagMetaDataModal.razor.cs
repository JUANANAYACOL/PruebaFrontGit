using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.DocumentaryTypologiesBag.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.DocumentaryTypologiesBag.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaField.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaField.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaTitles.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaTitles.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaValues.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VMetaField.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.Pdf.Native.BouncyCastle.Asn1;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Dynamic;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Telerik.DataSource.Extensions;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.DocumentaryTypologiesBag
{
    public partial class DocumentaryTypologiesBagMetaDataModal
    {
        #region Variables

        #region Inject

        [Inject]
        private IJSRuntime Js { get; set; }

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; } = new();

        #endregion Inject

        #region Models

        private int metaFieldidToAdd { get; set; } = new();
        private MetaFieldsByFilterTotalDtoRequest metaFieldFilter { get; set; } = new();

        #endregion Models

        #region Components

        private NotificationsComponentModal notificationModal { get; set; } = new();
        private InputModalComponent orderInput { get; set; } = new();

        #endregion Components

        #region Parameters

        [Parameter] public int ConfigurationId { get; set; } = 1;
        [Parameter] public bool modalStatus { get; set; } = new();
        [Parameter] public EventCallback<bool> OnStatusChanged { get; set; } = new();
        [Parameter] public string TitleModal { get; set; } = string.Empty;
        [Parameter] public string Title { get; set; } = string.Empty;

        #endregion Parameters

        #region Environments

        #region Enviroments (Bool)

        private bool saveIsDisable { get; set; } = true;

        #endregion Enviroments (Bool)

        #region Environments(Numeric)

        private int DTBId { get; set; } = 0;

        #endregion Environments(Numeric)

        #region Environments(String)

        private string orderString { get; set; } = "";

        #endregion Environments(String)

        #region Environments(List && Dictionary)

        private MetaTitleCreateDtoRequest possibleMetaTitleToDelete { get; set; } = new();
        private List<MetaFieldsDtoResponse> metaFieldListModal { get; set; } = new();
        private List<MetaTitleCreateDtoRequest> metaTitlesListDtoRequest { get; set; } = new();

        private List<DeleteGeneralDtoRequest> metaTitlesListToDelete { get; set; } = new();

        private List<MetaTitleCreateDtoRequest> metaTitlesListToCreate { get; set; } = new();

        #endregion Environments(List && Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleValidSubmit()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            await HandleFormCreate();

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #region HandleFormCreate

        private async Task HandleFormCreate()
        {
            bool create = true;
            bool delete = true;

            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (metaTitlesListToCreate.Any())
                {
                    var responseApi = await HttpClient!.PostAsJsonAsync("paramsdocumentary/MetaTitle/CreateMetaTitleList", metaTitlesListToCreate);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<MetaTitleDtoResponse>>>();

                    if (!deserializeResponse.Succeeded)
                    {
                        create = false;
                    }
                }

                if (metaTitlesListToDelete.Any())
                {
                    var responseApi = await HttpClient!.PostAsJsonAsync("paramsdocumentary/MetaTitle/DeleteMetaTitleList", metaTitlesListToDelete);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<int>>>();

                    if (!deserializeResponse.Succeeded)
                    {
                        delete = false;
                    }
                }

                if (create && delete)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"], "");
                }
                else if (!create && delete)
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation["CreateErrorMessage"], true, Translation["Accept"], "");
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation["CreateErrorMessage"], true, Translation["Accept"], "");
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleFormCreate

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success && args.IsAccepted)
            {
                await OnStatusChanged.InvokeAsync(false);
            }

            if (notificationModal.Type == ModalType.Warning && args.IsAccepted)
            {
                if (possibleMetaTitleToDelete.MetaTitleId != 0)
                {
                    metaTitlesListDtoRequest = metaTitlesListDtoRequest.Where(item => item.MetaTitleId != ( possibleMetaTitleToDelete.MetaTitleId )).ToList();
                    metaTitlesListToDelete.Add(new() { Id = possibleMetaTitleToDelete.MetaTitleId });
                    metaTitlesListToDelete = metaTitlesListToDelete.DistinctBy(item => item.Id).ToList();

                    metaTitlesListToCreate = metaTitlesListToCreate.Where(item => item.MetaTitleId != ( possibleMetaTitleToDelete.MetaTitleId )).ToList();
                }
                else
                {
                    metaTitlesListDtoRequest = metaTitlesListDtoRequest.Where(item => item.MetaFieldId != possibleMetaTitleToDelete.MetaFieldId).ToList();
                    metaTitlesListToCreate = metaTitlesListToCreate.Where(item => item.MetaFieldId != possibleMetaTitleToDelete.MetaFieldId).ToList();
                }
            }
            else
            {
                possibleMetaTitleToDelete = new();
            }
            StateHasChanged();
        }

        #endregion HandleMethods

        #region OthersMethods

        private async Task HandleModalClosed(bool status)
        {
            await ResetFormAsync();
            modalStatus = status;

            StateHasChanged();
        }

        public async Task UpdateModalStatus(bool newValue)
        {
            if (!newValue)
            {
                await ResetFormAsync(false);
                Title = string.Empty;
                StateHasChanged();
            }

            modalStatus = newValue;
            StateHasChanged();
            await GetmetaFieldsDropDown();
        }

        private async Task ResetFormAsync(bool spinner = true)
        {
            if (spinner)
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
            }

            possibleMetaTitleToDelete = new();
            metaFieldListModal = new();
            metaTitlesListDtoRequest = new();

            metaTitlesListToDelete = new();
            await GetmetaFieldsDropDown();
            metaTitlesListToCreate = new();
            metaFieldidToAdd = 0;
            orderString = string.Empty;

            if (DTBId != 0)
            {
                await GetmetaFieldsOfBag(DTBId);
            }

            StateHasChanged();
            EnableSaveButton();

            if (spinner)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
        }

        private void AddToList()
        {
            try
            {
                if (orderInput.IsInputValid && metaFieldidToAdd != 0)
                {
                    if (metaTitlesListDtoRequest.Select(x => x.MetaFieldId).ToList().Contains(metaFieldidToAdd))
                    {
                        notificationModal.UpdateModal(ModalType.Information, Translation["RecordAlreadyAssigned"], true, Translation["Accept"], "");
                    }
                    else if (metaTitlesListDtoRequest.Select(x => x.OrderData).ToList().Contains(int.Parse(orderString)))
                    {
                        notificationModal.UpdateModal(ModalType.Information, Translation["CannotAddRepeated"], true, Translation["Accept"], "");
                    }
                    else
                    {
                        var info = metaFieldListModal.Find(metafield => metafield.MetaFieldId.Equals(metaFieldidToAdd));

                        var itemToAdd = new MetaTitleCreateDtoRequest()
                        {
                            MetaFieldId = metaFieldidToAdd,
                            OrderData = int.Parse(orderString),
                            ActiveState = info!.ActiveState,
                            FieldTypeValue = info!.FieldTypeValue,
                            NameMetaField = info!.NameMetaField,
                            CreateDate = info!.CreateDate,
                            CreateUser = info!.CreateUser,
                        };

                        switch (ConfigurationId)
                        {
                            case 1:
                                itemToAdd.DocumentaryTypologyBagId = DTBId;
                                break;

                            case 2:
                                itemToAdd.SeriesId = DTBId;
                                break;

                            case 3:
                                itemToAdd.SubSeriesId = DTBId;
                                break;
                        }

                        var copy = metaTitlesListDtoRequest;
                        copy.Add(itemToAdd);

                        metaTitlesListToCreate.Add(itemToAdd);
                        metaTitlesListDtoRequest = copy;
                        metaTitlesListDtoRequest = metaTitlesListDtoRequest.OrderBy(x => x.OrderData).ToList();
                        metaFieldidToAdd = 0;
                        orderString = string.Empty;
                    }
                }

                StateHasChanged();
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["MetaValuesAddErrorMessage"], true, Translation["Accept"]);
            }
        }

        #region DeleteToList

        private void DeleteFromList(MetaTitleCreateDtoRequest request)
        {
            try
            {
                possibleMetaTitleToDelete = request;

                notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, Translation["Accept"], Translation["Cancel"]);

                StateHasChanged();
            }
            catch { notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"], ""); }
        }

        #endregion DeleteToList

        public async Task GetmetaFieldsDropDown()
        {
            metaFieldFilter = new()
            {
                ActiveState = true
            };

            var responseApi = await HttpClient.PostAsJsonAsync("paramsdocumentary/MetaFields/ByFilterList", metaFieldFilter);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<MetaFieldsDtoResponse>>>();
            if (deserializeResponse.Succeeded && ( deserializeResponse.Data != null && deserializeResponse.Data.Any() ))
            {
                metaFieldListModal = deserializeResponse.Data;
            }
            else
            {
                metaFieldListModal = new();
            }
            StateHasChanged();
        }

        public async Task GetmetaFieldsOfBag(int data = 0, bool spinner = true)
        {
            if (spinner)
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
            }
            DTBId = data;
            metaFieldFilter = new();

            switch (ConfigurationId)
            {
                case 1:

                    metaFieldFilter.DocumentaryTypologyBagId = data;
                    break;

                case 2:
                    metaFieldFilter.SeriesId = data;
                    break;

                case 3:
                    metaFieldFilter.SubSeriesId = data;
                    break;
            }

            var responseApi = await HttpClient.PostAsJsonAsync("paramsdocumentary/MetaFields/ByFilterTotal", metaFieldFilter);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<MetaTitleCreateDtoRequest>>>();
            if (deserializeResponse.Succeeded && ( deserializeResponse.Data != null && deserializeResponse.Data.Any() ))
            {
                metaTitlesListDtoRequest = deserializeResponse.Data;
            }
            else
            {
                metaTitlesListDtoRequest = new();
            }

            StateHasChanged();
            if (spinner)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
        }

        #region EnableSaveButton

        public void EnableSaveButton()
        {
            int value = 0;
            orderString = ( !string.IsNullOrEmpty(orderString) && int.TryParse(orderString, out value) ) ? value.ToString() : "0";

            if (string.IsNullOrEmpty(orderString) || ( metaFieldidToAdd ) == 0)
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