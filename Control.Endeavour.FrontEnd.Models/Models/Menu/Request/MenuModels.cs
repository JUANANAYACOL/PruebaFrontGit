namespace Control.Endeavour.Frontend.Client.Models.ComponentViews.Menu.Request
{
    public partial class MenuModels
    {
        public int MenuId { get; set; }

        public int MenuType { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Icon { get; set; } = null!;

        public string ToolTip { get; set; } = null!;

        public bool Active { get; set; }

        public string? CreateUser { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public long? CreateCorrelationId { get; set; }

        public long? UpdateCorrelationId { get; set; }

        public virtual ICollection<MenuItems1> MenuItems1s { get; set; } = new List<MenuItems1>();
    }

}
