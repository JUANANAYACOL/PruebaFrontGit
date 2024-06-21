using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaField.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaField.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaValues.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaValues.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VMetaField.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using Telerik.DataSource.Extensions;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.MetaData
{
    public partial class MetaDataModal : ComponentBase
    {
        #region Variables

        #region Inject

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Modals

        public NotificationsComponentModal notificationModal { get; set; } = new();

        #endregion Modals

        #region Parameters

        [Parameter] public bool modalStatus { get; set; }

        #endregion Parameters

        #region Components

        private string showPanel = "d-none";

        private InputModalComponent codeInput { get; set; } = new();
        private InputModalComponent nameInput { get; set; } = new();

        private InputModalComponent orderInput { get; set; } = new();
        private InputModalComponent valueInput { get; set; } = new();

        #endregion Components

        #region Models

        [Parameter] public EventCallback<bool> OnStatusChanged { get; set; }
        private NotificationService NotificationService { get; set; } = new();
        private MetaFieldCreateDtoRequest metaFieldRequest { get; set; } = new();
        private List<VSystemParamDtoResponse> systemParamListModal { get; set; } = new();

        private List<MetaValueCreateDtoRequest> metaValueCreateList { get; set; } = new();

        #endregion Models

        #region Environments

        #region Enviroments (bool)

        private bool delteAllMetaValues { get; set; } = false;
        private bool isEditForm { get; set; } = false;
        private bool mandatory { get; set; } = false;
        private bool topograhpy { get; set; } = false;
        private bool isAnonymous { get; set; } = false;
        private bool saveIsDisable { get; set; } = true;
        private bool saveIsDisableAdd { get; set; } = true;

        private bool active { get; set; } = true;

        #endregion Enviroments (bool)

        #region Enviroments (Numeric)

        private int metaFiledIdToUpdate { get; set; } = new();

        private int metaTitleIdToUpdate { get; set; } = new();

        #endregion Enviroments (Numeric)

        #region Enviroments (String)

        private string fieldTypeCodeModal { get; set; } = "";
        private string orderString { get; set; } = "";
        private string valueString { get; set; } = "";

        #endregion Enviroments (String)

        #region Enviroments (List && Dictionaries)

        private List<MetaValueCreateDtoRequest> listOfMetaValuesToDelete { get; set; } = new();
        private List<MetaValueCreateDtoRequest> listOfMetaValuesToUpdate { get; set; } = new();

        #endregion Enviroments (List && Dictionaries)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetFieldType();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleValidSubmit

        private async Task HandleValidSubmit()
        {
            if (isEditForm)
            {
                await HandleFormUpdate();
            }
            else
            {
                await HandleFormCreate();
            }
        }

        #endregion HandleValidSubmit

        #region HandleFormCreate

        private async Task HandleFormCreate()
        {
            try
            {
                if (codeInput.IsInputValid && nameInput.IsInputValid && metaFieldRequest.FieldType.HasValue())
                {
                    if (metaFieldRequest.FieldType.Equals("FTY,19") && metaValueCreateList.Count < 2)
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["MetaDataInfoMessage"], true, Translation["Accept"]);
                    }
                    else
                    {
                        metaFieldRequest.Anonymization = isAnonymous;

                        metaFieldRequest.Mandatory = mandatory;

                        metaFieldRequest.TopographicLocation = topograhpy;

                        metaFieldRequest.Anonymization = isAnonymous;
                        metaFieldRequest.TopographicLocation = topograhpy;
                        metaFieldRequest.Mandatory = mandatory;
                        metaFieldRequest.ActiveState = active;

                        if (metaValueCreateList.Count != 0 && metaFieldRequest.FieldType.Equals("FTY,19"))
                        {
                            metaFieldRequest.MetaValues = metaValueCreateList;
                        }

                        var responseApi = await HttpClient!.PostAsJsonAsync("paramsdocumentary/MetaFields/CreateMetaFields", metaFieldRequest);
                        var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<VMetaFieldDtoResponse>>();

                        var NotiResult = NotificationService.NotificationDecrypt(Translation, deserializeResponse);
                        notificationModal.UpdateModal(NotiResult.ModalType, NotiResult.NotificationMessage, true, Translation["Accept"], "");
                    }
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["FieldsRequired"], true, Translation["Accept"]);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true, Translation["Accept"]);
            }
        }

        #endregion HandleFormCreate

        #region HandleFormUpdate

        private async Task HandleFormUpdate()
        {
            try
            {
                if (codeInput.IsInputValid && nameInput.IsInputValid && metaFieldRequest.FieldType.HasValue())
                {
                    if (metaFieldRequest.FieldType.Equals("FTY,19") && metaValueCreateList.Count < 2)
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["MetaDataInfoMessage"], true, Translation["Accept"]);
                    }
                    else
                    {
                        metaFieldRequest.Anonymization = isAnonymous;

                        metaFieldRequest.Mandatory = mandatory;

                        metaFieldRequest.TopographicLocation = topograhpy;

                        metaFieldRequest.Anonymization = isAnonymous;
                        metaFieldRequest.TopographicLocation = topograhpy;
                        metaFieldRequest.Mandatory = mandatory;
                        metaFieldRequest.ActiveState = active;

                        var updateMetaFiled = new MetaFieldUpdateDtoRequest()
                        {
                            ActiveState = active,
                            Anonymization = metaFieldRequest.Anonymization,
                            Code = metaFieldRequest.Code,
                            FieldType = metaFieldRequest.FieldType,
                            Mandatory = mandatory,
                            MetaFieldId = metaFiledIdToUpdate,

                            NameMetaField = metaFieldRequest.NameMetaField,
                            TopographicLocation = topograhpy,
                            DeleteMetaValues = delteAllMetaValues
                        };

                        var responseApi = await HttpClient!.PostAsJsonAsync("paramsdocumentary/MetaFields/UpdateMetaFields", updateMetaFiled);
                        var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<VMetaFieldDtoResponse>>();
                        var NotiResult = NotificationService.NotificationDecrypt(Translation, deserializeResponse);

                        if (deserializeResponse!.Succeeded)
                        {
                            if (listOfMetaValuesToDelete.Count != 0)
                            {
                                foreach (var metaValue in listOfMetaValuesToDelete)
                                {
                                    var responseDelete = await HttpClient!.PostAsJsonAsync("paramsdocumentary/MetaValues/DeleteMetaValues", new DeleteGeneralDtoRequest() { Id = metaValue.MetaValueId });
                                    var deserializeResponseDelete = await responseDelete.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                                    //if (!deserializeResponseDelete!.Succeeded)
                                    //{
                                    //    notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"]);
                                    //}
                                }
                            }

                            if (listOfMetaValuesToUpdate.Count != 0)
                            {
                                foreach (var item in listOfMetaValuesToUpdate)
                                {
                                    var newMetaValue = new MetaValueCreateDtoRequest()
                                    {
                                        ActiveState = item.ActiveState,
                                        MetaFieldId = metaFiledIdToUpdate,
                                        ValueOrder = item.ValueOrder,
                                        ValueText = item.ValueText
                                    };

                                    var responseUpdate = await HttpClient!.PostAsJsonAsync("paramsdocumentary/MetaValues/CreateMetaValues", newMetaValue);
                                    var deserializeResponseUpdate = await responseUpdate.Content.ReadFromJsonAsync<HttpResponseWrapperModel<MetaValuesDtoResponse>>();
                                    //if (!deserializeResponseUpdate!.Succeeded)
                                    //{
                                    //    notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                                    //}
                                }
                            }
                            notificationModal.UpdateModal(NotiResult.ModalType, NotiResult.NotificationMessage, true, Translation["Accept"], "");
                        }
                        else
                        {
                            notificationModal.UpdateModal(NotiResult.ModalType, NotiResult.NotificationMessage, true, Translation["Accept"], "");
                        }
                    }
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["FieldsRequired"], true, Translation["Accept"]);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true, Translation["Accept"]);
            }
        }

        #endregion HandleFormUpdate

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region GetFieldTypeCode

        public void GetFieldTypeCode(string code)
        {
            try
            {
                metaFieldRequest.FieldType = code;

                if (code.Equals("FTY,19"))
                {
                    showPanel = "";
                    delteAllMetaValues = false;
                }
                else
                {
                    showPanel = "d-none";
                    delteAllMetaValues = true;
                }

                if (isEditForm && code.Equals("FTY,19"))
                {
                    listOfMetaValuesToUpdate = new();
                    listOfMetaValuesToDelete = new();
                }

                EnableSaveButton();
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["SelectTypeError"], true, Translation["Accept"]);
            }
        }

        #endregion GetFieldTypeCode

        private void OnChangeSwitch()
        {
            active = !active;
        }

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)

        {
            isEditForm = false;
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region ResetFrom

        public void ResetForm()
        {
            fieldTypeCodeModal = "";
            topograhpy = false;
            isAnonymous = false;
            mandatory = false;
            active = true;
            metaFieldRequest.FieldType = "";
            metaFieldRequest.Anonymization = false;
            metaFieldRequest.Mandatory = false;
            metaFieldRequest.TopographicLocation = false;
            metaFieldRequest.Code = "";
            metaFieldRequest.NameMetaField = "";
            metaValueCreateList = new();
            GetFieldTypeCode(fieldTypeCodeModal);

            orderString = "";
            valueString = "";
            listOfMetaValuesToDelete = new();
            listOfMetaValuesToUpdate = new();
            EnableSaveAddButton();
            EnableSaveButton();
        }

        #endregion ResetFrom

        #region GetFieldType

        private async Task GetFieldType()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "FTY");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                if (deserializeResponse!.Succeeded && ( deserializeResponse.Data != null || deserializeResponse.Data?.Count != 0 ))
                {
                    systemParamListModal = deserializeResponse.Data ?? new();
                }
                else
                {
                    systemParamListModal = new();
                }
            }
            catch
            {
                systemParamListModal = new();
            }
        }

        #endregion GetFieldType

        #region HandleModalClosed

        private void HandleModalClosed(bool status)
        {
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
                ResetForm();
                StateHasChanged();
            }
        }

        #endregion HandleModalNotiClose

        #region AddToList

        private void AddToList()
        {
            try
            {
                if (metaValueCreateList.Select(x => x.ValueOrder).ToList().Contains(int.Parse(orderString)))
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation["CannotAddRepeated"], true, Translation["Accept"], "");
                }
                else
                {
                    var itemToAdd = new MetaValueCreateDtoRequest()
                    {
                        ActiveState = true,
                        ValueOrder = int.Parse(orderString),
                        ValueText = valueString
                    };

                    if (isEditForm)
                    {
                        listOfMetaValuesToUpdate.Add(itemToAdd);
                    }

                    var copy = metaValueCreateList;
                    copy.Add(itemToAdd);
                    metaValueCreateList = copy;
                    metaValueCreateList = metaValueCreateList.OrderBy(x => x.ValueOrder).ToList();
                    valueString = string.Empty;
                    orderString = string.Empty;
                }

                StateHasChanged();
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["AddErrorMessage"], true, Translation["Accept"]);
            }
        }

        #endregion AddToList

        #region DeleteToList

        private void DeleteToList(MetaValueCreateDtoRequest request)
        {
            try
            {
                if (isEditForm)
                {
                    listOfMetaValuesToDelete.Add(request);
                }

                metaValueCreateList.Remove(request);
                metaValueCreateList = metaValueCreateList.OrderBy(x => x.ValueOrder).ToList();

                StateHasChanged();
            }
            catch { notificationModal.UpdateModal(ModalType.Error, Translation["RemoveErrorMessage"], true, Translation["Accept"]); }
        }

        #endregion DeleteToList

        #region UpdateMetaData

        public void UpdateMetaData(MetaFieldsDtoResponse data)
        {
            isAnonymous = ( data.Anonymization ?? false );
            topograhpy = ( data.TopographicLocation ?? false );
            mandatory = ( data.Mandatory ?? false );
            active = ( data.ActiveState );
            metaFieldRequest = new()
            {
                ActiveState = data.ActiveState,
                Anonymization = data.Anonymization,
                Code = data.Code,

                FieldType = data.FieldType,
                Mandatory = data.Mandatory,
                NameMetaField = data.NameMetaField,

                TopographicLocation = data.TopographicLocation
            };

            List<MetaValueCreateDtoRequest> existingMetaValues = new();

            foreach (var item in data.MetaValues ?? new())
            {
                existingMetaValues.Add(
                    new MetaValueCreateDtoRequest()
                    {
                        ActiveState = item.ActiveState,
                        MetaFieldId = item.MetaFieldId,
                        MetaValueId = item.MetaValueId,
                        ValueOrder = item.ValueOrder,
                        ValueText = item.ValueText
                    });
            }
            /*    existingMetaValues.OrderBy(x => x.ValueOrder);*/

            metaFieldRequest.MetaValues = existingMetaValues;
            metaValueCreateList = existingMetaValues;

            metaFiledIdToUpdate = data.MetaFieldId;
            isEditForm = true;

            GetFieldTypeCode(data.FieldType);
            EnableSaveAddButton();
            EnableSaveButton();
            StateHasChanged();
        }

        #endregion UpdateMetaData

        #region HandleCheckBoxes

        private void HandleCheckBoxes(bool newValue, int checkBoxCase)
        {
            switch (checkBoxCase)
            {
                case 1:
                    metaFieldRequest.Anonymization = newValue;
                    isAnonymous = newValue;
                    break;

                case 2:
                    metaFieldRequest.Mandatory = newValue;
                    mandatory = newValue;
                    break;

                case 3:
                    metaFieldRequest.TopographicLocation = newValue;
                    topograhpy = newValue;
                    break;
            }
        }

        #endregion HandleCheckBoxes

        #region EnableSaveButton

        public void EnableSaveButton()
        {
            if (string.IsNullOrEmpty(metaFieldRequest.Code) || string.IsNullOrEmpty(metaFieldRequest.NameMetaField) || string.IsNullOrEmpty(metaFieldRequest.FieldType))
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

        #region EnableSaveAddButton

        public void EnableSaveAddButton()
        {
            int value = 0;
            orderString = ( !string.IsNullOrEmpty(orderString) && int.TryParse(orderString, out value) ) ? value.ToString() : "0";

            if (string.IsNullOrEmpty(orderString) || string.IsNullOrEmpty(valueString))
            {
                saveIsDisableAdd = true;
            }
            else
            {
                saveIsDisableAdd = false;
            }
            StateHasChanged();
        }

        #endregion EnableSaveAddButton

        #endregion Methods
    }
}