using AppointmentManagementService.Data.UnitOfWork;
using AppointmentManagementService.Domain.Patient;
using AppointmentManagementService.Entities;
using AppointmentManagementService.Services.Contracts;
using AutoMapper;
using FluentResults;

namespace AppointmentManagementService.Services
{
    public class PatientService : IPatientService
    {
        private readonly IAppointmentUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PatientService(IAppointmentUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PatientDto>> CreatePatient(CreatePatientDto patientDto)
        {
            if (patientDto.DateOfBirth is null || patientDto.DateOfBirth == default(DateTime) || patientDto.DateOfBirth == DateTime.MinValue)
                return Result.Fail("Date of Birth is required.");

            // Check if email already exists
            var existingPatient = await _unitOfWork.Patients.GetByEmailAsync(patientDto.Email);
            if (existingPatient != null)
            {
                return Result.Fail("A patient with this email already exists.");
            }

            var patient = _mapper.Map<Patient>(patientDto);
            await _unitOfWork.Patients.AddAsync(patient);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<PatientDto> GetPatientById(Guid id)
        {
            var patient = await _unitOfWork.Patients.GetByIdIncludingAppointmentsAsync(id);
            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatients()
        {
            var patients = await _unitOfWork.Patients.GetAllAsync();
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        public async Task<Result<PatientDto>> UpdatePatient(Guid id, CreatePatientDto patientDto)
        {
            var daoPatient = await _unitOfWork.Patients.GetByIdAsync(id);
            if (daoPatient == null)
            {
                return Result.Fail("The patient does not exists.");
            }

            // Check if the new email is already taken by another patient
            if (!string.Equals(daoPatient.Email, patientDto.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingPatient = await _unitOfWork.Patients.GetByEmailAsync(patientDto.Email);
                if (existingPatient != null && existingPatient.Id != id)
                {
                    return Result.Fail("A patient with this email already exists.");
                }
            }

            _mapper.Map(patientDto, daoPatient);

            await _unitOfWork.Patients.UpdateAsync(daoPatient);
            await _unitOfWork.CompleteAsync();

            return Result.Ok(_mapper.Map<PatientDto>(daoPatient));
        }
    }
}
