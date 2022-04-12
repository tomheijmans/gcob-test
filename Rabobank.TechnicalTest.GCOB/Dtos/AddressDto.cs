namespace Rabobank.TechnicalTest.GCOB.Dtos
{
    public sealed class AddressDto
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public int CountryId { get; set; }
    }
}
