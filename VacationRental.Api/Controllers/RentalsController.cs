using Microsoft.AspNetCore.Mvc;
using System.Linq;
using VacationRental.Api.Models;
using VacationRental.Api.Repositories;
using VacationRental.Api.Services;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalRepository _rentals;
        private readonly IUnitRepository _units;
        private readonly IValidator _validator;
        private readonly IChangeRentalService _changeRentalService;

        public RentalsController(IRentalRepository rentals, IUnitRepository units, IValidator validator, IChangeRentalService changeRentalService)
        {
            _rentals = rentals;
            _units = units;
            _validator = validator;
            _changeRentalService = changeRentalService;
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
            _validator.ValidatePreparationTime(model.PreparationTimeInDays);
            var newRentalId = _rentals.Add(model.Units, model.PreparationTimeInDays);
            return new ResourceIdViewModel() { Id = newRentalId };
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Put([FromQuery] int id, [FromBody] RentalUpdate rentalUpdate)
        {
            _validator.ValidateUpdateRentalChanges(id, rentalUpdate.Units, rentalUpdate.PreparationTimeInDays);

            _changeRentalService.ApplyChanges(id, rentalUpdate.Units, rentalUpdate.PreparationTimeInDays);
            return Ok();
        }
    }
}
