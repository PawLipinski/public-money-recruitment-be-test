using System;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services;
using Xunit;

namespace VacationRental.Api.Tests
{
    public class ChangeRentalServiceTests
    {
        private IRentalRepository _rentals;
        private IUnitRepository _units;
        private IBookingRepository _bookings;
        private IBookingService _bookingService;

        [Fact]
        public void CheckAreChangesApplicableMethod_TooLittleUnits()
        {
            PrepareServices();
            var rentalId = _rentals.Add(2, 0);
            _bookingService.MakeBooking(rentalId, DateTime.Now.Date, 3);
            _bookingService.MakeBooking(rentalId, DateTime.Now.Date, 3);
            var changeRentalService = new ChangeRentalService(_bookings, _rentals, _bookingService);

            var result = changeRentalService.AreChangesApplicable(rentalId, 1, 0);

            Assert.Equal(false, result);
        }

        [Fact]
        public void AreChangesApplicableMethod_OverlappingPreparationTime()
        {
            PrepareServices();
            var rentalId = _rentals.Add(1, 0);
            _bookingService.MakeBooking(rentalId, DateTime.Now.Date, 3);
            _bookingService.MakeBooking(rentalId, DateTime.Now.Date.AddDays(3), 3);
            var changeRentalService = new ChangeRentalService(_bookings, _rentals, _bookingService);

            var result = changeRentalService.AreChangesApplicable(rentalId, 1, 2);

            Assert.Equal(false, result);
        }

        [Fact]
        public void MakeSureAreChangesApplicableDoesNotModifyRepositories()
        {
            PrepareServices();
            var rentalId = _rentals.Add(1, 0);
            _bookingService.MakeBooking(rentalId, DateTime.Now.Date, 3);
            _bookingService.MakeBooking(rentalId, DateTime.Now.Date.AddDays(3), 3);
            var changeRentalService = new ChangeRentalService(_bookings, _rentals, _bookingService);

            changeRentalService.AreChangesApplicable(rentalId, 1, 2);
            var rentals = _rentals.GetAll();
            var bookings = _bookings.GetByRental(rentalId);

            Assert.Equal(1, rentals.Count);
            Assert.Equal(2, bookings.Count);
        }

        [Fact]
        public void ApplyRentalChangesCheck()
        {
            PrepareServices();
            var rentalId = _rentals.Add(3, 0);
            _bookingService.MakeBooking(rentalId, DateTime.Now.Date, 3);
            _bookingService.MakeBooking(rentalId, DateTime.Now.Date, 3);
            _bookingService.MakeBooking(rentalId, DateTime.Now.Date.AddDays(3), 2);
            var changeRentalService = new ChangeRentalService(_bookings, _rentals, _bookingService);

            changeRentalService.ApplyChanges(rentalId, 2, 0);
            var units = _units.GetByRental(rentalId);
            var bookings = _bookings.GetByRental(rentalId);

            Assert.Equal(2, units.Count);
            Assert.Equal(3, bookings.Count);
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
