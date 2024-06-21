using Control.Endeavour.FrontEnd.Components.Components.ButtonGroup;
using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD
{
    public partial class TRDCBehaviourModal
    {
        #region Variables

        #region Inject

        [Inject]
        private IJSRuntime Js { get; set; }

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        [Parameter]
        public EventCallback<bool> OnStatusChanged { get; set; }

        [Parameter]
        public EventCallback<bool> OnChangeData { get; set; }

        #endregion Inject

        #region Components

        private ButtonGroupComponent inputManagerLeader { get; set; } = new();
        private InputModalComponent inputDaysAlarm1 { get; set; } = new();
        private InputModalComponent inputPercentAlarm1 { get; set; } = new();
        private InputModalComponent inputDaysAlarm2 { get; set; } = new();
        private InputModalComponent inputPercentAlarm2 { get; set; } = new();
        private InputModalComponent inputDaysAlarm3 { get; set; } = new();
        private InputModalComponent inputPercentAlarm3 { get; set; } = new();
        private InputModalComponent inputdaysOrHoursNumber { get; set; } = new();

        #endregion Components

        #region Modals

        private GenericSearchModal genericSearchModal { get; set; } = new();
        private NotificationsComponentModal notificationModal { get; set; } = new();

        #endregion Modals

        #region Models

        private VUserDtoResponse leaderManaegerUser { get; set; } = new();
        private DocumentaryTypologiesBehaviorsAllDtoRequest createRequest { get; set; } = new();
        private DocumentaryTypologiesBehaviorsDtoResponse _selectedRecord { get; set; } = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string managerLeaderName { get; set; } = string.Empty;
        private string correspondenceCode { get; set; } = string.Empty;
        private string classCode { get; set; } = string.Empty;
        private string instructioLeadernCode { get; set; } = string.Empty;

        public string daysAlarm1 { get; set; } = "";
        public string percentAlarm1 { get; set; } = "20";
        public string daysAlarm2 { get; set; } = "";
        public string percentAlarm2 { get; set; } = "60";
        public string daysAlarm3 { get; set; } = "";
        public string percentAlarm3 { get; set; } = "20";
        public string timeTypeSelected { get; set; } = "";
        public string daysOrHoursNumber { get; set; } = "";

        public string pqrCode { get; set; } = "PQR";
        public string requireResponseCode { get; set; } = "RR";
        public string alarm1Code { get; set; } = "A1";
        public string alarm2Code { get; set; } = "A2";
        public string alarm3Code { get; set; } = "A3";
        public string documentalTypologyResponseCode { get; set; } = "TDR";
        public string requireResponseDHCode { get; set; } = "RRDH";
        public string requireResponseRadicationCode { get; set; } = "RRD";
        public string requireResponseDHValueCode { get; set; } = "RRDHV";

        #endregion Environments(String)

        #region Environments(Numeric)

        private int ListToFill { get; set; } = new();
        private int managerToDelete { get; set; } = new();
        private int copyToDelete { get; set; } = new();
        public int documentaryTypologyBehaviourId { get; set; } = 0;
        public int documentaryTypologyId { get; set; } = 0;
        private int currentTab { get; set; } = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool LeaderManagerInUse { get; set; } = false;
        private bool IsEditForm { get; set; } = false;
        private bool multipleSelectionManager { get; set; } = new();
        private bool activeState { get; set; } = new();
        private bool modalStatus { get; set; } = new();
        private bool radicationCheck { get; set; } = false;
        private bool pqrCheck { get; set; } = false;
        private bool requireResponseCheck { get; set; } = false;
        private bool saveIsDisable { get; set; } = true;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<VUserDtoResponse> usersManageres { get; set; } = new();
        private List<VUserDtoResponse> userCopies { get; set; } = new();
        private List<VSystemParamDtoResponse>? classList { get; set; }
        private List<VSystemParamDtoResponse>? instructionList { get; set; }
        private List<VSystemParamDtoResponse>? correspondenceList { get; set; }
        private List<DocumentaryTypologiesBehaviorsDtoResponse>? docTypologyList { get; set; } = new();
        private List<BehaviorBagDtoResponse>? behaviorBagList { get; set; } = new();

        private string[] timeType = { "", "" };

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            timeType[0] = Translation["Days"];
            timeType[1] = Translation["Hours"];
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (LeaderManagerInUse)
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["ManagerAE"], true, Translation["Accept"], "");
                LeaderManagerInUse = false;
            };
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        #region HandleValidSubmit

        private async Task HandleValidSubmit()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (IsEditForm)
            {
                await HandleFormUpdate();
            }
            else
            {
                await HandleFormCreate();
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleValidSubmit

        #region HandleFormCreate

        private async Task HandleFormCreate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                await GetBehaviorBag();
                createRequest = new();
                createRequest.DocumentaryTypologyId = documentaryTypologyId;
                createRequest.ClassCode = classCode;
                createRequest.CorrespondenceType = correspondenceCode;
                createRequest.TypologyManagers.Add(new()
                {
                    InstructionCode = instructioLeadernCode,
                    Leader = true,
                    ManagerId = leaderManaegerUser.UserId
                });

                foreach (var item in usersManageres)
                {
                    createRequest.TypologyManagers.Add(new()
                    {
                        InstructionCode = null,
                        Leader = false,
                        ManagerId = item.UserId
                    });
                }

                foreach (var item in userCopies)
                {
                    createRequest.UserCopies.Add(new()
                    {
                        DocumentaryTypologyBehaviorId = documentaryTypologyBehaviourId,
                        UserId = item.UserId
                    });
                }

                createRequest.BehaviorTypologies.Add(
                new()
                {
                    BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{requireResponseRadicationCode}")).FirstOrDefault().BehaviorBagId,
                    BehaviorValue = radicationCheck ? "SI" : "NO"
                });

                createRequest.BehaviorTypologies.Add(
                new()
                {
                    BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{pqrCode}")).FirstOrDefault().BehaviorBagId,
                    BehaviorValue = pqrCheck ? "SI" : "NO"
                });

                createRequest.BehaviorTypologies.Add(
                new()
                {
                    BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{requireResponseCode}")).FirstOrDefault().BehaviorBagId,
                    BehaviorValue = requireResponseCheck ? "SI" : "NO"
                });

                createRequest.BehaviorTypologies.Add(
                new()
                {
                    BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{requireResponseDHCode}")).FirstOrDefault().BehaviorBagId,
                    BehaviorValue = timeTypeSelected.Equals(Translation["Days"]) ? "D" : "H"
                });

                createRequest.BehaviorTypologies.Add(
                new()
                {
                    BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{requireResponseDHValueCode}")).FirstOrDefault().BehaviorBagId,
                    BehaviorValue = requireResponseCheck ? daysOrHoursNumber : "0"
                });

                createRequest.BehaviorTypologies.Add(
                new()
                {
                    BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{alarm1Code}")).FirstOrDefault().BehaviorBagId,
                    BehaviorValue = daysAlarm1
                });

                createRequest.BehaviorTypologies.Add(
                new()
                {
                    BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{alarm2Code}")).FirstOrDefault().BehaviorBagId,
                    BehaviorValue = daysAlarm2
                });

                createRequest.BehaviorTypologies.Add(
                new()
                {
                    BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{alarm3Code}")).FirstOrDefault().BehaviorBagId,
                    BehaviorValue = daysAlarm3
                });

                if (documentaryTypologyBehaviourId != 0)
                {
                    createRequest.BehaviorTypologies.Add(
                    new()
                    {
                        BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{documentalTypologyResponseCode}")).FirstOrDefault().BehaviorBagId,
                        BehaviorValue = documentaryTypologyBehaviourId.ToString()
                    });
                }

                var responseApi = await HttpClient!.PostAsJsonAsync("documentarytypologies/DocumentaryTypologiesBehaviors/CreateDocumentaryTypologiesBehaviorsAll", createRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<DocumentaryTypologiesBehaviorsAllDtoResponse>>>();

                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                    await OnChangeData.InvokeAsync(true);
                    ResetFormAsync();
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation["CreateErrorMessage"], true, Translation["Accept"], "");
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["CreateErrorMessage"], true, Translation["Accept"], "");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleFormCreate

        #region HandleFormUpdate

        private async Task HandleFormUpdate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                await GetBehaviorBag();
                var updateRequest = new DocumentaryTypologiesBehaviorsAllUpdateDtoRequest();
                updateRequest.DocumentaryTypologyBehaviorId = _selectedRecord.DocumentaryTypologyBehaviorId;
                updateRequest.DocumentaryTypologyId = documentaryTypologyId;
                updateRequest.ClassCode = classCode;
                updateRequest.CorrespondenceType = correspondenceCode;

                var idTypoLeader = 0;
                var searchTypo = _selectedRecord.TypologyManagers.Where(x => x.ManagerId == leaderManaegerUser.UserId).FirstOrDefault();

                if (searchTypo != null)
                {
                    idTypoLeader = searchTypo.TypologyManagerId;
                }

                updateRequest.TypologyManagers.Add(new()
                {
                    InstructionCode = instructioLeadernCode,
                    Leader = true,
                    ManagerId = leaderManaegerUser.UserId,
                    TypologyManagerId = idTypoLeader
                });

                foreach (var item in usersManageres ?? new())
                {
                    var id = 0;
                    var search = _selectedRecord.TypologyManagers.Where(x => x.ManagerId == item.UserId).FirstOrDefault();

                    if (search != null)
                    {
                        id = search.TypologyManagerId;
                    }

                    updateRequest.TypologyManagers.Add(new()
                    {
                        InstructionCode = null,
                        Leader = false,
                        ManagerId = item.UserId,
                        TypologyManagerId = id
                    });
                }

                foreach (var item in userCopies ?? new())
                {
                    var id = 0;
                    var serach = _selectedRecord.UserCopies.FirstOrDefault(x => x.UserId == item.UserId);
                    if (serach != null)
                    {
                        id = serach.UserCopiesId;
                    }

                    updateRequest.UserCopies.Add(new()
                    {
                        DocumentaryTypologyBehaviorId = documentaryTypologyBehaviourId,
                        UserId = item.UserId,
                        UserCopiesId = id
                    });
                }

                #region AddBehaviorTypologies

                var reRespRadId = _selectedRecord.BehaviorTypologies.FirstOrDefault(x => x.BehaviorBagId == behaviorBagList.Where(x => x.BehaviorCode.Equals($"{requireResponseRadicationCode}")).FirstOrDefault().BehaviorBagId).BehaviorTypologyId;

                updateRequest.BehaviorTypologies.Add(
                new()
                {
                    BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{requireResponseRadicationCode}")).FirstOrDefault().BehaviorBagId,
                    BehaviorValue = radicationCheck ? "SI" : "NO",
                    BehaviorTypologyId = ( reRespRadId != null && reRespRadId > 0 ) ? reRespRadId : 0
                });

                var pqrId = _selectedRecord.BehaviorTypologies.FirstOrDefault(x => x.BehaviorBagId == behaviorBagList.FirstOrDefault(x => x.BehaviorCode.Equals($"{pqrCode}")).BehaviorBagId).BehaviorTypologyId;
                updateRequest.BehaviorTypologies.Add(
                new()
                {
                    BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{pqrCode}")).FirstOrDefault().BehaviorBagId,
                    BehaviorValue = pqrCheck ? "SI" : "NO",
                    BehaviorTypologyId = ( pqrId != null && pqrId > 0 ) ? pqrId : 0
                });

                var reRespId = _selectedRecord.BehaviorTypologies.FirstOrDefault(x => x.BehaviorBagId == behaviorBagList.FirstOrDefault(x => x.BehaviorCode.Equals($"{requireResponseCode}")).BehaviorBagId).BehaviorTypologyId;
                updateRequest.BehaviorTypologies.Add(
                new()
                {
                    BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{requireResponseCode}")).FirstOrDefault().BehaviorBagId,
                    BehaviorValue = requireResponseCheck ? "SI" : "NO",
                    BehaviorTypologyId = ( reRespId != null && reRespId > 0 ) ? reRespId : 0
                });

                var reRespDHId = _selectedRecord.BehaviorTypologies.FirstOrDefault(x => x.BehaviorBagId == behaviorBagList.FirstOrDefault(x => x.BehaviorCode.Equals($"{requireResponseDHCode}")).BehaviorBagId).BehaviorTypologyId;
                updateRequest.BehaviorTypologies.Add(
                new()
                {
                    BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{requireResponseDHCode}")).FirstOrDefault().BehaviorBagId,
                    BehaviorValue = timeTypeSelected.Equals(Translation["Days"]) ? "D" : "H",
                    BehaviorTypologyId = ( reRespDHId != null && reRespDHId > 0 && requireResponseCheck ) ? reRespDHId : 0
                });

                var reRespValueId = _selectedRecord.BehaviorTypologies.FirstOrDefault(x => x.BehaviorBagId == behaviorBagList.FirstOrDefault(x => x.BehaviorCode.Equals($"{requireResponseDHValueCode}")).BehaviorBagId).BehaviorTypologyId;
                updateRequest.BehaviorTypologies.Add(
                new()
                {
                    BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{requireResponseDHValueCode}")).FirstOrDefault().BehaviorBagId,
                    BehaviorValue = daysOrHoursNumber,
                    BehaviorTypologyId = ( reRespValueId != null && reRespValueId > 0 ) ? reRespValueId : 0
                });

                var alam1Id = _selectedRecord.BehaviorTypologies.FirstOrDefault(x => x.BehaviorBagId == behaviorBagList.FirstOrDefault(x => x.BehaviorCode.Equals($"{alarm1Code}")).BehaviorBagId).BehaviorTypologyId;
                updateRequest.BehaviorTypologies.Add(
                new()
                {
                    BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{alarm1Code}")).FirstOrDefault().BehaviorBagId,
                    BehaviorValue = daysAlarm1,
                    BehaviorTypologyId = ( alam1Id != null && alam1Id > 0 ) ? alam1Id : 0
                });

                var alam2Id = _selectedRecord.BehaviorTypologies.FirstOrDefault(x => x.BehaviorBagId == behaviorBagList.FirstOrDefault(x => x.BehaviorCode.Equals($"{alarm2Code}")).BehaviorBagId).BehaviorTypologyId;
                updateRequest.BehaviorTypologies.Add(
                new()
                {
                    BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{alarm2Code}")).FirstOrDefault().BehaviorBagId,
                    BehaviorValue = daysAlarm2,
                    BehaviorTypologyId = ( alam2Id != null && alam2Id > 0 ) ? alam2Id : 0
                });

                var alam3Id = _selectedRecord.BehaviorTypologies.FirstOrDefault(x => x.BehaviorBagId == behaviorBagList.FirstOrDefault(x => x.BehaviorCode.Equals($"{alarm3Code}")).BehaviorBagId).BehaviorTypologyId;
                updateRequest.BehaviorTypologies.Add(
                new()
                {
                    BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{alarm3Code}")).FirstOrDefault().BehaviorBagId,
                    BehaviorValue = daysAlarm3,
                    BehaviorTypologyId = ( alam3Id != null && alam3Id > 0 ) ? alam3Id : 0
                });

                #endregion AddBehaviorTypologies

                if (documentaryTypologyBehaviourId != 0)
                {
                    var docTypoId = _selectedRecord.BehaviorTypologies.FirstOrDefault(x => x.BehaviorBagId == behaviorBagList.FirstOrDefault(x => x.BehaviorCode.Equals($"{documentalTypologyResponseCode}")).BehaviorBagId).BehaviorTypologyId;
                    updateRequest.BehaviorTypologies.Add(
                    new()
                    {
                        BehaviorBagId = behaviorBagList.Where(x => x.BehaviorCode.Equals($"{documentalTypologyResponseCode}")).FirstOrDefault().BehaviorBagId,
                        BehaviorValue = documentaryTypologyBehaviourId.ToString(),
                        BehaviorTypologyId = ( docTypoId != null && docTypoId > 0 ) ? docTypoId : 0
                    });
                }

                var responseApi = await HttpClient!.PostAsJsonAsync("documentarytypologies/DocumentaryTypologiesBehaviors/UpdateDocumentaryTypologiesBehaviorsAll", updateRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<DocumentaryTypologiesBehaviorsAllDtoResponse>>>();

                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                    await OnChangeData.InvokeAsync(true);
                    IsEditForm = false;
                    ResetFormAsync();
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation["UpdateErrorMessage"], true, Translation["Accept"], "");
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["UpdateErrorMessage"], true, Translation["Accept"], "");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleFormUpdate

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #endregion HandleMethods

        #region OthersMethods

        #region GetDocTypologyId

        public void HandleDocumentaryTypologyId(int id)
        {
            documentaryTypologyId = id;
        }

        #endregion GetDocTypologyId

        #region FillDropDownLists

        #region FillCLassList

        private async Task FillCLassList()
        {
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            HttpClient?.DefaultRequestHeaders.Add("paramCode", "CL");
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            if (deserializeResponse!.Succeeded)
            {
                classList = deserializeResponse.Data;
            }
            else { classList = new(); }
        }

        #endregion FillCLassList

        #region FillCorrespondencelList

        private async Task FillCorrespondencelList()
        {
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            HttpClient?.DefaultRequestHeaders.Add("paramCode", "TCR");
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            if (deserializeResponse!.Succeeded)
            {
                correspondenceList = deserializeResponse.Data;
            }
            else { correspondenceList = new(); }
        }

        #endregion FillCorrespondencelList

        #region GetBehaviorBag

        private async Task GetBehaviorBag()
        {
            var request = new BehaviorBagDtoRequest();
            var responseApi = await HttpClient!.PostAsJsonAsync("documentarytypologies/BehaviorBag/ByFilter", request);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<BehaviorBagDtoResponse>>>();
            if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
            {
                behaviorBagList = deserializeResponse.Data.Data;
            }
            else { behaviorBagList = new(); }
        }

        #endregion GetBehaviorBag

        #region FillInstructionlList

        private async Task FillInstructionlList()
        {
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            HttpClient?.DefaultRequestHeaders.Add("paramCode", "INTM");
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
            HttpClient?.DefaultRequestHeaders.Remove("paramCode");
            if (deserializeResponse!.Succeeded)
            {
                instructionList = deserializeResponse.Data;
            }
            else { instructionList = new(); }
        }

        #endregion FillInstructionlList

        #region GetDocTypologies

        public async Task GetDocTypologies()
        {
            try
            {
                var filterDocTypology = new DocumentaryTypologiesBehaviorsFilterDtoRequest() { ClassCode = "CL,E", DocumentaryTypologyId = documentaryTypologyId };
                var responseApi = await HttpClient!.PostAsJsonAsync("documentarytypologies/DocumentaryTypologiesBehaviors/ByFilter", filterDocTypology);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<DocumentaryTypologiesBehaviorsDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    docTypologyList = deserializeResponse.Data!.Data!;
                }
                else
                {
                    docTypologyList = new();
                }
            }
            catch (Exception ex)
            {
                docTypologyList = new();
                Console.WriteLine($"Error al obtener las Tipologias Documentales: {ex.Message}");
            }
        }

        #endregion GetDocTypologies

        #endregion FillDropDownLists

        #region HandleManagers

        #region HandleGenericSearchStatusChanged

        private async Task HandleGenericSearchStatusChanged(MyEventArgs<VUserDtoResponse> user)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            LeaderManagerInUse = false;

            leaderManaegerUser = user.Data!;

            managerLeaderName = leaderManaegerUser!.FullName;

            if (usersManageres.Select(x => x.UserId).Contains(leaderManaegerUser.UserId))
            {
                usersManageres = usersManageres.Where(x => !x.UserId.Equals(leaderManaegerUser.UserId)).ToList();
                LeaderManagerInUse = true;
            }
            genericSearchModal.UpdateModalStatus(false);
            await UpdateModalStatus(true, false);

            StateHasChanged();
            EnableSaveButton();
            SpinnerLoaderService.HideSpinnerLoader(Js);

            if (LeaderManagerInUse)
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["ManagerAE"], true, Translation["Accept"], "");
            };
            StateHasChanged();
        }

        #endregion HandleGenericSearchStatusChanged

        #region HandleGenericMultipleSearchStatusChanged

        private async Task HandleGenericMultipleSearchStatusChanged(MyEventArgs<List<Object>> user)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            LeaderManagerInUse = false;
            //await UpdateModalStatus(true, false);
            if (ListToFill.Equals(0))
            {
                usersManageres = (List<VUserDtoResponse>)user.Data![0];
                if (usersManageres.Select(x => x.UserId).Contains(leaderManaegerUser.UserId))
                {
                    usersManageres = usersManageres.Where(x => !x.UserId.Equals(leaderManaegerUser.UserId)).ToList();

                    LeaderManagerInUse = true;
                }
            }
            else
            {
                userCopies = (List<VUserDtoResponse>)user.Data![0];
            }

            genericSearchModal.UpdateModalStatus(false);
            await UpdateModalStatus(true, false);
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
            EnableSaveButton();

            StateHasChanged();
        }

        #endregion HandleGenericMultipleSearchStatusChanged

        #region HandleClosed

        private async Task HandleClosed(bool state)
        {
            await UpdateModalStatus(!state, false);
        }

        #endregion HandleClosed

        #region OpenLeaderManager

        private async Task OpenLeaderManager(bool value, bool multipleUsers = false, int origin = 0)
        {
            multipleSelectionManager = multipleUsers;
            ListToFill = origin;
            multipleSelectionManager = value;
            genericSearchModal.UpdateModalStatus(true);
            await UpdateModalStatus(false, false);
        }

        #endregion OpenLeaderManager

        #endregion HandleManagers

        #region HandleRecordToDelete

        private void HandleRecordToDelete(VUserDtoResponse record)
        {
            managerToDelete = record.UserId;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, Translation["Yes"], Translation["No"], modalOrigin: "DeleteManagers");
        }

        #endregion HandleRecordToDelete

        #region HandleCopyToDelete

        private void HandleCopyToDelete(VUserDtoResponse record)
        {
            copyToDelete = record.UserId;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, Translation["Yes"], Translation["No"], modalOrigin: "DeleteManagers");
        }

        #endregion HandleCopyToDelete

        #region ResetFormAsync

        private void ResetFormAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            correspondenceCode = "";
            classCode = "";
            daysAlarm1 = "";
            daysAlarm2 = "";
            daysAlarm3 = "";
            percentAlarm1 = "20";
            percentAlarm2 = "60";
            percentAlarm3 = "20";
            daysOrHoursNumber = "";
            timeTypeSelected = "";
            timeTypeSelected = "";
            managerLeaderName = "";
            instructioLeadernCode = "";
            leaderManaegerUser = new();
            usersManageres = new();
            userCopies = new();
            pqrCheck = false;
            radicationCheck = false;
            requireResponseCheck = false;
            documentaryTypologyBehaviourId = 0;
            documentaryTypologyId = 0;
            EnableSaveButton();
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ResetFormAsync

        #region UpdateSelectedRecordAsync

        public async Task UpdateSelectedRecordAsync(DocumentaryTypologiesBehaviorsDtoResponse response)
        {
            _selectedRecord = response;

            IsEditForm = true;
            correspondenceCode = _selectedRecord.CorrespondenceType;
            classCode = _selectedRecord.ClassCode;

            #region LeaderManager

            var leader = _selectedRecord.TypologyManagers.Where(x => x.Leader).FirstOrDefault();
            if (leader != null)
            {
                managerLeaderName = leader.ManagerName;
                leaderManaegerUser = new() { UserId = leader.ManagerId, };
                instructioLeadernCode = leader.InstructionCode;
            }

            #endregion LeaderManager

            #region OtherManagers

            var otherManagers = _selectedRecord.TypologyManagers.Where(x => !x.Leader).ToList();
            if (otherManagers.Any())
            {
                foreach (var manager in otherManagers)
                {
                    usersManageres.Add(new()
                    {
                        UserId = manager.ManagerId,
                        FullName = manager.ManagerName
                    });
                }
            }

            #endregion OtherManagers

            #region UserCopies

            var userCopiesR = _selectedRecord.UserCopies;
            if (userCopiesR.Any())
            {
                foreach (var userCopy in userCopiesR)
                {
                    userCopies.Add(new()
                    {
                        UserId = userCopy.UserId,
                        FullName = userCopy.FullName
                    });
                }
            }

            #endregion UserCopies

            #region AsignBehaviorTypologies

            await GetBehaviorBag();

            var rad = _selectedRecord.BehaviorTypologies.Where(y => y.BehaviorBagId == behaviorBagList.Where((x) => x.BehaviorCode.Equals(requireResponseRadicationCode)).FirstOrDefault().BehaviorBagId).FirstOrDefault();
            radicationCheck = ( rad != null && rad.BehaviorValue.Contains("SI") );

            var pqr = _selectedRecord.BehaviorTypologies.Where(y => y.BehaviorBagId == behaviorBagList.Where((x) => x.BehaviorCode.Equals(pqrCode)).FirstOrDefault().BehaviorBagId).FirstOrDefault();
            pqrCheck = ( pqr != null && pqr.BehaviorValue.Contains("SI") );

            var reqResp = _selectedRecord.BehaviorTypologies.Where(y => y.BehaviorBagId == behaviorBagList.Where((x) => x.BehaviorCode.Equals(requireResponseCode)).FirstOrDefault().BehaviorBagId).FirstOrDefault();
            requireResponseCheck = ( reqResp != null && reqResp.BehaviorValue.Contains("SI") );

            var reqRespDH = _selectedRecord.BehaviorTypologies.Where(y => y.BehaviorBagId == behaviorBagList.Where((x) => x.BehaviorCode.Equals(requireResponseDHCode)).FirstOrDefault().BehaviorBagId).FirstOrDefault();
            timeTypeSelected = ( reqRespDH != null && reqRespDH.BehaviorValue.Equals("D") ) ? Translation["Days"] : Translation["Hours"];

            var reqRespDHValue = _selectedRecord.BehaviorTypologies.Where(y => y.BehaviorBagId == behaviorBagList.Where((x) => x.BehaviorCode.Equals(requireResponseDHValueCode)).FirstOrDefault().BehaviorBagId).FirstOrDefault();
            daysOrHoursNumber = ( reqRespDHValue != null ) ? reqRespDHValue.BehaviorValue : daysOrHoursNumber;

            var alm1 = _selectedRecord.BehaviorTypologies.Where(y => y.BehaviorBagId == behaviorBagList.Where((x) => x.BehaviorCode.Equals(alarm1Code)).FirstOrDefault().BehaviorBagId).FirstOrDefault();
            daysAlarm1 = ( alm1 != null ) ? alm1.BehaviorValue : daysAlarm1;

            var alm2 = _selectedRecord.BehaviorTypologies.Where(y => y.BehaviorBagId == behaviorBagList.Where((x) => x.BehaviorCode.Equals(alarm2Code)).FirstOrDefault().BehaviorBagId).FirstOrDefault();
            daysAlarm2 = ( alm2 != null ) ? alm2.BehaviorValue : daysAlarm2;

            var alm3 = _selectedRecord.BehaviorTypologies.Where(y => y.BehaviorBagId == behaviorBagList.Where((x) => x.BehaviorCode.Equals(alarm3Code)).FirstOrDefault().BehaviorBagId).FirstOrDefault();
            daysAlarm3 = ( alm3 != null ) ? alm3.BehaviorValue : daysAlarm3;

            var docTypo = _selectedRecord.BehaviorTypologies.Where(y => y.BehaviorBagId == behaviorBagList.Where((x) => x.BehaviorCode.Equals(documentalTypologyResponseCode)).FirstOrDefault().BehaviorBagId).FirstOrDefault();
            documentaryTypologyBehaviourId = ( docTypo == null ) ? 0 : int.Parse(docTypo.BehaviorValue);

            EnableSaveButton();

            #endregion AsignBehaviorTypologies
        }

        #endregion UpdateSelectedRecordAsync

        #region Modal

        #region UpdateModalStatus

        public async Task UpdateModalStatus(bool newValue, bool search = true)
        {
            modalStatus = newValue;
            if (search)
            {
                currentTab = 0;
                await FillCLassList();
                await FillCorrespondencelList();
                await FillInstructionlList();
                await GetDocTypologies();
                IsEditForm = false;
            }
            EnableSaveButton();

            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success && args.IsAccepted)
            {
                await OnStatusChanged.InvokeAsync(false);
            }
            if (notificationModal.Type == ModalType.Warning && args.IsAccepted && args.ModalOrigin != null && args.ModalOrigin.Equals("DeleteManagers"))
            {
                usersManageres = usersManageres.Where(item => item.UserId != managerToDelete).ToList();
            }
            if (notificationModal.Type == ModalType.Warning && args.IsAccepted && args.ModalOrigin != null && args.ModalOrigin.Equals("DeleteCopies"))
            {
                usersManageres = usersManageres.Where(item => item.UserId != copyToDelete).ToList();
            }

            StateHasChanged();
        }

        #endregion HandleModalNotiClose

        #region HandleModalClosed

        private async Task HandleModalClosed(bool status)
        {
            ResetFormAsync();
            modalStatus = status;
            StateHasChanged();
            await OnStatusChanged.InvokeAsync(false);
        }

        #endregion HandleModalClosed

        #endregion Modal

        #region EnableSaveButton

        public void EnableSaveButton()
        {
            var response = false;
            if (requireResponseCheck)
            {
                var num = ( !string.IsNullOrEmpty(daysOrHoursNumber) && !daysOrHoursNumber.Equals("0") ) ? int.Parse(daysOrHoursNumber) : 0;
                response = ( string.IsNullOrEmpty(timeTypeSelected) || num <= 0 ) ? true : false;
            }

            if (response || string.IsNullOrEmpty(correspondenceCode) || string.IsNullOrEmpty(classCode) || string.IsNullOrEmpty(instructioLeadernCode) || leaderManaegerUser.UserId <= 0)
            {
                saveIsDisable = true;
            }
            else
            {
                saveIsDisable = false;
            }

            StateHasChanged();
        }

        public void checkAsignValue(bool newValue)
        {
            requireResponseCheck = newValue;
            EnableSaveButton();
        }

        #endregion EnableSaveButton

        #endregion OthersMethods

        #endregion Methods
    }
}