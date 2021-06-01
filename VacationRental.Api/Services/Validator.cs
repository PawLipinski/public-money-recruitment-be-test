using System;
using VacationRental.Api.Repositories;

namespace VacationRental.Api.Services
{
    public interface IValidator
    {
        void ValidateRentalExistence(int rentalId);
        void ValidateNightsNumber(int nightsNumber);
        void ValidatePreparationTime(int preparationDaysNumber);
        void ValidateUpdateRentalChanges(int rentalId, int newUnitsNumber, int newPreparationDays);
    }

    public class Validator : IValidator
    {
        private IRentalRepository _rentals;
        private IChangeRentalService _simulationService;

        public Validator(IRentalRepository rentals, IChangeRentalService simulationService)
        {
            _rentals = rentals;
            _simulationService = simulationService;
        }

        public void ValidateNightsNumber(int nightsNumber)
        {
            if (nightsNumber < 0) throw new ApplicationException("Nights must be positive");
        }

        public void ValidatePreparationTime(int preparationDaysNumber)
        {
            if (preparationDaysNumber < 0) throw new ApplicationException("Preparation time must be positive");
        }

        public void ValidateRentalExistence(int rentalId)
        {
            var rental = _rentals.Get(rentalId);
            if (rental == null) throw new ApplicationException("Rental not found");
        }

        public void ValidateUpdateRentalChanges(int rentalId, int newUnitsNumber, int newPreparationDays)
        {
            if (!_simulationService.AreChangesApplicable(rentalId, newUnitsNumber, newPreparationDays)) throw new ApplicationException("Cannot update rental");
        }
    }
}
