namespace Control.Endeavour.Frontend.Client.Models.ComponentViews.Menu.Request
{
    public class MenuItems3
    {
        public int MenuItem3Id { get; set; }

        public int MenuItem2Id { get; set; }

        public int? ViewId { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public bool Active { get; set; }

        public string? CreateUser { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public long? CreateCorrelationId { get; set; }

        public long? UpdateCorrelationId { get; set; }

        public virtual MenuItems2 MenuItem2 { get; set; } = null!;

        public virtual View? View { get; set; }
    }

}
