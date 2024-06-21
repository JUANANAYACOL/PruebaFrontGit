using Control.Endeavour.FrontEnd.Components.Components.ButtonGroup;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaData.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaValues.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaValues.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Records.Request;
using Control.Endeavour.FrontEnd.Models.Models.Records.Response;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.Blazor.Primitives.Internal;
using DevExpress.Data.ODataLinq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using OpenAI_API.Moderation;
using System.Net.Http.Json;
using Telerik.SvgIcons;
using System.Text.RegularExpressions;
using Telerik.DataSource.Extensions;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.MetaData
{
    public partial class MetaDataValueModal
    {
        #region Variables

        #region Inject

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject] private HttpClient? HttpClient { get; set; }
        [Inject] private IStringLocalizer<Translation>? Translation { get; set; }
        [Inject] private IJSRuntime Js { get; set; }
        #endregion Inject

        #region Parameters

        [Parameter]
        public bool modalStatus { get; set; }

        [Parameter]
        public EventCallback<MyEventArgs<MetaDataRelationDtoRequest>> OnStatusChanged { get; set; } = new();

        [Parameter]
        public EventCallback<MyEventArgs<SearchConfigurationArgs>> ConfigurationToUse { get; set; } = new();

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public EventCallback<bool> OnModalClose { get; set; } = new();

        public List<VUserDtoResponse> usersList { get; set; } = new();

        #endregion Parameters

        #region Models

        private MetaDataRelationDtoRequest metaDataSelected { get; set; } = new();

        private List<MetaValuesDtoResponse> metaValues { get; set; } = new();

        private ThirdPartyDtoResponse thirdPartySelected { get; set; } = new();

        private List<VUserDtoResponse> vUsersSelected { get; set; } = new();
        private MetaDataTypologyDtoResponse metaDataTypology { get; set; } = new();

        #endregion Models

        #region Modals

        public NotificationsComponentModal notificationModal { get; set; } = new();
        public GenericSearchModal serachModal { get; set; } = new();

        private ButtonGroupComponent inputModal { get; set; } = new();

        #endregion Modals

        #region Environments

        #region Environments(Numeric)

        private decimal CharacterCounter { get; set; } = 0;
        private int configurationInUse { get; set; } = 0;

        #endregion Environments(Numeric)

        #region Environments(String)

        private string selectedDropDown { get; set; } = "";

        private string color { get; set; } = "";
        private string MetaDataValue { get; set; } = "";
        private string showPanelNumber = "d-none";
        private string showPanelAlphaNumeric = "d-none";
        private string showPanelDate = "d-none";
        private string showPanelBool = "d-none";
        private string showPanelUser = "d-none";
        private string showPanelThirdParty = "d-none";
        private string showPanelList = "d-none";
        private string origin = "";

        #endregion Environments(String)

        #region Environments(DateTime)

        private DateTime? date { get; set; } = DateTime.Now;
        private DateTime minValueTo { get; set; } = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime maxValueTo { get; set; } = new DateTime(3900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion Environments(DateTime)

        #region Environments(Bool)

        private bool isMandatory { get; set; } = false;
        private bool saveIsDisable { get; set; } = true;
        private bool multipleSelection { get; set; } = false;

        private bool isActive { get; set; } = false;

        private bool complete { get; set; } = false;
        private bool absent { get; set; } = false;
        private bool incomplete { get; set; } = false;
        private bool none { get; set; } = false;

        #endregion Environments(Bool)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            Title = Translation["MetadataEditingCreation"];
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
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
            modalStatus = status;
            StateHasChanged();
            OnModalClose.InvokeAsync(true);
        }

        private void HandleValidSubmit()
        {
            switch (metaDataSelected.FieldType)
            {
                case "FTY,13":
                    metaDataSelected.ColorData = color;
                    metaDataSelected.DataText = MetaDataValue ?? string.Empty;

                    break;

                case "FTY,14":
                    metaDataSelected.ColorData = color;
                    metaDataSelected.DataText = date.ToString();

                    break;

                case "FTY,15":
                    metaDataSelected.ColorData = color;
                    metaDataSelected.DataText = isActive.ToString();

                    break;

                case "FTY,16":
                    metaDataSelected.ColorData = color;
                    metaDataSelected.DataText = MetaDataValue ?? string.Empty;
                    break;

                case "FTY,17":
                    metaDataSelected.ColorData = color;
                    metaDataSelected.DataText = MetaDataValue ?? string.Empty;
                    break;

                case "FTY,18":
                    metaDataSelected.ColorData = color;
                    metaDataSelected.DataText = MetaDataValue ?? string.Empty;
                    break;

                case "FTY,19":
                    metaDataSelected.ColorData = color;
                    metaDataSelected.DataText = selectedDropDown;
                    break;
            }
            metaDataSelected.ColorData = color;
            notificationModal.UpdateModal(ModalType.Information, Translation["SaveQuestionMessage"], true, Translation["Yes"], Translation["No"]);
        }

        private void HandleCheckBoxes(bool newValue, int checkBoxCase)
        {
            if (!newValue)
            {
                newValue = !newValue;
            }

            switch (checkBoxCase)
            {
                case 1:
                    color = "MDC,V";
                    complete = newValue;
                    absent = false;
                    incomplete = false;
                    none = false;
                    break;

                case 2:
                    color = "MDC,AZ";
                    absent = newValue;
                    complete = false;
                    incomplete = false;
                    none = false;
                    break;

                case 3:
                    color = "MDC,A";
                    incomplete = newValue;
                    absent = false;
                    complete = false;
                    none = false;
                    break;

                case 4:
                    color = "MDC,NE";
                    none = newValue;
                    absent = false;
                    complete = false;
                    incomplete = false;

                    break;
            }
            StateHasChanged();
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {

            if (notificationModal.Type == ModalType.Information && args.IsAccepted && origin == "MetadataTypology")
            {
                metaDataTypology.MetaValue = metaDataSelected.DataText;
                metaDataTypology.Color = metaDataSelected.ColorData;
                await HandleUpdateMetadataTypology(metaDataTypology);

                var result = new MyEventArgs<MetaDataRelationDtoRequest>()
                {
                    Data = metaDataSelected,
                    ModalStatus = false
                };

                await OnStatusChanged.InvokeAsync(result);
                //notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
            }
            else if (notificationModal.Type == ModalType.Information && args.IsAccepted)
            {
                var result = new MyEventArgs<MetaDataRelationDtoRequest>()
                {
                    Data = metaDataSelected,
                    ModalStatus = false
                };

                await OnStatusChanged.InvokeAsync(result);

                notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
            }

            StateHasChanged();
        }

        private void HandleUsersSelected(MyEventArgs<List<object>> request)
        {
            vUsersSelected = (List<VUserDtoResponse>)request.Data[0];
            serachModal.UpdateModalStatus(request.ModalStatus);
            EnabledButton();
        }

        private void HandleThirdPartySelected(MyEventArgs<ThirdPartyDtoResponse> request)
        {
            thirdPartySelected = request.Data;
            serachModal.UpdateModalStatus(request.ModalStatus);
            EnabledButton();
        }

        #endregion HandleMethods

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            EnabledButton();
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region MetaFieldSelected

        public void MetaFieldSelected(MetaDataRelationDtoRequest newValue)
        {
            metaDataSelected = newValue;
            showPanelNumber = "d-none";
            showPanelAlphaNumeric = "d-none";
            showPanelDate = "d-none";
            showPanelBool = "d-none";
            showPanelUser = "d-none";
            showPanelThirdParty = "d-none";
            showPanelList = "d-none";

            switch (metaDataSelected.FieldType)
            {
                case "FTY,13":
                    showPanelNumber = "";
                    MetaDataValue = string.IsNullOrEmpty(metaDataSelected.DataText) ? "" : metaDataSelected.DataText;
                    CharacterCounter = string.IsNullOrEmpty(metaDataSelected.DataText) ? 0 : metaDataSelected.DataText.Length;
                    break;

                case "FTY,14":
                    showPanelDate = "";
                    date = string.IsNullOrEmpty(metaDataSelected.DataText) ? DateTime.Now : DateTime.Parse(metaDataSelected.DataText);
                    break;

                case "FTY,15":
                    showPanelBool = "";
                    isActive = string.IsNullOrEmpty(metaDataSelected.DataText) ? false : bool.Parse(metaDataSelected.DataText);
                    break;

                case "FTY,16":
                    showPanelAlphaNumeric = "";
                    MetaDataValue = metaDataSelected.DataText;
                    break;

                case "FTY,17":
                    showPanelThirdParty = "";
                    MetaDataValue = metaDataSelected.DataText;
                    break;

                case "FTY,18":
                    showPanelUser = "";
                    MetaDataValue = metaDataSelected.DataText;
                    break;

                case "FTY,19":
                    showPanelList = "";
                    metaValues = (List<MetaValuesDtoResponse>)metaDataSelected.MetaValues.OrderBy(x => x.ValueOrder).ToList();
                    selectedDropDown = string.IsNullOrEmpty(metaDataSelected.DataText) ? "" : metaDataSelected.DataText;
                    break;
            }

            switch (metaDataSelected.ColorData)
            {
                case "MDC,V":
                    HandleCheckBoxes(true, 1);
                    break;

                case "MDC,AZ":
                    HandleCheckBoxes(true, 2);
                    break;

                case "MDC,A":
                    HandleCheckBoxes(true, 3);
                    break;

                case "MDC,NE":
                    HandleCheckBoxes(true, 4);
                    break;
            }

            StateHasChanged();
        }

        public void MetaFieldSelectedRecords(VMetaDataDtoResponse newValue, MetaDataTypologyDtoResponse model = null)//MetadataTypology
        {
            metaDataSelected = new();
            metaDataSelected.MetaFieldId = newValue.MetaFieldId.Value;
            metaDataSelected.DataText = newValue.MetaValue;
            metaDataSelected.ColorData = newValue.Color;
            metaDataSelected.FieldType = newValue.FieldTypeCode;
            metaDataSelected.FieldTypeValue = Char.ToUpper(newValue.FieldTypeName[0]) + newValue.FieldTypeName.Substring(1).ToLower();
            metaDataSelected.NameMetaField = Char.ToUpper(newValue.NameMetaField[0]) + newValue.NameMetaField.Substring(1).ToLower();
            showPanelNumber = "d-none";
            showPanelAlphaNumeric = "d-none";
            showPanelDate = "d-none";
            showPanelBool = "d-none";
            showPanelUser = "d-none";
            showPanelThirdParty = "d-none";
            showPanelList = "d-none";

            if(model != null && model.DocumentId != 0)
            {
                origin = "MetadataTypology";
                metaDataTypology = model;
            }

            switch (metaDataSelected.FieldType)
            {
                case "FTY,13":
                    showPanelNumber = "";
                    MetaDataValue = string.IsNullOrEmpty(metaDataSelected.DataText) ? "" : metaDataSelected.DataText;
                    CharacterCounter = string.IsNullOrEmpty(metaDataSelected.DataText) ? 0 : metaDataSelected.DataText.Length;
                    break;

                case "FTY,14":
                    showPanelDate = "";
                    date = string.IsNullOrEmpty(metaDataSelected.DataText) ? DateTime.Now : DateTime.Parse(metaDataSelected.DataText);
                    break;

                case "FTY,15":
                    showPanelBool = "";
                    isActive = string.IsNullOrEmpty(metaDataSelected.DataText) ? false : bool.Parse(metaDataSelected.DataText);
                    break;

                case "FTY,16":
                    showPanelAlphaNumeric = "";
                    MetaDataValue = string.IsNullOrEmpty(metaDataSelected.DataText) ? "" : metaDataSelected.DataText;
                    CharacterCounter = string.IsNullOrEmpty(metaDataSelected.DataText) ? 0 : metaDataSelected.DataText.Length;
                    break;

                case "FTY,17":
                    showPanelThirdParty = "";
                    MetaDataValue = string.IsNullOrEmpty(metaDataSelected.DataText) ? "" : metaDataSelected.DataText;
                    CharacterCounter = string.IsNullOrEmpty(metaDataSelected.DataText) ? 0 : metaDataSelected.DataText.Length;
                    break;

                case "FTY,18":
                    showPanelUser = "";
                    MetaDataValue = string.IsNullOrEmpty(metaDataSelected.DataText) ? "" : metaDataSelected.DataText;
                    CharacterCounter = string.IsNullOrEmpty(metaDataSelected.DataText) ? 0 : metaDataSelected.DataText.Length;
                    break;

                case "FTY,19":
                    showPanelList = "";
                    metaValues = (List<MetaValuesDtoResponse>)metaDataSelected.MetaValues.OrderBy(x => x.ValueOrder).ToList();
                    selectedDropDown = string.IsNullOrEmpty(metaDataSelected.DataText) ? "" : metaDataSelected.DataText;
                    break;
            }

            switch (metaDataSelected.ColorData)
            {
                case "MDC,V":
                    HandleCheckBoxes(true, 1);
                    break;

                case "MDC,AZ":
                    HandleCheckBoxes(true, 2);
                    break;

                case "MDC,A":
                    HandleCheckBoxes(true, 3);
                    break;

                case "MDC,NE":                    
                    HandleCheckBoxes(true, 4);
                    break;

                case "MDC,R":
                    HandleCheckBoxes(true, 3);
                    break;
            }

            StateHasChanged();
        }

        #endregion MetaFieldSelected

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
            EnabledButton();
        }

        #endregion CountCharacters

        #region ResetFormAsync

        private void ResetFormAsync()
        {
            isActive = false;

            complete = false;
            absent = false;
            incomplete = false;
            none = false;
            date = null;
            CharacterCounter = 0;
            MetaDataValue = string.Empty;
            usersList = new();
            HandleCheckBoxes(true, 2);
            StateHasChanged();
        }

        #endregion ResetFormAsync

        #region UpdateMetadataTypology
        public async Task HandleUpdateMetadataTypology(MetaDataTypologyDtoResponse data)
        {

            if(data.MetaDataId == 0)
            {
                CreateMetaDataTypologyDtoRequest CreateMetaDataTypology = new CreateMetaDataTypologyDtoRequest()
                {
                    DocumentId = data.DocumentId,
                    MetaFieldId = data.MetaFieldId,
                    DataText = data.MetaValue,
                    ColorData = data.Color
                };

                SpinnerLoaderService.ShowSpinnerLoader(Js);
                var responseApi = await HttpClient.PostAsJsonAsync("records/ControlBoard/CreateMetaDataTypology", CreateMetaDataTypology);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<MetaDataDtoResponse>>();
                if (deserializeResponse.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["FileAttachSuccess"], true, Translation["Accept"], modalOrigin: "ManagementTray");
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["FileAttachError"], true, modalOrigin: "ManagementTray");
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            else
            {
                UpdateMetaDataTypologyDtoRequest UpdateMetaDataTypology = new UpdateMetaDataTypologyDtoRequest()
                {
                    DataText = data.MetaValue,
                    ColorData = data.Color,
                    MetaDataId = data.MetaDataId,
                    Active = true
                };

                SpinnerLoaderService.ShowSpinnerLoader(Js);
                var responseApi = await HttpClient.PostAsJsonAsync("records/ControlBoard/UpdateMetaDataTypology", UpdateMetaDataTypology);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<MetaDataDtoResponse>>();
                if (deserializeResponse.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["FileAttachSuccess"], true, Translation["Accept"], modalOrigin: "ManagementTray");
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["FileAttachError"], true, modalOrigin: "ManagementTray");
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }

           

        }
        #endregion

        private async Task openSerachModal(int configuration)
        {
            SearchConfigurationArgs valuesToReturn = new();

            switch (configuration)
            {
                case 1:
                    //Funcionarios
                    valuesToReturn.Configuration = configuration;
                    valuesToReturn.MultipleSelection = true;
                    valuesToReturn.Title = Translation["User"] + " - " + Translation["MetaData"];

                    break;

                case 2:
                    //Terceros
                    valuesToReturn.Configuration = configuration;
                    valuesToReturn.MultipleSelection = false;
                    valuesToReturn.Title = Translation["Third"] + " - " + Translation["MetaData"];
                    break;
            }

            MyEventArgs<SearchConfigurationArgs> args = new()
            {
                Data = valuesToReturn,
                ModalStatus = true,
            };

            await ConfigurationToUse.InvokeAsync(args);
        }

        private void OnDropDownValueChanged(string newValue)
        {
            try
            {
                selectedDropDown = newValue;
            }
            catch (Exception ex)
            {
                // Manejar excepciones, logearlas o tomar medidas apropiadas según tu aplicación
                Console.WriteLine($"Error en OnDropDownValueChanged: {ex.Message}");
                //notificationModal.UpdateModal(ModalType.Error, $"Error en OnDropDownValueChanged: {ex.Message}", true, Translation["Accept"]);
            }
        }

        public void UserSelectionMetaData(List<VUserDtoResponse> request)
        {
            usersList = request;
            foreach (var user in usersList)
            {
                MetaDataValue += string.IsNullOrEmpty(user.FullName) ? "" : $"{user.FullName},";
            }
            MetaDataValue = MetaDataValue.TrimEnd(',');
            StateHasChanged();
        }

        public void ThirdPartySelectionMetaData(ThirdPartyDtoResponse request)
        {
            MetaDataValue = request.Names;
        }

        private void DeleteToList(VUserDtoResponse request)
        {
            try
            {
                usersList.Remove(request);
                MetaDataValue = "";

                foreach (var user in usersList)
                {
                    MetaDataValue += string.IsNullOrEmpty(user.FullName) ? "" : $"{user.FullName},";
                }

                StateHasChanged();
            }
            catch { notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"]); }
        }

        private void EnabledButton()
        {
            saveIsDisable = false;
            if (metaDataSelected.Mandatory ?? false)
            {
                if (string.IsNullOrEmpty(showPanelDate))
                {
                    if (date.HasValue)
                    {
                        saveIsDisable = false;
                    }
                    else
                    {
                        saveIsDisable = true;
                    }
                }
                else if (string.IsNullOrEmpty(showPanelList))
                {
                    if (string.IsNullOrEmpty(selectedDropDown))
                    {
                        saveIsDisable = false;
                    }
                    else
                    {
                        saveIsDisable = true;
                    }
                }
                else if (!string.IsNullOrEmpty(showPanelBool))
                {
                    saveIsDisable = false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(MetaDataValue))
                    {
                        saveIsDisable = false;
                    }
                    else
                    {
                        saveIsDisable = true;
                    }
                }
            }

            StateHasChanged();
        }

        #endregion Methods
    }
}