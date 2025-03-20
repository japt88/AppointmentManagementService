using AppointmentManagementService.Data.Repositories;
using AppointmentManagementService.Data.UnitOfWork;
using AppointmentManagementService.Entities;
using AppointmentManagementService.Services.Contracts;
using FluentResults;

namespace AppointmentManagementService.Services
{
    public class PatientService : IPatientService
    {
        private readonly IAppointmentUnitOfWork _unitOfWork;

        public PatientService(IAppointmentUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Patient>> CreatePatient(Patient patient)
        {
            await _unitOfWork.Patients.AddAsync(patient);
            await _unitOfWork.CompleteAsync();
            return Result.Ok(patient);
        }

        public async Task<Patient> GetPatientById(Guid id)
        {
            return await _unitOfWork.Patients.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Patient>> GetAllPatients()
        {
            return await _unitOfWork.Patients.GetAllAsync();
        }

        public async Task<Result<Patient>> UpdatePatient(Guid id, Patient patient)
        {
            var existingPatient = await _unitOfWork.Patients.GetByIdAsync(patient.Id);
            if (existingPatient == null)
            {
                return Result.Fail("The patient does not exists.");
            }

            existingPatient.FirstName = patient.FirstName;
            existingPatient.LastName = patient.LastName;
            existingPatient.Email = patient.Email;
            existingPatient.PhoneNumber = patient.PhoneNumber;
            existingPatient.DateOfBirth = patient.DateOfBirth;
            await _unitOfWork.Patients.UpdateAsync(existingPatient);
            await _unitOfWork.CompleteAsync();
            return Result.Ok(existingPatient);
        }
    }
}
