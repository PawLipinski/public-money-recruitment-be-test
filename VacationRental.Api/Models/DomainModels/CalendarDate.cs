using System;
using System.Collections.Generic;
using VacationRental.Api.Models.DomainModels;

namespace VacationRental.Api.Models
{
    public class CalendarDate
    {
        public DateTime Date { get; set; }
        public List<Booking> Bookings { get; set; }
        public List<Preparation> PreparationTimes { get; set; }
    }
}
