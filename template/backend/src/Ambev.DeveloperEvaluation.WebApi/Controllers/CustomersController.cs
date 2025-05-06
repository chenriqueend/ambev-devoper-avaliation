using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Ambev.DeveloperEvaluation.Application.Commands.Customers;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.WebApi.Models.Customers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Ambev.DeveloperEvaluation.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(IMediator mediator, ICustomerRepository customerRepository)
        {
            _mediator = mediator;
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Gets all customers in the system
        /// </summary>
        /// <returns>List of all customers</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CustomerResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var response = customers.Select(MapToResponse);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while getting all customers: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerRequest request)
        {
            var command = new CreateCustomerCommand
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result.Error);

            var customer = await _customerRepository.GetByIdAsync(result.Data);
            if (customer == null)
                return NotFound();

            return CreatedAtAction(nameof(Get), new { id = customer.Id }, MapToResponse(customer));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            return Ok(MapToResponse(customer));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CustomerRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            customer.Update(request.Name, request.Email, request.Phone);
            await _customerRepository.UpdateAsync(customer);

            return Ok(new { message = "Customer successfully updated." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            await _customerRepository.DeleteAsync(customer);
            return Ok(new { message = "Customer successfully deleted." });
        }

        private static CustomerResponse MapToResponse(Customer customer)
        {
            return new CustomerResponse
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone
            };
        }
    }
}