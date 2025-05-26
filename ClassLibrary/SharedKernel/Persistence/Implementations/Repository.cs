using System;
using System.Collections.Generic;
using System.IO; // Tilføjet for filoperationer
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json; // Tilføjet for JSON serialisering/deserialisering
using System.Text.Json.Serialization; // Tilføjet for JsonStringEnumConverter
using System.Threading; // Tilføjet for SemaphoreSlim
using System.Threading.Tasks;
using ClassLibrary.SharedKernel.Exceptions; // Opdateret
using ClassLibrary.SharedKernel.Domain.Abstractions; // Opdateret for IEntity, ISoftDelete
using ClassLibrary.SharedKernel.Persistence.Abstractions; // Opdateret for IRepository

namespace ClassLibrary.SharedKernel.Persistence.Implementations
{
    /// <summary>
    /// Generisk repository klasse der implementerer grundlæggende CRUD operationer
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly string _filePath; // Skal indeholde stien til JSON-filen
        private static readonly SemaphoreSlim _fileLock = new SemaphoreSlim(1, 1); // Simpel lås for filadgang

        // JsonSerializer options for pæn formatering og korrekt håndtering af enums som strenge
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true, // Gør JSON-filen mere læselig
            Converters = { new JsonStringEnumConverter() } // Serialiserer enums som strenge
        };

        public Repository(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            // Sikrer at mappen til datafilen eksisterer
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        protected async Task<List<T>> LoadDataAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                if (!File.Exists(_filePath))
                {
                    return new List<T>(); // Returner tom liste hvis filen ikke eksisterer
                }

                var jsonData = await File.ReadAllTextAsync(_filePath);
                if (string.IsNullOrWhiteSpace(jsonData))
                {
                    return new List<T>(); // Returner tom liste hvis filen er tom
                }
                // Håndterer tilfælde hvor deserialisering returnerer null for tom JSON-array "[]"
                return JsonSerializer.Deserialize<List<T>>(jsonData, _jsonOptions) ?? new List<T>();
            }
            finally
            {
                _fileLock.Release();
            }
        }

        protected async Task SaveDataAsync(List<T> data)
        {
            await _fileLock.WaitAsync();
            try
            {
                var jsonData = JsonSerializer.Serialize(data, _jsonOptions);
                await File.WriteAllTextAsync(_filePath, jsonData);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// Henter alle enheder
        /// </summary>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                var items = await LoadDataAsync();
                return items.Where(x => !((x is ISoftDelete softDeleteEntity) && softDeleteEntity.IsDeleted)).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved hentning af alle {typeof(T).Name} fra fil: {_filePath}", ex);
            }
        }

        /// <summary>
        /// Henter en enhed baseret på ID
        /// </summary>
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            try
            {
                var items = await LoadDataAsync();
                return items.FirstOrDefault(x => x.Id == id && !((x is ISoftDelete softDeleteEntity) && softDeleteEntity.IsDeleted));
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved hentning af {typeof(T).Name} med ID {id} fra fil: {_filePath}", ex);
            }
        }

        /// <summary>
        /// Tilføjer en ny enhed
        /// </summary>
        public virtual async Task<T> AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            try
            {
                ValidateEntity(entity);
                var items = await LoadDataAsync();

                if (entity.Id == 0) // Generer nyt ID hvis det ikke er sat
                {
                     entity.Id = items.Any() ? items.Max(x => x.Id) + 1 : 1;
                }
                else if (items.Any(x => x.Id == entity.Id && !((x is ISoftDelete sd) && sd.IsDeleted)))
                {
                    throw new RepositoryException($"En aktiv {typeof(T).Name} med ID {entity.Id} eksisterer allerede. Overvej at bruge UpdateAsync eller slet den eksisterende enhed først.");
                }


                if (entity is ISoftDelete softDelete)
                {
                    softDelete.IsDeleted = false;
                    softDelete.DeletedAt = null;
                }
                
                // Håndter case hvor en slettet enhed med samme ID eksisterer
                var existingDeletedIndex = items.FindIndex(x => x.Id == entity.Id && (x is ISoftDelete sd && sd.IsDeleted));
                if (existingDeletedIndex != -1)
                {
                    items.RemoveAt(existingDeletedIndex); // Fjern den gamle slettede version
                }


                items.Add(entity);
                await SaveDataAsync(items);
                return entity;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved tilføjelse af {typeof(T).Name} til fil: {_filePath}", ex);
            }
        }

        /// <summary>
        /// Opdaterer en eksisterende enhed
        /// </summary>
        public virtual async Task<T> UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            try
            {
                ValidateEntity(entity);
                var items = await LoadDataAsync();
                
                var index = items.FindIndex(x => x.Id == entity.Id && !((x is ISoftDelete sd) && sd.IsDeleted));
                if (index == -1)
                {
                    throw new RepositoryException($"{typeof(T).Name} med ID {entity.Id} blev ikke fundet for opdatering i fil: {_filePath}.");
                }

                items[index] = entity;
                await SaveDataAsync(items);
                return entity;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved opdatering af {typeof(T).Name} med ID {entity.Id} i fil: {_filePath}", ex);
            }
        }

        /// <summary>
        /// Sletter en enhed
        /// </summary>
        public virtual async Task DeleteAsync(int id)
        {
            try
            {
                var items = await LoadDataAsync();
                var entity = items.FirstOrDefault(x => x.Id == id && !((x is ISoftDelete sd) && sd.IsDeleted));
                
                if (entity == null)
                {
                    // Tjek om enheden eksisterer men allerede er soft-deleted
                    var alreadyDeletedEntity = items.FirstOrDefault(x => x.Id == id && (x is ISoftDelete sd && sd.IsDeleted));
                    if (alreadyDeletedEntity != null)
                    {
                        throw new RepositoryException($"{typeof(T).Name} med ID {id} er allerede slettet i fil: {_filePath}.");
                    }
                    throw new RepositoryException($"{typeof(T).Name} med ID {id} blev ikke fundet for sletning i fil: {_filePath}.");
                }

                if (entity is ISoftDelete softDelete)
                {
                    softDelete.IsDeleted = true;
                    softDelete.DeletedAt = DateTime.UtcNow; // Brug UtcNow for konsistens
                    // Opdater entiteten i listen
                    var index = items.FindIndex(x => x.Id == id);
                    if (index != -1) items[index] = entity;
                }
                else
                {
                    items.RemoveAll(x => x.Id == id);
                }
                await SaveDataAsync(items);
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved sletning af {typeof(T).Name} med ID {id} fra fil: {_filePath}", ex);
            }
        }

        /// <summary>
        /// Finder enheder baseret på et predikat
        /// </summary>
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                var items = await LoadDataAsync();
                Expression<Func<T, bool>> softDeletePredicate = x => !(x is ISoftDelete && ((ISoftDelete)x).IsDeleted);
                
                var parameter = predicate.Parameters.First();
                var body = Expression.AndAlso(
                    Expression.Invoke(softDeletePredicate, parameter),
                    Expression.Invoke(predicate, parameter)
                );
                var combinedPredicate = Expression.Lambda<Func<T, bool>>(body, parameter);

                return items.AsQueryable().Where(combinedPredicate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved søgning i {typeof(T).Name} i fil: {_filePath}", ex);
            }
        }

        /// <summary>
        /// Finder enheder baseret på flere predikater
        /// </summary>
        public virtual async Task<IEnumerable<T>> FindAllAsync(IEnumerable<Expression<Func<T, bool>>> predicates)
        {
            try
            {
                var items = await LoadDataAsync();
                var query = items.AsQueryable().Where(x => !(x is ISoftDelete && ((ISoftDelete)x).IsDeleted));
                foreach (var p in predicates) // 'predicate' var allerede brugt i ydre scope
                {
                    query = query.Where(p);
                }
                return query.AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved avanceret søgning i {typeof(T).Name} fra fil: {_filePath}", ex);
            }
        }

        /// <summary>
        /// Finder enheder baseret på et predikat og sorterer dem
        /// </summary>
        public virtual async Task<IEnumerable<T>> FindAndSortAsync<TKey>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TKey>> sortKey,
            bool ascending = true)
        {
            try
            {
                var items = await LoadDataAsync();
                Expression<Func<T, bool>> softDeletePredicate = x => !(x is ISoftDelete && ((ISoftDelete)x).IsDeleted);
                var parameter = predicate.Parameters.First(); // Antager at predicate altid har mindst én parameter
                var body = Expression.AndAlso(
                    Expression.Invoke(softDeletePredicate, parameter),
                    Expression.Invoke(predicate, parameter)
                );
                var combinedPredicate = Expression.Lambda<Func<T, bool>>(body, parameter);

                var query = items.AsQueryable().Where(combinedPredicate);
                query = ascending ? query.OrderBy(sortKey) : query.OrderByDescending(sortKey);
                return query.AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved sorteret søgning i {typeof(T).Name} fra fil: {_filePath}", ex);
            }
        }

        /// <summary>
        /// Finder enheder baseret på et predikat og returnerer et bestemt antal
        /// </summary>
        public virtual async Task<IEnumerable<T>> FindAndTakeAsync(
            Expression<Func<T, bool>> predicate,
            int count)
        {
            try
            {
                var items = await LoadDataAsync();
                Expression<Func<T, bool>> softDeletePredicate = x => !(x is ISoftDelete && ((ISoftDelete)x).IsDeleted);
                var parameter = predicate.Parameters.First();
                var body = Expression.AndAlso(
                    Expression.Invoke(softDeletePredicate, parameter),
                    Expression.Invoke(predicate, parameter)
                );
                var combinedPredicate = Expression.Lambda<Func<T, bool>>(body, parameter);

                return items.AsQueryable()
                        .Where(combinedPredicate)
                        .Take(count)
                        .AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved begrænset søgning i {typeof(T).Name} fra fil: {_filePath}", ex);
            }
        }

        /// <summary>
        /// Finder enheder baseret på et predikat og springer et bestemt antal over
        /// </summary>
        public virtual async Task<IEnumerable<T>> FindAndSkipAsync(
            Expression<Func<T, bool>> predicate,
            int count)
        {
            try
            {
                var items = await LoadDataAsync();
                Expression<Func<T, bool>> softDeletePredicate = x => !(x is ISoftDelete && ((ISoftDelete)x).IsDeleted);
                var parameter = predicate.Parameters.First();
                var body = Expression.AndAlso(
                    Expression.Invoke(softDeletePredicate, parameter),
                    Expression.Invoke(predicate, parameter)
                );
                var combinedPredicate = Expression.Lambda<Func<T, bool>>(body, parameter);

                return items.AsQueryable()
                        .Where(combinedPredicate)
                        .Skip(count)
                        .AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved 'skip' søgning i {typeof(T).Name} fra fil: {_filePath}", ex);
            }
        }

        /// <summary>
        /// Finder enheder baseret på et predikat og grupperer dem
        /// </summary>
        public virtual async Task<IDictionary<TKey, IEnumerable<T>>> FindAndGroupAsync<TKey>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TKey>> groupKey) where TKey : notnull
        {
            try
            {
                var items = await LoadDataAsync();
                Expression<Func<T, bool>> softDeletePredicate = x => !(x is ISoftDelete && ((ISoftDelete)x).IsDeleted);
                var parameter = predicate.Parameters.First();
                var body = Expression.AndAlso(
                    Expression.Invoke(softDeletePredicate, parameter),
                    Expression.Invoke(predicate, parameter)
                );
                var combinedPredicate = Expression.Lambda<Func<T, bool>>(body, parameter);

                return items.AsQueryable()
                    .Where(combinedPredicate)
                    .GroupBy(groupKey)
                    .ToDictionary(g => g.Key, g => g.AsEnumerable());
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved grupperet søgning i {typeof(T).Name} fra fil: {_filePath}", ex);
            }
        }

        /// <summary>
        /// Validerer enheden
        /// </summary>
        protected virtual void ValidateEntity(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), $"{typeof(T).Name} må ikke være null.");
            }
            // Yderligere validering kan tilføjes her om nødvendigt
        }

        /// <summary>
        /// Henter sideinddelte resultater
        /// </summary>
        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null)
        {
            try
            {
                var items = await LoadDataAsync();
                var query = items.AsQueryable().Where(x => !(x is ISoftDelete && ((ISoftDelete)x).IsDeleted));

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                var totalCount = query.Count();
                var pagedItems = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                return (pagedItems, totalCount);
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Fejl ved sideinddelt hentning fra {typeof(T).Name} i fil: {_filePath}", ex);
            }
        }
    }
} 