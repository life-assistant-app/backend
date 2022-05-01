using LifeAssistant.Core.Domain.Entities.AppointmentState;
using LifeAssistant.Core.Domain.Exceptions;

namespace LifeAssistant.Core.Domain.Entities.Appointments;

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
                throw new EntityStateException(
                    $"Appointment State {state.Name} does not accept {value.Name} as next state");
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
        set => dateTime = value > DateTime.Now || State.Name is "Finished"
            ? value
            : throw new EntityStateException("Appointment date should be in the past");
    }

    public DateOnly CreatedDate { get; private set; }

    public Appointment(Guid id, DateTime dateTime, IAppointmentStateFactory stateFactory, string stateName, DateOnly createdDate)
        : base(id)
    {
        state = stateFactory.BuildStateFromAppointment(this, stateName);
        DateTime = dateTime;
        CreatedDate = createdDate;
    }

    public Appointment(DateTime dateTime)
        : base(Guid.NewGuid())
    {
        State = new PlannedAppointmentState();
        DateTime = dateTime;
        CreatedDate = DateOnly.FromDateTime(DateTime.Now);
    }
}