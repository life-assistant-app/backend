using LifeAssistant.Core.Domain.Entities.AppointmentState;

namespace LifeAssistant.Core.Domain.Entities;

public class Appointment : BaseEntity
{
    private IAppointmentState state;
    private ApplicationUser lifeAssistant;
    private DateTime dateTime;
    
    /// Reflexion to simplify state :
    /// Allow setting the state of the appointment, and not using Accept and Pickup
    /// methods (to have only on application method to update state)
    /// The state can be set but the setter check if the current state accepts the
    /// incoming state (have a method on the state interface to get whether the
    /// state supports the upcoming state as next state)
    /// hence, in the application method (need juste one) factory construct the
    /// state passed as string to the application method
    /// and you can safely set it to the appointment
    public IAppointmentState State
    {
        get => state;
        set
        {
            if (IsSettingInitialState(value) || state.AcceptState(value))
            {
                state = value;
                state.Appointment = this;
            }
            else
            {
                throw new InvalidOperationException($"State {state.Name} does not accept {value.Name} as next state");
            }

        }
    }

    private bool IsSettingInitialState(IAppointmentState value)
    {
        return (state is null && value is PlannedAppointmentState);
    }

    public DateTime DateTime
    {
        get => dateTime;
        set => dateTime = value > DateTime.Now
            ? value
            : throw new ArgumentException("Appointment date should be in the past");
    }

    private Appointment(Guid id, DateTime dateTime)
        : base(id)
    {
        this.DateTime = dateTime;
    }

    public Appointment(Guid id, DateTime dateTime, IAppointmentStateFactory stateFactory)
        : this(id, dateTime)
    {
        state = stateFactory.BuildStateFromAppointment(this);
    }

    public Appointment(DateTime dateTime)
        : this(Guid.NewGuid(), dateTime)
    {
        State = new PlannedAppointmentState();
    }
}