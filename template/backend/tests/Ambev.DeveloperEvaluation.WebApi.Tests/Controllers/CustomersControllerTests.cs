using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.Application.Commands.Customers;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Controllers;
using Ambev.DeveloperEvaluation.WebApi.Models.Customers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.WebApi.Tests.Controllers
{
    public class CustomersControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly CustomersController _controller;

        public CustomersControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _controller = new CustomersController(_mediatorMock.Object, _customerRepositoryMock.Object);
        }

        [Fact]
        public async Task Create_WithValidRequest_ShouldReturnCreated()
        {
            // Arrange
            var request = new CustomerRequest
            {
                Name = "Test Customer",
                Email = "test@email.com",
                Phone = "1234567890"
            };

            var customerId = Guid.NewGuid();
            var customer = new Customer(request.Name, request.Email, request.Phone)
            {
                Id = customerId
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(CustomersController.GetById), createdResult.ActionName);
            Assert.Equal(customerId, createdResult.RouteValues["id"]);
            var response = Assert.IsType<CustomerResponse>(createdResult.Value);
            Assert.Equal(customerId, response.Id);
            Assert.Equal(request.Name, response.Name);
            Assert.Equal(request.Email, response.Email);
            Assert.Equal(request.Phone, response.Phone);
        }

        [Fact]
        public async Task GetById_WithExistingCustomer_ShouldReturnOk()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer("Test Customer", "test@email.com", "1234567890")
            {
                Id = customerId
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCustomerByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            // Act
            var result = await _controller.GetById(customerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<CustomerResponse>(okResult.Value);
            Assert.Equal(customerId, response.Id);
            Assert.Equal(customer.Name, response.Name);
            Assert.Equal(customer.Email, response.Email);
            Assert.Equal(customer.Phone, response.Phone);
        }

        [Fact]
        public async Task GetById_WithNonExistingCustomer_ShouldReturnNotFound()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCustomerByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _controller.GetById(customerId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_WithValidRequest_ShouldReturnNoContent()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var request = new CustomerRequest
            {
                Name = "Updated Customer",
                Email = "updated@email.com",
                Phone = "0987654321"
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateCustomerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Update(customerId, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_WithNonExistingCustomer_ShouldReturnNotFound()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var request = new CustomerRequest
            {
                Name = "Updated Customer",
                Email = "updated@email.com",
                Phone = "0987654321"
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateCustomerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Update(customerId, request);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_WithExistingCustomer_ShouldReturnNoContent()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteCustomerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(customerId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_WithNonExistingCustomer_ShouldReturnNotFound()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteCustomerCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(customerId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer("Customer 1", "12345678900", "customer1@email.com") { Id = Guid.NewGuid() },
                new Customer("Customer 2", "09876543210", "customer2@email.com") { Id = Guid.NewGuid() }
            };

            _customerRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(customers);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<CustomerResponse>>(okResult.Value);
            var customerList = response.ToList();
            Assert.Equal(2, customerList.Count);
            Assert.Equal(customers[0].Name, customerList[0].Name);
            Assert.Equal(customers[1].Name, customerList[1].Name);
        }

        [Fact]
        public async Task GetAll_WhenRepositoryThrowsException_ShouldReturnBadRequest()
        {
            // Arrange
            _customerRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Database error", badRequestResult.Value.ToString());
        }
    }
} 