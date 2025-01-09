using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Controllers;
using StargateAPI.Business.Commands;
using StargateAPI.Helpers;
using StargateAPI.Business.Queries;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;

namespace StarBaseApiUnitTest.Controllers
{
    [TestClass]
    public class PersonControllerTests
    {
        private Mock<StargateContext> _contextMock;

        private Mock<IMediator> _mediatorMock;
        private StarbaseApiCallLogger _apiLogger;
        private PersonController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            var options = new DbContextOptionsBuilder<StargateContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _contextMock = new Mock<StargateContext>(options);
            _apiLogger = new StarbaseApiCallLogger(_contextMock.Object);
            _controller = new PersonController(_mediatorMock.Object, _apiLogger);
        }

        [TestMethod]
        public async Task GetPeople_ReturnsOkResult()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPeople>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new GetPeopleResult { People = new List<PersonAstronaut>() });

            // Act
            var result = await _controller.GetPeople();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.IsInstanceOfType(okResult.Value, typeof(List<PersonAstronaut>));
        }

        [TestMethod]
        public async Task GetPersonByName_ReturnsBadRequest_WhenNameIsNullOrEmpty()
        {
            // Arrange
            var name = "";

            // Act
            var result = await _controller.GetPersonByName(name);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(badRequestResult.Value, typeof(StargateAPI.Controllers.BaseResponse));
            var response = (StargateAPI.Controllers.BaseResponse)badRequestResult.Value;
            Assert.AreEqual("Name cannot be null or empty", ((StargateAPI.Controllers.BaseResponse)response).Message);
        }
        // [TestMethod]
        // public async Task GetPersonByName_ReturnsOkResult_WhenNameIsValid()
        // {
        //     // Arrange
        //     var name = "John Doe";
        //     _mediatorMock.Setup(m => m.Send(It.IsAny<GetPersonByName>(), It.IsAny<CancellationToken>()))
        //                  .ReturnsAsync(new Person { Name = name });

        //     // Act
        //     var result = await _controller.GetPersonByName(name);

        //     // Assert
        //     var okResult = Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        //     var response = Assert.IsInstanceOfType(((OkObjectResult)okResult).Value, typeof(Person));
        //     Assert.AreEqual(name, ((Person)response).Name);
        // }

        // [TestMethod]
        // public async Task CreatePerson_ReturnsBadRequest_WhenNameIsNullOrEmpty()
        // {
        //     // Arrange
        //     var command = new CreatePerson { Name = "" };

        //     // Act
        //     var result = await _controller.CreatePerson(command);

        //     // Assert
        //     var badRequestResult = Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        //     var response = Assert.IsInstanceOfType(((BadRequestObjectResult)badRequestResult).Value, typeof(BaseResponse));
        //     Assert.AreEqual("Name cannot be null or empty", ((BaseResponse)response).Message);
        // }

        [TestMethod]
        public async Task CreatePerson_ReturnsOkResult_WhenCommandIsValid()
        {
            // Arrange
            var command = new CreatePerson { Name = "John Doe" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePerson>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new CreatePersonResult { Id = 1, Name = "John Doe" });

            // Act
            var result = await _controller.CreatePerson(command);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.IsInstanceOfType(okResult.Value, typeof(CreatePersonResult));
            var response = (CreatePersonResult)okResult.Value;
            Assert.AreEqual("John Doe", ((CreatePersonResult)response).Name);
        }

        // [TestMethod]
        // public async Task UpdatePerson_ReturnsBadRequest_WhenNameOrNewNameIsNullOrEmpty()
        // {
        //     // Arrange
        //     var options = new DbContextOptionsBuilder<StargateContext>()
        //         .UseInMemoryDatabase(databaseName: "TestDatabase")
        //         .Options;
        
        //     using var context = new StargateContext(options);
        //     var apiLogger = new StarbaseApiCallLogger(context);
        //     var controller = new PersonController(context, apiLogger);
        
        //     var command = new UpdatePerson { Name = "", NewName = "" };
        
        //     // Act
        //     var result = await controller.UpdatePerson(command);
        
        //     // Assert
        //     Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        //     var badRequestResult = (BadRequestObjectResult)result;
        //     Assert.IsInstanceOfType(badRequestResult.Value, typeof(StargateAPI.Helpers.BaseResponse));
        //     var response = (StargateAPI.Helpers.BaseResponse)badRequestResult.Value;
        //     Assert.AreEqual("Name cannot be null or empty", response.Message);
        // }

        [TestMethod]
        public async Task UpdatePerson_ReturnsOkResult_WhenCommandIsValid()
        {
            // Arrange
            var command = new UpdatePerson { Name = "John Doe", NewName = "John doing it again" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePerson>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new UpdatePersonResult { Name = "John Smith" });

            // Act
            var result = await _controller.UpdatePerson(command);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.IsInstanceOfType(okResult.Value, typeof(UpdatePersonResult));
            var response = (UpdatePersonResult)okResult.Value;
            Assert.AreEqual("John Smith", response.Name);
        }
    }
}
