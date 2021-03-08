using System;
using System.Threading.Tasks;

namespace FlightSearch.Data
{
    public interface IFlightRepository
    {
        Task<object> FindFlight(string OriginCode, string DestinationCode, DateTime departureDate, string sortType);
    }
}
