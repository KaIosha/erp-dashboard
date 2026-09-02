using backend.models;
using backend.repositories.Interfaces;

namespace backend.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        ICustomerRepository Customers { get; }
        IProductRepository Products { get; }
        ISupplierRepository Suppliers { get; }
        IPurchaseOrderRepository PurchaseOrders { get; }
        IOrderRepository Orders { get; }
        IInvoiceRepository Invoices { get; }
        IEmployeeRepository Employees { get; }
        IDepartmentsRepository Departments { get; }
        IRolesRepository Roles { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}