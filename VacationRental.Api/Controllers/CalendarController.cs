using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarWithPreparationsBuilder _calendarBuilder;
        private readonly IUnitRepository _units;
        private readonly IValidator _validator;

        public CalendarController(ICalendarWithPreparationsBuilder calendarBuilder, IUnitRepository units, IValidator validator)
        {
            _calendarBuilder = calendarBuilder;
            _units = units;
            _validator = validator;
        }

        [HttpGet]
        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            _validator.ValidateRentalExistence(rentalId);
            _validator.ValidateNightsNumber(nights);

            var calendar = _calendarBuilder.Build(rentalId, start, nights);

            return GetAsCalendarViewModel(calendar);
        }

        private CalendarViewModel GetAsCalendarViewModel(Calendar calendar)
        {
            var calendarViewModel = new CalendarViewModel();
            calendarViewModel.RentalId = calendar.RentalId;
            calendarViewModel.Dates = new List<CalendarDateViewModel>();
            foreach (var date in calendar.Dates)
            {
                var bookingViewModels = new List<CalendarBookingViewModel>();
                bookingViewModels = date.Bookings.Select(b => new CalendarBookingViewModel()
                {
                    Id = b.Id,
                    Unit = _units.GetUnitPerRentalId(b.UnitId)
                }).ToList();

                var preparations = new List<CalendarPreparationTimeViewModel>();
                preparations = date.PreparationTimes.Select(p => new CalendarPreparationTimeViewModel()
                {
                    Unit = _units.GetUnitPerRentalId(p.UnitId)
                }).ToList();
                    
                calendarViewModel.Dates.Add(new CalendarDateViewModel()
                {
                    Date = date.Date,
                    Bookings = bookingViewModels,
                    PreparationTimes = preparations
                });
            }
            return calendarViewModel;
        }
    }
}
