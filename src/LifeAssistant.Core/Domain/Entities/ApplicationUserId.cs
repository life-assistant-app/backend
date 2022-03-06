using StronglyTypedIds;

namespace LifeAssistant.Core.Domain.Entities;

[StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson | StronglyTypedIdConverter.EfCoreValueConverter)]
public partial struct ApplicationUserId
{
    
}