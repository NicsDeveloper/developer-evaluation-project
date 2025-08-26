using Ambev.DeveloperEvaluation.Application.Sales.AddSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Common.Security;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class ApplicationModuleInitializer : IModuleInitializer
{
  public void Initialize(WebApplicationBuilder builder)
  {
    builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationModuleInitializer).Assembly));

    builder.Services.AddScoped<IRequestHandler<CreateSaleCommand, CreateSaleResult>, CreateSaleHandler>();
    builder.Services.AddScoped<IRequestHandler<GetSaleQuery, GetSaleResult?>, GetSaleHandler>();
    builder.Services.AddScoped<IRequestHandler<GetSalesQuery, GetSalesResult>, GetSalesHandler>();
    builder.Services.AddScoped<IRequestHandler<UpdateSaleCommand, UpdateSaleResult>, UpdateSaleHandler>();
    builder.Services.AddScoped<IRequestHandler<CancelSaleCommand, CancelSaleResult>, CancelSaleHandler>();
    builder.Services.AddScoped<IRequestHandler<AddSaleItemCommand, AddSaleItemResult>, AddSaleItemHandler>();
    builder.Services.AddScoped<IRequestHandler<RemoveSaleItemCommand, RemoveSaleItemResult>, RemoveSaleItemHandler>();

    builder.Services.AddScoped<IValidator<CreateSaleCommand>, CreateSaleValidator>();
    builder.Services.AddScoped<IValidator<UpdateSaleCommand>, UpdateSaleValidator>();
    builder.Services.AddScoped<IValidator<CancelSaleCommand>, CancelSaleValidator>();
    builder.Services.AddScoped<IValidator<GetSaleQuery>, GetSaleValidator>();
    builder.Services.AddScoped<IValidator<GetSalesQuery>, GetSalesValidator>();
    builder.Services.AddScoped<IValidator<AddSaleItemCommand>, AddSaleItemValidator>();
    builder.Services.AddScoped<IValidator<RemoveSaleItemCommand>, RemoveSaleItemValidator>();
  }
}