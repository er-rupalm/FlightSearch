using Neo4j.Driver;
using System;
using System.Threading.Tasks;

namespace FlightSearch.Data
{
    public class FlightRepository : IFlightRepository
    {
        private readonly IDriver _driver;

        public FlightRepository(IDriver driver)
        {
            _driver = driver;
        }
        public async Task<object> FindFlight(string OriginCode, string DestinationCode, DateTime departureDate, string sortType)
        {
            var session = _driver.AsyncSession(WithDatabase);
            string startDate = departureDate.Date.ToString("yyyy-MM-dd");

            //string sortBy = "DEPARTURE_DATETIME";
            string sortBy = sortType ?? "COST";

            string query = @"
CALL{
    match (origin:AIRPORT_DATE_DESTINATION)-[:ORIGIN_OF]->(flight:FLIGHT)
    where origin.airport = ($origin)
    and origin.destination = ($destination)
    and origin.date = date(($startDt))
    return [flight] as ROUTE, flight.cost as COST, flight.originTime as DEPARTURE_DATETIME
    union all
    match(origin: AIRPORT_DATE) -[:HAS_FLIGHT_TO]->(:AIRPORT_DATE_DESTINATION) -[:ORIGIN_OF]->(flight1: FLIGHT) -[:LANDS_AT]->(:AIRPORT_DATE) -[:HAS_FLIGHT_TO]->(:AIRPORT_DATE_DESTINATION) -[:ORIGIN_OF]->(flight2: FLIGHT) -[:LANDS_AT]->(destinationDate: AIRPORT_DATE)
    where origin.airport = ($origin)
    and destinationDate.airport = ($destination)
    and origin.date = date(($startDt))
    and(flight1.destinationTime.epochMillis + (3600 * 1000)) <= flight2.originTime.epochMillis
    and destinationDate.date = date(flight2.destinationTime)
    and(flight1.destinationTime.epochMillis + (6 * 3600 * 1000)) >= flight2.originTime.epochMillis
    return [flight1, flight2] as ROUTE, flight1.cost + flight2.cost as COST, flight1.originTime as DEPARTURE_DATETIME
    union all
    match(origin: AIRPORT_DATE) -[:HAS_FLIGHT_TO]->(:AIRPORT_DATE_DESTINATION) -[:ORIGIN_OF]->(flight1: FLIGHT) -[:LANDS_AT]->(airportDate1: AIRPORT_DATE) -[:HAS_FLIGHT_TO]->(:AIRPORT_DATE_DESTINATION) -[:ORIGIN_OF]->(flight2: FLIGHT) -[:LANDS_AT]->(airportDate2: AIRPORT_DATE) -[:HAS_FLIGHT_TO]->(:AIRPORT_DATE_DESTINATION) -[:ORIGIN_OF]->(flight3: FLIGHT) -[:LANDS_AT]->(destinationDate: AIRPORT_DATE)
    where origin.airport = ($origin)
    and destinationDate.airport = ($destination)
    and origin.date = date(($startDt))
    and origin.airport<> airportDate2.airport
    and airportDate1.airport<> destinationDate.airport
    and(flight1.destinationTime.epochMillis + (3600 * 1000)) <= flight2.originTime.epochMillis
    and(flight2.destinationTime.epochMillis + (3600 * 1000)) <= flight3.originTime.epochMillis
    and destinationDate.date = date(flight3.destinationTime)
    and(flight1.destinationTime.epochMillis + (6 * 3600 * 1000)) >= flight2.originTime.epochMillis
    and(flight2.destinationTime.epochMillis + (6 * 3600 * 1000)) >= flight3.originTime.epochMillis
    return [flight1, flight2, flight3] as ROUTE, flight1.cost + flight2.cost + flight3.cost as COST, flight1.originTime as DEPARTURE_DATETIME
    }
    RETURN ROUTE, COST, DEPARTURE_DATETIME
    ORDER BY  " + sortBy;

            try
            {
                var result = await session.ReadTransactionAsync(async tx =>
                    {
                        var cursor = await tx.RunAsync(query, new { origin = OriginCode, destination = DestinationCode, startDt = startDate, orderBy = sortBy });
                        var res = await cursor.ToListAsync();
                        return res;
                    });

                return result;

            }
            finally
            {
                await session.CloseAsync();
            }
        }

        private static void WithDatabase(SessionConfigBuilder sessionConfigBuilder)
        {
            sessionConfigBuilder.WithDatabase(Database());
        }

        private static string Database()
        {
            return System.Environment.GetEnvironmentVariable("NEO4J_DATABASE") ?? "neo4j";
        }
    }
}
