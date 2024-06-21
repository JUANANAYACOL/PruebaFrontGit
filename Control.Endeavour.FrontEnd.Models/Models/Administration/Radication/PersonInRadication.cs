namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Radication
{
    public class PersonInRadication
    {
        public string TypeOfPersonInRadication { get; set; } = null!;
        public int Id { get; set; }
        public int CompanyId { get; set; }

        public int AddressId { get; set; }

        public string FullName { get; set; } = null!;
        public string IdentificationType { get; set; } = null!;
        public string IdentificationTypeName { get; set; } = null!;

        public string IdentificationNumber { get; set; } = null!;

        public int AdministrativeUnitId { get; set; }

        public string AdministrativeUnitCode { get; set; } = null!;

        public string AdministrativeUnitName { get; set; } = null!;

        public int ProductionOfficeId { get; set; }

        public string ProductionOfficeCode { get; set; } = null!;

        public string ProductionOfficeName { get; set; } = null!;

        public string Email1 { get; set; } = null!;

        public string Email2 { get; set; } = null!;
        public string ChargeCode { get; set; } = null!;

        public string Charge { get; set; } = null!;

        public string Phone1 { get; set; } = null!;

        public string Phone2 { get; set; } = null!;
    }
}