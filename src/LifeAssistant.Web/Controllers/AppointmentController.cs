using LifeAssistant.Core.Application.Appointments;
using LifeAssistant.Core.Application.Appointments.Contracts;
using LifeAssistant.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LifeAssistant.Web.Controllers;

[ApiController]
[Route("/api")]
public class AppointmentController : ControllerBase
{
    private readonly AppointmentsApplication application;

    public AppointmentController(AppointmentsApplication application)
    {
        this.application = application;
    }

    /// <summary>
    /// Get the list of existing appointments
    /// </summary>
    /// <returns></returns>
    [HttpGet("appointments")]
    public async Task<ActionResult<List<GetAppointmentResponse>>> GetAppointments()
    {
        return Ok(await this.application.GetAllAppointments());
    }

    /// <summary>
    /// Creates a new appointment for a life assistant.
    /// Only Agency Employees may call this endpoint
    /// </summary>
    /// <param name="id">Id of the life assistant to create the appointment</param>
    /// <param name="request">Appointment creation payload</param>
    /// <returns>The resulting appointment</returns>
    [HttpPost("assistants/{id:guid}/appointments")]
    public async Task<ActionResult<GetAppointmentResponse>> CreateAppointment(Guid id,
        [FromBody] CreateAppointmentRequest request)
    {
        return Created("/api/assistants/{id:guid}/appointments",
            await application.CreateAppointment(id, request.DateTime));
    }
    
    /// <summary>
    /// Get the appointments of a life assistant
    /// The appointment can optionally be filtered by state
    /// </summary>
    /// <param name="id">The life assistant to get the appointments from</param>
    /// <param name="state">Optional state to filter the appointments</param>
    /// <returns>The list of corresponding appointments</returns>
    [HttpGet("assistants/{id:guid}/appointments")]
    public async Task<ActionResult<GetAppointmentResponse>> GetLifeAssistantAppointment(Guid id, [FromQuery] string? state)
    {
        List<GetAppointmentResponse> lifeAssistantAppointments = await application.GetLifeAssistantAppointments(id, state);
        return Ok(lifeAssistantAppointments);
    }

    /// <summary>
    /// Sets the state of an existing appointment
    /// Must be called by the life assistant who owns the appointment or an agency employee
    /// </summary>
    /// <returns>The updated appointment</returns>
    [HttpPut("assistants/{lifeAssistantId:guid}/appointments/{appointmentId:guid}")]
    public async Task<ActionResult<GetAppointmentResponse>> SetAppointmentState([FromBody] SetAppointStateRequest dto,
        Guid lifeAssistantId, Guid appointmentId)
    {
        GetAppointmentResponse getAppointmentResponse = await application.SetAppointmentState(lifeAssistantId, appointmentId, dto);
        return Ok(getAppointmentResponse);
    }
}