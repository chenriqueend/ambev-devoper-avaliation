using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Ambev.DeveloperEvaluation.Application.Commands.Branches;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.WebApi.Models.Branches;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Ambev.DeveloperEvaluation.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBranchRepository _branchRepository;

        public BranchesController(IMediator mediator, IBranchRepository branchRepository)
        {
            _mediator = mediator;
            _branchRepository = branchRepository;
        }

        /// <summary>
        /// Gets all branches in the system
        /// </summary>
        /// <returns>List of all branches</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BranchResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var branches = await _branchRepository.GetAllAsync();
                var response = branches.Select(MapToResponse);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while getting all branches: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BranchRequest request)
        {
            var command = new CreateBranchCommand
            {
                Name = request.Name,
                Address = request.Address,
                Phone = request.Phone
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result.Error);

            var branch = await _branchRepository.GetByIdAsync(result.Data);
            if (branch == null)
                return NotFound();

            return CreatedAtAction(nameof(Get), new { id = branch.Id }, MapToResponse(branch));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch == null)
                return NotFound();

            return Ok(MapToResponse(branch));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BranchRequest request)
        {
            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch == null)
                return NotFound();

            branch.Update(request.Name, request.Address, request.Phone);
            await _branchRepository.UpdateAsync(branch);

            return Ok(new { message = "Branch updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch == null)
                return NotFound();

            await _branchRepository.DeleteAsync(branch);
            return Ok(new { message = "Branch deleted successfully" });
        }

        private static BranchResponse MapToResponse(Branch branch)
        {
            return new BranchResponse
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address,
                Phone = branch.Phone
            };
        }
    }
}