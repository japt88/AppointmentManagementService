using AppointmentManagementService.Domain.Patient;
using AppointmentManagementService.Entities;
using AutoMapper;

namespace AppointmentManagementService.Services.Mappers
{
    public class AppointmentMappingProfile : Profile
    {
        public AppointmentMappingProfile()
        {
            CreateMap<Patient, PatientDto>()
               .ForMember(d => d.Phone, opt => opt.MapFrom(x => x.PhoneNumber));

            CreateMap<CreatePatientDto, Patient>()
               .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(x => x.Phone));
        }
    }
}
