using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using StoreOnline.Application.Common.Behaviours;
using StoreOnline.Application.Common.Interfaces;
using StoreOnline.Application.Orders.Commands.CreateOrder;
using StoreOnline.Application.Orders.Commands.UpdateOrder;
using StoreOnline.Application.Services;
using StoreOnline.Application.Validations;
using StoreOnline.Domain.Common;

namespace StoreOnline.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        });

        services.AddKeyedScoped<ICreateOrderServices<CreateOrderCommand>, CreateOrderServices>(
            nameof(CreateOrderServices));
        services.AddKeyedScoped<ICreateOrderServices<UpdateOrderCommand>, UpdateOrderServices>(
            nameof(UpdateOrderServices));
        services.AddScoped<IDomainValidator<IOrderCommand>, OrderValidatorManager>();
        return services;
    }
}
