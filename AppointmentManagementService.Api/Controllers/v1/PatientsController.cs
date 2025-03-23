using AppointmentManagementService.Domain.Patient;
using AppointmentManagementService.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/patients")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    /// <summary>
    /// Creates a new patient.
    /// </summary>
    /// <param name="patientDto">The patient data to create.</param>
    /// <returns>The created patient.</returns>
    /// <response code="200">Returns the created patient.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpPost]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDto patientDto)
    {
        var result = await _patientService.CreatePatient(patientDto);
        if (result.IsFailed)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a patient by ID.
    /// </summary>
    /// <param name="id">The patient ID.</param>
    /// <returns>The patient details.</returns>
    /// <response code="200">Returns the patient data.</response>
    /// <response code="404">If the patient is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPatientById(Guid id)
    {
        var result = await _patientService.GetPatientById(id);
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Gets all patients.
    /// </summary>
    /// <returns>A list of patients.</returns>
    /// <response code="200">Returns a list of patients.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PatientDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPatients()
    {
        var patients = await _patientService.GetAllPatients();
        return Ok(patients);
    }

    /// <summary>
    /// Updates a patient's information.
    /// </summary>
    /// <param name="id">The patient ID.</param>
    /// <param name="patientDto">The updated patient data.</param>
    /// <returns>The updated patient.</returns>
    /// <response code="200">Returns the updated patient.</response>
    /// <response code="404">If the patient is not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePatient(Guid id, [FromBody] CreatePatientDto patientDto)
    {
        var result = await _patientService.UpdatePatient(id, patientDto);
        if (result.IsFailed)
            return NotFound(result.Errors);

        return Ok(result.Value);
    }
}
