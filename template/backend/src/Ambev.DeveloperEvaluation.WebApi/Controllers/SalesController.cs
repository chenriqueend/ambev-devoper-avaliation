using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Commands.Sales;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.WebApi.Models.Sales;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace Ambev.DeveloperEvaluation.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISaleRepository _saleRepository;
        private readonly ILogger<SalesController> _logger;

        public SalesController(
            IMediator mediator,
            ISaleRepository saleRepository,
            ILogger<SalesController> logger)
        {
            _mediator = mediator;
            _saleRepository = saleRepository;
            _logger = logger;
        }

        /// <summary>
        /// Gets all sales in the system
        /// </summary>
        /// <returns>List of all sales</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SaleResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("[SalesController] Getting all sales");
                var sales = await _saleRepository.GetAllAsync();
                var response = sales.Select(MapToResponse);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SalesController] Error getting all sales");
                return BadRequest($"An error occurred while getting all sales: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaleRequest request)
        {
            try
            {
                _logger.LogInformation("[SalesController] Creating sale with number: {SaleNumber}", request.SaleNumber);

                var command = new CreateSaleCommand
                {
                    SaleNumber = request.SaleNumber,
                    CustomerId = request.CustomerId,
                    CustomerName = request.CustomerName,
                    BranchId = request.BranchId,
                    BranchName = request.BranchName,
                    Items = request.Items.Select(item => new CreateSaleCommand.SaleItemDto
                    {
                        ProductId = item.ProductId,
                        ProductDescription = item.ProductDescription,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    }).ToList()
                };

                _logger.LogInformation("[SalesController] Sending command to create sale");
                var result = await _mediator.Send(command);

                if (!result.Success)
                {
                    _logger.LogWarning("[SalesController] Failed to create sale: {Error}", result.Error);
                    return BadRequest(result.Error);
                }

                _logger.LogInformation("[SalesController] Sale created successfully with ID: {SaleId}", result.Data);
                return CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "[SalesController] Database error while creating sale");
                _logger.LogError("[SalesController] Inner exception: {InnerException}", dbEx.InnerException?.Message);

                if (dbEx.InnerException is SqliteException sqliteEx)
                {
                    _logger.LogError("[SalesController] SQLite error code: {ErrorCode}", sqliteEx.SqliteErrorCode);
                    _logger.LogError("[SalesController] SQLite error message: {ErrorMessage}", sqliteEx.Message);
                    return BadRequest($"Database error: {sqliteEx.Message} (Error code: {sqliteEx.SqliteErrorCode})");
                }

                return BadRequest($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SalesController] Unexpected error while creating sale");
                _logger.LogError("[SalesController] Error details: {ErrorDetails}", ex.ToString());
                return BadRequest($"An unexpected error occurred: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var sale = await _saleRepository.GetByIdAsync(id);
                return Ok(MapToResponse(sale));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}/items/{productId}/quantity")]
        public async Task<IActionResult> UpdateItemQuantity(Guid id, Guid productId, [FromQuery] int newQuantity)
        {
            try
            {
                _logger.LogInformation("[SalesController] Updating quantity for sale {SaleId}, product {ProductId} to {NewQuantity}",
                    id, productId, newQuantity);

                var command = new UpdateSaleItemCommand(id, productId, newQuantity);

                var result = await _mediator.Send(command);
                if (!result.Success)
                {
                    _logger.LogWarning("[SalesController] Failed to update quantity: {Error}", result.Error);
                    return BadRequest(result.Error);
                }

                _logger.LogInformation("[SalesController] Successfully updated quantity");
                return Ok(new { message = "Item quantity successfully updated." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SalesController] Unexpected error while updating quantity");
                return BadRequest("An unexpected error occurred while updating the item quantity.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _saleRepository.DeleteAsync(id);
            if (!result)
                return NotFound();

            return Ok(new { message = "Sale successfully deleted." });
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            try
            {
                _logger.LogInformation("[SalesController] Attempting to cancel sale {SaleId}", id);
                var command = new CancelSaleCommand(id);

                var result = await _mediator.Send(command);
                if (!result.Success)
                {
                    _logger.LogWarning("[SalesController] Failed to cancel sale {SaleId}: {Error}", id, result.Error);
                    return BadRequest(result.Error);
                }

                _logger.LogInformation("[SalesController] Successfully cancelled sale {SaleId}", id);
                return Ok(new { message = "Sale successfully cancelled." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SalesController] Error cancelling sale {SaleId}", id);
                return BadRequest($"An error occurred while cancelling the sale: {ex.Message}");
            }
        }

        private static SaleResponse MapToResponse(Sale sale)
        {
            return new SaleResponse
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                SaleDate = sale.SaleDate,
                CustomerId = sale.CustomerId,
                CustomerName = sale.CustomerName,
                BranchId = sale.BranchId,
                BranchName = sale.BranchName,
                TotalWithDiscount = sale.TotalWithDiscount,
                IsCancelled = sale.IsCancelled,
                Items = sale.Items.Select(item => new SaleResponse.SaleItemResponse
                {
                    ProductId = item.ProductId,
                    ProductDescription = item.ProductDescription,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount,
                    TotalAmount = item.TotalAmount
                }).ToList()
            };
        }
    }
}