using System.Collections.Concurrent;
using backend.Data;
using backend.repositories.Implementations;
using backend.repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace backend.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IUserRepository Users { get; }
        public IRefreshTokenRepository RefreshTokenRepository { get; }
        public ICustomerRepository Customers { get; }
        public IProductRepository Products { get; }
        public ISupplierRepository Suppliers { get; }
        public IPurchaseOrderRepository PurchaseOrders { get; }
        public IOrderRepository Orders { get; }
        public IInvoiceRepository Invoices { get; }
        public IEmployeeRepository Employees { get; }
        public IDepartmentsRepository Departments { get; }
        public IRolesRepository Roles { get; }
        private bool _disposed;
        private IDbContextTransaction? _currentTransaction;

        // ConcurrentDictionary for thread-safe repository caching
        private readonly ConcurrentDictionary<Type, object> _repositories = new();

        public UnitOfWork(ApplicationDbContext context,
            IUserRepository user,
            IRefreshTokenRepository refreshTokenRepository,
            ICustomerRepository customers,
            IProductRepository products,
            ISupplierRepository suppliers,
            IPurchaseOrderRepository purchaseOrders,
            IOrderRepository orders,
            IInvoiceRepository invoices,
            IEmployeeRepository employees,
            IDepartmentsRepository departments,
            IRolesRepository roles)
        {
            _context = context;
            Users = user;
            RefreshTokenRepository = refreshTokenRepository;
            Customers = customers;
            Products = products;
            Suppliers = suppliers;
            PurchaseOrders = purchaseOrders;
            Orders = orders;
            Invoices = invoices;
            Employees = employees;
            Departments = departments;
            Roles = roles;
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            return (IGenericRepository<T>)_repositories.GetOrAdd(
                typeof(T),
                _ => new GenericRepository<T>(_context)
            );
        }

        public int SaveChanges() => _context.SaveChanges();
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _context.SaveChangesAsync(cancellationToken);

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
                return;

            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction == null)
                return;

            await _currentTransaction.CommitAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction == null)
                return;

            await _currentTransaction.RollbackAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _context.Dispose();
                _currentTransaction?.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await _context.DisposeAsync();
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                }
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}