using AppointmentManagementService.Data;
using AppointmentManagementService.Data.UnitOfWork;
using AppointmentManagementService.Domain.Patient;
using AppointmentManagementService.Entities;
using AppointmentManagementService.Services;
using AppointmentManagementService.Services.Mappers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace AppointmentManagementService.Tests.IntegrationTests
{
    public class PatientServiceIntegrationTests : IDisposable
    {
        private readonly AppointmentDbContext _dbContext;
        private readonly IAppointmentUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PatientService _patientService;
        private readonly IDbContextTransaction _transaction;
        private readonly IConfiguration _configuration;

        public PatientServiceIntegrationTests()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
                .Build();

            _configuration = configBuilder;

            string connectionString = _configuration.GetConnectionString("TestDb");

            var options = new DbContextOptionsBuilder<AppointmentDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            _dbContext = new AppointmentDbContext(options);
            _unitOfWork = new AppointmentUnitOfWork(_dbContext);
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<AppointmentMappingProfile>()).CreateMapper();
            _patientService = new PatientService(_unitOfWork, _mapper);

            _dbContext.Database.EnsureCreated();
            _transaction = _dbContext.Database.BeginTransaction(); // Start transaction
        }

        [Theory]
        [InlineData("judypinilla@outlook.com", "Judy", "Pinilla", "1988-07-27", "3116742281")]
        [InlineData("jhon.doe@example.com", "Jhon", "Doe", "1993-09-21", "3215647687")]
        public async Task CreatePatient_ShouldSucceed_WhenValidDataIsProvided(string email, string firstName, string lastName, string dateOfBirth, string phoneNumber)
        {
            // Arrange
            var dto = new CreatePatientDto
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = DateTime.Parse(dateOfBirth),
                Phone = phoneNumber
            };

            // Act
            var result = await _patientService.CreatePatient(dto);

            var patient = await _unitOfWork.Patients.GetByIdAsync(result.Value.Id);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(dto.Email, result.Value.Email);
            Assert.True(patient != null);
        }


        [Theory]
        [InlineData("judypinilla88@gmail.com", "Judy", "Pinilla", "1988-07-27", "3116742281")]
        [InlineData("jhon.doe88@example.com", "Jhon", "Doe", "1993-09-21", "3215647687")]
        public async Task CreatePatient_ShouldFail_WhenEmailAlreadyExists(string email, string firstName, string lastName, string dateOfBirth, string phoneNumber)
        {
            // Arrange
            var dto = new CreatePatientDto
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = DateTime.Parse(dateOfBirth),
                Phone = phoneNumber
            };

            // Act
            var result = await _patientService.CreatePatient(dto);

            var resultExistingPatient = await _patientService.CreatePatient(dto);


            // Assert
            Assert.False(resultExistingPatient.IsSuccess);
            Assert.Contains("A patient with this email already exists.", resultExistingPatient.Errors.Select(e => e.Message));
        }


        public void Dispose()
        {
            _transaction.Rollback(); // Rollback transaction after each test
            _dbContext.Dispose();
        }
    }
}
