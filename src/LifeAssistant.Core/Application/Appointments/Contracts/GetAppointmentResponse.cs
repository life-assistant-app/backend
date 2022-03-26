namespace LifeAssistant.Core.Application.Appointments.Contracts;

public record GetAppointmentResponse(Guid Id, string State, DateTime DateTime);