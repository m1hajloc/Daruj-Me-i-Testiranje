using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
namespace PlaywrightTests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class AdminTests : PageTest
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
                BaseURL = "https://localhost:7294",
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

            await page.GotoAsync("http://localhost:3000/pages/SignIn");

        }


        [Test]
        [Order(4)]
        public async Task Admin_DodajeiBrise_Uspesno()
        {
            Assert.That(page.Url, Is.EqualTo("http://localhost:3000/pages/SignIn"));

            await page.FillAsync("#email", "Test11@gmail.com");

            await page.FillAsync("#password", "123");



            await page.ClickAsync("#signin1");
            await page.ClickAsync("#collapsible-nav-dropdown");
            await page.ClickAsync("#prType");

            await page.FillAsync("#ptname", "Test Tip");
            await page.ClickAsync("#creatept");
            var productTypeItems = await page.QuerySelectorAllAsync(".type-item");

            Assert.That(productTypeItems.Count, Is.GreaterThan(0));

            var lastProductTypeItem = productTypeItems[^1];
            var deleteButton = await lastProductTypeItem.QuerySelectorAsync(".type-trash");
            await deleteButton.ClickAsync();

        }
        [Test]
        [Order(5)]
        public async Task Admin_ReportDobar_Uspesno()
        {
            Assert.That(page.Url, Is.EqualTo("http://localhost:3000/pages/SignIn"));

            await page.FillAsync("#email", "Test11@gmail.com");

            await page.FillAsync("#password", "123");



            await page.ClickAsync("#signin1");
            var component = await page.QuerySelectorAsync(".myProducts");
            string reserveBtn = $"#reserveButton_65fb26d0542c960d2df03d77";

            await page.ClickAsync(reserveBtn);
            await page.FillAsync("#textmodal1", "Test Test");

            await page.ClickAsync("#btnmodal1");

            await page.ClickAsync("#collapsible-nav-dropdown");
            await page.ClickAsync("#repprod");
            await page.ClickAsync("#okprod");

        }
        [Test]
        [Order(6)]
        public async Task Admin_ReportLos_Uspesno()
        {
            Assert.That(page.Url, Is.EqualTo("http://localhost:3000/pages/SignIn"));

            await page.FillAsync("#email", "Test11@gmail.com");

            await page.FillAsync("#password", "123");



            await page.ClickAsync("#signin1");
            var component = await page.QuerySelectorAsync(".myProducts");
            string reserveBtn = $"#reserveButton_65fb26d0542c960d2df03d77";

            await page.ClickAsync(reserveBtn);
            await page.FillAsync("#textmodal1", "Test Test");

            await page.ClickAsync("#btnmodal1");

            await page.ClickAsync("#collapsible-nav-dropdown");
            await page.ClickAsync("#repprod");
            await page.ClickAsync("#delprod");

        }
         [TearDown]
        public async Task Teardown()
        {
            await page.CloseAsync();
            await browser.DisposeAsync();
        }

    }
}

