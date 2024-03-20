using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
namespace PlaywrightTests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class UserTests : PageTest
    {
        IPage page;
        IBrowser browser;

        private IAPIRequestContext Request;
        [SetUp]
        public async Task Setup()
        {
            browser = await Playwright.Chromium.LaunchAsync(new()
            {
                Headless = false,
                SlowMo = 2000
            });

            page = await browser.NewPageAsync(new()
            {
                ViewportSize = new()
                {
                    Width = 1280,
                    Height = 720
                },
                ScreenSize = new()
                {
                    Width = 1280,
                    Height = 720
                },
                RecordVideoSize = new()
                {
                    Width = 1280,
                    Height = 720
                },
                RecordVideoDir = "../../../Videos"
            });

            var headers = new Dictionary<string, string>
            {
                { "Accept", "application/json" }
            };
            Request = await Playwright.APIRequest.NewContextAsync(new()
            {
                BaseURL = "https://localhost:5238",
                ExtraHTTPHeaders = headers,
                IgnoreHTTPSErrors = true
            });
            Dictionary<string, string> headers2 = new()
            {
                { "Content-Type", "application/json" }
            };
               await using var response2 = await Request.PostAsync("/api/User/Register", new APIRequestContextOptions()
               {

                   Headers = headers2,
                   DataObject = new
                   {
                       name = "admin2",
                       lastname = "admin2",
                       username = "admin2",
                       email = "admin2@gmail.com",
                       password = "admin2",
                       repeatedpassword = "admin2",
                       profilepicture = "",
                       phonenumber = "0625467",
                       city = "admin",
                       adress = "admin"
                   }
               });

            await page.GotoAsync("http://localhost:3000");

        }

        [Test]
        [Order(0)]
        public async Task Register_User_Uspesno()
        {
            await page.GotoAsync($"http://localhost:3000/pages/register");
            Assert.That(page.Url, Is.EqualTo("http://localhost:3000/pages/register"));

            await page.ScreenshotAsync(new() { Path = "../../../Slike/Slika1Registracija.png" });



            await page.FillAsync("#name", "test1");
            await page.FillAsync("#lastname", "test1");
            await page.FillAsync("#username", "test2!@");
            await page.FillAsync("#city", "test1");
            await page.FillAsync("#email", "test2@gmail.com");
            await page.FillAsync("#adress", "test1");
            await page.FillAsync("#phoneNumber", "06266345");
            await page.FillAsync("#password", "123");
            await page.FillAsync("#repeatedPassword", "123");



            await page.ScreenshotAsync(new() { Path = "../../../Slike/Slika2Registracija.png" });

            var formFilled = await page.EvaluateAsync<bool>(@"() => {
                const inputs = Array.from(document.getElementsByTagName('input'));
                return inputs.every(input => input.value !== '');
        }");
            await page.ClickAsync("#signin2");

            await page.GotoAsync($"http://localhost:3000/pages/SignIn");
            Assert.That(page.Url, Is.EqualTo("http://localhost:3000/pages/SignIn"));

            await page.ScreenshotAsync(new() { Path = "../../../Slike/Slika3Registracija.png" });

        }


        [Test]
        [Order(1)]
        public async Task LogIn_User_Uspesno()
        {
            await page.GotoAsync($"http://localhost:3000/pages/SignIn");
            Assert.That(page.Url, Is.EqualTo("http://localhost:3000/pages/SignIn"));

            await page.FillAsync("#email", "mi@gmail.com");

            await page.FillAsync("#password", "123");


            var formFilled = await page.EvaluateAsync<bool>(@"() => {
                const inputs = Array.from(document.getElementsByTagName('input'));
                return inputs.every(input => input.value !== '');
                }");
            Assert.That(formFilled, Is.True);


            await page.ClickAsync("#signin1");
            await Task.Delay(1000);


        }

        [Test]
        [Order(2)]
        public async Task Edit_User_Uspesno()
        {
            await page.GotoAsync($"http://localhost:3000/pages/SignIn");
            Assert.That(page.Url, Is.EqualTo("http://localhost:3000/pages/SignIn"));

            await page.FillAsync("#email", "test2@gmail.com");

            await page.FillAsync("#password", "123");


            var formFilled = await page.EvaluateAsync<bool>(@"() => {
                const inputs = Array.from(document.getElementsByTagName('input'));
                return inputs.every(input => input.value !== '');
                }");
            Assert.That(formFilled, Is.True);


            await page.ClickAsync("#signin1");
            //await Task.Delay(3000);
            await page.ClickAsync("#collapsible-nav-dropdown2");
            await page.ClickAsync("a[href='/pages/editprofile']");

            await page.FillAsync("#lastname", "TestPromenjeno");
            await page.FillAsync("#name", "TestPromenjeno");
            await page.ClickAsync("#editbtn1");

        }

        [Test]
        [Order(3)]
        public async Task Logout_User_Uspesno()
        {
            await page.GotoAsync($"http://localhost:3000/pages/SignIn");
            Assert.That(page.Url, Is.EqualTo("http://localhost:3000/pages/SignIn"));

            await page.FillAsync("#email", "test2@gmail.com");

            await page.FillAsync("#password", "123");


            var formFilled = await page.EvaluateAsync<bool>(@"() => {
                const inputs = Array.from(document.getElementsByTagName('input'));
                return inputs.every(input => input.value !== '');
                }");
            Assert.That(formFilled, Is.True);


            await page.ClickAsync("#signin1");
            await page.ClickAsync("#collapsible-nav-dropdown2");
            await page.ClickAsync("#logout1");
        }
        [TearDown]
        public async Task Teardown()
        {
            await page.CloseAsync();
            await browser.DisposeAsync();
        }
    }
}

