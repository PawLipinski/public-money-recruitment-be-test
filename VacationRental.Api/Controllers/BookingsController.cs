using Microsoft.AspNetCore.Mvc;
using System;
using VacationRental.Api.Helpers;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IBookingRepository _bookings;
        private readonly IValidator _validator;

        public BookingsController(IBookingService bookingService, IBookingRepository bookings, IValidator validator)
        {
            _bookingService = bookingService;
            _bookings = bookings;
            _validator = validator;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public BookingViewModel Get(int bookingId)
        {
            var booking = _bookings.Get(bookingId);
            return booking != null ?
                new BookingViewModel(booking):
                throw new ApplicationException("Booking not found");
        }

        [HttpPost]
        public ResourceIdViewModel Post(BookingBindingModel model)
        {
            _validator.ValidateNightsNumber(model.Nights);
            _validator.ValidateRentalExistence(model.RentalId);

            var bookingId = _bookingService.MakeBooking(model.RentalId, model.Start, model.Nights);

            return new ResourceIdViewModel() { Id = bookingId };
        }
    }
}
