using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models.DomainModels;

namespace VacationRental.Api.Repositories
{
    public interface IUnitRepository
    {
        Unit Create(int rentalId);
        Unit Get(int unitId);
        void CreateMany(int rentalId, int count);
        List<Unit> GetByRental(int rentalId);
        void RemoveByRental(int rentalId);
        int GetUnitPerRentalId(int unitId);
    }

    public class UnitRepository : IUnitRepository
    {
        private readonly List<Unit> _units = new List<Unit>();

        public UnitRepository() { }

        public Unit Create(int rentalId)
        {
            var newId = _units.Any() ? _units.Max(u => u.Id) + 1 : 1;
            var newUnit = new Unit() { RentalId = rentalId, Id = newId };
            _units.Add(newUnit);
            return new Unit(newUnit);
        }

        public void CreateMany(int rentalId, int count)
        {
            var newUnits = new List<Unit>();
            for (int i = 0; i < count; i++)
            {
                newUnits.Add(Create(rentalId));
            }
        }

        public Unit Get(int unitId)
        {
            return _units.Where(u => u.Id == unitId)
                         .Select(u => new Unit(u))
                         .FirstOrDefault();
        }

        public List<Unit> GetByRental(int rentalId)
        {
            return _units.Where(u => u.RentalId == rentalId)
                         .Select(u => new Unit(u))
                         .ToList();
        }

        public int GetUnitPerRentalId(int unitId)
        {
            var unit = Get(unitId);
            var id = _units.Where(u => u.RentalId == unit.RentalId)
                .OrderBy(u => u.Id)
                .Select((u, index) => new { number = index + 1, unit = u })
                .Where(e => e.unit.Id == unitId)
                .Select(e => e.number)
                .FirstOrDefault();
            return id;
        }

        public void RemoveByRental(int rentalId)
        {
            _units.RemoveAll(u => u.RentalId == rentalId);
        }
    }
}
