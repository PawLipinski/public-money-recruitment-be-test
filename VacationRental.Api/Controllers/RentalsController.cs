using Microsoft.AspNetCore.Mvc;
using System.Linq;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalRepository _rentals;
        private readonly IUnitRepository _units;

        public RentalsController(IRentalRepository rentals, IUnitRepository units)
        {
            _rentals = rentals;
            _units = units;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public RentalViewModel Get(int rentalId)
        {
            var rental = _rentals.Get(rentalId);
            var units = _units.GetByRental(rentalId);
            return new RentalViewModel()
            {
                Id = rental.Id,
                Units = units.Count(),
            };
        }

        [HttpPost]
        public ResourceIdViewModel Post(RentalBindingModel model)
        {
            var newRentalId = _rentals.Add(model.Units, model.PreparationTimeInDays);
            return new ResourceIdViewModel(){ Id = newRentalId };
        }
    }
}
