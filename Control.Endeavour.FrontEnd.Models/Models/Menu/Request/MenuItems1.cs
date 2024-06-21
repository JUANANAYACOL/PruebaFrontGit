namespace Control.Endeavour.Frontend.Client.Models.ComponentViews.Menu.Request
{
    public partial class MenuItems1
    {
        public int MenuItem1Id { get; set; }

        public int MenuId { get; set; }

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

        public virtual MenuModels Menu { get; set; } = null!;

        public virtual ICollection<MenuItems2> MenuItems2s { get; set; } = new List<MenuItems2>();

        public virtual View? View { get; set; }
    }

}
