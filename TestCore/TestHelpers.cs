using Grocery.Core.Helpers;

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
            Assert.Pass(); //Zelf uitwerken
        }

        [TestCase("user1", "IunRhDKa+fWo8+4/Qfj7Pg==.kDxZnUQHCZun6gLIE6d9oeULLRIuRmxmH2QKJv2IM08")]
        [TestCase("user3", "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA")]
        public void TestPasswordHelperReturnsFalse(string password, string passwordHash)
        {
            Assert.Fail(); //Zelf uitwerken zodat de test slaagt!
        }
    }
}