using System;
using System.Collections.Generic;
using VacationRental.Api.Helpers;
using VacationRental.Api.Models;
using VacationRental.Api.Models.DomainModels;
using VacationRental.Api.Repositories;

namespace VacationRental.Api.Services
{
    public interface ICalendarWithPreparationsBuilder
    {
        Calendar Build(int rentalId, DateTime start, int nights);
    }

    public class CalendarWithPreparationsBuilder : ICalendarWithPreparationsBuilder
    {
        private readonly ICalendarBuilder _calendarBuilder;
        private readonly IRentalRepository _rentals;
        private readonly IBookingRepository _bookings;
        
        public CalendarWithPreparationsBuilder(ICalendarBuilder calendarBuilder, IRentalRepository rentals, IBookingRepository bookings)
        {
            _calendarBuilder = calendarBuilder;
            _rentals = rentals;
            _bookings = bookings;
        }

        public Calendar Build(int rentalId, DateTime start, int nights)
        {
            var rental = _rentals.Get(rentalId);
            var bookings = _bookings.GetByRental(rentalId);
            var preparationTime = rental.PreparationTimeInDays;
            var calendar = _calendarBuilder.Build(rentalId, start, nights);
            foreach (var date in calendar.Dates)
            {
                date.PreparationTimes = GetPreparationsOnDate(bookings, date.Date, preparationTime);
            }
            return calendar;
        }

        private List<Preparation> GetPreparationsOnDate(List<Booking> bookings, DateTime date, int preparationTimeInDays)
        {
            var preparations = new List<Preparation>();
            foreach (var booking in bookings)
            {
                var preparation = GetPreparationPerBooking(booking, date, preparationTimeInDays);
                if (preparation != null) preparations.Add(preparation);
            }
            return preparations;
        }

        private Preparation GetPreparationPerBooking(Booking booking, DateTime date, int preparationTimeInDays)
        {
            var bookingDateRange = new DateRange(booking);
            var bookingDateRangeWithPreparations = new DateRange(bookingDateRange).Extend(preparationTimeInDays);
            if (!bookingDateRange.IsDateInRange(date) && bookingDateRangeWithPreparations.IsDateInRange(date))
            {
                return new Preparation()
                {
                    UnitId = booking.UnitId
                };
            }
            return null;
        }
    }
}
