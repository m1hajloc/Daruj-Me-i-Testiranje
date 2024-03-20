using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightTests
{
    [TestFixture]
    public class ReservationApiTest : PlaywrightTest
    {
        private IAPIRequestContext Request;

        [SetUp]
        public async Task Setup()
        {
            try
            {
                var headers = new Dictionary<string, string>
                {
                    { "Accept", "application/json" }
                };

                Request = await Playwright.APIRequest.NewContextAsync(new()
                {
                    BaseURL = "https://localhost:7294",
                    ExtraHTTPHeaders = headers,
                    IgnoreHTTPSErrors = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting up API request context: {ex.Message}");
                throw;
            }
        }

        [Test]
        public async Task TestCreateReservation_Ok()
        {
            var novaRezervacija = new 
            {
                ownerId="65c34f3b1b3f612af500363e",
                productId="65fb27fe542c960d2df03d79",
                customerId="65fb3948b51b84fdcc8c7064"
            };

            await using var response = await Request.PostAsync("/api/Reservation/CreateReservation", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
                DataObject = novaRezervacija
            });

            Assert.That(response.Status, Is.EqualTo(200));
            var textResponse = await response.TextAsync();
        }

        [Test]
        public async Task TestGetAllReservations()
        {
            try
            {
                await using var response = await Request.GetAsync("/api/Reservation/GetAllReservations");

                Assert.That(response.Status, Is.EqualTo(200));
                var jsonResponse = await response.JsonAsync();
                Assert.That(jsonResponse, Is.Not.Null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing test: {ex.Message}");
                throw;
            }
        }

        [Test]
        public async Task TestGetreservation_ValidId_ReturnOk()
        {
            string reservationId = "65fb3e8b727aff8e8953d82c";
            await using var response = await Request.GetAsync($"/api/Reservation/GetReservationById?id={reservationId}");

            Assert.That(response.Status, Is.EqualTo(200));
            var jsonResponse = await response.JsonAsync();
            Assert.That(jsonResponse, Is.Not.Null);
        }
        public async Task DeleteReservation_ReturnsOk()
        {
            string reservationId = "65c7bf61e941a2a23615015f";

            await using var response = await Request.DeleteAsync($"/api/Reservation/DeleteReservation?id={reservationId}");

            Assert.That(response.Status, Is.EqualTo(200));
            var textResponse = await response.TextAsync();
            Assert.That(textResponse, Does.Contain("Nije uspelo brisanje rezervacije!"));
        } 
    }
}