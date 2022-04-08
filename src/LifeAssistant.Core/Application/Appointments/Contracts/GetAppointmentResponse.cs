using LifeAssistant.Core.Domain.Entities;

namespace LifeAssistant.Core.Application.Appointments.Contracts;

public record GetAppointmentResponse(Guid Id, string State, DateTime DateTime, Guid LifeAssistantId);