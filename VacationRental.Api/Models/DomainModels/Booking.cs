using System;

namespace VacationRental.Api.Models
{
    public class Booking
    {
        public Booking() { }
        public Booking(Booking prototype)
        {
            Id = prototype.Id;
            RentalId = prototype.RentalId;
            UnitId = prototype.UnitId;
            Start = prototype.Start;
            Nights = prototype.Nights;
        }
        public int Id { get; set; }
        public int RentalId { get; set; }
        public int UnitId { get; set; }
        public DateTime Start { get; set; }
        public int Nights { get; set; }
    }
}
