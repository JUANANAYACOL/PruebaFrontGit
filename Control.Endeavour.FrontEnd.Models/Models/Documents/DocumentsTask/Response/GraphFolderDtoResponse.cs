using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response
{
    public class GraphFolderDtoResponse
    {
        public string context { get; set; }
        public List<information> value { get; set; }
    }

    public class information
    {
        public string name { get; set; }
        public reference parentReference { get; set; }
        public string cTag { get; set; }
    }

    public class creation
    {
        public application application { get; set; }
        public user user { get; set; }
    }

    public class application
    {
        public string id { get; set; }
        public string displayName { get; set; }
    }

    public class user
    {
        public string email { get; set; }
        public string id { get; set; }
        public string displayName { get; set; }
    }

    public class reference
    {
        public string driveType { get; set; }
        public string driveId { get; set; }
    }

    public class systemInfo
    {
        public string createdDateTime { get; set; }
        public string lastModifiedDateTime { get; set; }
    }

    public class folder
    {
        public int childCount { get; set; }

    }

    public class specialFolder
    {
        public string name { get; set; }
    }
}
