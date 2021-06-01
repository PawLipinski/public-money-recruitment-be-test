using System;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services;
using Xunit;

namespace VacationRental.Api.Tests
{
    public class BookingServiceTests
    {
        private IBookingRepository _bookings;
        private IRentalRepository _rentals;
        private IUnitRepository _units;
        private IBookingService _bookingService;

        [Fact]
        public void AddBookingOnEmpty()
        {
            PrepareServices();
            var startDate = DateTime.Now.Date;
            var rentalId = _rentals.Add(2, 2);
            var bookingId = _bookingService.MakeBooking(rentalId, startDate, 3);
            var booking = _bookings.Get(bookingId);
            Assert.True(booking.Start == startDate);
        }

        [Fact]
        public void AddBookingOnDateCollision()
        {
            PrepareServices();
            var startDate = DateTime.Now.Date;
            var rentalId = _rentals.Add(1, 2);
            _bookingService.MakeBooking(rentalId, startDate, 3);
            Assert.Throws<ApplicationException>(() => _bookingService.MakeBooking(rentalId, startDate, 3));
        }

        [Fact]
        public void AddBookingOnPreparationCollision()
        {
            PrepareServices();
            var firstBookingStartDate = DateTime.Now.Date;
            var secondBookingStartDate = firstBookingStartDate.AddDays(2);
            var rentalId = _rentals.Add(1, 2);
            _bookingService.MakeBooking(rentalId, firstBookingStartDate, 1);
            Assert.Throws<ApplicationException>(() => _bookingService.MakeBooking(rentalId, secondBookingStartDate, 1));
        }


        [Fact]
        public void AddBookingOnBookingPreparationOverlappingOtherBooking()
        {
            PrepareServices();
            var firstBookingStartDate = DateTime.Now.Date.AddDays(2);
            var secondBookingStartDate = DateTime.Now.Date;
            var rentalId = _rentals.Add(1, 2);
            _bookingService.MakeBooking(rentalId, firstBookingStartDate, 1);
            Assert.Throws<ApplicationException>(() => _bookingService.MakeBooking(rentalId, secondBookingStartDate, 1));
        }

        private void PrepareServices()
        {
            _bookings = new BookingRepository();
            _units = new UnitRepository();
            _rentals = new RentalRepository(_units);
            _bookingService = new BookingService(_bookings, _units, _rentals);
        }
    }
}
