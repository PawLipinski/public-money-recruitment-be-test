using System;
using System.Collections.Generic;
using System.Linq;

namespace VacationRental.Api.Helpers
{
    public class DateRange
    {
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;

        public List<DateTime> DatesInRange { get; private set; }

        public DateRange(DateTime startDate, int nights)
        {
            _startDate = startDate;
            _endDate = startDate.AddDays(nights - 1);
            InitiateDatesInRange(startDate, nights);
        }

        public bool IsDateInRange(DateTime date)
        {
            return date >= _startDate && date <= _endDate;
        }

        public bool IsColliding(DateRange otherDateRange)
        {
            if (otherDateRange.DatesInRange.Any(d => IsDateInRange(d)))
            {
                return true;
            }
            return false;
        }

        private void InitiateDatesInRange(DateTime startDate, int nights)
        {
            DatesInRange = new List<DateTime>();
            for (int i = 0; i < nights; i++)
            {
                DatesInRange.Add(startDate.AddDays(i));
            }
        }
    }
}
