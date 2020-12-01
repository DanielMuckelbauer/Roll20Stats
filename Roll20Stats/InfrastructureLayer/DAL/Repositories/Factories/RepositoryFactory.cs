﻿using Roll20Stats.InfrastructureLayer.DAL.Context;
using Roll20Stats.InfrastructureLayer.DAL.Repositories.ReadOnlyRepository;
using Roll20Stats.InfrastructureLayer.DAL.Repositories.SavingRepositories;

namespace Roll20Stats.InfrastructureLayer.DAL.Repositories.Factories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IApplicationContext _applicationContext;

        public RepositoryFactory(IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public IReadOnlyRepository<TModel> CreateOnlyRepository<TModel>() where TModel : class
        {
            return new ReadOnlyRepository<TModel>(_applicationContext);
        }

        public ISavingRepository<TModel> CreateSavingRepository<TModel>() where TModel : class
        {
            return new SavingRepository<TModel>(_applicationContext);
        }
    }
}