using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.User.Request
{
    public class UserProfilePictureDtoRequest
    {
        public string FileExt { get; set; } = null!;

        public string? FileName { get; set; }

        public byte[] DataFile { get; set; } = null!;
        public int UserId { get; set; }
    }
}