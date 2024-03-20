using Controllers;
using Services.IServices;
using Services;
using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using System.IO;
using DTOs;
using Context;
using System.Threading.Tasks;

namespace Nunittests
{
    [TestFixture]
    public class ProductTests
    {
        private IConfiguration _configuration;
        private MongoDbContext _mongoDbContext;
        private ProductController _productController;
        private ProductTypeController _productTypeController;
        private UserController _userController;
        private IProductTypeService _productTypeService;

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _mongoDbContext = new MongoDbContext(_configuration);
            _productController = new ProductController(_mongoDbContext);
            _productTypeController = new ProductTypeController(_mongoDbContext);
            _userController = new UserController(_mongoDbContext);
        }

        [Test]
        public async Task TestDodajProduct()
        {
            CreateProductDTO product = new CreateProductDTO("Prod", "Opis", null, "65c3a22be57c67d2cbabe710", "65c29ae93e251046bd5287ae");

            var result = await _productController.CreateProduct(product, null);

            Assert.IsInstanceOf<OkObjectResult>(result);

            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult, "Povratna vrednost nije instanca OkObjectResult-a.");

            var addedProduct = okResult.Value as Product;

            Assert.IsNotNull(addedProduct, "Dodati proizvod nije instanciran.");

            Assert.IsNotNull(addedProduct.Id, "ID nije postavljen nakon dodavanja proizvoda.");

            Assert.AreEqual(addedProduct.Id, addedProduct.Id, "ID dodatog proizvoda se ne podudara sa očekivanim ID-em.");
        }

        [Test]
        public async Task TestDodajProduct_Neuspesno()
        {
            CreateProductDTO product = new CreateProductDTO(
                name: null,
                desc: "Description",
                pp: "ProfilePicture",
                productTypeId: null,
                ownerId: null
            );

            var result = await _productController.CreateProduct(product, null);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
        [Test]
        public async Task TestDodajProduct_NullProductDTO()
        {
            CreateProductDTO product = null;

            var result = await _productController.CreateProduct(product, null);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
        [Test]
        public async Task TestCreateAndDeleteProduct()
        {
            CreateProductDTO product = new CreateProductDTO("12345", "Opisssss", null, "65c3a22be57c67d2cbabe710", "65c29ae93e251046bd5287ae");

            var createResult = await _productController.CreateProduct(product, null);

            Assert.IsInstanceOf<OkObjectResult>(createResult);

            var createdProduct = (createResult as OkObjectResult).Value as Product;

            var deleteResult = await _productController.DeleteProduct(createdProduct.Id);

            Assert.IsInstanceOf<OkResult>(deleteResult);
        }

        [Test]
        public async Task TestDeleteNonExistingProduct()
        {
            string nonExistingProductId = "65ed984c454619ba306c8c64";

            var deleteResult = await _productController.DeleteProduct(nonExistingProductId);

            Assert.IsInstanceOf<BadRequestObjectResult>(deleteResult);

            var badRequestResult = deleteResult as BadRequestObjectResult;

            Assert.IsNotNull(badRequestResult.Value);

            Assert.AreEqual($"Object with ID '{nonExistingProductId}' does not exist.", badRequestResult.Value);
        }

        [Test]
        public async Task TestCreateDeleteProduct()
        {
            CreateProductDTO product = new CreateProductDTO("TestProduct", "Description", null, "65c3a22be57c67d2cbabe710", "65c29ae93e251046bd5287ae");

            var createResult = await _productController.CreateProduct(product, null);
            Assert.IsInstanceOf<OkObjectResult>(createResult);
            var createOkResult = createResult as OkObjectResult;
            Assert.IsNotNull(createOkResult, "Povratna vrednost nije instanca OkObjectResult-a.");
            var createdProduct = createOkResult.Value as Product;
            Assert.IsNotNull(createdProduct, "Dodati proizvod nije instanciran.");

            var deleteResult = await _productController.DeleteProduct(createdProduct.Id);

            var getProductResult = await _productController.GetProductById(createdProduct.Id);

            Assert.IsNotNull(getProductResult, "Jelo nije obrisano iz baze.");
        }

        [Test]
        public async Task TestGetProductsByOwnerId()
        {
            string ownerId = "65c29ae93e251046bd5287ae";

            CreateProductDTO newProduct = new CreateProductDTO("Prod", "Opis", null, "65c3a22be57c67d2cbabe710", ownerId);
            var resultCreate = await _productController.CreateProduct(newProduct, null);
            Assert.IsInstanceOf<OkObjectResult>(resultCreate);

            var actionResult = await _productController.GetProductsByOwnerId(ownerId);
            Assert.IsInstanceOf<OkObjectResult>(actionResult);

            var okResult = actionResult as OkObjectResult;
            Assert.IsNotNull(okResult);

            var products = okResult.Value as List<Product>;
            Assert.IsNotNull(products);

            Assert.IsTrue(products.Count > 0);
        }

        [Test]
        public async Task TestGetAllProductsByNullOrEmptyOwnerId()
        {
            // Arrange
            string emptyOwnerId = string.Empty;
            string nullOwnerId = null;

            // Act
            var emptyOwnerResult = await _productController.GetProductsByOwnerId(emptyOwnerId);
            var nullOwnerResult = await _productController.GetProductsByOwnerId(nullOwnerId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(emptyOwnerResult);
            Assert.IsInstanceOf<BadRequestObjectResult>(nullOwnerResult);
        }

        [Test]
        public async Task TestGetAllProductsByOwnerId_InvalidOwnerId()
        {
            string invalidOwnerId = "65c34f3b1b3f612af500363";

            var userResult = await _userController.GetUserById(invalidOwnerId);

            Assert.IsInstanceOf<BadRequestObjectResult>(userResult);
        }

        [Test]
        public async Task TestGetProductById_ValidId()
        {
            // Arrange
            string validProductId = "65fb27fe542c960d2df03d79";

            // Act
            var result = await _productController.GetProductById(validProductId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task TestGetProductById_InvalidId()
        {
            // Arrange
            string invalidProductId = null;

            // Act
            var result = await _productController.GetProductById(invalidProductId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task TestGetProductById_NonExistentId()
        {
            // Arrange
            string nonExistentProductId = "65c29ae93e251046bd5287bc";

            // Act
            var result = await _productController.GetProductById(nonExistentProductId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

    }
}
