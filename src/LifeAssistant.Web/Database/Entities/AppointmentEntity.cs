using LifeAssistant.Core.Domain.Entities;

namespace LifeAssistant.Web.Database.Entities;

public class AppointmentEntity : BaseDbEntity
{
    public string State { get; set; }
    public DateTime DateTime { get; set; }

    public AppointmentEntity()
    {
    }

    public AppointmentEntity(Appointment domainEntity)
    {
        Id = domainEntity.Id;
        State = domainEntity.State.Name;
        DateTime = domainEntity.DateTime;
    }


    public Appointment ToDomainEntity(IAppointmentStateFactory factory)
    {
        return new Appointment(this.Id, this.DateTime, factory, State);
    }
}