using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response
{
    public class GraphDocumentDtoResponse
    {
        public string context { get; set; }
        public List<Value> value { get; set; }
    }

    public class CreatedBy
    {
        public User user { get; set; }
    }

    public class File
    {
        public Hashes hashes { get; set; }
        public string mimeType { get; set; }
    }

    public class FileSystemInfo
    {
        public DateTime createdDateTime { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
    }

    public class Hashes
    {
        public string quickXorHash { get; set; }
    }

    public class LastModifiedBy
    {
        public User user { get; set; }
    }

    public class ParentReference
    {
        public string driveType { get; set; }
        public string driveId { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string siteId { get; set; }
    }

    public class User
    {
        public string email { get; set; }
        public string id { get; set; }
        public string displayName { get; set; }
    }

    public class Value
    {
        public string microsoftgraphdownloadUrl { get; set; }
        public string microsoftgraphDecorator { get; set; }
        public CreatedBy createdBy { get; set; }
        public DateTime createdDateTime { get; set; }
        public string eTag { get; set; }
        public string id { get; set; }
        public LastModifiedBy lastModifiedBy { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public ParentReference parentReference { get; set; }
        public string webUrl { get; set; }
        public string cTag { get; set; }
        public File file { get; set; }
        public FileSystemInfo fileSystemInfo { get; set; }
    }
}
