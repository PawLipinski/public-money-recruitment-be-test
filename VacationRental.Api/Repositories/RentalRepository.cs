using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models;

namespace VacationRental.Api.Repositories
{
    public interface IRentalRepository
    {
        Rental Get(int id);
        int Add(int unitsNumber, int preparationDaysNumber);
    }

    public class RentalRepository : IRentalRepository
    {
        private readonly IUnitRepository _unitRepository;
        private readonly List<Rental> _rentals = new List<Rental>();

        public RentalRepository(IUnitRepository unitRepository)
        {
            _unitRepository = unitRepository;
        }

        public int Add(int unitsNumber, int preparationDaysNumber)
        {
            var newRentalId = _rentals.Any() ? _rentals.Max(r => r.Id) + 1 : 1;
            _unitRepository.CreateMany(newRentalId, unitsNumber);
            _rentals.Add(new Rental() { Id = newRentalId, PreparationTimeInDays = preparationDaysNumber });

            return newRentalId;
        }

        public Rental Get(int id)
        {
            return _rentals.Where(r => r.Id == id)
                           .FirstOrDefault();
        }
    }
}
