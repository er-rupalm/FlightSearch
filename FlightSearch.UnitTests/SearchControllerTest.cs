using AutoFixture;
using FlightSearch.Controllers;
using FlightSearch.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace FlightSearch.UnitTests
{
    public class SearchControllerTest
    {
        private Fixture _fixture;
        private Mock<IFlightSearchService> _mockService;

        public SearchControllerTest()
        {
            _fixture = new Fixture();
            _mockService = new Mock<IFlightSearchService>();
        }

        [Fact]
        public async Task SearchController_SearchFlight_ReturnBadRequest()
        {
            //Arrange
            var data = _fixture.Create<object>();
            _mockService.Setup(x => x.FindFlight(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .Returns(Task.Run(()=> { return data; }));
            var controller = new SearchController(_mockService.Object);

            //Act
            var response = await controller.SearchFlight(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>());
            var okResult = response as BadRequestObjectResult ;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(StatusCodes.Status400BadRequest, okResult.StatusCode);
        }

        [Fact]
        public async Task SearchController_SearchFlight_ReturnData()
        {
            //Arrange
            var data = _fixture.Create<object>();
            _mockService.Setup(x => x.FindFlight(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .Returns(Task.Run(() => { return data; }));
            var controller = new SearchController(_mockService.Object);

            //Act
            var response = await controller.SearchFlight("AAA","BBB",DateTime.Today.AddDays(1),"COST");
            var okResult = response as OkObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }
    }
}
