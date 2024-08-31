using api.Data;
using api.Dtos.Product;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Repository;

public class ProductService : IProductService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public ProductService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    
    
    
    public async Task<ProductResponseDto> SetProduct(ProductRequestDto newProduct)
    {
        if (newProduct.CardNumber is null || newProduct.ProductId is null || newProduct.Category is null)
            return new ProductResponseDto(){Message = "Enter the empty fields"};
        if (newProduct.Paid == 0)
            return new ProductResponseDto(){Message = "Enter the amount of money"};

        var card =await _context.Card.FindAsync(newProduct.CardNumber);
        if (card is null)
            return new ProductResponseDto() { Message = "Card isn't exist" };
        
        var userId = card.AppUserId;
        
        
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new ProductResponseDto(){Message = "User isn't exist"};

        if (card.TotalBalance < newProduct.Paid)
            return new ProductResponseDto() {Message = "Don't have enough money in this card" };
        var product = new Products()
        {
            ProductIdnetifier = newProduct.ProductId,
            Category = newProduct.Category,
            CardNumber = newProduct.CardNumber,
            Paid = newProduct.Paid,
            PaymentTime = DateTime.Now,
            AppUserId = user.Id
        };

        card.TotalBalance -= newProduct.Paid;
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        
        var response = new ProductResponseDto() {
            Message = "",
            ProductId = newProduct.ProductId,
            Category = newProduct.Category,
            CardNumber = newProduct.CardNumber,
            Paid = newProduct.Paid,
            PaymentTime = DateTime.Now,
            
        };

        return response;
    }

    public async Task<Dictionary<string, TransactionsResponseDto>> GetTransactions(string Email)
    {
        var user = await _userManager.FindByEmailAsync(Email);
        if (user is null)
            return new Dictionary<string, TransactionsResponseDto>();

        var userProducts = await _context.Products
            .Where(x => x.AppUserId == user.Id)
            .ToListAsync();

        var result = userProducts
            .GroupBy(x => x.Category)
            .ToDictionary(
                g => g.Key,
                g => new TransactionsResponseDto
                {
                    TotalPaid = g.Sum(p => p.Paid),
                    LastDate = g.Max(p => p.PaymentTime),
                });

        return result;
    }


}