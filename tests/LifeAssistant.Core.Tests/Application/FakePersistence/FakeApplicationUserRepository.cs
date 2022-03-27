using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Persistence;

namespace LifeAssistant.Core.Tests.Application.FakePersistence;

public class FakeApplicationUserRepository : IApplicationUserRepository
{
    public IReadOnlyList<IApplicationUserWithAppointments> Data { get; private set; }
    private List<IApplicationUserWithAppointments> newData;

    public FakeApplicationUserRepository()
    {
        Data = new List<IApplicationUserWithAppointments>();
        newData = new List<IApplicationUserWithAppointments>();
    }

    public async Task<IApplicationUser> FindByUsername(string username)
    {
        return this.Data.First(user => user.UserName == username);
    }

    public async Task<IApplicationUserWithAppointments> FindByIdWithAppointments(Guid entityId)
    {
        return this.Data.First(user => user.Id == entityId);
    }

    public Task<IList<IApplicationUser>> FindValidatedByRole(ApplicationUserRole role)
    {
        IList<IApplicationUser> applicationUserWithAppointmentsList = this
            .Data
            .Where(user => user.Role == role)
            .Select(user => user as IApplicationUser)
            .ToList();
        
        return Task.FromResult(applicationUserWithAppointmentsList);
    }


    public Task Save()
    {
        this.Data = newData.ToList();
        return Task.CompletedTask;
    }

    public Task Insert(IApplicationUserWithAppointments entity)
    {
        this.newData.Add(entity);
        return Task.CompletedTask;
    }


    public Task Update(IApplicationUserWithAppointments entity)
    {
        int index = this.newData.FindIndex(elt => elt.Id == entity.Id);
        this.newData[index] = entity;

        return Task.CompletedTask;
    }

    public async Task<IApplicationUser> FindById(Guid entityId)
    {
        return await this.FindByIdWithAppointments(entityId);
    }
}