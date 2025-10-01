using CommunityToolkit.Mvvm.ComponentModel;

namespace Grocery.Core.Models
{
    public partial class GroceryListItem : ObservableObject
    {
        public int GroceryListId { get; set; }
        public int ProductId { get; set; }

        [ObservableProperty]
        private int amount;

        [ObservableProperty]
        private Product product;

        public GroceryListItem(int id, int groceryListId, int productId, int amount) 
        {
            Id = id;
            GroceryListId = groceryListId;
            ProductId = productId;
            Amount = amount;
            Product = new Product(0, "None", 0);
        }

        public int Id { get; set; }  
    }
}
