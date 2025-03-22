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
            var patient = _mapper.Map<Patient>(patientDto);
            await _unitOfWork.Patients.AddAsync(patient);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<PatientDto> GetPatientById(Guid id)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id);
            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatients()
        {
            var patients = await _unitOfWork.Patients.GetAllAsync();
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        public async Task<Result<PatientDto>> UpdatePatient(Guid id, CreatePatientDto patientDto)
        {
            var existingPatient = await _unitOfWork.Patients.GetByIdAsync(id);
            if (existingPatient == null)
            {
                return Result.Fail("The patient does not exists.");
            }

            _mapper.Map(patientDto, existingPatient);

            await _unitOfWork.Patients.UpdateAsync(existingPatient);
            await _unitOfWork.CompleteAsync();

            return Result.Ok(_mapper.Map<PatientDto>(existingPatient));
        }
    }
}
