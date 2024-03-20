using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
namespace PlaywrightTests;

public class ProductTests : PageTest
{
    IPage page;
    IBrowser browser;

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
    }

     [Test]
     [Order(0)]
    public async Task DodajProduct_Uspesno()
    {
        await page.GotoAsync($"http://localhost:3000/pages/SignIn");
        Assert.That(page.Url, Is.EqualTo("http://localhost:3000/pages/SignIn"));

        await page.FillAsync("#email", "Test11@gmail.com");

        await page.FillAsync("#password", "123");

        var formFilled = await page.EvaluateAsync<bool>(@"() => {
                const inputs = Array.from(document.getElementsByTagName('input'));
                return inputs.every(input => input.value !== '');
                }");
        Assert.That(formFilled, Is.True);

        await page.ClickAsync("#signin1");

        await page.GotoAsync("http://localhost:3000/pages/addProduct");

        await page.FillAsync("#name", "Laptop22");
        await page.FillAsync("#description", "Aspire522");

        await page.SelectOptionAsync("#selectMenu", new SelectOptionValue { Index = 2 });

        await page.ClickAsync("#addproduct");
    }


    [Test]
    [Order(1)]
    public async Task KreirajRezervaciju_Uspesno()
    {

        await page.GotoAsync($"http://localhost:3000/pages/SignIn");
        Assert.That(page.Url, Is.EqualTo("http://localhost:3000/pages/SignIn"));

        await page.FillAsync("#email", "Test11@gmail.com");

        await page.FillAsync("#password", "123");

        var formFilled = await page.EvaluateAsync<bool>(@"() => {
                const inputs = Array.from(document.getElementsByTagName('input'));
                return inputs.every(input => input.value !== '');
                }");
        Assert.That(formFilled, Is.True);

        await page.ClickAsync("#signin1");

        await page.GotoAsync($"http://localhost:3000");

        string reserveButtonId = $"#reserve1Button_65c7d98dd62339c590adf065";
        await page.ClickAsync(reserveButtonId);

        await page.GotoAsync($"http://localhost:3000/pages/myReservations");

    } 

    [Test]
    [Order(2)]
    public async Task DeleteProduct()
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

        await page.GotoAsync($"http://localhost:3000/pages/myproducts");

        string deleteBtn = $"#deleteButton_65fb26d0542c960d2df03d77";

        await page.ClickAsync(deleteBtn);

        await page.GotoAsync($"http://localhost:3000/pages/myproducts");

    }


    [TearDown]
    public async Task Teardown()
    {
        await page.CloseAsync();
        await browser.DisposeAsync();
    }
}

