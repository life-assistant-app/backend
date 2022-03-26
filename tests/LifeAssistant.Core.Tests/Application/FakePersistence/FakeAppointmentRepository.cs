using System.Collections.Generic;
using System.Threading.Tasks;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Persistence;

namespace LifeAssistant.Core.Tests.Application.FakePersistence;

public class FakeAppointmentRepository : IAppointmentRepository
{
    private readonly IList<Appointment> data;


    public FakeAppointmentRepository(IList<Appointment> data)
    {
        this.data = data;
    }

    public Task<IList<Appointment>> GetAppointments()
    {
        return Task.FromResult(data);
    }
}