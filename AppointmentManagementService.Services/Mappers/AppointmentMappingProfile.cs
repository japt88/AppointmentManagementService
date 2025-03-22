using AppointmentManagementService.Domain.Patient;
using AppointmentManagementService.Entities;
using AutoMapper;

namespace AppointmentManagementService.Services.Mappers
{
    public class AppointmentMappingProfile : Profile
    {
        public AppointmentMappingProfile()
        {
            CreateMap<Patient, PatientDto>();

            CreateMap<CreatePatientDto, Patient>();
            CreateMap<CreatePatientDto, Patient>().ReverseMap();
        }
    }
}
