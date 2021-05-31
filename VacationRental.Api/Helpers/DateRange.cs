using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models;

namespace VacationRental.Api.Helpers
{
    public class DateRange
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public List<DateTime> DatesInRange { get; private set; }

        public DateRange(DateTime startDate, int nights)
        {
            StartDate = startDate;
            var actualDays = nights - 1;
            EndDate = startDate.AddDays(actualDays);
            InitiateDatesInRange();
        }

        public DateRange(Booking booking) : this(booking.Start, booking.Nights) { }

        public DateRange(DateRange prototype)
        {
            StartDate = prototype.StartDate;
            EndDate = prototype.EndDate;
            DatesInRange = prototype.DatesInRange;
        }

        public bool IsDateInRange(DateTime date)
        {
            return date >= StartDate && date <= EndDate;
        }

        public bool IsColliding(DateRange otherDateRange)
        {
            if (otherDateRange.DatesInRange.Any(d => IsDateInRange(d)))
            {
                return true;
            }
            return false;
        }

        public DateRange Extend(int days)
        {
            var dateRangeToReturn = new DateRange(this);
            dateRangeToReturn.EndDate = dateRangeToReturn.EndDate.AddDays(days);
            dateRangeToReturn.InitiateDatesInRange();
            return dateRangeToReturn;
        }

        private void InitiateDatesInRange()
        {
            DatesInRange = new List<DateTime>();
            var nights = (EndDate - StartDate).TotalDays;
            DatesInRange.Add(StartDate);
            for (int i = 1; i < nights; i++)
            {
                DatesInRange.Add(StartDate.AddDays(i));
            }
            DatesInRange.Add(EndDate);
        }
    }
}
