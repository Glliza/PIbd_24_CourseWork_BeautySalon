using Microsoft.EntityFrameworkCore;
using BeautySalon.Entities;

internal class SalonDbContext : DbContext
{
    public DbSet<Staff> Workers { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<Request> Requests { get; set; } = null!;
    public DbSet<Shift> Shifts { get; set; } = null!;
    public DbSet<CashBox> CashBoxes { get; set; } = null!;
    public DbSet<Visit> Visits { get; set; } = null!;
    public DbSet<Receipt> Receipts { get; set; } = null!;
    public DbSet<ProductListItem> ProductListItems { get; set; } = null!;
    public DbSet<ServiceListItem> ServiceListItems { get; set; } = null!;

    public SalonDbContext(DbContextOptions<SalonDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global Query Filters for Soft Deletes
        modelBuilder.Entity<Staff>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Customer>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Product>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Service>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Request>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Shift>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CashBox>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Visit>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Receipt>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ProductListItem>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ServiceListItem>().HasQueryFilter(e => !e.IsDeleted);

        // Relationships
        modelBuilder.Entity<Staff>()
            .HasMany(s => s.Shifts)
            .WithOne(sh => sh.Staff)
            .HasForeignKey(sh => sh.StaffID);

        modelBuilder.Entity<Staff>()
            .HasMany(s => s.Receipts)
            .WithOne(r => r.Staff)
            .HasForeignKey(r => r.StaffID);

        modelBuilder.Entity<Staff>()
            .HasMany(s => s.Visits)
            .WithOne(v => v.Staff)
            .HasForeignKey(v => v.StaffID);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Requests)
            .WithOne(r => r.Customer)
            .HasForeignKey(r => r.CustomerID);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Visits)
            .WithOne(v => v.Customer)
            .HasForeignKey(v => v.CustomerID);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Receipts)
            .WithOne(r => r.Customer)
            .HasForeignKey(r => r.CustomerID)
            .IsRequired(false);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.ProductListItems)
            .WithOne(pli => pli.Product)
            .HasForeignKey(pli => pli.ProductID);

        modelBuilder.Entity<Service>()
            .HasMany(s => s.ServiceListItems)
            .WithOne(sli => sli.Service)
            .HasForeignKey(sli => sli.ServiceID);

        modelBuilder.Entity<CashBox>()
            .HasMany(cb => cb.Shifts)
            .WithOne(s => s.CashBox)
            .HasForeignKey(s => s.CashBoxID);

        // Request [ ! ]
        modelBuilder.Entity<Request>()
            .HasMany(r => r.Products)
            .WithOne(pli => pli.ParentRequest)
            .HasForeignKey(pli => pli.ParentRequestID)
            .IsRequired(false);

        modelBuilder.Entity<Request>()
            .HasMany(r => r.Services)
            .WithOne(sli => sli.ParentRequest)
            .HasForeignKey(sli => sli.ParentRequestID)
            .IsRequired(false);

        // Visit [ ! ]
        modelBuilder.Entity<Visit>()
            .HasMany(v => v.Services)
            .WithOne(sli => sli.ParentVisit)
            .HasForeignKey(sli => sli.ParentVisitID)
            .IsRequired(false);

        // Value Conversions
        modelBuilder.Entity<Request>()
            .Property(r => r.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Product>()
            .Property(p => p.Type)
            .HasConversion<string>();

        // CHECK Constraints (Requires raw SQL in migrations)
        modelBuilder.Entity<ProductListItem>()
            .HasCheckConstraint("CK_ProductListItems_OneParent", "\"ParentReceiptID\" IS NOT NULL XOR \"ParentRequestID\" IS NOT NULL");

        modelBuilder.Entity<ServiceListItem>()
            .HasCheckConstraint("CK_ServiceListItems_OneParent", "\"ParentRequestID\" IS NOT NULL XOR \"ParentVisitID\" IS NOT NULL");
    }
}
