using FlightSearch.Data;
using System;
using System.Threading.Tasks;

namespace FlightSearch.Service
{
    public class FlightSearchService : IFlightSearchService
    {
        private readonly IFlightRepository _flightRepository;
        public FlightSearchService(IFlightRepository flightRepository)
        {
            _flightRepository = flightRepository;
        }
        public async Task<object> FindFlight(string OriginCode, string DestinationCode, DateTime departureDate, string sortType)
        {
            return await _flightRepository.FindFlight(OriginCode, DestinationCode, departureDate, sortType);
        }
    }
}
