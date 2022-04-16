using LifeAssistant.Core.Application.Appointments.Contracts;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Entities.ApplicationUser;
using LifeAssistant.Core.Domain.Exceptions;
using LifeAssistant.Core.Domain.Rules;
using LifeAssistant.Core.Persistence;

namespace LifeAssistant.Core.Application.Appointments;

public class AppointmentsApplication
{
    private readonly IApplicationUserRepository applicationUserRepository;
    private readonly AccessControlManager accessControlManager;
    private readonly IAppointmentStateFactory appointmentStateFactory;

    public AppointmentsApplication(IApplicationUserRepository applicationUserRepository,
        AccessControlManager accessControlManager, IAppointmentStateFactory appointmentStateFactory)
    {
        this.applicationUserRepository = applicationUserRepository;
        this.accessControlManager = accessControlManager;
        this.appointmentStateFactory = appointmentStateFactory;
    }

    public async Task<GetAppointmentResponse> SetAppointmentState(Guid lifeAssistantId, Guid appointmentId,
        SetAppointStateRequest state)
    {
        await this.accessControlManager.EnsureUserCanUpdateAppointmentState(lifeAssistantId);
        
        IApplicationUserWithAppointments lifeAssistant = await this.applicationUserRepository
            .FindByIdWithAppointments(lifeAssistantId);

        Appointment appointmentToUpdate =
            lifeAssistant.Appointments.FirstOrDefault(appointment => appointment.Id == appointmentId) ??
            throw new EntityNotFoundException(
                $"No appointment with id {appointmentId} for life assistant with id {lifeAssistantId}");

        appointmentToUpdate.State =
            this.appointmentStateFactory.BuildStateFromAppointment(appointmentToUpdate, state.StateName);

        await this.applicationUserRepository.Update(lifeAssistant);
        await this.applicationUserRepository.Save();
        
        return new GetAppointmentResponse(
            appointmentToUpdate.Id,
            appointmentToUpdate.State.Name,
            appointmentToUpdate.DateTime,
            lifeAssistantId
        );
    }

    public async Task<GetAppointmentResponse> CreateAppointment(Guid lifeAssistantId, DateTime dateTime)
    {
        await this.accessControlManager.EnsureUserCanCreateAppointment();

        IApplicationUserWithAppointments lifeAssistant =
            await applicationUserRepository.FindByIdWithAppointments(lifeAssistantId);
        var appointment = new Appointment(dateTime);
        lifeAssistant.Appointments.Add(appointment);

        await this.applicationUserRepository.Update(lifeAssistant);
        await this.applicationUserRepository.Save();

        return new GetAppointmentResponse(appointment.Id,
            appointment.State.Name,
            appointment.DateTime,
            lifeAssistant.Id
        );
    }

    public async Task<List<GetAppointmentResponse>> GetAppointments()
    {
        await this.accessControlManager.EnsureIsAgencyEmployee();
        
        List<IApplicationUserWithAppointments> applicationUsers = await this.applicationUserRepository
            .FindValidatedWithAppointmentByRole(ApplicationUserRole.LifeAssistant);

        return applicationUsers
            .SelectMany(applicationUser =>
                applicationUser
                    .Appointments
                    .Select(appointment => new GetAppointmentResponse(
                        appointment.Id,
                        appointment.State.Name,
                        appointment.DateTime,
                        applicationUser.Id
                    ))
            )
            .ToList();
    }
}