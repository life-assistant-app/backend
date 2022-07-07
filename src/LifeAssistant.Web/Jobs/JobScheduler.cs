using Hangfire;
using LifeAssistant.Core.Application.Appointments;

namespace LifeAssistant.Web.Jobs;

public class JobScheduler : JobActivator
{
    private readonly AppointmentsApplication appointmentsApplication;

    public JobScheduler(AppointmentsApplication appointmentsApplication)
    {
        this.appointmentsApplication = appointmentsApplication;
    }

    public override object ActivateJob(Type jobType)
    {
        return base.ActivateJob(jobType);
    }
}