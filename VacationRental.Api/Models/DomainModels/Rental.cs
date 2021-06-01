namespace VacationRental.Api.Models
{
    public class Rental
    {
        public Rental() { }
        public Rental(Rental prototype)
        {
            Id = prototype.Id;
            PreparationTimeInDays = prototype.PreparationTimeInDays;
        }
        public int Id { get; set; }
        public int PreparationTimeInDays { get; set; }
    }
}
