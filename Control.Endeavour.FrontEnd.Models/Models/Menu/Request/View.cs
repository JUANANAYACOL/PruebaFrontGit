using Control.Endeavour.FrontEnd.Models.Models.Menu.Request;

namespace Control.Endeavour.Frontend.Client.Models.ComponentViews.Menu.Request
{
    public partial class View
    {
        public int ViewId { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool Active { get; set; }
        public List<ViewParameters> ViewParameters { get; set; } = new List<ViewParameters>();

        public string? CreateUser { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public long? CreateCorrelationId { get; set; }

        public long? UpdateCorrelationId { get; set; }

        public virtual ICollection<MenuItems1> MenuItems1s { get; set; } = new List<MenuItems1>();

        public virtual ICollection<MenuItems2> MenuItems2s { get; set; } = new List<MenuItems2>();

        public virtual ICollection<MenuItems3> MenuItems3s { get; set; } = new List<MenuItems3>();
    }

}
