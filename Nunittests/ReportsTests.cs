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
    public class ReportsTests
    {
        private IConfiguration _configuration;
        private MongoDbContext _mongoDbContext;
        private ReportController _reportsController;
        private ProductController _productController;
        private List<Report> testReports;
        private Product testproduct;
        private string idUser = "65c681579111249f03cffa39";

        [SetUp]
        public async Task Setup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _mongoDbContext = new MongoDbContext(_configuration);
            _reportsController = new ReportController(_mongoDbContext);
            _productController = new ProductController(_mongoDbContext);
            testReports = new List<Report>();

            var testproductDTO = new CreateProductDTO("Product", "Product Description", null, "65c3a223e57c67d2cbabe70f", "65c681579111249f03cffa39");
            var CreationResult = await _productController.CreateProduct(testproductDTO, null);
            var CreationOkResult = CreationResult as OkObjectResult;
            testproduct = CreationOkResult?.Value as Product;
        }

        [Test]
        public async Task TestDodajreport()
        {
            Report report = new Report
            {
                Description = "Desc",
                Product = testproduct,
                IdUser = idUser
            };

            var result = await _reportsController.CreateReport(report);
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;

            var reportt = okResult?.Value as Report;
            testReports.Add(reportt);

            Assert.IsNotNull(okResult, "Povratna vrednost nije instanca OkObjectResult-a.");
        }

        [Test]
        public async Task TestDodajReportProductNull()
        {
            Report report = new Report
            {
                Description = "Desc",
                Product = null,
                IdUser = idUser
            };

            var result = await _reportsController.CreateReport(report);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);

            Assert.AreEqual("Product is null", badRequestResult.Value);
        }

        [Test]
        public async Task TestObrisiReportUspesnoBrisanje()
        {
            Report report = new Report
            {
                Description = "Desc",
                Product = testproduct,
                IdUser = idUser
            };


            var result = await _reportsController.CreateReport(report);
            var okResult = result as OkObjectResult;
            var reportt = okResult?.Value as Report;

            var result2 = await _reportsController.DeleteReport(reportt.Id);

            Assert.IsInstanceOf<OkResult>(result2);
        }

        [Test]
        public async Task TestObrisiReportNeuspesnoBrisanje()
        {
            string Id = "65c6bc387b3cef9d80d81f18";

            var result = await _reportsController.DeleteReport(Id);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;

            Assert.IsNotNull(badRequestResult.Value);

        }

        [Test]
        public async Task TestPreuzmiReport()
        {
            Report report = new Report
            {
                Description = "Desc",
                Product = testproduct,
                IdUser = idUser
            };

            var result = await _reportsController.CreateReport(report);
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var reportt = okResult?.Value as Report;
            testReports.Add(reportt);

            Assert.IsNotNull(okResult, "Povratna vrednost nije instanca OkObjectResult-a.");

            var result2 = await _reportsController.GetReportById(reportt.Id);
            Assert.IsInstanceOf<OkObjectResult>(result2);
            var okk = result2 as OkObjectResult;

            var retrievedreport = okk?.Value as Report;
            Assert.IsNotNull(retrievedreport, "Povratna vrednost je null.");
            Assert.AreEqual(retrievedreport.Id, reportt.Id, "Retrieved user ID does not match registered user ID");
        }


        [Test]
        public async Task TestPreuzmiReportLosId()
        {
            string Id = "65c6bc387b3cef9d80d81f18";

            var rezultat = await _reportsController.GetReportById(Id);

            Assert.IsInstanceOf<BadRequestObjectResult>(rezultat);
            var result = (BadRequestObjectResult)rezultat;

            Assert.IsNotNull(result);


        }

        [Test]
        public async Task TestPreuzmiSveReporte()
        {

            Report report = new Report
            {
                Description = "Desc1",
                Product = testproduct,
                IdUser = idUser
            };

            var result = await _reportsController.CreateReport(report);
            var okResult = result as OkObjectResult;
            var reportt = okResult?.Value as Report;
            testReports.Add(reportt);


            Report report2 = new Report
            {
                Description = "Desc2",
                Product = testproduct,
                IdUser = idUser
            };

            var result2 = await _reportsController.CreateReport(report2);
            var okResult2 = result2 as OkObjectResult;
            var reportt2 = okResult2?.Value as Report;
            testReports.Add(reportt2);

            var reportss = await _reportsController.GetAllReports();
            var okreports = reportss as OkObjectResult;
            var reports = okreports?.Value as IQueryable<Report>;
            Assert.IsNotNull(okreports, "Failed to retrieve reports");

            Assert.IsTrue(reports.Any(r => r.Id == reportt.Id), $"Report with ID {reportt.Id} not found in retrieved reports");
            Assert.IsTrue(reports.Any(r => r.Id == reportt2.Id), $"Report with ID {reportt2.Id} not found in retrieved reports");

        }





        [TearDown]
        public async Task Cleanup()
        {
            // Delete the test users
            foreach (var report in testReports)
            {
                await _reportsController.DeleteReport(report.Id);
            }
            _productController.DeleteProduct(testproduct.Id);

        }
    }

}