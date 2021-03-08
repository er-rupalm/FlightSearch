using System;
using System.Threading.Tasks;

namespace FlightSearch.Service
{
    public interface IFlightSearchService
    {
        Task<object> FindFlight(string OriginCode, string DestinationCode, DateTime departureDate, string sortType);
    }
}
