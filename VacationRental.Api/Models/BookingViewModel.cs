using System;

namespace VacationRental.Api.Models
{
    public class BookingViewModel
    {
        public BookingViewModel() {}
        public BookingViewModel(Booking booking)
        {
            Id = booking.Id;
            RentalId = booking.RentalId;
            Start = booking.Start;
            Nights = booking.Nights;
        }
        public int Id { get; set; }
        public int RentalId { get; set; }
        public DateTime Start { get; set; }
        public int Nights { get; set; }
    }
}
