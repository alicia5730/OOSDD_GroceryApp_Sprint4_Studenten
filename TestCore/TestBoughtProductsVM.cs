// Test-only VM (zet dit in TestCore, bv. in UC13_TestDoubles.cs)
using System.Collections.ObjectModel;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace TestCore.TestDoubles
{
    public class TestBoughtProductsVM
    {
        private readonly IBoughtProductsService _service;

        public ObservableCollection<BoughtProducts> BoughtProductsList { get; } = new();

        public TestBoughtProductsVM(IBoughtProductsService service)
        {
            _service = service;
        }

        // Simuleert OnSelectedProductChanged
        public void SelectProduct(Product? product)
        {
            BoughtProductsList.Clear();
            if (product == null) return;

            foreach (var bp in _service.Get(product.Id))
            {
                BoughtProductsList.Add(bp);
            }
        }
    }
}