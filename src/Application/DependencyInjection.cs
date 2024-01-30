﻿using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using StoreOnline.Application.Common.Behaviours;
using StoreOnline.Application.Services;
using StoreOnline.Application.Validations;

namespace StoreOnline.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        });

        services.AddScoped<CreateOrderServices>();
        services.AddScoped<UpdateOrderServices>();
        services.AddScoped<CustomerExistsValidator>(); 
        services.AddScoped<ProductOnStockValidator>(); 
        return services;
    }
}
