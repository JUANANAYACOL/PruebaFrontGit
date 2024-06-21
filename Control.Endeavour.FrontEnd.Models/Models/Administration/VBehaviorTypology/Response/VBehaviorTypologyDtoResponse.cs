namespace Control.Endeavour.FrontEnd.Models.Models.Administration.VBehaviorTypology.Response
{
    public class VBehaviorTypologyDtoResponse
    {
        public int DocumentaryTypologyId { get; set; }

        public int DocumentaryTypologyBehaviorId { get; set; }

        public string BehaviorCode { get; set; } = null!;

        public string BehaviorName { get; set; } = null!;

        public string BehaviorValue { get; set; } = null!;
    }
}