using Grocery.Core.Services;
using Grocery.App.ViewModels;
using Grocery.App.Views;
using Microsoft.Extensions.Logging;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Data.Repositories;
using CommunityToolkit.Maui;
using Grocery.Core.Models;

namespace Grocery.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // ✅ Repositories
            builder.Services.AddSingleton<IGroceryListRepository, GroceryListRepository>();
            builder.Services.AddSingleton<IGroceryListItemsRepository, GroceryListItemsRepository>();
            builder.Services.AddSingleton<IProductRepository, ProductRepository>();
            builder.Services.AddSingleton<IClientRepository, ClientRepository>();

            // ✅ Services
            builder.Services.AddSingleton<IGroceryListService, GroceryListService>();
            builder.Services.AddSingleton<IGroceryListItemsService, GroceryListItemsService>();
            builder.Services.AddSingleton<IProductService, ProductService>();
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IClientService, ClientService>();
            builder.Services.AddSingleton<IFileSaverService, FileSaverService>();
            builder.Services.AddSingleton<IBoughtProductsService, BoughtProductsService>();

            // ✅ Global session / context
            builder.Services.AddSingleton<GlobalViewModel>();  // dit is jouw "session"

            // ✅ ViewModels & Views
            builder.Services.AddTransient<GroceryListsView>().AddTransient<GroceryListViewModel>();
            builder.Services.AddTransient<GroceryListItemsView>().AddTransient<GroceryListItemsViewModel>();
            builder.Services.AddTransient<ProductView>().AddTransient<ProductViewModel>();
            builder.Services.AddTransient<ChangeColorView>().AddTransient<ChangeColorViewModel>();
            builder.Services.AddTransient<LoginView>().AddTransient<LoginViewModel>();
            builder.Services.AddTransient<BestSellingProductsView>().AddTransient<BestSellingProductsViewModel>();
            builder.Services.AddTransient<BoughtProductsView>().AddTransient<BoughtProductsViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
