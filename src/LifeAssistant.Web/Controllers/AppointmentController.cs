using LifeAssistant.Core.Application.Appointments;
using LifeAssistant.Core.Application.Appointments.Contracts;
using LifeAssistant.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LifeAssistant.Web.Controllers;

[ApiController]
[Route("/api/assistants/{id:guid}/appointments")]
public class AppointmentController : ControllerBase
{
    private readonly AppointmentsApplication application;

    public AppointmentController(AppointmentsApplication application)
    {
        this.application = application;
    }

    /// <summary>
    /// Creates a new appointment for a life assistant.
    /// Only Agency Employees may call this endpoint
    /// </summary>
    /// <param name="id">Id of the life assistant to create the appointment</param>
    /// <param name="request">Appointment creation payload</param>
    /// <returns>The resulting appointment</returns>
    [HttpPost]
    public async Task<ActionResult<GetAppointmentResponse>> CreateAppointment(Guid id, [FromBody] CreateAppointmentRequest request)
    {
        return Created("/api/assistants/{id:guid}/appointments",
            await application.CreateAppointment(id, request.DateTime));
    }
}