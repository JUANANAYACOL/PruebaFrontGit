using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Control.Endeavour.FrontEnd.Components.Components.Modals
{
    public partial class GenericModalComponent : ComponentBase
    {
        #region Parameters
        [Parameter]
        public string Title { get; set; } = string.Empty;

        [Parameter]
        public bool IsVisible { get; set; } = false;

        [Parameter]
        public string Width { get; set; } = "50%";

        [Parameter]
        public string CancelText { get; set; } = "Cancel";

        [Parameter]
        public string ConfirmText { get; set; } = "Ok";

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        [Parameter]
        public EventCallback<bool> OnModalClosed { get; set; }

        [Parameter] public bool IsModalExpand { get; set; } = true;
        #endregion

        private string regularHeight = "fit-content";
        private string expandedHeight = "100vh";
        private string expandedWidth = "100%";
        private string regularWidth = string.Empty;
        private string modalExpanded = string.Empty;
        private string regularMaxHeight = "90%";
        private string classExpand = string.Empty;
        private bool IsExpanded = false;

        #region Methods
        protected override async Task OnInitializedAsync()
        {
            regularWidth = Width;
            if(!IsModalExpand)
            {
                modalExpanded = "d-md-none";
            }

        }
        private async Task CloseModal()
        {

            await OnModalClosed.InvokeAsync(false);
            regularHeight = "fit-content";
            regularWidth = Width;
            IsExpanded = false;
            classExpand = string.Empty;
            regularMaxHeight = "90%";
        }

        private void ExpandModal()
        {
            if (IsExpanded)
            {
                regularHeight = "fit-content";
                regularWidth = Width;
                IsExpanded = false;
                classExpand = string.Empty;
                regularMaxHeight = "90%";
            }
            else
            {
                regularMaxHeight = "100%";
                regularWidth = expandedWidth;
                regularHeight = expandedHeight;
                classExpand = "expanded-modal";
                IsExpanded = true;
            }
            
        }
        #endregion
    }
}
