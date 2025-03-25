using AppointmentManagementService.Data.Repositories;
using AppointmentManagementService.Data.UnitOfWork;
using AppointmentManagementService.Domain.Appointment;
using AppointmentManagementService.Entities;
using AppointmentManagementService.Services;
using AppointmentManagementService.Shared.Enums;
using AutoMapper;
using Moq;

namespace AppointmentManagementService.Tests
{
    public class AppointmentServiceTests
    {
        private readonly Mock<IAppointmentUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
        private readonly Mock<IPatientRepository> _patientRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AppointmentService _appointmentService;

        public AppointmentServiceTests()
        {
            _unitOfWorkMock = new Mock<IAppointmentUnitOfWork>();
            _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _mapperMock = new Mock<IMapper>();

            // Setup UnitOfWork to return mocked repositories
            _unitOfWorkMock.Setup(u => u.Appointments).Returns(_appointmentRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.Patients).Returns(_patientRepositoryMock.Object);

            _appointmentService = new AppointmentService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAppointmentAsync_ShouldReturnFail_WhenPatientDoesNotExist()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var dto = new CreateAppointmentDto
            {
                PatientId = patientId,
                AppointmentDate = DateTime.UtcNow.AddDays(1),
                AppointmentType = AppointmentType.GeneralMedicine
            };

            _patientRepositoryMock.Setup(r => r.GetByIdAsync(dto.PatientId)).ReturnsAsync((Patient)null);

            _appointmentRepositoryMock.Setup(r => r.GetAllActivePatientAppointmentsAsync(dto.PatientId))
                .ReturnsAsync(new List<Appointment> {  });

            // Act
            var result = await _appointmentService.ScheduleAppointment(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("The patient does not exist.", result.Errors.Select(e => e.Message));
        }

        [Fact]
        public async Task CreateAppointmentAsync_ShouldReturnFail_WhenPatientAlreadyHasAppointmentOnSameDay()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var appointmentDate = DateTime.UtcNow.Date.AddHours(10);
            var dto = new CreateAppointmentDto
            {
                PatientId = patientId,
                AppointmentDate = appointmentDate,
                AppointmentType = AppointmentType.GeneralMedicine
            };

            var existingAppointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                AppointmentDate = appointmentDate.AddHours(-2), // Same day, but earlier time
                AppointmentType = AppointmentType.Dentistry
            };

            _patientRepositoryMock.Setup(r => r.GetByIdAsync(dto.PatientId))
                .ReturnsAsync(new Patient { Id = patientId });

            _appointmentRepositoryMock.Setup(r => r.GetAllActivePatientAppointmentsAsync(dto.PatientId))
                .ReturnsAsync(new List<Appointment> { existingAppointment });

            // Act
            var result = await _appointmentService.ScheduleAppointment(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("A patient cannot book more than one appointment on the same day.", result.Errors.Select(e => e.Message));
        }

        [Fact]
        public async Task CreateAppointmentAsync_ShouldReturnSuccess_WhenNoConflicts()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var appointmentDate = DateTime.UtcNow.Date.AddHours(10);
            var dto = new CreateAppointmentDto
            {
                PatientId = patientId,
                AppointmentDate = appointmentDate,
                AppointmentType = AppointmentType.GeneralMedicine
            };

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = dto.PatientId,
                AppointmentDate = dto.AppointmentDate,
                AppointmentType = AppointmentType.Dentistry
            };

            _patientRepositoryMock.Setup(r => r.GetByIdAsync(dto.PatientId)).ReturnsAsync(new Patient { Id = patientId });

            _appointmentRepositoryMock.Setup(r => r.GetAllActivePatientAppointmentsAsync(patientId))
                .ReturnsAsync(new List<Appointment> { });

            _mapperMock.Setup(m => m.Map<Appointment>(dto)).Returns(appointment);
            _appointmentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Appointment>()));

            // Act
            var result = await _appointmentService.ScheduleAppointment(dto);

            // Assert
            Assert.True(result.IsSuccess);

            _appointmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Appointment>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

    }
}
