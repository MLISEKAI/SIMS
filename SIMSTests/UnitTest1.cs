using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SIMS;
using SIMS.Controllers;
using SIMS.Data;
using SIMS.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SIMSTests
{
    public class UsersControllerTests
    {
        private readonly SIMSContext _context;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            // Setup in-memory database with unique name for isolation
            var options = new DbContextOptionsBuilder<SIMSContext>()
                .UseInMemoryDatabase(databaseName: "SIMSTestDatabase_" + Guid.NewGuid().ToString())
                .Options;
            _context = new SIMSContext(options);

            // Seed database
            SeedDatabase();

            // Setup controller
            _controller = new UsersController(_context);
        }

        private void SeedDatabase()
        {
            // Clear existing data to avoid conflicts
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();

            // Seed initial data for testing
            _context.Users.Add(new Users { UserName = "TranThanhLoc", Pass = "12345", UserRole = "Student" });
            _context.Users.Add(new Users { UserName = "DaoTranChung", Pass = "1", UserRole = "Teacher" });
            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfUsers()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Users>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Details_ReturnsViewResult_ForValidId()
        {
            // Arrange
            var user = _context.Users.First();
            int validId = user.ID;

            // Act
            var result = await _controller.Details(validId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Users>(viewResult.ViewData.Model);
            Assert.Equal(validId, model.ID);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_ForInvalidId()
        {
            // Arrange
            int invalidId = 99;

            // Act
            var result = await _controller.Details(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_RedirectsToIndex_WhenModelStateIsValid()
        {
            // Arrange
            var user = new Users { UserName = "NguyenDinhHieu", Pass = "123", UserRole = "Student" };

            // Act
            var result = await _controller.Create(user);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal(3, _context.Users.Count());
        }
       

        [Fact]
        public async Task DeleteConfirmed_RemovesUser_AndRedirectsToIndex()
        {
            // Arrange
            var user = _context.Users.First(); // Get an existing user
            int userId = user.ID;

            // Act
            var result = await _controller.DeleteConfirmed(userId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Null(await _context.Users.FindAsync(userId));
        }

       

        [Fact]
        public async Task Export_ReturnsFileResult_WithCsvContent()
        {
            // Act
            var result = await _controller.Export();

            // Assert
            var fileResult = Assert.IsType<FileStreamResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal("Users.csv", fileResult.FileDownloadName);

            using var reader = new StreamReader(fileResult.FileStream);
            var content = await reader.ReadToEndAsync();

            // Adjust the expected content based on your actual test data
            Assert.Contains("ID,UserName,Pass,UserRole", content);
            Assert.Contains("1,TranThanhLoc,12345,Student", content); // Adjust based on actual data
            Assert.Contains("2,DaoTranChung,1,Teacher", content); // Adjust based on actual data
        }

    }
}
