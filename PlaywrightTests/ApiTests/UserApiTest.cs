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
    public class Tests12 : PlaywrightTest
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
        public async Task DodajKorisnika_DuplikatEmail()
        {

            var user2 = new
            {
                name = "admad",
                lastname = "d",
                username = "admiaan2",
                email = "mi@gmail.com",
                password = "admin",
                repeatedpassword = "admin",
                profilepicture = "",
                phonenumber = "0625467",
                city = "admin",
                adress = "admin"
            };

            await using var response2 = await Request.PostAsync("/api/User/Register", new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
                DataObject = user2
            }); ;


            Assert.That(response2.Status, Is.EqualTo(400));
            var textResponse = await response2.TextAsync();

        }
        [Test]
        public async Task PreuzmiUsera_Uspesno()
        {
            await using var response = await Request.GetAsync($"/api/User/GetUserById?id=65c34f3b1b3f612af500363e");

            Assert.That(response.Status, Is.EqualTo(200));
            var textResponse = await response.TextAsync();
            Assert.IsNotNull(textResponse);

            var jsonResponse = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(textResponse);

            Assert.That(jsonResponse["id"].ToString(), Is.EqualTo("65c34f3b1b3f612af500363e"));
        }
        [Test]
        public async Task PreuzmiUsera_Neuspesno()
        {
            string nepostojeciId = "65ed984c454619ba306c8c63";

            await using var response = await Request.GetAsync($"/api/User/GetUserById?id={nepostojeciId}");
            Assert.That(response.Status, Is.EqualTo(404).Or.EqualTo(400));
        }
        public async Task ObrisiUsera_Uspesno()
        {
            await using var response = await Request.GetAsync($"/api/User/DeleteUser?Id=65fb4207b51b84fdcc8c7066");

            Assert.That(response.Status, Is.EqualTo(200));
            var textResponse = await response.TextAsync();
            Assert.IsNotNull(textResponse);
        }
        [Test]
        public async Task ObrisiUsera_Neuspesno()
        {
            string nepostojeciId = "65ed984c454619ba306c8c63";

            await using var response = await Request.GetAsync($"/api/User/DeleteUser?id=65ed984c454619ba306c8c63");

            Assert.That(response.Status, Is.EqualTo(405).Or.EqualTo(400)); ;

        }

    }
}

