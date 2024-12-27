using AspireShop.BasketService.Models;

namespace AspireShop.BasketService.Repositories;

public interface IBasketRepository
{
    Task<CustomerBasket?> FindBasketAsync(string customerId);
    IEnumerable<string> GetUsers();
    Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket);
    Task<bool> DeleteBasketAsync(string id);
}

