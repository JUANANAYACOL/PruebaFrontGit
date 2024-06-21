namespace Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response
{
    public class AddressDtoResponse
    {

        public int CountryId { get; set; }

        public int StateId { get; set; }

        public int CityId { get; set; }

        public string? StType { get; set; }

        public string? StNumber { get; set; }

        public string? StLetter { get; set; }

        public bool? StBis { get; set; }

        public string? StComplement { get; set; }

        public string? StCardinality { get; set; }

        public string? CrType { get; set; }

        public string? CrNumber { get; set; }

        public string? CrLetter { get; set; }

        public bool? CrBis { get; set; }

        public string? CrComplement { get; set; }

        public string? CrCardinality { get; set; }

        public string? HouseType { get; set; }

        public string? HouseClass { get; set; }

        public string? HouseNumber { get; set; }
    }
}