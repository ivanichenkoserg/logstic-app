﻿using Logistics.Domain.Shared;
using Logistics.WebApi.Authorization.Requirements;

namespace Logistics.WebApi.Authorization.Handlers;

public class TenantCanReadHandler : AuthorizationHandler<TenantCanReadRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantCanReadRequirement requirement)
    {
        if (context.User.IsInRole(AppRoles.Admin))
        {
            context.Succeed(requirement);
        }
    
        return Task.CompletedTask;
    }
}