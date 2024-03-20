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
    public class ReportApiTest : PlaywrightTest
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
        public async Task PreuzmiReport_Uspesno()
        {
            await using var response = await Request.GetAsync($"/api/Report/GetReportById?Id=65fb3e7ab51b84fdcc8c7065");

            Assert.That(response.Status, Is.EqualTo(200));
            var textResponse = await response.TextAsync();
            Assert.IsNotNull(textResponse);

            var jsonResponse = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(textResponse);

            Assert.That(jsonResponse["id"].ToString(), Is.EqualTo("65fb3e7ab51b84fdcc8c7065"));
        }
        [Test]
        public async Task PreuzmiReport_Neuspesno()
        {
            string nepostojeciId = "65fb3e7ab51b84fdcc8c7061";

            await using var response = await Request.GetAsync($"/api/User/GetReportById?id={nepostojeciId}");

           Assert.That(response.Status, Is.EqualTo(404).Or.EqualTo(400));

        }
         public async Task ObrisiReport_Uspesno()
        {
            await using var response = await Request.GetAsync($"/api/Report/DeleteReport?id=65fb3e7ab51b84fdcc8c7061");

            Assert.That(response.Status, Is.EqualTo(200));
            var textResponse = await response.TextAsync();
            Assert.IsNotNull(textResponse);
        }
        [Test]
        public async Task ObrisiReport_Neuspesno()
        {
            string nepostojeciId = "65fb3e7ab51b84fdcc8c7060";

            await using var response = await Request.GetAsync($"/api/Report/DeleteReport?id=65fb3e7ab51b84fdcc8c7060");

            Assert.That(response.Status, Is.EqualTo(405).Or.EqualTo(400)); ;

        }
        [Test]
        public async Task TestGetAllReports()
        {
            try
            {
                await using var response = await Request.GetAsync("/api/Report/GetAllReports");

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







    }
}