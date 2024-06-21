using Control.Endeavour.FrontEnd.Models.Models.Administration.VBehaviorTypology.Response;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.VDocumentaryTypology.Response
{
    public class VDocumentaryTypologyDtoResponse
    {
        public int DocumentaryTypologyBehaviorId { get; set; }

        public int? DocumentaryTypologyId { get; set; }

        public int? DocumentaryTypologyBagId { get; set; }

        public string ClassCode { get; set; } = null!;

        public string? Class { get; set; }

        public string CorrespondenceTypeCode { get; set; } = null!;

        public string? CorrespondenceType { get; set; }

        public string? TypologyName { get; set; }

        public string? TypologyDescription { get; set; }

        public int? SubSeriesId { get; set; }

        public string? SubSeriesCode { get; set; }

        public string? SubSeriesName { get; set; }

        public string? SubSeriesDescription { get; set; }

        public int? SeriesId { get; set; }

        public string? SeriesCode { get; set; }

        public string? SeriesName { get; set; }

        public string? SeriesDescription { get; set; }

        public int ProductionOfficeId { get; set; }

        public string ProductionOfficeCode { get; set; } = null!;

        public string? ProductionOfficeName { get; set; }

        public string? ProductionOfficeDescription { get; set; }

        public int AdministrativeUnitId { get; set; }

        public string AdministrativeUnitCode { get; set; } = null!;

        public string? AdministrativeUnitName { get; set; }

        public string? AdministrativeUnitDescription { get; set; }

        public int DocumentalVersionId { get; set; }

        public string DocumentalVersionCode { get; set; } = null!;

        public string? DocumentalVersionName { get; set; }

        public string? DocumentalVersionDescription { get; set; }

        public int? LeadManagerId { get; set; }

        public string? LeadManagerUuId { get; set; }

        public string? LmfirstName { get; set; }

        public string? LmmiddleName { get; set; }

        public string? LmlastName { get; set; }

        public string LmfullName { get; set; } = null!;

        public int LmproductionOfficeId { get; set; }

        public string LmproductionOfficeCode { get; set; } = null!;

        public string? LmproductionOfficeName { get; set; }

        public string? LmproductionOfficeDescription { get; set; }

        public int LmadministrativeUnitId { get; set; }

        public string LmadministrativeUnitCode { get; set; } = null!;

        public string? LmadministrativeUnitName { get; set; }

        public string? LmadministrativeUnitDescription { get; set; }

        public string? LmchargeCode { get; set; }

        public string? Lmcharge { get; set; }

        public List<VBehaviorTypologyDtoResponse>? VBehaviors { get; set; }

        public bool Selected { get; set; } = false;
        public string CreateUser { get; set; } = null;
        public string? CreateDate { get; set; }
    }
}