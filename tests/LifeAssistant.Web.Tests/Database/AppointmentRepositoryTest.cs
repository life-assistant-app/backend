using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Entities.ApplicationUser;
using LifeAssistant.Core.Domain.Entities.Appointments;
using LifeAssistant.Core.Domain.Entities.AppointmentState;
using LifeAssistant.Web.Database.Entities;
using LifeAssistant.Web.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LifeAssistant.Web.Tests.Database;

public class AppointmentRepositoryTest : DatabaseTest
{
    [Fact]
    public async Task FindAppointmentToDelete_ReturnsRefusedAppointments()
    {
        // Given
        ApplicationUserEntity lifeAssistant = await this.dbDataFactory.InsertLifeAssistant(true);

        AppointmentEntity[] appointmentEntities = {
            new()
            {
                Id = Guid.NewGuid(), 
                State = "Refused", 
                DateTime = DateTime.Now.AddDays(30),
                LifeAssistantId = lifeAssistant.Id,
                CreatedDate = DateOnly.FromDateTime(DateTime.Today)
            },
            new()
            {
                Id = Guid.NewGuid(), 
                State = "Planned", 
                DateTime = DateTime.Now.AddDays(1),
                LifeAssistantId = lifeAssistant.Id,
                CreatedDate = DateOnly.FromDateTime(DateTime.Today.Subtract(TimeSpan.FromDays(3)))
            }
        };

        Guid secondAppointmentId = appointmentEntities[0].Id;

        await this.context.Appointments.AddRangeAsync(appointmentEntities);
        await this.context.SaveChangesAsync();

        AppointmentRepository repository = new AppointmentRepository(this.context, new AppointmentStateFactory());

        // When
        List<Appointment> appointmentsToDelete = await repository.FindAppointmentsToDelete();
        
        // Then
        appointmentsToDelete.Should().ContainSingle();
        appointmentsToDelete.First().Id.Should().Be(secondAppointmentId);
        
    }
    
    [Fact]
    public async Task FindAppointmentToDelete_ReturnsPlannedAppointmentNotAccepted()
    {
        // Given
        ApplicationUserEntity lifeAssistant = await this.dbDataFactory.InsertLifeAssistant(true);

        AppointmentEntity[] appointmentEntities = {
            new()
            {
                Id = Guid.NewGuid(), 
                State = "Planned", 
                DateTime = DateTime.Now.AddDays(30),
                LifeAssistantId = lifeAssistant.Id,
                CreatedDate = DateOnly.FromDateTime(DateTime.Today.Subtract(TimeSpan.FromDays(15)))
            },
            new()
            {
                Id = Guid.NewGuid(), 
                State = "Planned", 
                DateTime = DateTime.Now.AddDays(1),
                LifeAssistantId = lifeAssistant.Id,
                CreatedDate = DateOnly.FromDateTime(DateTime.Today.Subtract(TimeSpan.FromDays(3)))
            }
        };

        Guid secondAppointmentId = appointmentEntities[0].Id;

        await this.context.Appointments.AddRangeAsync(appointmentEntities);
        await this.context.SaveChangesAsync();

        AppointmentRepository repository = new AppointmentRepository(this.context, new AppointmentStateFactory());

        // When
        List<Appointment> appointmentsToDelete = await repository.FindAppointmentsToDelete();
        
        // Then
        appointmentsToDelete.Should().ContainSingle();
        appointmentsToDelete.First().Id.Should().Be(secondAppointmentId);
        
    }
    
    [Fact]
    public async Task FindAppointmentToDelete_ReturnsOldFinishedAppointments()
    {
        // Given
        ApplicationUserEntity lifeAssistant = await this.dbDataFactory.InsertLifeAssistant(true);

        AppointmentEntity[] appointmentEntities = {
            new()
            {
                Id = Guid.NewGuid(), 
                State = "Finished", 
                DateTime = DateTime.Now.Subtract(TimeSpan.FromDays(360)),
                LifeAssistantId = lifeAssistant.Id,
                CreatedDate = DateOnly.FromDateTime(DateTime.Today.Subtract(TimeSpan.FromDays(370)))
            },
            new()
            {
                Id = Guid.NewGuid(), 
                State = "Planned", 
                DateTime = DateTime.Now.AddDays(1),
                LifeAssistantId = lifeAssistant.Id,
                CreatedDate = DateOnly.FromDateTime(DateTime.Today.Subtract(TimeSpan.FromDays(3)))
            }
        };

        Guid secondAppointmentId = appointmentEntities[0].Id;

        await this.context.Appointments.AddRangeAsync(appointmentEntities);
        await this.context.SaveChangesAsync();

        AppointmentRepository repository = new AppointmentRepository(this.context, new AppointmentStateFactory());

        // When
        List<Appointment> appointmentsToDelete = await repository.FindAppointmentsToDelete();
        
        // Then
        appointmentsToDelete.Should().ContainSingle();
        appointmentsToDelete.First().Id.Should().Be(secondAppointmentId);
        
    }
    
    [Fact]
    public async Task DeleteAppointments_DeletesOnlyExpectedAppointments()
    {
        // Given
        ApplicationUserEntity lifeAssistant = await this.dbDataFactory.InsertLifeAssistantWithAppointments(true);

        Guid secondAppointmentId = lifeAssistant.Appointments[1].Id;
        
        AppointmentRepository repository = new AppointmentRepository(this.context, new AppointmentStateFactory());
        AppointmentStateFactory appointmentStateFactory = new AppointmentStateFactory();
        
        // When
        await repository.DeleteAppointments(new List<Appointment>
        {
            lifeAssistant.Appointments[0].ToDomainEntity(appointmentStateFactory),
        });
        await repository.Save();
        
        // Then
        int appointmentsCount = await this.context.Appointments.CountAsync();
        appointmentsCount.Should().Be(1);

        AppointmentEntity entity = await this.context.Appointments.FirstAsync();
        entity.Id.Should().Be(secondAppointmentId);
    }
}