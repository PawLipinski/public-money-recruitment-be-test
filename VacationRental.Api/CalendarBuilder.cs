using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Helpers;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;

namespace VacationRental.Api
{
    public interface ICalendarBuilder
    {
        Calendar Build(int rentalId, DateTime start, int nights);
    }

    public class CalendarBuilder : ICalendarBuilder
    {
        private IBookingRepository _bookings;
        public CalendarBuilder(IBookingRepository bookings)
        {
            _bookings = bookings;
        }

        public Calendar Build(int rentalId, DateTime start, int nights)
        {
            var bookings = _bookings.GetByRental(rentalId);
            var dateRange = new DateRange(start, nights);

            return new Calendar()
            {
                RentalId = rentalId,
                Dates = GetCalendarDates(bookings, dateRange)
            };
        }


        private List<CalendarDate> GetCalendarDates(List<Booking> bookings, DateRange datesRange)
        {
            var result = new List<CalendarDate>();
            datesRange.DatesInRange.ForEach(
                date => result.Add(
                    GetParticularCalendarDate(date, bookings)
                    )
                );
            return result;
        }

        private CalendarDate GetParticularCalendarDate(DateTime date, List<Booking> bookings)
        {
            var calendarDate = new CalendarDate
            {
                Date = date,
                Bookings = new List<CalendarBookingViewModel>()
            };
            var applicableBookings = bookings.Where(b => IsBookingOnSpecifiedDate(b, date.Date));
            var calendarBookingViewModels = applicableBookings.Select(ab => new CalendarBookingViewModel() { Id = ab.Id });
            calendarDate.Bookings.AddRange(calendarBookingViewModels);
            return calendarDate;
        }

        private bool IsBookingOnSpecifiedDate(Booking booking, DateTime date)
        {
            var dateRange = new DateRange(booking.Start, booking.Nights);
            return dateRange.IsDateInRange(date);
        }
    }
}
