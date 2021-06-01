using System;
using VacationRental.Api.Repositories;

namespace VacationRental.Api.Services
{
    public interface IChangeRentalService
    {
        bool AreChangesApplicable(int rentalId, int newUnitsNumber, int newPreparationTime);
        void ApplyChanges(int rentalId, int newUnitsNumber, int newPreparationTime);
    }

    public class ChangeRentalService : IChangeRentalService
    {
        private readonly IUnitRepository _unitRepositoryMock;
        private readonly IRentalRepository _rentalRepositoryMock;
        private readonly IBookingService _bookingServiceMock;

        private readonly IBookingRepository _bookingRepository;
        private readonly IRentalRepository _rentalRepository;
        //private readonly IUnitRepository _actualUnitRepository;
        private readonly IBookingService _bookingService;

        public ChangeRentalService(IBookingRepository bookings, IRentalRepository rentals, IBookingService bookingService)
        {
            var bookingRepositoryMock = new BookingRepository();
            _unitRepositoryMock = new UnitRepository();
            _rentalRepositoryMock = new RentalRepository(_unitRepositoryMock);
            _bookingServiceMock = new BookingService(bookingRepositoryMock, _unitRepositoryMock, _rentalRepositoryMock);

            _bookingRepository = bookings;
            _rentalRepository = rentals;
            _bookingService = bookingService;
        }

        public void ApplyChanges(int rentalId, int newUnitsNumber, int newPreparationTime)
        {
            var bookings = _bookingRepository.GetByRental(rentalId);
            bookings.ForEach(b => b.UnitId = 0);
            _rentalRepository.Update(rentalId, newUnitsNumber, newPreparationTime);
            _bookingRepository.RemoveByRental(rentalId);

            foreach (var booking in bookings)
            {
                _bookingService.MakeBooking(rentalId, booking.Start, booking.Nights);
            }
        }

        public bool AreChangesApplicable(int rentalId, int newUnitsNumber, int newPreparationTime)
        {
            _rentalRepositoryMock.Add(newUnitsNumber, newPreparationTime);
            var bookings = _bookingRepository.GetByRental(rentalId);
            var units = _unitRepositoryMock.GetByRental(rentalId);

            bookings.ForEach(b => b.UnitId = 0);
            foreach (var booking in bookings)
            {
                try
                {
                    _bookingServiceMock.MakeBooking(rentalId, booking.Start, booking.Nights);
                }
                catch (ApplicationException)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
