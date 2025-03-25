using AppointmentManagementService.Data;
using AppointmentManagementService.Data.UnitOfWork;
using AppointmentManagementService.Domain.Appointment;
using AppointmentManagementService.Entities;
using AppointmentManagementService.Services;
using AppointmentManagementService.Services.Contracts;
using AppointmentManagementService.Services.Mappers;
using AppointmentManagementService.Shared.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace AppointmentManagementService.Tests.IntegrationTests
{
    public class AppointmentServiceIntegrationTests : IDisposable
    {
        private readonly AppointmentDbContext _dbContext;
        private readonly IAppointmentService _appointmentService;
        private readonly IDbContextTransaction _transaction;
        private readonly IConfiguration _configuration;

        public AppointmentServiceIntegrationTests()
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
            _dbContext.Database.EnsureCreated();

            // Begin a transaction (to rollback after each test)
            _transaction = _dbContext.Database.BeginTransaction();

            var unitOfWork = new AppointmentUnitOfWork(_dbContext);
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<AppointmentMappingProfile>()).CreateMapper();

            _appointmentService = new AppointmentService(unitOfWork, mapper);
        }

        [Theory]
        [InlineData("chrisdeford@outlook.com", "Chris", "Deford", "3145678923", 30, AppointmentType.Neurology)]
        public async Task ScheduleAppointment_ShouldFail_WhenPatientAlreadyHasAppointmentOnSameDay(string email, string firstName, string lastName, string phoneNumber, int age, AppointmentType appointmentType)
        {
            // Arrange
            var appointmentDate = DateTime.UtcNow.Date;


            var patientId = await AddPatientAsync(email, firstName, lastName, phoneNumber, age);
            await AddAppointmentAsync(patientId, appointmentDate, appointmentType);

            var newAppointmentDto = new CreateAppointmentDto
            {
                PatientId = patientId,
                AppointmentDate = appointmentDate
            };

            // Act
            var result = await _appointmentService.ScheduleAppointment(newAppointmentDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("A patient cannot book more than one appointment on the same day.",
                result.Errors.Select(e => e.Message));
        }

        [Theory]
        [InlineData("johndoe@gmail.com", "John", "Doe", "1234567890", 28, AppointmentType.Dentistry)]
        public async Task ScheduleAppointment_ShouldSucceed_WhenPatientHasNoExistingAppointment(
    string email, string firstName, string lastName, string phoneNumber, int age, AppointmentType appointmentType)
        {
            // Arrange
            var patientId = await AddPatientAsync(email, firstName, lastName, phoneNumber, age);
            var appointmentDate = DateTime.UtcNow.Date.AddDays(2);

            var appointmentDto = new CreateAppointmentDto
            {
                PatientId = patientId,
                AppointmentDate = appointmentDate,
                AppointmentType = appointmentType
            };

            // Act
            var result = await _appointmentService.ScheduleAppointment(appointmentDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(appointmentDate, result.Value.AppointmentDate);
            Assert.Equal(appointmentType, result.Value.AppointmentType);

            // Verify database contains the appointment
            var savedAppointment = await _dbContext.Appointments
                .FirstOrDefaultAsync(a => a.PatientId == patientId && a.AppointmentDate == appointmentDate);

            Assert.NotNull(savedAppointment);
            Assert.Equal(appointmentType, savedAppointment.AppointmentType);
        }

        private async Task<Guid> AddPatientAsync(string email, string firstName, string lastName, string phoneNumber, int age)
        {
            var patient = new Patient { Email = email, FirstName = firstName, LastName = lastName, PhoneNumber = phoneNumber, DateOfBirth = DateTime.UtcNow.AddYears(-age) };
            _dbContext.Patients.Add(patient);
            await _dbContext.SaveChangesAsync();

            return patient.Id;
        }

        private async Task AddAppointmentAsync(Guid patientId, DateTime appointmentDate, AppointmentType type)
        {
            var appointment = new Appointment { PatientId = patientId, AppointmentDate = appointmentDate, AppointmentType = type };
            _dbContext.Appointments.Add(appointment);
            await _dbContext.SaveChangesAsync();
        }

        // Rollback the transaction after each test to keep the database clean
        public void Dispose()
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _dbContext.Dispose();
        }
    }
}
