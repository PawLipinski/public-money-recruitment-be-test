using System;
using VacationRental.Api.Repositories;

namespace VacationRental.Api.Helpers
{
    public interface IValidator
    {
        void ValidateRentalExistence(int rentalId);
        void ValidateNightsNumber(int nightsNumber);
    }

    public class Validator : IValidator
    {
        private IRentalRepository _rentals;
        public Validator(IRentalRepository rentals)
        {
            _rentals = rentals;
        }

        public void ValidateNightsNumber(int nightsNumber)
        {
            if (nightsNumber < 0) throw new ApplicationException("Nights must be positive");
        }

        public void ValidateRentalExistence(int rentalId)
        {
            var rental = _rentals.Get(rentalId);
            if (rental == null) throw new ApplicationException("Rental not found");
        }
    }
}
