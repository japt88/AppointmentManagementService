using AppointmentManagementService.Data.Repositories;
using AppointmentManagementService.Data.UnitOfWork;
using AppointmentManagementService.Domain.Patient;
using AppointmentManagementService.Entities;
using AppointmentManagementService.Services;
using AutoMapper;
using Moq;

namespace AppointmentManagementService.Tests;

public class PatientServiceTests
{
    private readonly Mock<IAppointmentUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly PatientService _patientService;

    public PatientServiceTests()
    {
        _unitOfWorkMock = new Mock<IAppointmentUnitOfWork>();
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _mapperMock = new Mock<IMapper>();

        _unitOfWorkMock.Setup(u => u.Patients).Returns(_patientRepositoryMock.Object);
        _patientService = new PatientService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreatePatient_ShouldReturnSuccess_WhenPatientDoesNotExist()
    {
        // Arrange
        var dto = new CreatePatientDto { Email = "judypinilla@gmail.com", FirstName = "Judy", LastName = "Pinilla", DateOfBirth = new DateTime(1988, 07, 27) };
        Guid newPatientId = Guid.NewGuid();
        var newPatient = new Patient { Id = newPatientId, Email = dto.Email, FirstName = dto.FirstName, LastName = dto.LastName, DateOfBirth = (DateTime)dto.DateOfBirth };
        var expectedPatientDto = new PatientDto { Id = newPatientId, Email = dto.Email, FirstName = dto.FirstName, LastName = dto.LastName, DateOfBirth = (DateTime)dto.DateOfBirth };

        _patientRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((Patient)null);
        _mapperMock.Setup(m => m.Map<Patient>(dto)).Returns(newPatient);
        _mapperMock.Setup(m => m.Map<PatientDto>(newPatient)).Returns(expectedPatientDto);

        // Act
        var result = await _patientService.CreatePatient(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedPatientDto.Email, result.Value.Email);

        _patientRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task CreatePatient_ShouldReturnFail_WhenDateOfBirthIsMissing()
    {
        // Arrange
        var dto = new CreatePatientDto { Email = "judypinilla@gmail.com", FirstName = "Judy", LastName = "Pinilla" };

        // Act
        var result = await _patientService.CreatePatient(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Date of Birth is required.", result.Errors.Select(e => e.Message));
    }

    [Fact]
    public async Task GetPatientByIdAsync_ShouldReturnSuccess_WhenPatientExists()
    {
        // Arrange 
        Guid patientId = Guid.NewGuid(); 
        var existingPatient = new Patient { Id = patientId, Email = "judypinilla@gmail.com" };
        var expectedPatientDto = new PatientDto { Id = patientId, Email = "judypinilla@gmail.com" };

        _patientRepositoryMock.Setup(r => r.GetByIdIncludingAppointmentsAsync(patientId)).ReturnsAsync(existingPatient);
        _mapperMock.Setup(m => m.Map<PatientDto>(existingPatient)).Returns(expectedPatientDto);

        // Act
        var result = await _patientService.GetPatientById(patientId);

        // Assert
        Assert.True(result is not null);
        Assert.Equal(expectedPatientDto.Email, result.Email);
    }

    [Fact]
    public async Task GetPatientByIdAsync_ShouldReturnFail_WhenPatientNotFound()
    {
        // Arrange
        Guid patientId = Guid.NewGuid();
        _patientRepositoryMock.Setup(r => r.GetByIdIncludingAppointmentsAsync(patientId)).ReturnsAsync((Patient)null);

        // Act
        var result = await _patientService.GetPatientById(patientId);

        // Assert
        Assert.False(result is not null);
    }

    [Fact]
    public async Task UpdatePatient_ShouldReturnSuccess_WhenPatientExists()
    {
        // Arrange
        Guid patientId = Guid.NewGuid();
        var dto = new CreatePatientDto { Email = "judypinilla88@gmail.com" };
        var existingPatient = new Patient { Id = patientId, Email = "judypinilla@gmail.com" };

        _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync(existingPatient);
        _mapperMock.Setup(m => m.Map(dto, existingPatient)).Verifiable();

        // Act
        var result = await _patientService.UpdatePatient(patientId, dto);

        // Assert
        Assert.True(result.IsSuccess);

        _mapperMock.Verify(m => m.Map(dto, existingPatient), Times.Once);
        _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
    }
}
