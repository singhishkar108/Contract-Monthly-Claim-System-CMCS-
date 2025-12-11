using CMCS.Areas.Identity.Data;
using CMCS.Controllers;
using CMCS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace TestProject1
{
    public class ClaimApprovalTest
    {
        private readonly ClaimsController _controller; //controller for claim actions
        private readonly ApplicationDbContext _context; //database context for application
        private readonly UserManager<ApplicationUser> _userManager; //user manager for identity operations
        private readonly Mock<IWebHostEnvironment> _mockEnv; //mock environment for testing

        public ClaimApprovalTest()
        {
            //setup in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "CMCS")
                .Options;

            _context = new ApplicationDbContext(options);

            //initializing user store and user manager
            var userStore = new UserStore<ApplicationUser>(_context);
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
            );

            //setting up mock web host environment
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockEnv.Setup(env => env.WebRootPath).Returns("C:\\fakepath");

            //creating and adding a test user
            var testUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "sonal@gmail.com",
                UserName = "sonal@gmail.com"
            };
            _userManager.CreateAsync(testUser).Wait();

            //initializing claims controller with context, environment, and user manager
            _controller = new ClaimsController(_context, _mockEnv.Object, _userManager);

            //creating claims principal for the test user
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, testUser.Id),
                new Claim(ClaimTypes.Name, "TestingUser")
            };
            var claimsIdentity = new ClaimsIdentity(userClaims, "TestingAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            //setting controller context with the claims principal
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
        }

        [Fact] //denotes a test method
        public async Task UpdateClaimStatus_ApproveClaim_ChangesToProcessedInDb()
        {
            //creating a new claim object
            var claim = new Claims
            {
                LecturerID = Guid.NewGuid().ToString(),
                FirstName = "Nikhile",
                LastName = "Reddy",
                ClaimsPeriodStart = DateTime.Now.AddDays(-7),
                ClaimsPeriodEnd = DateTime.Now,
                HoursWorked = 5,
                RatePerHour = 5,
                TotalAmount = 25,
                DescriptionOfWork = "Testing",
                SupportingDocuments = "acmeDocument.pdf",
                UserID = Guid.NewGuid().ToString(),
                ClaimStatus = "Pending",
                DateSubmitted = DateTime.Now
            };

            //adding claim to the context and saving changes
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            //updating claim status to 'Processed'
            await _controller.UpdateClaimStatus(claim.Id, "Processed");

            //retrieving the updated claim from the context
            var updatedClaim = await _context.Claims.FindAsync(claim.Id);
            Assert.NotNull(updatedClaim); //assert that the claim exists
            Assert.Equal("Processed", updatedClaim.ClaimStatus); //assert that the status is updated
        }
    }
}