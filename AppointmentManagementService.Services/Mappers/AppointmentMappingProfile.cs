﻿using AppointmentManagementService.Domain.Appointment;
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
               .ForMember(d => d.Phone, opt => opt.MapFrom(x => x.PhoneNumber))
               .ForMember(d => d.Appointments, opt => opt.MapFrom(x => x.Appointments));

            CreateMap<CreatePatientDto, Patient>()
               .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(x => x.Phone));

            CreateMap<Appointment, AppointmentDto>();

            CreateMap<CreateAppointmentDto, Appointment>();
        }
    }
}
