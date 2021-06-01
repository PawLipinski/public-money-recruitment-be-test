using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class GetCalendarTests
    {
        private readonly HttpClient _client;

        public GetCalendarTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenGetCalendar_ThenAGetReturnsTheCalculatedCalendar()
        {
            var rentals = new List<RentalBindingModel>()
            {
                new RentalBindingModel()
                {
                    Units = 2
                }
            };
            var rentalIds = await PopulateRentals(rentals);
            var bookings = new List<BookingBindingModel>()
            {
                new BookingBindingModel()
                {
                    RentalId = rentalIds[0],
                    Nights = 2,
                    Start = new DateTime(2000, 01, 02)
                },
                new BookingBindingModel()
                {
                    RentalId = rentalIds[0],
                    Nights = 2,
                    Start = new DateTime(2000, 01, 03)
                },
            };
            var bookingIds = await PopulateBookings(bookings);

            using (var getCalendarResponse = await _client.GetAsync($"/api/v1/calendar?rentalId={rentalIds[0]}&start=2000-01-01&nights=5"))
            {
                Assert.True(getCalendarResponse.IsSuccessStatusCode);

                var getCalendarResult = await getCalendarResponse.Content.ReadAsAsync<Calendar>();

                Assert.Equal(rentalIds[0], getCalendarResult.RentalId);
                Assert.Equal(5, getCalendarResult.Dates.Count);

                Assert.Equal(new DateTime(2000, 01, 01), getCalendarResult.Dates[0].Date);
                Assert.Empty(getCalendarResult.Dates[0].Bookings);

                Assert.Equal(new DateTime(2000, 01, 02), getCalendarResult.Dates[1].Date);
                Assert.Single(getCalendarResult.Dates[1].Bookings);
                Assert.Contains(getCalendarResult.Dates[1].Bookings, x => x.Id == bookingIds[0]);

                Assert.Equal(new DateTime(2000, 01, 03), getCalendarResult.Dates[2].Date);
                Assert.Equal(2, getCalendarResult.Dates[2].Bookings.Count);
                Assert.Contains(getCalendarResult.Dates[2].Bookings, x => x.Id == bookingIds[0]);
                Assert.Contains(getCalendarResult.Dates[2].Bookings, x => x.Id == bookingIds[1]);

                Assert.Equal(new DateTime(2000, 01, 04), getCalendarResult.Dates[3].Date);
                Assert.Single(getCalendarResult.Dates[3].Bookings);
                Assert.Contains(getCalendarResult.Dates[3].Bookings, x => x.Id == bookingIds[1]);

                Assert.Equal(new DateTime(2000, 01, 05), getCalendarResult.Dates[4].Date);
                Assert.Empty(getCalendarResult.Dates[4].Bookings);
            }
        }


        [Fact]
        public async Task TestCalendarGetWithPreparation()
        {
            var rentals = new List<RentalBindingModel>()
            {
                new RentalBindingModel()
                {
                    Units = 2,
                    PreparationTimeInDays = 2
                }
            };
            var rentalIds = await PopulateRentals(rentals);
            var bookings = new List<BookingBindingModel>()
            {
                new BookingBindingModel()
                {
                    RentalId = rentalIds[0],
                    Nights = 2,
                    Start = new DateTime(2000, 01, 01)
                },
                new BookingBindingModel()
                {
                    RentalId = rentalIds[0],
                    Nights = 2,
                    Start = new DateTime(2000, 01, 03)
                },
            };
            var bookingIds = await PopulateBookings(bookings);

            using (var getCalendarResponse = await _client.GetAsync($"/api/v1/calendar?rentalId={rentalIds[0]}&start=2000-01-01&nights=7"))
            {
                Assert.True(getCalendarResponse.IsSuccessStatusCode);

                var getCalendarResult = await getCalendarResponse.Content.ReadAsAsync<CalendarViewModel>();
                Assert.Equal(new DateTime(2000, 01, 02), getCalendarResult.Dates[1].Date);
                Assert.Contains(getCalendarResult.Dates[1].Bookings, x => x.Id == bookingIds[0]);
                var firstBookingUnitId = getCalendarResult.Dates[1].Bookings[0].Unit;
                
                Assert.Equal(new DateTime(2000, 01, 03), getCalendarResult.Dates[2].Date);
                Assert.Contains(getCalendarResult.Dates[2].Bookings, x => x.Id == bookingIds[1]);
                var secondBookingUnitId = getCalendarResult.Dates[2].Bookings[0].Unit;
                Assert.True(getCalendarResult.Dates[2].PreparationTimes[0].Unit == firstBookingUnitId);

                Assert.Equal(new DateTime(2000, 01, 04), getCalendarResult.Dates[3].Date);
                Assert.Contains(getCalendarResult.Dates[3].Bookings, x => x.Id == bookingIds[1]);
                Assert.True(getCalendarResult.Dates[3].PreparationTimes[0].Unit == firstBookingUnitId);
                Assert.True(getCalendarResult.Dates[3].Bookings[0].Unit == secondBookingUnitId);

                Assert.Equal(new DateTime(2000, 01, 05), getCalendarResult.Dates[4].Date);
                Assert.True(getCalendarResult.Dates[4].PreparationTimes[0].Unit == secondBookingUnitId);
            }
        }

        private async Task<List<int>> PopulateRentals(List<RentalBindingModel> rentals)
        {
            var ids = new List<int>();
            foreach (var postRentalRequest in rentals)
            {
                ResourceIdViewModel postRentalResult;
                using (var postRentalResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", postRentalRequest))
                {
                    Assert.True(postRentalResponse.IsSuccessStatusCode);
                    postRentalResult = await postRentalResponse.Content.ReadAsAsync<ResourceIdViewModel>();
                    ids.Add(postRentalResult.Id);
                }
            }
            return ids;
        }

        private async Task<List<int>> PopulateBookings(List<BookingBindingModel> bookings)
        {
            var ids = new List<int>();
            foreach (var postBookingRequest in bookings)
            {
                ResourceIdViewModel postBookingResult;
                using (var postBooking1Response = await _client.PostAsJsonAsync($"/api/v1/bookings", postBookingRequest))
                {
                    Assert.True(postBooking1Response.IsSuccessStatusCode);
                    postBookingResult = await postBooking1Response.Content.ReadAsAsync<ResourceIdViewModel>();
                    ids.Add(postBookingResult.Id);
                }
            }
            return ids;
        }
    }
}
