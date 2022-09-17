using Chameleon.Common.Context;
using Chameleon.Faas.Management.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chameleon.Faas.Management.Repository
{
    public interface IRepositoryBase<TEntity> where TEntity : EntityBase
    {
        List<TEntity> GetUnDeletedList();
        void Add(TEntity entity);
        void BatchAdd(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void BatchUpdate(IEnumerable<TEntity> entities);
        void LogicDelete(TEntity entity);
        void Recover(TEntity entity);
    }

    public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase
    {
        protected readonly FaasDbContext _faasDbContext;
        protected int UserId => ChameleonContext.Current.UserId;

        protected RepositoryBase(FaasDbContext faasDbContext)
        {
            _faasDbContext = faasDbContext;
        }

        public void Add(TEntity entity)
        {
            entity.CreateTime = DateTime.Now;
            entity.CreateBy = UserId;
            entity.ModifyTime = DateTime.Now;
            entity.ModifyBy = UserId;

            _faasDbContext.Add(entity);
            _faasDbContext.SaveChanges();
        }

        public void BatchAdd(IEnumerable<TEntity> entities)
        {
            foreach (var item in entities)
            {
                item.CreateTime = DateTime.Now;
                item.CreateBy = UserId;
                item.ModifyTime = DateTime.Now;
                item.ModifyBy = UserId;
            }

            _faasDbContext.AddRange(entities);
            _faasDbContext.SaveChanges();
        }

        public void BatchUpdate(IEnumerable<TEntity> entities)
        {
            foreach (var item in entities)
            {
                item.ModifyTime = DateTime.Now;
                item.ModifyBy = UserId;
            }

            _faasDbContext.UpdateRange(entities);
            _faasDbContext.SaveChanges();
        }

        public List<TEntity> GetUnDeletedList()
        {
            return _faasDbContext.Set<TEntity>().Where(t => t.IsDeleted == 0).ToList();
        }

        public void LogicDelete(TEntity entity)
        {
            entity.IsDeleted = 1;
            entity.ModifyTime = DateTime.Now;
            entity.ModifyBy = UserId;

            _faasDbContext.Update(entity);
            _faasDbContext.SaveChanges();
        }

        public void Recover(TEntity entity)
        {
            entity.IsDeleted = 0;
            entity.ModifyTime = DateTime.Now;
            entity.ModifyBy = UserId;

            _faasDbContext.Update(entity);
            _faasDbContext.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            entity.ModifyTime = DateTime.Now;
            entity.ModifyBy = UserId;

            _faasDbContext.Update(entity);
            _faasDbContext.SaveChanges();
        }
    }
}
