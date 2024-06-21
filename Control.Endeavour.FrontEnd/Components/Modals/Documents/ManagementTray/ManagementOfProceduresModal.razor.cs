using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Documents;
using DevExpress.Blazor.Internal.ComponentStructureHelpers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Newtonsoft.Json;

using System.Net.Http.Json;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray;

public partial class ManagementOfProceduresModal
{
    #region Variables

    #region Injects

    [Inject]
    private IJSRuntime Js { get; set; }

    [Inject]
    private HttpClient? HttpClient { get; set; }

    /*[Inject]
    private EventAggregatorService? EventAggregator { get; set; }*/

    [Inject]
    private IStringLocalizer<Translation>? Translation { get; set; }

    [Inject]
    private DocumentsStateContainer? documentsStateContainer { get; set; }

    [Inject]
    private NavigationManager navigation { get; set; }

    #endregion Injects

    #region Components

    private NotificationsComponentModal notificationModalSucces { get; set; } = new();
    private NotificationsComponentModal notificationModal { get; set; } = new();
    private InputModalComponent inputRadicado { get; set; } = new();
    private InputModalComponent inputIdDocumento { get; set; } = new();
    private InputModalComponent inputAnio { get; set; } = new();
    private int ValueTipoAction { get; set; }

    #endregion Components

    #region Models

    private List<ActionsDtoResponse>? LstTypeActions { get; set; } = new();

    private List<InstructionsDtoResponse>? LstTypeInstructions { get; set; } = new();
    private List<ObjectTransaction>? UserSenderTramite { get; set; } = new();
    private List<RetunUserSearch> userListSenders = new();
    private List<RetunUserSearch> userListCopies = new();
    private DocumentDtoResponse managementTray = new();

    private GenericUserSearchModal genericUserSearch = new();

    #endregion Models

    #region Enviroment

    private string panel_1 = "";
    private string panel_2 = "d-none";
    private string panel_3 = "d-none";
    private string panel_2_type_user = "d-none";
    private string panel_2_type_close = "d-none";
    private string commentaryClosed = string.Empty;
    private string classSimpleProcessing = string.Empty;
    private string classBulkProcessing = "d-none";
    private string pnlAdjunto = "d-none";
    private int positionNumber = 0;
    private string managementInstructions = "INI";
    private string documentoId = "";
    private string numRadicado = "";
    private string anio = "";
    private int controlId = 0;
    private int characterCounter = 0;
    private string? texAcctionType = "";
    private string typeOfProcedure = string.Empty;
    private bool UserModalStatus = false;

    #region MassiveProcess

    private List<int> IdsControl = new();
    private bool Massive = false;
    private string DocumentTitle = "ID Documento";

    #endregion MassiveProcess

    #endregion Enviroment

    #region Parameters

    [Parameter]
    public string idModalIdentifier { get; set; } = null!;

    [Parameter]
    public bool modalStatus { get; set; } = false;

    [Parameter]
    public EventCallback<bool> OnChangeData { get; set; }

    #endregion Parameters

    #endregion Variables

    #region Initialization

    protected override async Task OnInitializedAsync()
    {
        try
        {
            texAcctionType = Translation["SelectAnOption"];
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            modalStatus = false;
            await GetActionType("CL,I");
            await GetInstructionsType("CL,I");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al realizar la initialización: {ex.Message}");
        }
    }

    #endregion Initialization

    #region HandleMethods

    private async Task HandleLanguageChanged()
    {
        StateHasChanged();
    }

    private void HandleUserSelectedChanged(List<RetunUserSearch> usersData)
    {
        UserSenderTramite.Clear();


        userListSenders = usersData
            .Where(user => user.IsSelected == true)
            .ToList();

        userListCopies = usersData
            .Where(user => user.IsCopy == true)
            .ToList();

        foreach (var sender in userListSenders)
        {
            var tramite = new ObjectTransaction
            {
                UserInfo = sender,
                InstructionId = 0,
                Days = null,
                Hours = null,
                Subject = null,
                Position = positionNumber
            };

            UserSenderTramite.Add(tramite);
            positionNumber++;
        }

        if (UserSenderTramite.Count > 0)
        {
            panel_3 = panel_1;
        }

        
    }

    private void HandleModalClosed(bool status)
    {
        modalStatus = status;
        panel_2_type_close = "d-none";
        panel_2_type_user = "d-none";
        classBulkProcessing = "d-none";
        classSimpleProcessing = string.Empty;
        StateHasChanged();
    }

    private void HandleTramite()
    {
        var errorMessage = ValidateTransactions();
        if (!string.IsNullOrEmpty(errorMessage))
        {
            notificationModalSucces.UpdateModal(ModalType.Warning, errorMessage, true, Translation["Accept"], Translation["Cancel"]);
        }
        else
        {
            notificationModal.UpdateModal(ModalType.Information, Translation["ProcessDocumentQuestion"], true, Translation["Accept"], Translation["Cancel"], modalOrigin: "HandleTramiteMethod");
        }
    }

    private async Task HandleModalTramiteClosed(ModalClosedEventArgs args)
    {
        if (args.ModalOrigin.Equals("GEX"))
        {
            if (args.IsAccepted)
            {
                HttpClient?.DefaultRequestHeaders.Remove("ControlId");
                HttpClient?.DefaultRequestHeaders.Add("ControlId", $"{controlId}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<DocumentResponseComentaryClosedDtoResponse>>("documentmanagement/DocumentManagement/GetDocumentClosedBehavior");
                HttpClient?.DefaultRequestHeaders.Remove("ControlId");

                if (deserializeResponse.Succeeded)
                {
                    documentsStateContainer.ParametrosManagementTray(deserializeResponse.Data, ValueTipoAction);
                    navigation.NavigateTo("/CreateDocumentaryTask");
                }

                //var docTemp = new
                //{
                //    Identification = controlId,
                //    Value = ValueTipoAction
                //};

                //EncriptService.Encrypt(HttpClient, Js, docTemp, "container");
            }
            else
            {
                if (args.IsCancelled)
                {
                    panel_2 = panel_1;
                    panel_2_type_close = panel_2;
                    typeOfProcedure = "WithoutUserProcedure";
                    panel_3 = panel_1;
                }
            }
        }
        else
        {
            if (args.IsAccepted && args.ModalOrigin.Equals("HandleTramiteMethod"))
            {
                await AddManagementOfProcedure();
                panel_2_type_user = "d-none";
                panel_2_type_close = "d-none";
            }
        }
    }

    private async Task HandleModalSuccesClosed(ModalClosedEventArgs args)
    {
        if (notificationModalSucces.Type == ModalType.Success)
        {
            ResetFormAsync();
            modalStatus = args.ModalStatus;
        }
    }

    private async Task AddManagementOfProcedure()
    {
        try
        {
            var model = new ManagementTrayDtoRequest();
            var massiveModel = new List<ManagementTrayDtoRequest>();
            switch (typeOfProcedure)
            {
                case "WithUsersProcedure":
                    //se crea la lista de assignedUserIds
                    var assignedUsers = new List<AssignedUserIdDtoRequest>();
                    UserSenderTramite.ForEach(a =>
                    {
                        assignedUsers.Add(new AssignedUserIdDtoRequest
                        {
                            AssignUserId = a.UserInfo.UserId,
                            Commentary = a.Subject ?? "",
                            InstructionId = a.InstructionId,
                            ItsCopy = false
                        });
                    });
                    // los usuarios que se asignarón como copias
                    userListCopies?.ForEach(c =>
                    {
                        assignedUsers.Add(new AssignedUserIdDtoRequest
                        {
                            AssignUserId = c.UserId,
                            Commentary = "COPIA DESDE TRAMITE",
                            InstructionId = 0,
                            ItsCopy = true
                        });
                    });
                    // se genera el objeto de tramite
                    switch (Massive)
                    {
                        case true:

                            foreach (var item in IdsControl)
                            {
                                model = new ManagementTrayDtoRequest
                                {
                                    ControlId = item,
                                    ActionId = ValueTipoAction,
                                    AssignedUserIds = assignedUsers
                                };
                                massiveModel.Add(model);
                            }
                            break;

                        case false:

                            model = new ManagementTrayDtoRequest
                            {
                                ControlId = controlId,
                                ActionId = ValueTipoAction,
                                AssignedUserIds = assignedUsers
                            };
                            break;
                    }

                    break;

                case "WithoutUserProcedure":
                    // Código para el caso "WithoutUserProcedure"
                    switch (Massive)
                    {
                        case true:

                            foreach (var item in IdsControl)
                            {
                                model = new ManagementTrayDtoRequest
                                {
                                    ControlId = item,
                                    ActionId = ValueTipoAction,
                                    CommentaryClosed = commentaryClosed
                                };
                                massiveModel.Add(model);
                            }
                            break;

                        case false:

                            model = new ManagementTrayDtoRequest
                            {
                                ControlId = controlId,
                                ActionId = ValueTipoAction,
                                CommentaryClosed = commentaryClosed
                            };
                            break;
                    }
                    break;
            }
            string PossitiveMessage = Translation["DocumentProcessedSatisfactorily"];
            HttpResponseMessage responseApi = new();
            switch (Massive)
            {
                case true:
                    MassiveProcess massive = new() { Documents = massiveModel };
                    responseApi = await HttpClient!.PostAsJsonAsync("documentmanagement/DocumentManagement/CreateMassiveProcess", massive);
                    PossitiveMessage = Translation["DocumentsMassiveSuccedMessage"];

                    break;

                case false:
                    responseApi = await HttpClient!.PostAsJsonAsync("documentmanagement/DocumentManagement/CreateProcess", model);

                    break;
            }

            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();
            if (deserializeResponse.Succeeded)
            {
                notificationModalSucces.UpdateModal(ModalType.Success, PossitiveMessage, true);
                await OnChangeData.InvokeAsync(true);
            }
            else
            {
                notificationModalSucces.UpdateModal(ModalType.Error, Translation["ProcedureErrorMessage"], true, Translation["Accept"]);
            }
        }
        catch (Exception ex)
        {
            notificationModalSucces.UpdateModal(ModalType.Error, Translation["ErrorProcessingRequest"], true, Translation["Accept"]);
        }
    }

    #endregion HandleMethods

    #region Methods

    #region MethodsGenerales

    public async Task UpdateModalStatus(bool newValue, string classCode)
    {
        modalStatus = newValue;
        await GetActionType(classCode);
        await GetInstructionsType(classCode);
        StateHasChanged();
    }

    public void PrepareModal(List<int> idsControl)
    {
        IdsControl = idsControl!.ToList();
        Massive = true;
        DocumentTitle = "IDs Documento";
        classBulkProcessing = string.Empty;
        classSimpleProcessing = "d-none";
        documentoId = string.Join(",", idsControl);
        StateHasChanged();
    }

    private async Task EnablePanel(int value)
    {
        genericUserSearch.ClearUserCheckeds();
        UserSenderTramite.Clear();
        panel_2_type_close = "d-none";
        panel_2_type_user = "d-none";
        panel_3 = "d-none";

        var actionObject = LstTypeActions?.FirstOrDefault(a => a.ActionId == value);

        string flowStateCode = actionObject.FlowStateCode;
        if (value == 0)
        {
            panel_2 = "d-none";
            panel_3 = "d-none";
            ValueTipoAction = value;
        }
        else
        {
            ValueTipoAction = value;
            int panel = (flowStateCode.Equals("ES,ETR") || flowStateCode.Equals("ES,ENP")) ? 1 : 2;
            switch (panel)
            {
                case 1:
                    panel_2 = panel_1;
                    panel_2_type_user = panel_2;
                    typeOfProcedure = "WithUsersProcedure";
                    panel_3 = "d-none";
                    break;

                case 2:
                    if (flowStateCode.Equals("ES,GEX"))
                    {
                        try
                        {
                            switch (Massive)
                            {
                                case true:
                                    var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/DocumentManagement/ValidateDocumentBehaviors", IdsControl);
                                    var deserializeResponseMassive = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<int>>>();
                                    if (deserializeResponseMassive.Succeeded && deserializeResponseMassive.Data.Count != 0)
                                    {
                                        IdsControl = IdsControl.Where(id => !deserializeResponseMassive.Data.Contains(id)).ToList();
                                        notificationModal.UpdateModal(ModalType.Information, Translation["DocumentsExcludedMessage"] + ":\n" + string.Join(",", deserializeResponseMassive.Data), true, buttonTextCancel: "");
                                        panel_2 = panel_1;
                                        panel_2_type_close = panel_2;
                                        typeOfProcedure = "WithoutUserProcedure";
                                        panel_3 = panel_1;
                                    }
                                    break;

                                case false:
                                    HttpClient?.DefaultRequestHeaders.Remove("ControlId");
                                    HttpClient?.DefaultRequestHeaders.Add("ControlId", $"{controlId}");
                                    var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<bool>>("documentmanagement/DocumentManagement/ValidateDocumentBehavior");
                                    HttpClient?.DefaultRequestHeaders.Remove("ControlId");

                                    if (deserializeResponse.Succeeded)
                                    {
                                        notificationModal.UpdateModal(ModalType.Information, Translation["ClosedDocumentaryTask"], true, Translation["Yes"], Translation["No"], modalOrigin: "GEX");
                                    }
                                    else
                                    {
                                        panel_2 = panel_1;
                                        panel_2_type_close = panel_2;
                                        typeOfProcedure = "WithoutUserProcedure";
                                        panel_3 = panel_1;
                                    }
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        panel_2 = panel_1;
                        panel_2_type_close = panel_2;
                        typeOfProcedure = "WithoutUserProcedure";
                        panel_3 = panel_1;
                    }

                    break;
            }
        }
    }

    private void ChangeValueAction(string fieldName, ObjectTransaction tramite, int aux, int value = 0, string subject = "")
    {
        if (aux == 0)
        {
            foreach (var userTramite in UserSenderTramite)
            {
                switch (fieldName.ToLower())
                {
                    case "action":
                        userTramite.InstructionId = value;
                        break;

                    case "asunto":
                        userTramite.Subject = subject;
                        userTramite.CountCharacters = tramite.CountCharacters;
                        break;
                }
            }
        }
        else
        {
            switch (fieldName.ToLower())
            {
                case "action":
                    tramite.InstructionId = value;
                    break;

                case "asunto":
                    tramite.Subject = subject;
                    break;

                default:
                    throw new ArgumentException("Campo no válido", nameof(fieldName));
            }
        }
    }

    private void CountCharacters(ChangeEventArgs e, ObjectTransaction tramite = null)
    {
        string value = e.Value?.ToString() ?? string.Empty;
        int countCarac = value.Length;

        if (tramite != null)
        {
            tramite.CountCharacters = countCarac;
            ChangeValueAction("asunto", tramite, tramite.Position, subject: value);
        }
        else
        {
            characterCounter = countCarac;
        }
    }

    public string ValidateTransactions()
    {
        List<int> missingInstructionIndexes = new List<int>();
        List<int> missingSubjectIndexes = new List<int>();
        List<string> errorMessages = new List<string>();

        if (typeOfProcedure == "WithUsersProcedure")
        {
            for (int i = 0; i < UserSenderTramite.Count; i++)
            {
                if (UserSenderTramite[i].InstructionId <= 0)
                {
                    missingInstructionIndexes.Add(i + 1);
                }
                if (string.IsNullOrEmpty(UserSenderTramite[i].Subject))
                {
                    missingSubjectIndexes.Add(i + 1);
                }
            }

            if (missingInstructionIndexes.Count > 0)
            {
                if (missingInstructionIndexes.Count == 1)
                {
                    errorMessages.Add($"Hay un error en el registro {missingInstructionIndexes[0]}: No se seleccionó la instrucción.");
                }
                else
                {
                    errorMessages.Add("No se seleccionaron las instrucciones en varios registros.");
                }
            }

            if (missingSubjectIndexes.Count > 0)
            {
                if (missingSubjectIndexes.Count == 1)
                {
                    errorMessages.Add($"Hay un error en el registro {missingSubjectIndexes[0]}: El asunto no puede estar vacío.");
                }
                else
                {
                    errorMessages.Add("Los asuntos no pueden estar vacíos en varios registros.");
                }
            }
        }
        else if (typeOfProcedure == "WithoutUserProcedure")
        {
            if (string.IsNullOrEmpty(commentaryClosed))
            {
                errorMessages.Add("La justificación no puede estar vacía.");
            }
        }

        return string.Join("\n", errorMessages);
    }

    public void ResetFormAsync()
    {
        panel_2 = "d-none";
        panel_3 = "d-none";
        UserSenderTramite = new();
        userListCopies = new();
        ValueTipoAction = 0;
        commentaryClosed = string.Empty;
    }

    #endregion MethodsGenerales

    #region MethodsAsync

    private async Task GetActionType(string classCode)
    {
        try
        {
            ActionsFilterDtoRequest filter = new()
            {
                ClassCode = classCode
            };
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            var responseApi = await HttpClient!.PostAsJsonAsync("documentmanagement/Actions/ByFilter", filter);

            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ActionsDtoResponse>>>();
            if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
            {
                LstTypeActions = deserializeResponse.Data;
            }
            else
            {
                LstTypeActions = new List<ActionsDtoResponse>();
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcedureErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener la acción: {ex.Message}");
        }
    }

    private async Task GetInstructionsType(string classCode)
    {
        try
        {
            InstructionsFilterDtoRequest filter = new()
            {
                ClassCode = classCode
            };
            var responseApi = await HttpClient!.PostAsJsonAsync("documentmanagement/Instruction/ByFilter", filter);

            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<InstructionsDtoResponse>>>();
            if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
            {
                LstTypeInstructions = deserializeResponse.Data;
            }
            else
            {
                LstTypeInstructions = new();
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener los tipos de instruciones: {ex.Message}");
        }
    }

    public async Task ManagementOfProcedures(DocumentDtoResponse model)
    {
        controlId = model.ControlId;
        managementTray = model;
        documentoId = $"{model.ControlId}";
        numRadicado = model.FilingCode;
        anio = $"{model.CreateDate:yyyy}";
    }

    private async Task showModalSearchUser()
    {
        UserModalStatus = true;
        StateHasChanged();
    }

    #endregion MethodsAsync

    #region CloseModalSearchUser
    private void HandleModalUserClosed(bool status)
    {

        UserModalStatus = status;
        genericUserSearch.ClearUserCheckeds();

    }

    #endregion
    #endregion Methods
}