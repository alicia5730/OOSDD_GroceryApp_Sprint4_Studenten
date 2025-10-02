using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;


namespace Grocery.App.ViewModels
{
    public partial class BoughtProductsViewModel : BaseViewModel
    {
        private readonly IBoughtProductsService _boughtProductsService;

        [ObservableProperty] private Product? selectedProduct;
        public ObservableCollection<BoughtProducts> BoughtProductsList { get; set; } = new();
        public ObservableCollection<Product> Products { get; set; }

        public BoughtProductsViewModel(IBoughtProductsService boughtProductsService, IProductService productService)
        {
            _boughtProductsService = boughtProductsService;
            Products = new(productService.GetAll());
            
            foreach (var p in Products)
            {
                System.Diagnostics.Debug.WriteLine($"[VM] Product: {p?.Id} | {p?.Name} | {p?.GetType().FullName}");
            }

        }

        partial void OnSelectedProductChanged(Product? oldValue, Product? newValue)
        {
            BoughtProductsList.Clear();
            if (newValue != null)
            {
                foreach (var bp in _boughtProductsService.Get(newValue.Id))
                {
                    BoughtProductsList.Add(bp);
                }
            }        
        }

        [RelayCommand]
        public void NewSelectedProduct(Product product)
        {
            SelectedProduct = product;
        }
    }
}
