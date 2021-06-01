namespace VacationRental.Api.Models.DomainModels
{
    public class Unit
    {
        public Unit() {}
        public Unit(Unit prototype)
        {
            RentalId = prototype.RentalId;
            Id = prototype.Id;
        }
        public int RentalId { get; set; }
        public int Id { get; set; }
    }
}
