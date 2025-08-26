using Ambev.DeveloperEvaluation.Application.Sales.AddSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using WebApiCreateSale = Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using WebApiUpdateSale = Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using WebApiAddSaleItem = Ambev.DeveloperEvaluation.WebApi.Features.Sales.AddSaleItem;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SalesController> _logger;

    public SalesController(IMediator mediator, ILogger<SalesController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> CreateSale([FromBody] WebApiCreateSale.CreateSaleRequest request)
    {
        try
        {
            _logger.LogInformation("Creating sale with number: {SaleNumber}", request.SaleNumber);

            var command = new CreateSaleCommand
            {
                SaleNumber = request.SaleNumber,
                CustomerId = request.CustomerId,
                CustomerName = request.CustomerName,
                BranchId = request.BranchId,
                BranchName = request.BranchName,
                Date = request.Date,
                Items = request.Items?.Select(i => new CreateSaleItemRequest
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList() ?? []
            };

            var result = await _mediator.Send(command);
            var response = new ApiResponseWithData<CreateSaleResult>
            {
                Success = true,
                Message = "Venda criada com sucesso",
                Data = result
            };

            _logger.LogInformation("Sale created successfully with ID: {SaleId}", result.Id);
            return CreatedAtAction(nameof(GetSale), new { id = result.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sale with number: {SaleNumber}", request.SaleNumber);
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Erro ao criar venda: " + ex.Message
            });
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> GetSale(Guid id)
    {
        try
        {
            _logger.LogInformation("Retrieving sale with ID: {SaleId}", id);

            var query = new GetSaleQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
            {
                _logger.LogWarning("Sale not found with ID: {SaleId}", id);
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Venda não encontrada"
                });
            }

            var response = new ApiResponseWithData<GetSaleResult>
            {
                Success = true,
                Message = "Venda recuperada com sucesso",
                Data = result
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sale with ID: {SaleId}", id);
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "Erro interno do servidor"
            });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSalesResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> GetSales(
        [FromQuery] Guid? customerId,
        [FromQuery] Guid? branchId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] bool? cancelled,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var query = new GetSalesQuery
            {
                CustomerId = customerId,
                BranchId = branchId,
                StartDate = startDate,
                EndDate = endDate,
                Cancelled = cancelled,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            var response = new ApiResponseWithData<GetSalesResult>
            {
                Success = true,
                Message = "Vendas recuperadas com sucesso",
                Data = result
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sales with filters");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = "Erro interno do servidor"
            });
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateSaleResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> UpdateSale(Guid id, [FromBody] WebApiUpdateSale.UpdateSaleRequest request)
    {
        try
        {
            _logger.LogInformation("Updating sale with ID: {SaleId}", id);

            var command = new UpdateSaleCommand
            {
                Id = id,
                CustomerName = request.CustomerName,
                BranchName = request.BranchName
            };

            var result = await _mediator.Send(command);
            var response = new ApiResponseWithData<UpdateSaleResult>
            {
                Success = true,
                Message = "Venda atualizada com sucesso",
                Data = result
            };

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("already cancelled") || ex.Message.Contains("At least one field must be provided"))
            {
                _logger.LogWarning(ex, "Business rule validation failed for sale with ID: {SaleId}", id);
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }

            _logger.LogWarning(ex, "Sale not found for update with ID: {SaleId}", id);
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Venda não encontrada"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sale with ID: {SaleId}", id);
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Erro ao atualizar venda: " + ex.Message
            });
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<CancelSaleResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> CancelSale(Guid id)
    {
        try
        {
            _logger.LogInformation("Cancelling sale with ID: {SaleId}", id);

            var command = new CancelSaleCommand { Id = id };
            var result = await _mediator.Send(command);

            var response = new ApiResponseWithData<CancelSaleResult>
            {
                Success = true,
                Message = "Venda cancelada com sucesso",
                Data = result
            };

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("already cancelled"))
            {
                _logger.LogWarning(ex, "Business rule validation failed for sale with ID: {SaleId}", id);
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }

            _logger.LogWarning(ex, "Sale not found for cancellation with ID: {SaleId}", id);
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Venda não encontrada"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling sale with ID: {SaleId}", id);
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Erro ao cancelar venda: " + ex.Message
            });
        }
    }

    [HttpPost("{id:guid}/items")]
    [ProducesResponseType(typeof(ApiResponseWithData<AddSaleItemResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> AddSaleItem(Guid id, [FromBody] WebApiAddSaleItem.AddSaleItemRequest request)
    {
        try
        {
            _logger.LogInformation("Adding item to sale with ID: {SaleId}", id);

            var command = new AddSaleItemCommand
            {
                SaleId = id,
                ProductId = request.ProductId,
                ProductName = request.ProductName,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice
            };

            var result = await _mediator.Send(command);
            var response = new ApiResponseWithData<AddSaleItemResult>
            {
                Success = true,
                Message = "Item adicionado à venda com sucesso",
                Data = result
            };

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Sale not found for adding item with ID: {SaleId}", id);
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Venda não encontrada"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to sale with ID: {SaleId}", id);
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Erro ao adicionar item: " + ex.Message
            });
        }
    }

    [HttpDelete("{id:guid}/items/{itemId:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<RemoveSaleItemResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse), 400)]
    [ProducesResponseType(typeof(ApiResponse), 404)]
    [ProducesResponseType(typeof(ApiResponse), 500)]
    public async Task<IActionResult> RemoveSaleItem(Guid id, Guid itemId)
    {
        try
        {
            _logger.LogInformation("Removing item {ItemId} from sale {SaleId}", itemId, id);

            var command = new RemoveSaleItemCommand
            {
                SaleId = id,
                ItemId = itemId
            };

            var result = await _mediator.Send(command);
            var response = new ApiResponseWithData<RemoveSaleItemResult>
            {
                Success = true,
                Message = "Item removido da venda com sucesso",
                Data = result
            };

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Sale or item not found for removal. Sale ID: {SaleId}, Item ID: {ItemId}", id, itemId);
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Venda ou item não encontrado"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing item {ItemId} from sale {SaleId}", itemId, id);
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = "Erro ao remover item: " + ex.Message
            });
        }
    }
}
