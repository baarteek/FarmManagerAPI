using FarmManagerAPI.Data;
using FarmManagerAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmManagerAPI.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly FarmContext context;
        private readonly DbSet<T> dbSet;

        public GenericRepository(FarmContext context)
        {
            this.context = context;
            dbSet = this.context.Set<T>();
        }

        public async Task Add(T entity)
        {
            await dbSet.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var entity = await dbSet.FindAsync(id);
            if(entity != null) 
            {
                dbSet.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        public virtual async Task<T> GetById(Guid id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await dbSet.ToListAsync();
        }

        public async Task Update(T entity)
        {
            dbSet.Update(entity);
            await context.SaveChangesAsync();
        }
    }
}
