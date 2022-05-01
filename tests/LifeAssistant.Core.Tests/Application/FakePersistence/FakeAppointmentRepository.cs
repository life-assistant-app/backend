using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LifeAssistant.Core.Domain.Entities.Appointments;
using LifeAssistant.Core.Persistence;

namespace LifeAssistant.Core.Tests.Application.FakePersistence;

public class FakeAppointmentRepository : IAppointmentRepository
{
    private readonly List<Appointment> data;
    public List<Appointment> DeletedAppointments { get; private set; }

    public FakeAppointmentRepository(List<Appointment> data)
    {
        this.data = data;
        DeletedAppointments = new List<Appointment>();
    }

    public Task<List<Appointment>> FindAppointmentsToDelete()
    {
        return Task.FromResult(data.Where(appointment => appointment.State.Name is "Refused").ToList());
    }

    public Task<List<Appointment>> FindAppointments(int pageIndex)
    {
        return Task.FromResult(data.Skip(pageIndex * 20).Take(20).ToList());
    }

    public Task DeleteAppointments(List<Appointment> appointments)
    {
        DeletedAppointments.AddRange(appointments);
        return Task.CompletedTask;
    }
}