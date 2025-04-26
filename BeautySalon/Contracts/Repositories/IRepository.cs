namespace BeautySalon.Contracts.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByID(int id);
    Task<IEnumerable<T>> GetAll();
    Task Add(T entity);
    Task Update(T entity);
    Task Delete(T entity);
    Task<int> SaveChanges();
}
