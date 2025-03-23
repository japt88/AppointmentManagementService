using AppointmentManagementService.Domain.Appointment;
using AppointmentManagementService.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/v1/appointments")]
[ApiExplorerSettings(GroupName = "v1")]
[Produces("application/json")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    /// <summary>
    /// Schedules a new appointment.
    /// </summary>
    /// <param name="appointmentDto">The appointment details.</param>
    /// <returns>The scheduled appointment.</returns>
    /// <response code="200">Returns the scheduled appointment.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpPost]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ScheduleAppointment([FromBody] CreateAppointmentDto appointmentDto)
    {
        var result = await _appointmentService.ScheduleAppointment(appointmentDto);
        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    /// <summary>
    /// Retrieves appointments for a specific patient.
    /// </summary>
    /// <param name="patientId">The patient's ID.</param>
    /// <returns>A list of appointments for the given patient.</returns>
    /// <response code="200">Returns the list of appointments.</response>
    /// <response code="404">If no appointments are found for the patient.</response>
    [HttpGet("patient/{patientId}")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentsByPatient(Guid patientId)
    {
        var result = await _appointmentService.GetPatientAppointments(patientId);
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Cancels an appointment.
    /// </summary>
    /// <param name="appointmentId">The appointment ID.</param>
    /// <returns>A success message if cancellation is successful.</returns>
    /// <response code="200">If the appointment was successfully canceled.</response>
    /// <response code="400">If the cancellation request is invalid.</response>
    [HttpPut("{appointmentId}/cancel")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelAppointment(Guid appointmentId)
    {
        var result = await _appointmentService.CancelAppointment(appointmentId);
        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok("Appointment canceled successfully.");
    }
}
