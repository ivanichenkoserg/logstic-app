﻿using System.Collections;
using Logistics.Domain.Core;
using Logistics.Domain.Persistence;
using Logistics.Infrastructure.EF.Data;

namespace Logistics.Infrastructure.EF.Persistence;

public class MasterUnitOfWork : IMasterUnityOfWork
{
    private readonly MasterDbContext _masterDbContext;
    private readonly Hashtable _repositories = new();

    public MasterUnitOfWork(MasterDbContext masterDbContext)
    {
        _masterDbContext = masterDbContext;
    }

    public IMasterRepository2<TEntity> Repository<TEntity>() where TEntity : class, IEntity<string>
    {
        var type = typeof(TEntity).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(MasterRepository2<>);

            var repositoryInstance =
                Activator.CreateInstance(repositoryType
                    .MakeGenericType(typeof(TEntity)), _masterDbContext);

            _repositories.Add(type, repositoryInstance);
        }

        if (_repositories[type] is not IMasterRepository2<TEntity> repository)
        {
            throw new InvalidOperationException("Could not create a repository");
        }
        
        return repository;
    }

    public Task<int> CommitAsync()
    {
        return _masterDbContext.SaveChangesAsync();
    }
    
    public void Dispose()
    {
        _masterDbContext.Dispose();
    }
}
