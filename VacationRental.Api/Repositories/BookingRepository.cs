using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models;

namespace VacationRental.Api.Repositories
{
    public interface IBookingRepository
    {
        Booking Get(int id);
        int Add(Booking booking);
        List<Booking> GetByRental(int rentalId);
        void RemoveByRental(int rentalId);
        List<Booking> GetByUnit(int unitId);
    }

    public class BookingRepository : IBookingRepository
    {
        private readonly List<Booking> _bookings = new List<Booking>();

        public int Add(Booking booking)
        {
            var newBookingId = _bookings.Any() ? _bookings.Max(b => b.Id) + 1 : 1;
            booking.Id = newBookingId;
            _bookings.Add(booking);
            return newBookingId;
        }

        public Booking Get(int id)
        {
            return new Booking(_bookings.Where(b => b.Id == id)
                            .FirstOrDefault());
        }

        public List<Booking> GetByRental(int rentalId)
        {
            return _bookings.Where(b => b.RentalId == rentalId)
                            .Select(b => new Booking(b))
                            .ToList();
        }

        public List<Booking> GetByUnit(int unitId)
        {
            return _bookings.Where(b => b.UnitId == unitId)
                            .Select(b => new Booking(b))
                            .ToList();
        }

        public void RemoveByRental(int rentalId)
        {
            _bookings.RemoveAll(u => u.RentalId == rentalId);
        }
    }
}
