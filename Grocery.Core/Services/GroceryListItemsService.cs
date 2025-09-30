using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class GroceryListItemsService : IGroceryListItemsService
    {
        private readonly IGroceryListItemsRepository _groceriesRepository;
        private readonly IProductRepository _productRepository;

        public GroceryListItemsService(IGroceryListItemsRepository groceriesRepository, IProductRepository productRepository)
        {
            _groceriesRepository = groceriesRepository;
            _productRepository = productRepository;
        }

        public List<GroceryListItem> GetAll()
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int groceryListId)
        {
            List<GroceryListItem> groceryListItems = _groceriesRepository.GetAll().Where(g => g.GroceryListId == groceryListId).ToList();
            FillService(groceryListItems);
            return groceryListItems;
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            return _groceriesRepository.Add(item);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        public GroceryListItem? Get(int id)
        {
            return _groceriesRepository.Get(id);
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            return _groceriesRepository.Update(item);
        }

        //een lijst met de top X best verkochte producten.
        public List<BestSellingProducts> GetBestSellingProducts(int topX = 5)
        {
            var items = _groceriesRepository.GetAll();// Hier wordt alles opgehaald van de repository
            var grouped = GroupProducts(items); // Daarna wordt het naar GroupProducts verstuurd om te groeperen
            return MapToBestSellingProducts(grouped, topX);//De gegroepeerde data wordt verstuurds naar MapToBestSellingProducts om er een object van te maken.
        }
        //tellen per product.
        private IEnumerable<(int ProductId, int NrOfSells)> GroupProducts(List<GroceryListItem> items)
        {
            return items
                .GroupBy(i => i.ProductId)
                .Select(g => (ProductId: g.Key, NrOfSells: g.Sum(i => i.Amount)));
        }
        //Objecten maken voor de ViewModel
        private List<BestSellingProducts> MapToBestSellingProducts(IEnumerable<(int ProductId, int NrOfSells)> grouped, int topX)
        {
            var result = new List<BestSellingProducts>(); //Maakt een lege lijst waar straks de resultaten in komen.
            int rank = 1; // Start de ranking lijst mee

            //Hier wordt de lijst gegroepeerd, Soorteerd aflopend op aantal verkopen, Pakt alleen de eerste x producten.
            foreach (var g in grouped.OrderByDescending(x => x.NrOfSells).Take(topX))
            {
                var product = _productRepository.Get(g.ProductId);//Haalt product info dus naam en voorraad bijv.
                result.Add(new BestSellingProducts( // maakt een nieuwe modelobject
                    g.ProductId, //Id van het product
                    product?.Name ?? "Onbekend",// product naam of onbekend als "null"
                    product?.Stock ?? 0, //voorraad of 0 als "null"
                    g.NrOfSells, // aantal keer verkocht
                    rank++ //huidige ranking + 1 erbij bij elke voeging in de result.
                ));
            }

            return result; //Geeft de complete lijst terug
        }


        private void FillService(List<GroceryListItem> groceryListItems)
        {
            foreach (GroceryListItem g in groceryListItems)
            {
                g.Product = _productRepository.Get(g.ProductId) ?? new(0, "", 0);
            }
        }
    }
}
