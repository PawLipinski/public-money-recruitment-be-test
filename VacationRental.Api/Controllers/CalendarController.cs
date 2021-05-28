using Microsoft.AspNetCore.Mvc;
using System;
using VacationRental.Api.Helpers;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarBuilder _calendarBuilder;
        private readonly IValidator _validator;

        public CalendarController(ICalendarBuilder calendarBuilder, IValidator validator)
        {
            _calendarBuilder = calendarBuilder;
            _validator = validator;
        }

        [HttpGet]
        public Calendar Get(int rentalId, DateTime start, int nights)
        {
            _validator.ValidateRentalExistence(rentalId);
            _validator.ValidateNightsNumber(nights);

            return _calendarBuilder.Build(rentalId, start, nights);
        }
    }
}
