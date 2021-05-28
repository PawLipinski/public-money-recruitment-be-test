using System;
using System.Collections.Generic;

namespace VacationRental.Api.Models
{
    public class CalendarDate
    {
        public DateTime Date { get; set; }
        public List<CalendarBookingViewModel> Bookings { get; set; }
    }
}
