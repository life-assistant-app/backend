using LifeAssistant.Core.Domain.Entities.AppointmentState;
using LifeAssistant.Core.Domain.Exceptions;

namespace LifeAssistant.Core.Domain.Entities;

public class Appointment : Entity
{
    private IAppointmentState state;
    private DateTime dateTime;
    
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
                throw new EntityStateException($"Appointment State {state.Name} does not accept {value.Name} as next state");
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
            : throw new EntityStateException("Appointment date should be in the past");
    }

    private Appointment(Guid id, DateTime dateTime)
        : base(id)
    {
        this.DateTime = dateTime;
    }

    public Appointment(Guid id, DateTime dateTime, IAppointmentStateFactory stateFactory, string stateName)
        : this(id, dateTime)
    {
        state = stateFactory.BuildStateFromAppointment(this, stateName);
    }

    public Appointment(DateTime dateTime)
        : this(Guid.NewGuid(), dateTime)
    {
        State = new PlannedAppointmentState();
    }
}