using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
  public record UpdateSaleCommand : IRequest<UpdateSaleResult>
  {
    public Guid Id { get; init; }
    public string? CustomerName { get; init; }
    public string? BranchName { get; init; }
  }
}
