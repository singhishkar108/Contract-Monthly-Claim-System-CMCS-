using CMCS.Areas.Identity.Data;
using CMCS.Controllers;
using CMCS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace ClaimsControllerTest
{
    public class ClaimSubmitTest
    {
        private readonly ClaimsController _controller; //claims controller instance
        private readonly ApplicationDbContext _context; //in-memory database context
        private readonly UserManager<ApplicationUser> _userManager; //user manager for application users
        private readonly Mock<IWebHostEnvironment> _mockEnv; //mock for web hosting environment

        public ClaimSubmitTest()
        {
            //setup in-memory database options
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "CMCS")
                .Options;

            _context = new ApplicationDbContext(options); //initialize the database context

            var userStore = new UserStore<ApplicationUser>(_context); //user store for managing users
            var userManagerOptions = new IdentityOptions(); //identity options for user manager
            _userManager = new UserManager<ApplicationUser>(
                userStore,
                null,
                new PasswordHasher<ApplicationUser>(),
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null,
                null
            ); //create user manager instance

            _mockEnv = new Mock<IWebHostEnvironment>(); //initialize mock for web host environment
            _mockEnv.Setup(env => env.WebRootPath).Returns("C:\\fakepath"); //setup mock web root path

            //create test user and add to user manager
            var testUser = new ApplicationUser
            {
                Id = "1088",
                Email = "pooja.ramroop@gmail.com",
                UserName = "pooja.ramroop@gmail.com"
            };
            _userManager.CreateAsync(testUser).Wait(); //create the test user asynchronously

            _controller = new ClaimsController(_context, _mockEnv.Object, _userManager); //initialize claims controller

            //setup user claims for authentication
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "108"),
                new Claim(ClaimTypes.Name, "TestUser")
            };
            var claimsIdentity = new ClaimsIdentity(userClaims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            //set up controller context with claims principal
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal //assign user claims to HttpContext
                }
            };
        }

        [Fact]
        public async Task ClaimsPost_ValidModelWithFile_RedirectToClaimSummary()
        {
            //create a sample claim object
            var claims = new Claims
            {
                LecturerID = "1008",
                FirstName = "Pooja",
                LastName = "Ramroop",
                ClaimsPeriodStart = DateTime.Now.AddDays(-7),
                ClaimsPeriodEnd = DateTime.Now,
                HoursWorked = 5,
                RatePerHour = 5,
                TotalAmount = 25,
                DescriptionOfWork = "Testing Db entry",
                SupportingDocuments = "test_file_108",
                UserID = "108",
                ClaimStatus = "Pending",
                DateSubmitted = DateTime.Now
            };

            //mock file upload for the claim
            var mockFile = new Mock<IFormFile>();
            var content = "This is a test file."; //file content
            var fileName = "test.pdf"; //file name
            var memoryStream = new MemoryStream(); //memory stream for file
            var writer = new StreamWriter(memoryStream);
            writer.Write(content); //write content to stream
            writer.Flush();
            memoryStream.Position = 0; //reset stream position

            //set up mock file properties
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(memoryStream.Length);
            mockFile.Setup(f => f.OpenReadStream()).Returns(memoryStream);
            mockFile.Setup(f => f.ContentType).Returns("application/pdf");
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                .Returns<Stream, CancellationToken>((stream, cancellationToken) => memoryStream.CopyToAsync(stream, cancellationToken));

            //submit the claim and validate the result
            var result = await _controller.Claims(claims, mockFile.Object);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result); //check for redirect result
            Assert.Equal("Summary", redirectResult.ActionName); //ensure action name is 'Summary'
            Assert.Equal(2, _context.Claims.Count()); //verify claim was added to the context
        }
    }
}
