using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaData.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaField.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaField.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.MetaData
{
    public partial class MetaDataRelationModal

    {
        #region Variables

        #region Inject

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }


        #endregion Inject

        #region Parameters

        [Parameter]
        public bool modalStatus { get; set; }

        [Parameter] public EventCallback<MyEventArgs<MetaDataRelationDtoRequest>> OnMetaDataSelected { get; set; } = new();
        [Parameter] public EventCallback<MyEventArgs<List<MetaDataRelationDtoRequest>>> OnMetaDataUpdated { get; set; } = new();

        #endregion Parameters

        #region Models

        private MetaFieldsByFilterTotalDtoRequest metaFieldFilter { get; set; } = new();
        private List<MetaDataRelationDtoRequest> metaDataList { get; set; } = new();

        #endregion Models

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private async Task HandleModalClosed(bool status)
        {
            modalStatus = status;

            var data = new MyEventArgs<List<MetaDataRelationDtoRequest>>()
            {
                Data = metaDataList,
                ModalStatus = status
            };

            await OnMetaDataUpdated.InvokeAsync(data);
            StateHasChanged();
        }

        #endregion HandleMethods

        #region OthersMethods

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;

            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region SearchByDocumentaryTypology

        public async Task SearchByDocumentaryTypology(int id)
        {
            metaFieldFilter.DocumentaryTypologyBagId = id;
            metaFieldFilter.ActiveState = true;

            var responseApi = await HttpClient!.PostAsJsonAsync("paramsdocumentary/MetaFields/ByFilterTotal", metaFieldFilter);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<MetaDataRelationDtoRequest>>>();
            if (deserializeResponse!.Succeeded)
            {
                metaDataList = deserializeResponse.Data!;
            }
            else
            {
                metaDataList = new();
            }
        }

        #endregion SearchByDocumentaryTypology

        #region MetaFieldSelected

        public async Task MetaFieldSelected(MetaDataRelationDtoRequest metaField)
        {
            var eventArgs = new MyEventArgs<MetaDataRelationDtoRequest>
            {
                Data = metaField,
                ModalStatus = true
            };
            await OnMetaDataSelected.InvokeAsync(eventArgs);
        }

        #endregion MetaFieldSelected

        public void existingMetaDataRelations(List<MetaDataRelationDtoRequest> request)
        {
            if (request != null)
            {
                metaDataList = request;
            }
        }

        #endregion OthersMethods

        #endregion Methods
    }
}