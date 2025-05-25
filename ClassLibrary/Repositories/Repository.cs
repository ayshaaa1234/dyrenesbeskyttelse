using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ClassLibrary.Exceptions;
using ClassLibrary.Models;
using ClassLibrary.Interfaces;

namespace ClassLibrary.Repositories
{
    /// <summary>
    /// Generisk repository klasse der implementerer grundlæggende CRUD operationer
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly List<T> _items;

        public Repository()
        {
            _items = new List<T>();
        }

        /// <summary>
        /// Henter alle enheder
        /// </summary>
        public virtual Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return Task.FromResult(_items.AsEnumerable());
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved hentning af alle {typeof(T).Name}", ex);
            }
        }

        /// <summary>
        /// Henter en enhed baseret på ID
        /// </summary>
        public virtual Task<T?> GetByIdAsync(int id)
        {
            try
            {
                return Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved hentning af {typeof(T).Name} med ID {id}", ex);
            }
        }

        /// <summary>
        /// Tilføjer en ny enhed
        /// </summary>
        public virtual Task<T> AddAsync(T entity)
        {
            try
            {
                ValidateEntity(entity);
                
                if (entity is ISoftDelete softDelete)
                {
                    softDelete.IsDeleted = false;
                    softDelete.DeletedAt = null;
                }

                _items.Add(entity);
                return Task.FromResult(entity);
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved tilføjelse af {typeof(T).Name}", ex);
            }
        }

        /// <summary>
        /// Opdaterer en eksisterende enhed
        /// </summary>
        public virtual async Task<T> UpdateAsync(T entity)
        {
            try
            {
                ValidateEntity(entity);
                
                var existingEntity = await GetByIdAsync(entity.Id);
                if (existingEntity == null)
                {
                    throw new RepositoryException($"{typeof(T).Name} med ID {entity.Id} blev ikke fundet");
                }

                var index = _items.FindIndex(x => x.Id == entity.Id);
                if (index != -1)
                {
                    _items[index] = entity;
                }

                return entity;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved opdatering af {typeof(T).Name} med ID {entity.Id}", ex);
            }
        }

        /// <summary>
        /// Sletter en enhed
        /// </summary>
        public virtual async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity == null)
                {
                    throw new RepositoryException($"{typeof(T).Name} med ID {id} blev ikke fundet");
                }

                if (entity is ISoftDelete softDelete)
                {
                    softDelete.IsDeleted = true;
                    softDelete.DeletedAt = DateTime.Now;
                    await UpdateAsync(entity);
                }
                else
                {
                    _items.RemoveAll(x => x.Id == id);
                }
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved sletning af {typeof(T).Name} med ID {id}", ex);
            }
        }

        /// <summary>
        /// Finder enheder baseret på et predikat
        /// </summary>
        public virtual Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return Task.FromResult(_items.AsQueryable().Where(predicate).AsEnumerable());
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved søgning i {typeof(T).Name}", ex);
            }
        }

        /// <summary>
        /// Finder enheder baseret på flere predikater
        /// </summary>
        public virtual Task<IEnumerable<T>> FindAllAsync(IEnumerable<Expression<Func<T, bool>>> predicates)
        {
            try
            {
                var query = _items.AsQueryable();
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate);
                }
                return Task.FromResult(query.AsEnumerable());
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved søgning i {typeof(T).Name}", ex);
            }
        }

        /// <summary>
        /// Finder enheder baseret på et predikat og sorterer dem
        /// </summary>
        public virtual Task<IEnumerable<T>> FindAndSortAsync<TKey>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TKey>> sortKey,
            bool ascending = true)
        {
            try
            {
                var query = _items.AsQueryable().Where(predicate);
                query = ascending ? query.OrderBy(sortKey) : query.OrderByDescending(sortKey);
                return Task.FromResult(query.AsEnumerable());
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved sorteret søgning i {typeof(T).Name}", ex);
            }
        }

        /// <summary>
        /// Finder enheder baseret på et predikat og returnerer et bestemt antal
        /// </summary>
        public virtual Task<IEnumerable<T>> FindAndTakeAsync(
            Expression<Func<T, bool>> predicate,
            int count)
        {
            try
            {
                return Task.FromResult(
                    _items.AsQueryable()
                        .Where(predicate)
                        .Take(count)
                        .AsEnumerable());
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved begrænset søgning i {typeof(T).Name}", ex);
            }
        }

        /// <summary>
        /// Finder enheder baseret på et predikat og springer et bestemt antal over
        /// </summary>
        public virtual Task<IEnumerable<T>> FindAndSkipAsync(
            Expression<Func<T, bool>> predicate,
            int count)
        {
            try
            {
                return Task.FromResult(
                    _items.AsQueryable()
                        .Where(predicate)
                        .Skip(count)
                        .AsEnumerable());
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved springende søgning i {typeof(T).Name}", ex);
            }
        }

        /// <summary>
        /// Finder enheder baseret på et predikat og grupperer dem
        /// </summary>
        public virtual Task<IDictionary<TKey, IEnumerable<T>>> FindAndGroupAsync<TKey>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TKey>> groupKey) where TKey : notnull
        {
            try
            {
                var result = _items.AsQueryable()
                    .Where(predicate)
                    .GroupBy(groupKey)
                    .ToDictionary(g => g.Key, g => g.AsEnumerable());
                return Task.FromResult<IDictionary<TKey, IEnumerable<T>>>(result);
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved grupperet søgning i {typeof(T).Name}", ex);
            }
        }

        /// <summary>
        /// Validerer en enhed
        /// </summary>
        protected virtual void ValidateEntity(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
        }

        /// <summary>
        /// Henter sideinddelte resultater
        /// </summary>
        public virtual Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null)
        {
            try
            {
                var query = _items.AsQueryable();
                
                if (filter != null)
                {
                    query = query.Where(filter);
                }

                var totalCount = query.Count();
                var items = query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsEnumerable();

                return Task.FromResult((items, totalCount));
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved hentning af sideinddelte {typeof(T).Name}", ex);
            }
        }
    }
} 