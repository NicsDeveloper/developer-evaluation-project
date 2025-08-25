using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales
{
  public class GetSalesHandler : IRequestHandler<GetSalesQuery, GetSalesResult>
  {
    private readonly ISaleRepository _saleRepository;

    public GetSalesHandler(ISaleRepository saleRepository)
    {
      _saleRepository = saleRepository;
    }

    public async Task<GetSalesResult> Handle(GetSalesQuery request, CancellationToken cancellationToken)
    {
      List<Domain.Entities.Sale> sales = [];

      sales = request.CustomerId.HasValue
        ? (await _saleRepository.GetByCustomerIdAsync(request.CustomerId.Value)).ToList()
        : request.BranchId.HasValue
          ? (await _saleRepository.GetByBranchIdAsync(request.BranchId.Value)).ToList()
          : request.StartDate.HasValue && request.EndDate.HasValue
                  ? (await _saleRepository.GetByDateRangeAsync(request.StartDate.Value, request.EndDate.Value)).ToList()
                  : request.Cancelled.HasValue
                          ? request.Cancelled.Value
                                        ? (await _saleRepository.GetCancelledSalesAsync()).ToList()
                                        : (await _saleRepository.GetActiveSalesAsync()).ToList()
                          : (await _saleRepository.GetAllAsync()).ToList();

      int totalCount = sales.Count;
      int totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
      List<Domain.Entities.Sale> pagedSales = sales
              .Skip((request.Page - 1) * request.PageSize)
              .Take(request.PageSize)
              .ToList();

      return new GetSalesResult
      {
        Sales = pagedSales.Select(static s => new GetSalesItemResult
        {
          Id = s.Id,
          SaleNumber = s.SaleNumber,
          Date = s.Date,
          CustomerId = s.CustomerId,
          CustomerName = s.CustomerName,
          BranchId = s.BranchId,
          BranchName = s.BranchName,
          NetTotal = s.NetTotal,
          Cancelled = s.Cancelled,
          CreatedAt = s.CreatedAt,
          ItemCount = s.Items.Count
        }).ToList(),
        TotalCount = totalCount,
        Page = request.Page,
        PageSize = request.PageSize,
        TotalPages = totalPages
      };
    }
  }
}
