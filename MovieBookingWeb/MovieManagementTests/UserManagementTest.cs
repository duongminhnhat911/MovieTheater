using Moq;
using UserManagement.Services;
using UserManagement.Repositories;
using UserManagement.Models.DTOs;
using UserManagement.Models.Entities;

namespace UserManagementTests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _service = new UserService(_mockRepo.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_ValidUser_ReturnsUser()
        {
            var dto = new LoginDto { UsernameOrEmail = "testuser" };
            var expectedUser = new User { Username = "testuser" };
            _mockRepo.Setup(r => r.FindUserAsync(dto.UsernameOrEmail)).ReturnsAsync(expectedUser);

            var result = await _service.AuthenticateAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task AuthenticateAsync_UserNotFound_ReturnsNull()
        {
            _mockRepo.Setup(r => r.FindUserAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

            var result = await _service.AuthenticateAsync(new LoginDto { UsernameOrEmail = "unknown" });

            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterAsync_NewUser_ReturnsTrue()
        {
            _mockRepo.Setup(r => r.AddUserAsync(It.IsAny<RegisterDto>())).ReturnsAsync(true);

            var result = await _service.RegisterAsync(new RegisterDto());

            Assert.True(result);
        }

        [Fact]
        public async Task RegisterAsync_UsernameExists_ReturnsFalse()
        {
            _mockRepo.Setup(r => r.AddUserAsync(It.IsAny<RegisterDto>())).ReturnsAsync(false);

            var result = await _service.RegisterAsync(new RegisterDto());

            Assert.False(result);
        }

        [Fact]
        public async Task GetProfileAsync_ExistingUser_ReturnsProfile()
        {
            var user = new User
            {
                Username = "test",
                FullName = "Test User",
                BirthDate = DateTime.Today,
                Gender = "Male",
                Email = "test@mail.com",
                IdCard = "123",
                PhoneNumber = "456",
                Address = "Somewhere"
            };

            _mockRepo.Setup(r => r.GetByUsernameAsync("test")).ReturnsAsync(user);

            var result = await _service.GetProfileAsync("test");

            Assert.NotNull(result);
            Assert.Equal("Test User", result.FullName);
        }

        [Fact]
        public async Task GetProfileAsync_UserNotFound_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetByUsernameAsync("unknown")).ReturnsAsync((User?)null);

            var result = await _service.GetProfileAsync("unknown");

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateProfileAsync_Success_ReturnsTrue()
        {
            _mockRepo.Setup(r => r.UpdateUserAsync("test", It.IsAny<UpdateProfileDto>())).ReturnsAsync(true);

            var result = await _service.UpdateProfileAsync("test", new UpdateProfileDto());

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateProfileAsync_Failure_ReturnsFalse()
        {
            _mockRepo.Setup(r => r.UpdateUserAsync("test", It.IsAny<UpdateProfileDto>())).ReturnsAsync(false);

            var result = await _service.UpdateProfileAsync("test", new UpdateProfileDto());

            Assert.False(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsList()
        {
            var users = new List<GetListUserDto> { new GetListUserDto { Username = "u1" } };
            _mockRepo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);

            var result = await _service.GetAllUsersAsync();

            Assert.Single(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_UserFound_ReturnsUser()
        {
            var user = new GetListUserDto { Id = 1 };
            _mockRepo.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);

            var result = await _service.GetUserByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetUserByIdAsync_UserNotFound_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync((GetListUserDto?)null);

            var result = await _service.GetUserByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task AdminUpdateUserAsync_Success_ReturnsTrue()
        {
            _mockRepo.Setup(r => r.UpdateUserByIdAsync(1, It.IsAny<AdminUpdateUserDto>())).ReturnsAsync(true);

            var result = await _service.AdminUpdateUserAsync(1, new AdminUpdateUserDto());

            Assert.True(result);
        }

        [Fact]
        public async Task AdminUpdateUserAsync_Failure_ReturnsFalse()
        {
            _mockRepo.Setup(r => r.UpdateUserByIdAsync(1, It.IsAny<AdminUpdateUserDto>())).ReturnsAsync(false);

            var result = await _service.AdminUpdateUserAsync(1, new AdminUpdateUserDto());

            Assert.False(result);
        }

        [Fact]
        public async Task ToggleUserLockAsync_Success_ReturnsTrue()
        {
            _mockRepo.Setup(r => r.ToggleUserLockAsync(1)).ReturnsAsync(true);

            var result = await _service.ToggleUserLockAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task ToggleUserLockAsync_Failure_ReturnsFalse()
        {
            _mockRepo.Setup(r => r.ToggleUserLockAsync(1)).ReturnsAsync(false);

            var result = await _service.ToggleUserLockAsync(1);

            Assert.False(result);
        }

        [Fact]
        public async Task ChangePasswordAsync_CorrectOldPassword_ReturnsTrue()
        {
            var user = new User { Username = "user", Password = "oldpass" };
            _mockRepo.Setup(r => r.GetByUsernameAsync("user")).ReturnsAsync(user);
            _mockRepo.Setup(r => r.UpdatePasswordAsync(user, "newpass")).ReturnsAsync(true);

            var result = await _service.ChangePasswordAsync("user", "oldpass", "newpass");

            Assert.True(result);
        }

        [Fact]
        public async Task ChangePasswordAsync_WrongOldPassword_ReturnsFalse()
        {
            var user = new User { Username = "user", Password = "wrong" };
            _mockRepo.Setup(r => r.GetByUsernameAsync("user")).ReturnsAsync(user);

            var result = await _service.ChangePasswordAsync("user", "oldpass", "newpass");

            Assert.False(result);
        }
    }
}

