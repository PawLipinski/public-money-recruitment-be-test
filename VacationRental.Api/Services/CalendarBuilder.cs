using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Helpers;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;

namespace VacationRental.Api.Services
{
    public interface ICalendarBuilder
    {
        Calendar Build(int rentalId, DateTime start, int nights);
    }

    public class CalendarBuilder : ICalendarBuilder
    {
        private IBookingRepository _bookings;
        private IRentalRepository _rentals;
        private IUnitRepository _units;
        public CalendarBuilder(IBookingRepository bookings, IRentalRepository rentals, IUnitRepository units)
        {
            _bookings = bookings;
            _rentals = rentals;
            _units = units;
        }

        public Calendar Build(int rentalId, DateTime start, int nights)
        {
            var rental = _rentals.Get(rentalId);
            var preparationTimeInDays = rental.PreparationTimeInDays;
            var bookings = _bookings.GetByRental(rentalId);
            var dateRange = new DateRange(start, nights);

            return new Calendar()
            {
                RentalId = rentalId,
                Dates = GetCalendarDates(bookings, dateRange, preparationTimeInDays)
            };
        }


        private List<CalendarDate> GetCalendarDates(List<Booking> bookings, DateRange datesRange, int preparationTimeInDays)
        {
            var result = new List<CalendarDate>();
            datesRange.DatesInRange.ForEach(
                date => result.Add(
                    GetParticularCalendarDate(date, bookings, preparationTimeInDays)
                    )
                );
            return result;
        }

        private CalendarDate GetParticularCalendarDate(DateTime date, List<Booking> bookings, int preparationTimeInDays)
        {
            var calendarDate = new CalendarDate
            {
                Date = date,
                Bookings = new List<Booking>(),
            };
            var applicableBookings = bookings.Where(b => IsBookingOnSpecifiedDate(b, date.Date));
            calendarDate.Bookings.AddRange(applicableBookings);
            return calendarDate;
        }

        private bool IsBookingOnSpecifiedDate(Booking booking, DateTime date)
        {
            var dateRange = new DateRange(booking);
            return dateRange.IsDateInRange(date);
        }



    }
}
