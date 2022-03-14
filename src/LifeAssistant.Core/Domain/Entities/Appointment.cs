using LifeAssistant.Core.Domain.Entities.AppointmentState;

namespace LifeAssistant.Core.Domain.Entities;

public class Appointment : BaseEntity
{
    private IAppointmentState state;
    private ApplicationUser lifeAssistant;
    private DateTime dateTime;
    
    /// Reflexion to simplify state :
    /// Allow setting the state of the appointment, and not using Accept and Pickup methods (to have only on application method to update state)
    /// The state can be set but the setter check if the current state accepts the incoming state (have a method on the state interface to get whether the state supports the upcoming state as next state)
    /// hence, in the application method (need juste one) factory construct the state passed as string to the application method
    /// and you can safely set it to the appointment
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