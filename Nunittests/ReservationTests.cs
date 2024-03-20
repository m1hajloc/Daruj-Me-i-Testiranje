using Controllers;
using Services;
using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.IO;
using DTOs;
using Context;
using System.Threading.Tasks;

namespace Nunittests
{

    [TestFixture]
    public class ReservationTests
    {
        private IConfiguration _configuration;
        private MongoDbContext _mongoDbContext;
        private ReservationController _reservationController;
        private UserController _userController;
        private ProductController _productController;

        private List<Reservation> testReservations;

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _mongoDbContext = new MongoDbContext(_configuration);
            _reservationController = new ReservationController(_mongoDbContext);
            _productController = new ProductController(_mongoDbContext);
            _userController = new UserController(_mongoDbContext);
            _productController = new ProductController(_mongoDbContext);
            testReservations = new List<Reservation>();
        }
        [Test]
        public async Task TestCreateReservation()
        {
            // Arrange
            var reservation = new Reservation
            {
                OwnerId = "65c34f3b1b3f612af500363e",
                ProductId = "65fb27fe542c960d2df03d79",
                CustomerId = "65c681579111249f03cffa39"
            };

            var result = await _reservationController.CreateReservation(reservation);
            var okResult = result as OkObjectResult;
            var createdReservation = okResult.Value as Reservation;

            Assert.IsNotNull(createdReservation);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(reservation.OwnerId, createdReservation.OwnerId);
            Assert.AreEqual(reservation.ProductId, createdReservation.ProductId);
            Assert.AreEqual(reservation.CustomerId, createdReservation.CustomerId);

            testReservations.Add(createdReservation);
        }

        [Test]
        public async Task TestCreateReservation_InvalidData()
        {
            // Arrange: Prepare invalid reservation data
            var invalidReservation = new Reservation
            {
                // Provide invalid data here, such as null or empty strings
                OwnerId = null, // Example of invalid owner ID
                ProductId = "65c7bda5d62339c590adf062", // Example of valid product ID
                CustomerId = "65c34f3b1b3f612af500363e" // Example of valid customer ID
            };

            // Act: Attempt to create the reservation with invalid data
            var result = await _reservationController.CreateReservation(invalidReservation);

            // Assert: Check if the result is a bad request
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task TestDeleteReservation()
        {
            // Arrange
            var reservation = new Reservation
            {
                OwnerId = "65c34f3b1b3f612af500363e",
                ProductId = "65fb27fe542c960d2df03d79",
                CustomerId = "65c681579111249f03cffa39"
            };

            var createResult = await _reservationController.CreateReservation(reservation);
            var createdReservation = (createResult as OkObjectResult)?.Value as Reservation;

            var deleteResult = await _reservationController.DeleteReservation(createdReservation.Id);

            Assert.IsInstanceOf<OkResult>(deleteResult);

            var getResult = await _reservationController.GetReservationById(createdReservation.Id);
            Assert.IsInstanceOf<BadRequestObjectResult>(getResult);
        }

        [Test]
        public async Task TestDeleteReservation_Unsuccessful()
        {
            string invalidReservationId = "65c94978f006912531ebea77";

            var result = await _reservationController.DeleteReservation(invalidReservationId);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task TestGetAllReservations()
        {
            var res1 = new Reservation
            {
                OwnerId = "65c34f3b1b3f612af500363e",
                ProductId = "65fb27fe542c960d2df03d79",
                CustomerId = "65c681579111249f03cffa39"
            };

            var res2 = new Reservation
            {
                OwnerId = "65c34f3b1b3f612af500363e",
                ProductId = "65fb27fe542c960d2df03d79",
                CustomerId = "65c681579111249f03cffa39"
            };

            var result1=await _reservationController.CreateReservation(res1);
            var okResult1 = result1 as OkObjectResult;
            var reservation1 = okResult1?.Value as Reservation;
            var result2=await _reservationController.CreateReservation(res2);
            var okResult2 = result2 as OkObjectResult;
            var reservation2 = okResult2?.Value as Reservation;

            var result = await _reservationController.GetAllReservations();

            Assert.IsInstanceOf<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            var reservations = okResult.Value as IQueryable<Reservation>;

            Assert.IsNotNull(reservations);

            Assert.IsTrue(reservations.Any(r => r.Id == reservation1.Id));
            Assert.IsTrue(reservations.Any(r => r.Id == reservation2.Id));
        }

        [Test]
        public async Task TestGetReservationById()
        {
            Reservation reservation = new Reservation
            {
                OwnerId = "65c34f3b1b3f612af500363e",
                ProductId = "65fb27fe542c960d2df03d79",
                CustomerId = "65c681579111249f03cffa39"
            };

            var result = await _reservationController.CreateReservation(reservation);
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var reservationn = okResult?.Value as Reservation;
            testReservations.Add(reservationn);

            Assert.IsNotNull(okResult, "Povratna vrednost nije instanca OkObjectResult-a.");

            var result2 = await _reservationController.GetReservationById(reservationn.Id);
            Assert.IsInstanceOf<OkObjectResult>(result2);
            var okk = result2 as OkObjectResult;

            var retrievedreservation = okk?.Value as Reservation;
            Assert.IsNotNull(retrievedreservation, "Povratna vrednost je null.");
            Assert.AreEqual(retrievedreservation.Id, reservationn.Id, "Retrieved user ID does not match registered user ID");
        }

         [Test]
        public async Task TestGetReservationBadId()
        {
            string Id = "65c6bc387b3cef9d80d81f18";

            var result = await _reservationController.GetReservationById(Id);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var resultBad = (BadRequestObjectResult)result;

            Assert.IsNotNull(resultBad);


        }



        [TearDown]
        public async Task Cleanup()
        {
            foreach (var reservation in testReservations)
            {
                await _reservationController.DeleteReservation(reservation.Id);
            }
        }

    }
}