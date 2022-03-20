using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Entities.AppointmentState;

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
        State = domainEntity.State.Name;
        DateTime = domainEntity.DateTime;
    }

    

    public Appointment ToDomainEntity(IAppointmentStateFactory factory)
    {
        return new Appointment(this.Id, this.DateTime, factory, State);
    }
}