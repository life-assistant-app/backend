using LifeAssistant.Core.Domain.Entities.AppointmentState;

namespace LifeAssistant.Core.Domain.Entities;

public class Appointment : BaseEntity
{
    private IAppointmentState state;
    private ApplicationUser lifeAssistant;
    private DateTime dateTime;

    public string State
    {
        get => state.Name;
    }

    public ApplicationUser LifeAssistant
    {
        get => lifeAssistant;
        set => lifeAssistant = value.Role is ApplicationUserRole.LifeAssistant
            ? value
            : throw new ArgumentException("Appointment date should be in the past");
    }

    public DateTime DateTime
    {
        get => dateTime;
        set => dateTime = value > DateTime.Now
            ? value
            : throw new ArgumentException("Appointment date should be in the past");
    }

    private Appointment(Guid id, ApplicationUser lifeAssistant, DateTime dateTime)
        : base(id)
    {
        this.DateTime = dateTime;
        this.LifeAssistant = lifeAssistant;
    }

    public Appointment(Guid id, ApplicationUser lifeAssistant, DateTime dateTime, IAppointmentStateFactory stateFactory)
        : this(id, lifeAssistant, dateTime)
    {
        state = stateFactory.BuildStateFromAppointment(this);
    }

    public Appointment(ApplicationUser lifeAssistant, DateTime dateTime)
        : this(Guid.NewGuid(), lifeAssistant, dateTime)
    {
        state = new PlannedAppointmentState(this);
    }

    public void Accept()
    {
        this.state = this.state.Accept();
    }

    public void Pickup()
    {
        this.state = this.state.PickUp();
    }
}