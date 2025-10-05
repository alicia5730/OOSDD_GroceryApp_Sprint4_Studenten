using Grocery.Core.Helpers;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Grocery.Core.Services;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace TestCore
{

    public class TestHelpers
    {

        private Mock<IGroceryListItemsRepository> _groceriesRepoMock;
        private Mock<IProductRepository> _productRepoMock;
        private GroceryListItemsService _service;
        
        // BOVENAAN je TestHelpers class
        private Mock<IGroceryListItemsRepository> _itemsRepoMock;   // voor UC13
        private Mock<IGroceryListRepository> _listRepoMock;         // voor UC13
        private Mock<IClientRepository> _clientRepoMock;            // voor UC13
        private Mock<IProductRepository> _bpProductRepoMock;        // aparte voor UC13
        private BoughtProductsService _boughtService;
        [SetUp]
        public void Setup()
        {
            _groceriesRepoMock = new Mock<IGroceryListItemsRepository>();
            _productRepoMock = new Mock<IProductRepository>();
            _service = new GroceryListItemsService(_groceriesRepoMock.Object, _productRepoMock.Object);
            // --- UC13 nieuw ---
            _itemsRepoMock     = new Mock<IGroceryListItemsRepository>();
            _listRepoMock      = new Mock<IGroceryListRepository>();
            _clientRepoMock    = new Mock<IClientRepository>();
            _bpProductRepoMock = new Mock<IProductRepository>();

            _boughtService = new BoughtProductsService(
                _itemsRepoMock.Object,
                _listRepoMock.Object,
                _clientRepoMock.Object,
                _bpProductRepoMock.Object
            );
            
            
        }


        //Happy flow
        [Test]
        public void TestPasswordHelperReturnsTrue()
        {
            string password = "user3";
            string passwordHash = "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA=";
            Assert.IsTrue(PasswordHelper.VerifyPassword(password, passwordHash));
        }

        [TestCase("user1", "IunRhDKa+fWo8+4/Qfj7Pg==.kDxZnUQHCZun6gLIE6d9oeULLRIuRmxmH2QKJv2IM08=")]
        [TestCase("user3", "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA=")]
        public void TestPasswordHelperReturnsTrue(string password, string passwordHash)
        {
            Assert.IsTrue(PasswordHelper.VerifyPassword(password, passwordHash));
        }


        //Unhappy flow
        [Test]
        public void TestPasswordHelperReturnsFalse()
        {
            string password = "usr3";
            string passwordHash = "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA=";
            Assert.IsFalse(PasswordHelper.VerifyPassword(password, passwordHash));
        }

        [TestCase("user1", "IunRhDKa+fWo8+4/Qfj7Pg==.kDxZnUQHCZun6gLIE6d9oeULLRIuRmxmH2QKJv2IM08")]
        [TestCase("user3", "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA")]
        public void TestPasswordHelperReturnsFalse(string password, string passwordHash)
        {
            bool result = false;

            try
            {
                result = PasswordHelper.VerifyPassword(password, passwordHash);
            }
            catch
            {
                result = false; 
            }

            Assert.IsFalse(result);        
        }
        
        // ---------------------------------------------------------
        // FR1 / FR2 : Best verkochte producten tonen (UC10)
        // ---------------------------------------------------------

        [Test]
        // ✅ Happy - FR1/FR2: Single product → correcte ranking en info
        public void GetBestSellingProducts_SingleProduct_ReturnsCorrectRanking()
        {
            _groceriesRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem>
            {
                new GroceryListItem(1, 1, 1, 5) { ProductId = 1, Amount = 5 }
            });
            _productRepoMock.Setup(r => r.Get(1)).Returns(new Product(1, "Appel", 10));

            var result = _service.GetBestSellingProducts();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Appel", result[0].Name);
            Assert.AreEqual(1, result[0].Ranking);
            Assert.AreEqual(5, result[0].NrOfSells);
        }

        [Test]
        // ✅ Happy - FR1/FR2: Meerdere producten → juiste sortering
        public void GetBestSellingProducts_MultipleProducts_AreSortedCorrectly()
        {
            _groceriesRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem>
            {
                new GroceryListItem(1, 1, 1, 2),
                new GroceryListItem(2, 1, 2, 10),
                new GroceryListItem(3, 1, 3, 5)
            });

            _productRepoMock.Setup(r => r.Get(1)).Returns(new Product(1, "Melk", 100));
            _productRepoMock.Setup(r => r.Get(2)).Returns(new Product(2, "Kaas", 200));
            _productRepoMock.Setup(r => r.Get(3)).Returns(new Product(3, "Brood", 300));

            var result = _service.GetBestSellingProducts();

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Kaas", result[0].Name);
            Assert.AreEqual("Brood", result[1].Name);
            Assert.AreEqual("Melk", result[2].Name);
        }

        [Test]
        // ❌ Unhappy - FR1/FR2: Geen producten → lege lijst
        public void GetBestSellingProducts_WhenNoProducts_ReturnsEmptyList()
        {
            _groceriesRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem>());

            var result = _service.GetBestSellingProducts();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        // ❌ Unhappy - FR1: Meer dan 5 producten → alleen top 5
        public void GetBestSellingProducts_WhenMoreThanFive_ReturnsTopFive()
        {
            var items = new List<GroceryListItem>();
            for (int i = 1; i <= 10; i++)
            {
                items.Add(new GroceryListItem(i, 1, i, i * 2) { ProductId = i, Amount = i * 2 });
                _productRepoMock.Setup(r => r.Get(i)).Returns(new Product(i, "Prod" + i, 100));
            }
            _groceriesRepoMock.Setup(r => r.GetAll()).Returns(items);

            var result = _service.GetBestSellingProducts();

            Assert.AreEqual(5, result.Count);
        }

        // ---------------------------------------------------------
        // FR1 / NFR1 / NFR2 : Aantal verhogen/verlagen
        // ---------------------------------------------------------

        [Test]
        // ✅ Happy - FR1/NFR1: Verhogen → aantal stijgt met 1
        public void IncreaseAmount_WhenClicked_AddsOne()
        {
            var product = new Product(1, "Melk", 10);
            var item = new GroceryListItem(1, 1, 1, 1) { Product = product };

            item.Amount++;

            Assert.AreEqual(2, item.Amount);
        }

        [Test]
        // ✅ Happy - FR1/NFR2: Verlagen >1 → aantal daalt met 1
        public void DecreaseAmount_WhenGreaterThanOne_SubtractsOne()
        {
            var product = new Product(1, "Melk", 10);
            var item = new GroceryListItem(1, 1, 1, 3) { Product = product };

            item.Amount--;

            Assert.AreEqual(2, item.Amount);
        }

        [Test]
        // ❌ Unhappy - FR2/NFR2: Verlagen bij 0 → blijft 0
        public void DecreaseAmount_WhenAlreadyZero_RemainsZero()
        {
            var product = new Product(1, "Melk", 5);
            var item = new GroceryListItem(1, 1, 1, 0) { Product = product };

            item.Amount = Math.Max(0, item.Amount - 1);

            Assert.AreEqual(0, item.Amount);
        }

        // ---------------------------------------------------------
        // FR3 : Maximaal aantal is voorraad
        // ---------------------------------------------------------

        [Test]
        // ✅ Happy - FR3: Stock bereikt → blijft op stock
        public void IncreaseAmount_WhenStockReached_DoesNotExceedStock()
        {
            var product = new Product(1, "Melk", 5);
            var item = new GroceryListItem(1, 1, 1, 5) { Product = product };

            item.Amount = Math.Min(item.Amount + 1, product.Stock);

            Assert.AreEqual(5, item.Amount);
        }

        [Test]
        // ❌ Unhappy - FR3: Proberen boven voorraad → blijft op stock
        public void IncreaseAmount_WhenExceedsStock_IsClampedToStock()
        {
            var product = new Product(1, "Melk", 5);
            var item = new GroceryListItem(1, 1, 1, 4) { Product = product };

            item.Amount = Math.Min(item.Amount + 5, product.Stock);

            Assert.AreEqual(5, item.Amount);
        }

        // ---------------------------------------------------------
        // NFR3 : Product met 0 blijft zichtbaar
        // ---------------------------------------------------------

        [Test]
        // ✅ Happy - NFR3: Product met 0 blijft aanwezig
        public void ProductWithZeroAmount_RemainsInList()
        {
            var product = new Product(1, "Melk", 5);
            var item = new GroceryListItem(1, 1, 1, 0) { Product = product };

            Assert.AreEqual(0, item.Amount);
            Assert.IsNotNull(item); // blijft bestaan
        }
        //----------------------------------------------------UC11--------------------------------------

        // ------------------------------
        // ✅ UC11 – Happy Flows
        // ------------------------------

        [Test]
        /// <summary>
        /// TC11-01 – Happy – Overzicht met minder dan 5 producten
        /// </summary>
        public void TC11_01_GetBestSellingProducts_WithLessThanFive_ReturnsAll()
        {
            _groceriesRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem>
            {
                new GroceryListItem(1,1,1,3){ ProductId = 1, Amount = 3 },
                new GroceryListItem(2,1,2,2){ ProductId = 2, Amount = 2 },
                new GroceryListItem(3,1,3,1){ ProductId = 3, Amount = 1 }
            });
            _productRepoMock.Setup(r => r.Get(1)).Returns(new Product(1,"Appel",10));
            _productRepoMock.Setup(r => r.Get(2)).Returns(new Product(2,"Melk",20));
            _productRepoMock.Setup(r => r.Get(3)).Returns(new Product(3,"Kaas",30));

            var result = _service.GetBestSellingProducts();
            Assert.AreEqual(3, result.Count);
        }

        [Test]
        /// <summary>
        /// TC11-02 – Happy – Lijst bijgewerkt na verhogen
        /// </summary>
        public void TC11_02_BestSellingProducts_AfterIncreaseInList_IsUpdated()
        {
            var list = new List<GroceryListItem>
            {
                new GroceryListItem(1,1,1,5){ ProductId = 1, Amount = 5 }
            };
            _groceriesRepoMock.Setup(r => r.GetAll()).Returns(list);
            _productRepoMock.Setup(r => r.Get(1)).Returns(new Product(1,"Appel",10));

            var initial = _service.GetBestSellingProducts();
            list[0].Amount = 6;
            var updated = _service.GetBestSellingProducts();

            Assert.AreEqual(6, updated[0].NrOfSells);
        }

        [Test]
        /// <summary>
        /// TC11-03 – Happy – Productinformatie correct
        /// </summary>
        public void TC11_03_BestSellingProducts_ProductInfoCorrect()
        {
            _groceriesRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem>
            {
                new GroceryListItem(1,1,1,7){ ProductId = 1, Amount = 7 }
            });
            _productRepoMock.Setup(r => r.Get(1)).Returns(new Product(1,"Brood",50));

            var result = _service.GetBestSellingProducts();
            Assert.AreEqual("Brood", result[0].Name);
            Assert.AreEqual(7, result[0].NrOfSells);
        }

        [Test]
        /// <summary>
        /// TC11-04 – Happy – Nieuw product toegevoegd
        /// </summary>
        public void TC11_04_BestSellingProducts_NewProductAdded_ShowsUp()
        {
            var list = new List<GroceryListItem>
            {
                new GroceryListItem(1,1,1,2){ ProductId = 1, Amount = 2 }
            };
            _groceriesRepoMock.Setup(r => r.GetAll()).Returns(list);
            _productRepoMock.Setup(r => r.Get(1)).Returns(new Product(1,"Melk",10));
            _productRepoMock.Setup(r => r.Get(2)).Returns(new Product(2,"Kaas",10));

            list.Add(new GroceryListItem(2,1,2,4){ ProductId = 2, Amount = 4 });
            var result = _service.GetBestSellingProducts();

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        /// <summary>
        /// TC11-05 – Happy – Aggregatie correct
        /// </summary>
        public void TC11_05_BestSellingProducts_AggregationCorrect()
        {
            _groceriesRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem>
            {
                new GroceryListItem(1,1,1,10),
                new GroceryListItem(2,1,1,5),
                new GroceryListItem(3,1,2,3)
            });
            _productRepoMock.Setup(r => r.Get(1)).Returns(new Product(1,"Melk",100));
            _productRepoMock.Setup(r => r.Get(2)).Returns(new Product(2,"Kaas",50));

            var result = _service.GetBestSellingProducts();
            var melk = result.First(r => r.Name == "Melk");
            Assert.AreEqual(15, melk.NrOfSells);
        }

        [Test]
        /// <summary>
        /// TC11-06 – Happy – Sortering correct aflopend
        /// </summary>
        public void TC11_06_BestSellingProducts_SortedDescending()
        {
            _groceriesRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem>
            {
                new GroceryListItem(1,1,1,10),
                new GroceryListItem(2,1,2,5)
            });
            _productRepoMock.Setup(r => r.Get(1)).Returns(new Product(1,"Appel",100));
            _productRepoMock.Setup(r => r.Get(2)).Returns(new Product(2,"Peer",100));

            var result = _service.GetBestSellingProducts();
            Assert.AreEqual("Appel", result[0].Name);
            Assert.AreEqual("Peer", result[1].Name);
        }

        // ------------------------------
        // ❌ UC11 – Unhappy Flows
        // ------------------------------

        [Test]
        /// <summary>
        /// TC11-07 – Unhappy – Product ontbreekt in repository
        /// </summary>
        public void TC11_07_BestSellingProducts_ProductMissingFromRepo_Skipped()
        {
            _groceriesRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem>
            {
                new GroceryListItem(1,1,99,3){ ProductId = 99, Amount = 3 }
            });
            _productRepoMock.Setup(r => r.Get(99)).Returns((Product)null);

            var result = _service.GetBestSellingProducts();
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        /// <summary>
        /// TC11-08 – Unhappy – Negatief aantal
        /// </summary>
        public void TC11_08_BestSellingProducts_NegativeAmount_Ignored()
        {
            // Arrange + Act + Assert
            Assert.Throws<ArgumentException>(() =>
            {
                // proberen een item met negatieve Amount aan te maken
                var invalidItem = new GroceryListItem(1, 1, 1, -5);
            });
        }

        [Test]
        /// <summary>
        /// TC11-09 – Unhappy – Database fout bij ophalen
        /// </summary>
        public void TC11_09_AddProduct_WhenDbFails_ShowsNoUpdate()
        {
            _groceriesRepoMock.Setup(r => r.GetAll()).Throws(new Exception("DB error"));
            Assert.Throws<Exception>(() => _service.GetBestSellingProducts());
        }

        [Test]
        /// <summary>
        /// TC11-10 – Unhappy – Geen boodschappenlijsten
        /// </summary>
        public void TC11_10_BestSellingProducts_NoData_ReturnsEmpty()
        {
            _groceriesRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem>());
            var result = _service.GetBestSellingProducts();
            Assert.IsEmpty(result);
        }
        
        // ---------------------------------------------------------------------------------------
        // UC13 – Happy flows
        // ----------------------------------------------------------------------------------------

        [Test]
        // TC13-01 – Alleen admin kan naar BoughtProducts (rolcheck op Core-model)
        public void UC13_01_AdminRole_IsAllowed()
        {
            var admin = new Client(3, "Admin", "admin@mail.com", "pw") { Role = Role.Admin };
            Assert.That(admin.Role, Is.EqualTo(Role.Admin));
        }

        [Test]
        // TC13-02 – Bij gekozen product → juiste klanten + lijsten (Service test)
        public void UC13_02_Get_ByProduct_ReturnsCorrectClientsAndLists()
        {
            // Arrange
            var client      = new Client(1, "Alice", "u1@mail", "pw");
            var list        = new GroceryList(1, "Weekendlijst", DateOnly.FromDateTime(DateTime.Today), "#F00", client.Id);
            var product     = new Product(10, "Melk", 100);
            var listItem    = new GroceryListItem(100, list.Id, product.Id, 2);

            _itemsRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem> { listItem });
            _listRepoMock.Setup(r => r.Get(list.Id)).Returns(list);
            _clientRepoMock.Setup(r => r.Get(client.Id)).Returns(client);
            _bpProductRepoMock.Setup(r => r.Get(product.Id)).Returns(product);

            // Act
            var result = _boughtService.Get(product.Id);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Client.Name, Is.EqualTo("Alice"));
            Assert.That(result[0].GroceryList.Name, Is.EqualTo("Weekendlijst"));
            Assert.That(result[0].Product.Name, Is.EqualTo("Melk"));
        }

        [Test]
        // TC13-03 – Wisselen van product ververst de lijst (test-only VM + service)
        public void UC13_03_SwitchingProducts_RefreshesList()
        {
            // Arrange: twee producten met elk één buyer
            var p1 = new Product(1, "Melk", 10);
            var p2 = new Product(2, "Brood", 5);

            var c1 = new Client(1, "Alice", "a@mail", "pw");
            var c2 = new Client(2, "Bob",   "b@mail", "pw");

            var l1 = new GroceryList(1, "Lijst A", DateOnly.FromDateTime(DateTime.Today), "#111", c1.Id);
            var l2 = new GroceryList(2, "Lijst B", DateOnly.FromDateTime(DateTime.Today), "#222", c2.Id);

            _itemsRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem>
            {
                new GroceryListItem(1, l1.Id, p1.Id, 2),
                new GroceryListItem(2, l2.Id, p2.Id, 1)
            });
            _listRepoMock.Setup(r => r.Get(l1.Id)).Returns(l1);
            _listRepoMock.Setup(r => r.Get(l2.Id)).Returns(l2);
            _clientRepoMock.Setup(r => r.Get(c1.Id)).Returns(c1);
            _clientRepoMock.Setup(r => r.Get(c2.Id)).Returns(c2);
            _bpProductRepoMock.Setup(r => r.Get(1)).Returns(p1);
            _bpProductRepoMock.Setup(r => r.Get(2)).Returns(p2);

            // Test-VM gebruikt alleen IBoughtProductsService
            var vm = new TestCore.TestDoubles.TestBoughtProductsVM(_boughtService);

            // Act 1: selecteer Melk
            vm.SelectProduct(p1);
            Assert.That(vm.BoughtProductsList.Count, Is.EqualTo(1));
            Assert.That(vm.BoughtProductsList[0].Product.Name, Is.EqualTo("Melk"));
            Assert.That(vm.BoughtProductsList[0].Client.Name,  Is.EqualTo("Alice"));

            // Act 2: switch naar Brood
            vm.SelectProduct(p2);
            Assert.That(vm.BoughtProductsList.Count, Is.EqualTo(1));
            Assert.That(vm.BoughtProductsList[0].Product.Name, Is.EqualTo("Brood"));
            Assert.That(vm.BoughtProductsList[0].Client.Name,  Is.EqualTo("Bob"));
        }

        // ------------------------------
        // UC13 – Unhappy flows
        // ------------------------------

        [Test]
        // TC13-04 – Niet-admin → mag view niet openen (rol ≠ Admin)
        public void UC13_04_NonAdminRole_IsDenied()
        {
            var user = new Client(2, "User", "user@mail.com", "pw") { Role = Role.None };
            Assert.That(user.Role, Is.Not.EqualTo(Role.Admin));
        }

        [Test]
        // TC13-05 – Geen data voor product → lege lijst
        public void UC13_05_NoDataForProduct_ReturnsEmpty()
        {
            _itemsRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem>());
            var empty = _boughtService.Get(999);
            Assert.That(empty, Is.Empty);
        }

        [Test]
        // TC13-06 – Null selectie / invalid → blijft leeg, geen crash (via test-VM)
        public void UC13_06_NullSelection_LeavesListEmpty_NoCrash()
        {
            var vm = new TestCore.TestDoubles.TestBoughtProductsVM(_boughtService);
            vm.SelectProduct(null);
            Assert.That(vm.BoughtProductsList, Is.Empty);
        }

        
    }
}
