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
public class SalesController : BaseController
{
  private readonly IMediator _mediator;

  public SalesController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<IActionResult> CreateSale([FromBody] WebApiCreateSale.CreateSaleRequest request)
  {
    var command = new CreateSaleCommand
    {
      SaleNumber = request.SaleNumber,
      CustomerId = request.CustomerId,
      CustomerName = request.CustomerName,
      BranchId = request.BranchId,
      BranchName = request.BranchName,
      Date = request.Date,
      Items = request.Items.Select(i => new CreateSaleItemRequest
      {
        ProductId = i.ProductId,
        ProductName = i.ProductName,
        Quantity = i.Quantity,
        UnitPrice = i.UnitPrice
      }).ToList()
    };

    var result = await _mediator.Send(command);
    var response = new ApiResponseWithData<CreateSaleResult>
    {
      Success = true,
      Message = "Sale created successfully",
      Data = result
    };
    return Ok(response);
  }

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetSale(Guid id)
  {
    var query = new GetSaleQuery { Id = id };
    var result = await _mediator.Send(query);

    if (result == null)
    {
      return NotFound(new ApiResponse
      {
        Success = false,
        Message = "Sale not found"
      });
    }

    var response = new ApiResponseWithData<GetSaleResult>
    {
      Success = true,
      Message = "Sale retrieved successfully",
      Data = result
    };
    return Ok(response);
  }

  [HttpGet]
  public async Task<IActionResult> GetSales(
      [FromQuery] Guid? customerId,
      [FromQuery] Guid? branchId,
      [FromQuery] DateTime? startDate,
      [FromQuery] DateTime? endDate,
      [FromQuery] bool? cancelled,
      [FromQuery] int page = 1,
      [FromQuery] int pageSize = 20)
  {
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
      Message = "Sales retrieved successfully",
      Data = result
    };
    return Ok(response);
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> UpdateSale(Guid id, [FromBody] WebApiUpdateSale.UpdateSaleRequest request)
  {
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
      Message = "Sale updated successfully",
      Data = result
    };
    return Ok(response);
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> CancelSale(Guid id)
  {
    var command = new CancelSaleCommand { Id = id };
    var result = await _mediator.Send(command);

    var response = new ApiResponseWithData<CancelSaleResult>
    {
      Success = true,
      Message = "Sale cancelled successfully",
      Data = result
    };
    return Ok(response);
  }

  [HttpPost("{id:guid}/items")]
  public async Task<IActionResult> AddSaleItem(Guid id, [FromBody] WebApiAddSaleItem.AddSaleItemRequest request)
  {
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
      Message = "Item added to sale successfully",
      Data = result
    };
    return Ok(response);
  }

  [HttpDelete("{id:guid}/items/{itemId:guid}")]
  public async Task<IActionResult> RemoveSaleItem(Guid id, Guid itemId)
  {
    var command = new RemoveSaleItemCommand
    {
      SaleId = id,
      ItemId = itemId
    };

    var result = await _mediator.Send(command);
    var response = new ApiResponseWithData<RemoveSaleItemResult>
    {
      Success = true,
      Message = "Item removed from sale successfully",
      Data = result
    };
    return Ok(response);
  }
}
