﻿using Logistics.Application.Core;
using Logistics.Domain.Entities;
using Logistics.Domain.Persistence;
using Logistics.Domain.Services;
using Logistics.Shared;

namespace Logistics.Application.Admin.Commands;

internal sealed class CreateTenantHandler : RequestHandler<CreateTenantCommand, ResponseResult>
{
    private readonly ITenantDatabaseService _tenantDatabase;
    private readonly IMasterRepository _repository;

    public CreateTenantHandler(
        ITenantDatabaseService tenantDatabase,
        IMasterRepository repository)
    {
        _tenantDatabase = tenantDatabase;
        _repository = repository;
    }

    protected override async Task<ResponseResult> HandleValidated(CreateTenantCommand req, CancellationToken cancellationToken)
    {
        var tenant = new Tenant
        {
            Name = req.Name.Trim().ToLower(),
            CompanyName = req.CompanyName,
            CompanyAddress = req.CompanyAddress
        };
        tenant.ConnectionString = _tenantDatabase.GenerateConnectionString(tenant.Name); 

        var existingTenant = await _repository.GetAsync<Tenant>(i => i.Name == tenant.Name);
        if (existingTenant != null)
        {
            return ResponseResult.CreateError($"Tenant name '{tenant.Name}' is already taken, please chose another name");
        }

        var created = await _tenantDatabase.CreateDatabaseAsync(tenant.ConnectionString);
        if (!created)
        {
            return ResponseResult.CreateError("Could not create the tenant's database");
        }

        await _repository.AddAsync(tenant);
        await _repository.UnitOfWork.CommitAsync();
        return ResponseResult.CreateSuccess();
    }
}
