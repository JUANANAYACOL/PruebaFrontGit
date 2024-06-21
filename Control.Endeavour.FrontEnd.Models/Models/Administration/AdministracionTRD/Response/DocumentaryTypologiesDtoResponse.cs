﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response
{
    public class DocumentaryTypologiesDtoResponse
    {
        public int DocumentaryTypologyId { get; set; }
        public int DocumentaryTypologyBagId { get; set; }
        public int SeriesId { get; set; }
        public int? SubSeriesId { get; set; }
        public bool ActiveState { get; set; }
        public string? SecurityLevel { get; set; }
        public string CreateUser { get; set; } = null!;
        public string UpdateUser { get; set; } = null!;
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
