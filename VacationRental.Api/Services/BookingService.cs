using System;
using System.Linq;
using VacationRental.Api.Helpers;
using VacationRental.Api.Models;
using VacationRental.Api.Models.DomainModels;
using VacationRental.Api.Repositories;

namespace VacationRental.Api.Services
{
    public interface IBookingService
    {
        int MakeBooking(int rentalId, DateTime start, int nights);
    }

    public class BookingService : IBookingService
    {
        private IBookingRepository _bookings;
        private IUnitRepository _units;
        private IRentalRepository _rentals;

        public BookingService(IBookingRepository bookings, IUnitRepository units, IRentalRepository rentals)
        {
            _bookings = bookings;
            _units = units;
            _rentals = rentals;
        }

        public int MakeBooking(int rentalId, DateTime start, int nights)
        {
            var dateRange = new DateRange(start, nights);

            ////---------------------------------------------------------------------------------------
            ////--------Commented code handles bookings the way it was working before -----------------
            ////---------------------------------------------------------------------------------------
            //var rental = _rentals.Get(rentalId);
            //var areAllDatesTaken = dateRange.DatesInRange.Any(d => IsDateFullyTaken(rentalId, d));
            //if (areAllDatesTaken)
            //{
            //    throw new ApplicationException("Not available");
            //}
            //var booking = new Booking()
            //{
            //    RentalId = rentalId,
            //    Start = start,
            //    Nights = nights
            //};
            //var newBookingId = _bookings.Add(booking);
            ////---------------------------------------------------------------------------------------

            var unitForBooking = GetUnitFreeForBooking(rentalId, dateRange);
            if (unitForBooking == null)
            {
                throw new ApplicationException("Not available");
            }

            var booking = new Booking()
            {
                RentalId = rentalId,
                UnitId = unitForBooking.Id,
                Start = start,
                Nights = nights
            };
            var newBookingId = _bookings.Add(booking);
            return newBookingId;

        }

        private Unit GetUnitFreeForBooking(int rentalId, DateRange newBookingDateRange)
        {
            var rental = _rentals.Get(rentalId);
            var units = _units.GetByRental(rentalId);
            var preparationTimeInDays = rental.PreparationTimeInDays;
            var dateRangeWithPreparationDays = newBookingDateRange.Extend(preparationTimeInDays);
            foreach (var unit in units)
            {
                var bookings = _bookings.GetByUnit(unit.Id);
                var areBookingsColliding = bookings.Select(b => new DateRange(b).Extend(preparationTimeInDays))
                                                    .Any(dr => dr.IsColliding(dateRangeWithPreparationDays));
                if (!areBookingsColliding) return unit;
            }
            return null;
        }

        private bool IsDateFullyTaken(int rentalId, DateTime date)
        {
            var bookings = _bookings.GetByRental(rentalId);
            var countOfCollidingBookings = bookings.Select(b => new DateRange(b.Start, b.Nights))
                                                    .Where(dr => dr.IsDateInRange(date))
                                                    .Count();
            if (countOfCollidingBookings == bookings.Count()) return true;
            return false;
        }
    }
}
