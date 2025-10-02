using CommunityToolkit.Mvvm.ComponentModel;

namespace Grocery.Core.Models
{
    public partial class GroceryListItem : ObservableObject
    {
        public int GroceryListId { get; set; }
        public int ProductId { get; set; }

        private int amount;
        public int Amount
        {
            get => amount;
            set
            {
                if (value < 0) // ❗ 0 is toegestaan, negatief niet
                    throw new ArgumentException("Amount cannot be negative");
                SetProperty(ref amount, value);
            }
        }

        [ObservableProperty]
        private Product product;

        public GroceryListItem(int id, int groceryListId, int productId, int amount) 
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative");

            Id = id;
            GroceryListId = groceryListId;
            ProductId = productId;
            Amount = amount;
            Product = new Product(0, "None", 0);
        }

        public int Id { get; set; }  
    }
}