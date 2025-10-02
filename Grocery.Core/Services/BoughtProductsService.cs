
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        public BoughtProductsService(IGroceryListItemsRepository groceryListItemsRepository, IGroceryListRepository groceryListRepository, IClientRepository clientRepository, IProductRepository productRepository)
        {
            _groceryListItemsRepository=groceryListItemsRepository;
            _groceryListRepository=groceryListRepository;
            _clientRepository=clientRepository;
            _productRepository=productRepository;
        }
        public List<BoughtProducts> Get(int? productId)
        {
            var result = new List<BoughtProducts>();

            if (productId == null) return result;

            // alle items met dit product ophalen
            var items = _groceryListItemsRepository.GetAll()
                .Where(i => i.ProductId == productId)
                .ToList();

            foreach (var item in items)
            {
                var groceryList = _groceryListRepository.Get(item.GroceryListId);
                var client = groceryList != null ? _clientRepository.Get(groceryList.ClientId) : null;
                var product = _productRepository.Get(item.ProductId);

                if (client != null && groceryList != null && product != null)
                {
                    result.Add(new BoughtProducts(client, groceryList, product));
                }
            }

            return result;
        }
    }
}
