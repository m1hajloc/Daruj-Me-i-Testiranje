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
    public class UserTests
    {
        private IConfiguration _configuration;
        private MongoDbContext _mongoDbContext;
        private Controllers.UserController _userController;
        private List<User> testUsers;

        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _mongoDbContext = new MongoDbContext(_configuration);
            _userController = new UserController(_mongoDbContext);
            testUsers = new List<User>();
        }

        [Test]
        public async Task TestDodajUsera()
        {
            UserRegisterDTO user = new UserRegisterDTO
            {
                Name = "Mihajlo",
                Lastname = "Cvetkovic",
                Username = "M12hajlocc",
                Email = "mihajlomihajlomihajlo@gmail.com",
                Password = "1234",
                RepeatedPassword = "1234",
                PhoneNumber = "0621234567",
                ProfilePicture = null,
                City = "Leskovac",
                Adress = "Bulevar Nemanjica"
            };

            var result = await _userController.Register(user, null);
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;

            var registeredUser = okResult?.Value as User;
            testUsers.Add(registeredUser);

            Assert.IsNotNull(okResult, "Povratna vrednost nije instanca OkObjectResult-a.");
        }

        [Test]
        public async Task TestDodajUseraPostojeciUsername()
        {
            UserRegisterDTO user = new UserRegisterDTO
            {
                Name = "Mihajlo",
                Lastname = "Cvetkovic",
                Username = "M12hajlocc2",
                Email = "mihajlomihajlomihajlo2@gmail.com",
                Password = "1234",
                RepeatedPassword = "1234",
                PhoneNumber = "0621234567",
                ProfilePicture = null,
                City = "Leskovac",
                Adress = "Bulevar Nemanjica"
            };
            UserRegisterDTO user2 = new UserRegisterDTO
            {
                Name = "Mihajlo",
                Lastname = "Cvetkovic",
                Username = "M12hajlocc2",
                Email = "mihajlomihajlomihajlo6@gmail.com",
                Password = "1234",
                RepeatedPassword = "1234",
                PhoneNumber = "0621234567",
                ProfilePicture = null,
                City = "Leskovac",
                Adress = "Bulevar Nemanjica"
            };
            var result1 = await _userController.Register(user, null);
            var result = await _userController.Register(user2, null);

            var okResult = result1 as OkObjectResult;

            var registeredUser = okResult?.Value as User;
            testUsers.Add(registeredUser);

            Assert.IsInstanceOf<BadRequestObjectResult>(result, "Vrednost nije kreirana!");
            var badRequestResult = result as BadRequestResult;
            Assert.IsNull(badRequestResult, "Vec postojeci username");

        }
        [Test]
        public async Task TestDodajUseraPostojeciEmail()
        {
            UserRegisterDTO user = new UserRegisterDTO
            {
                Name = "Mihajlo",
                Lastname = "Cvetkovic",
                Username = "M12hajlocc3",
                Email = "mihajlomihajlomihajlo3@gmail.com",
                Password = "1234",
                RepeatedPassword = "1234",
                PhoneNumber = "0621234567",
                ProfilePicture = null,
                City = "Leskovac",
                Adress = "Bulevar Nemanjica"
            };
            UserRegisterDTO user2 = new UserRegisterDTO
            {
                Name = "Mihajlo",
                Lastname = "Cvetkovic",
                Username = "M12hajlocc6",
                Email = "mihajlomihajlomihajlo3@gmail.com",
                Password = "1234",
                RepeatedPassword = "1234",
                PhoneNumber = "0621234567",
                ProfilePicture = null,
                City = "Leskovac",
                Adress = "Bulevar Nemanjica"
            };
            var result1 = await _userController.Register(user, null);
            var result = await _userController.Register(user2, null);

            var okResult = result1 as OkObjectResult;
            var registeredUser = okResult?.Value as User;
            testUsers.Add(registeredUser);

            Assert.IsInstanceOf<BadRequestObjectResult>(result, "Vrednost nije kreirana!");
            var badRequestResult = result as BadRequestResult;
            Assert.IsNull(badRequestResult, "Vec postojeci email");

        }
        [Test]
        public async Task TestDodajUseraRazliciteSifre()
        {
            UserRegisterDTO user = new UserRegisterDTO
            {
                Name = "Mihajlo",
                Lastname = "Cvetkovic",
                Username = "M12hajlocc4",
                Email = "mihajlomihajlomihajlo4@gmail.com",
                Password = "1234",
                RepeatedPassword = "1",
                PhoneNumber = "0621234567",
                ProfilePicture = null,
                City = "Leskovac",
                Adress = "Bulevar Nemanjica"
            };

            var result = await _userController.Register(user, null);

            Assert.IsInstanceOf<BadRequestObjectResult>(result, "Vrednost nije kreirana!");
            var badRequestResult = result as BadRequestResult;
            Assert.IsNull(badRequestResult, "Sifre se ne poklapaju");

        }

        [Test]
        public async Task TestObrisiKorisnikaUspesnoBrisanje()
        {
            UserRegisterDTO user = new UserRegisterDTO
            {
                Name = "Mihajlo",
                Lastname = "Cvetkovic",
                Username = "M12hajlocc10",
                Email = "mihajlomihajlomihajlo10@gmail.com",
                Password = "1234",
                RepeatedPassword = "1234",
                PhoneNumber = "0621234567",
                ProfilePicture = null,
                City = "Leskovac",
                Adress = "Bulevar Nemanjica"
            };


            var result = await _userController.Register(user, null);
            var okResult = result as OkObjectResult;
            var registeredUser = okResult.Value as User;

            var result2 = await _userController.DeleteUser(registeredUser.Id);

            Assert.IsInstanceOf<OkObjectResult>(result2);
            var okResult2 = result2 as OkObjectResult;
            Assert.AreEqual($"User with ID {registeredUser.Id} has been deleted successfully.", okResult2.Value);


        }

        [Test]
        public async Task TestObrisiKorisnikaNepostojeciKorisnik()
        {
            string Id = "65c7bf61e941a2a23615015f";

            var result = await _userController.DeleteUser(Id);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;

            Assert.IsNotNull(badRequestResult.Value);

            Assert.AreEqual($"Object with ID '65c7bf61e941a2a23615015f' does not exist.", badRequestResult.Value);
        }

        [Test]
        public async Task TestPreuzmiKorisnika()
        {
            // Arrange
            UserRegisterDTO user = new UserRegisterDTO
            {
                Name = "Mihajlo",
                Lastname = "Cvetkovic",
                Username = "M12hajldocc11",
                Email = "mihajlomihajlomihajlosss11@gmail.com",
                Password = "1234",
                RepeatedPassword = "1234",
                PhoneNumber = "0621234567",
                ProfilePicture = null,
                City = "Leskovac",
                Adress = "Bulevar Nemanjica"
            };

            var registrationResult = await _userController.Register(user, null);
            var registrationOkResult = registrationResult as OkObjectResult;
            var registeredUser = registrationOkResult?.Value as User;
            var getUserResult = await _userController.GetUserById(registeredUser.Id);

            testUsers.Add(registeredUser);

            Assert.IsInstanceOf<OkObjectResult>(getUserResult, "Failed to retrieve user by ID");
            var getUserOkResult = getUserResult as OkObjectResult;
            Assert.IsNotNull(getUserOkResult, "Invalid response when retrieving user by ID");

            var retrievedUser = getUserOkResult?.Value as User;

            Assert.IsNotNull(retrievedUser, "Retrieved user is null");
            Assert.AreEqual(registeredUser.Id, retrievedUser.Id, "Retrieved user ID does not match registered user ID");
        }

        [Test]
        public async Task TestLosId()
        {
            string Id = "65d7bf61e941a2a23615015f";

            var rezultat = await _userController.GetUserById(Id);

            Assert.IsInstanceOf<BadRequestObjectResult>(rezultat);
            var result = (BadRequestObjectResult)rezultat;

            Assert.IsNotNull(result);


        }

        [Test]
        public async Task TestLoginValidniPodaci()
        {

            UserRegisterDTO user = new UserRegisterDTO
            {
                Name = "Mihajlo",
                Lastname = "Cvetkovic",
                Username = "M12hajlocc12",
                Email = "mihajlomihajlomihajlo12@gmail.com",
                Password = "1234",
                RepeatedPassword = "1234",
                PhoneNumber = "0621234567",
                ProfilePicture = null,
                City = "Leskovac",
                Adress = "Bulevar Nemanjica"
            };

            var registrationResult = await _userController.Register(user, null);
            var registrationOkResult = registrationResult as OkObjectResult;
            var registeredUser = registrationOkResult?.Value as User;
            testUsers.Add(registeredUser);
            UserLoginDTO userlogin = new UserLoginDTO
            {
                Email = "mihajlomihajlomihajlo12@gmail.com",
                Password = "1234",
            };

            var result = await _userController.Login(userlogin);

            Assert.IsNotNull(result);

        }

        [Test]
        public async Task TestLoginNevalidniEmail()
        {

            UserRegisterDTO user = new UserRegisterDTO
            {
                Name = "Mihajlo",
                Lastname = "Cvetkovic",
                Username = "M12hajlocc13",
                Email = "mihajlomihajlomihajlo13@gmail.com",
                Password = "1234",
                RepeatedPassword = "1234",
                PhoneNumber = "0621234567",
                ProfilePicture = null,
                City = "Leskovac",
                Adress = "Bulevar Nemanjica"
            };


            var registrationResult = await _userController.Register(user, null);
            var registrationOkResult = registrationResult as OkObjectResult;
            var registeredUser = registrationOkResult?.Value as User;
            testUsers.Add(registeredUser);

            UserLoginDTO userlogin = new UserLoginDTO
            {
                Email = "abvgd@gmail.com",
                Password = "1234",
            };

            var result = await _userController.Login(userlogin);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);

        }

        [Test]
        public async Task TestLoginNevalidnaLozinka()
        {

            UserRegisterDTO user = new UserRegisterDTO
            {
                Name = "Mihajlo",
                Lastname = "Cvetkovic",
                Username = "M12hajlocc14",
                Email = "mihajlomihajlomihajlo14@gmail.com",
                Password = "1234",
                RepeatedPassword = "1234",
                PhoneNumber = "0621234567",
                ProfilePicture = null,
                City = "Leskovac",
                Adress = "Bulevar Nemanjica"
            };

            var registrationResult = await _userController.Register(user, null);
            var registrationOkResult = registrationResult as OkObjectResult;
            var registeredUser = registrationOkResult?.Value as User;
            testUsers.Add(registeredUser);


            UserLoginDTO userlogin = new UserLoginDTO
            {
                Email = "mihajlomihajlomihajlo14@gmail.com",
                Password = "1234123123",
            };

            var result = await _userController.Login(userlogin);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);

        }








        [Test]
        public async Task TestEditujKorisnikaUspesno()
        {

            UserRegisterDTO user = new UserRegisterDTO
            {
                Name = "Mihajlo",
                Lastname = "Cvetkovic",
                Username = "M12hajlocc15",
                Email = "mihajlomihajlomihajlo15@gmail.com",
                Password = "1234",
                RepeatedPassword = "1234",
                PhoneNumber = "0621234567",
                ProfilePicture = null,
                City = "Leskovac",
                Adress = "Bulevar Nemanjica"
            };

            var registrationResult = await _userController.Register(user, null);
            var registrationOkResult = registrationResult as OkObjectResult;
            var registeredUser = registrationOkResult?.Value as User;
            testUsers.Add(registeredUser);

            UserUpdateDTO user2 = new UserUpdateDTO
            {
                Name = registeredUser.Name,
                LastName = "Petrovic",
                PhoneNumber = registeredUser.PhoneNumber,
                City = registeredUser.City,
                Adress = registeredUser.Adress,
                Id = registeredUser.Id
            };

            var result = await _userController.EditUser(user2, null);

            Assert.IsInstanceOf<OkObjectResult>(result);

        }

        [Test]
        public async Task TestEditujKorisnikaNeuspesno()
        {
            var nepostojeciId = "65ed984c454619ba306c8c63";
            UserRegisterDTO user = new UserRegisterDTO
            {
                Name = "Mihajlo",
                Lastname = "Cvetkovic",
                Username = "M12hajlocc16",
                Email = "mihajlomihajlomihajlo16@gmail.com",
                Password = "1234",
                RepeatedPassword = "1234",
                PhoneNumber = "0621234567",
                ProfilePicture = null,
                City = "Leskovac",
                Adress = "Bulevar Nemanjica"
            };

            var registrationResult = await _userController.Register(user, null);
            var registrationOkResult = registrationResult as OkObjectResult;
            var registeredUser = registrationOkResult?.Value as User;
            testUsers.Add(registeredUser);

            UserUpdateDTO user2 = new UserUpdateDTO
            {
                Name = registeredUser.Name,
                LastName = "Petrovic",
                PhoneNumber = registeredUser.PhoneNumber,
                City = registeredUser.City,
                Adress = registeredUser.Adress,
                Id = nepostojeciId
            };

            var result = await _userController.EditUser(user2, null);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [TearDown]
        public async Task Cleanup()
        {
            // Delete the test users
            foreach (var user in testUsers)
            {
                await _userController.DeleteUser(user.Id);
            }


        }
    }
}