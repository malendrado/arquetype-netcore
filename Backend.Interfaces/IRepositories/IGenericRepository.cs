﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Backend.Interfaces.IRepositories
{
    public interface IGenericRepository<TModel, TEntity> where TModel : class where TEntity : class
    {
        #region Get
        TModel GetById(object id);
        Task<TModel> GetByIdAsync(object id);
        IList<TModel> GetAll();
        Task<IList<TModel>> GetAllAsync();
        #endregion

        #region Add
        TModel Add(TModel modelToAdd);
        Task<TModel> AddAsync(TModel modelToAdd);
        void AddRange(IList<TModel> modelsToAdd);
        Task AddRangeAsync(IList<TModel> modelsToAdd);
        #endregion

        #region Update
        void Update(TModel modelToUpdate);
        void UpdateRange(IList<TModel> modelsToUpdate);
        #endregion

        #region Delete
        void DeleteById(object id);
        void Delete(TModel modelToDelete);
        void DeleteRange(IList<TModel> modelsToDelete);
        #endregion

        #region Global
        Task<int> SaveChangesAsync();
        bool Exist(Expression<Func<TModel, bool>> predicate);
        #endregion
    }
}