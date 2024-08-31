using api.Dtos.Product;
using api.Models;

namespace api.Repository;

public interface IProductService
{
    public Task<ProductResponseDto> SetProduct(ProductRequestDto newProduct);
    public Task<Dictionary<string, TransactionsResponseDto>> GetTransactions(string Email);
}