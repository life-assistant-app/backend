namespace LifeAssistant.Core.Domain.Entities;

public interface IAppointmentState
{
    string Name { get; }
    
    IAppointmentState Accept();
    IAppointmentState PickUp();
}