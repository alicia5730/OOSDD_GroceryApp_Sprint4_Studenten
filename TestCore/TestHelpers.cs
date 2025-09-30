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
        [SetUp]
        public void Setup()
        {
            _groceriesRepoMock = new Mock<IGroceryListItemsRepository>();
            _productRepoMock = new Mock<IProductRepository>();
            _service = new GroceryListItemsService(_groceriesRepoMock.Object, _productRepoMock.Object);
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

        [Test]
        public void GetBestSellingProducts_EmptyList_ReturnsEmpty()
        {
            // Arrange
            _groceriesRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem>());

            // Act
            var result = _service.GetBestSellingProducts();

            // Assert
            Assert.IsEmpty(result);
        }
        [Test]
        public void GetBestSellingProducts_SingleProduct_RankingIs1()
        {
            // Arrange
            _groceriesRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem>
            {
                new GroceryListItem(1, 1, 1, 5) { ProductId = 1, Amount = 5 }
            });

            _productRepoMock.Setup(r => r.Get(1)).Returns(new Product(1, "Appel", 10));

            // Act
            var result = _service.GetBestSellingProducts();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Ranking);
            Assert.AreEqual("Appel", result[0].Name);
            Assert.AreEqual(5, result[0].NrOfSells);
        }

        // 3. Meerdere producten → juiste sortering en ranking
        [Test]
        public void GetBestSellingProducts_MultipleProducts_SortedByNrOfSells()
        {
            // Arrange
            _groceriesRepoMock.Setup(r => r.GetAll()).Returns(new List<GroceryListItem>
            {
                new GroceryListItem(1, 1, 1, 2),   // ProductId = 1, Amount = 2
                new GroceryListItem(2, 1, 2, 10),  // ProductId = 2, Amount = 10
                new GroceryListItem(3, 1, 3, 5)    // ProductId = 3, Amount = 5

            });

            _productRepoMock.Setup(r => r.Get(1)).Returns(new Product(1, "Melk", 300));
            _productRepoMock.Setup(r => r.Get(2)).Returns(new Product(2, "Kaas", 100));
            _productRepoMock.Setup(r => r.Get(3)).Returns(new Product(3, "Brood", 400));

            var result = _service.GetBestSellingProducts();

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Kaas", result[0].Name);   // hoogste verkopen
            Assert.AreEqual("Brood", result[1].Name); 
            Assert.AreEqual("Melk", result[2].Name);  
            Assert.AreEqual(1, result[0].Ranking);
            Assert.AreEqual(2, result[1].Ranking);
            Assert.AreEqual(3, result[2].Ranking);
        }
    }
}