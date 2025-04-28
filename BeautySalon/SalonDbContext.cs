using Microsoft.EntityFrameworkCore;
using BeautySalon.Entities;

namespace BeautySalon.Entities;

internal class SalonDbContext : DbContext
{
    public DbSet<Staff> Workers { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<CashBox> CashBoxes { get; set; }
    public DbSet<Visit> Visits { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<ProductListItem> ProductListItems { get; set; }
    public DbSet<ServiceListItem> ServiceListItems { get; set; }

    public SalonDbContext(DbContextOptions<SalonDbContext> options) : base(options) { }

    // Configure model mappings, relationships, and constraints
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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

        modelBuilder.Entity<Staff>()
            .HasMany(s => s.Shifts)
            .WithOne(sh => sh.Staff)
            .HasForeignKey(sh => sh.StaffID);

         modelBuilder.Entity<Staff>()
            .HasMany(s => s.Receipts)
            .WithOne(r => r.Staff)
            .HasForeignKey(r => r.StaffID);

        modelBuilder.Entity<Staff>()
           .HasMany(s => s.Visits) // Using Visits to match your Staff entity navigation property name
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

        // Shift Entity (relationships with CashBox and Staff handled by inverse)
        // CashBox Entity
        modelBuilder.Entity<CashBox>()
             .HasMany(cb => cb.Shifts)
             .WithOne(s => s.CashBox)
             .HasForeignKey(s => s.CashBoxID);

        // Request Entity
        // Customer relationship handled by inverse

        // Relationship with ProductListItem (1 Request to Many ProductListItems)
        modelBuilder.Entity<Request>()
            .HasMany(r => r.ProductItems)
            .WithOne(pli => pli.ParentRequest)
            .HasForeignKey(pli => pli.ParentRequestID)
            .IsRequired(false);

        // Relationship with ServiceListItem (1 Request to Many ServiceListItems)
        modelBuilder.Entity<Request>()
            .HasMany(r => r.ServiceItems)
            .WithOne(sli => sli.ParentRequest)
            .HasForeignKey(sli => sli.ParentRequestID)
            .IsRequired(false);

        // Relationship with Visit (1 Request to Many Visits) - FK is on Visit
        // Requires Visit entity to have string? RequestID and virtual Request? Request properties
        modelBuilder.Entity<Request>()
            .HasMany(r => r.Visits)
            .WithOne(v => v.Request) // Assuming Visit entity has 'Request' navigation property
            .HasForeignKey(v => v.RequestID) // Assuming Visit entity has 'RequestID' FK
            .IsRequired(false); // Assuming RequestID on Visit is nullable

        // Receipt Entity : Staff, Customer, CashBox relationships handled by inverse

        // Relationship with ProductListItem (1 Receipt to Many ProductListItems)
        modelBuilder.Entity<Receipt>()
            .HasMany(r => r.Products)
            .WithOne(pli => pli.ParentReceipt)
            .HasForeignKey(pli => pli.ParentReceiptID)
            .IsRequired(false);

        // Visit Entity : Customer, Staff relationships handled by inverse

        // Relationship with Request (1 Visit to 1 Request) - FK is on Visit
        // Correcting based on Request has Many Visits -> Visit has One Request
        // Requires Visit entity to have string? RequestID and virtual Request? Request properties
         modelBuilder.Entity<Visit>()
             .HasOne(v => v.Request) // Visit has one Request (nullable)
             .WithMany(r => r.Visits) // Request has many Visits (navigation property already exists)
             .HasForeignKey(v => v.RequestID) // Assuming Visit entity has 'RequestID' FK
             .IsRequired(false); // Assuming RequestID on Visit is nullable

         // Relationship with ServiceListItem (1 Visit to Many ServiceListItems) - FK is on item
         // Requires Visit entity to have virtual ICollection<ServiceListItem>? ServiceItems property
         modelBuilder.Entity<Visit>()
             .HasMany<ServiceListItem>() // Visit has many ServiceListItems (assuming Visit entity has ServiceItems collection)
             .WithOne(sli => sli.ParentVisit) // ServiceListItem points back to ParentVisit
             .HasForeignKey(sli => sli.ParentVisitID) // FK on ServiceListItem
             .IsRequired(false); // ParentVisitID is nullable on ServiceListItem

        // *
        // ProductListItem Entity (relationships with Product, Receipt, Request handled by inverse)
        // ServiceListItem Entity (relationships with Service, Request, Visit handled by inverse)

        // ProductListHeader Entity - Removing all relationships as it seems incompatible with item structure
        // ServiceListHeader Entity - Removing all relationships as it seems incompatible with item structure

        // [ ! ] Value Conversions
        modelBuilder.Entity<Request>()
            .Property(r => r.Status)
            .HasConversion<string>();

         modelBuilder.Entity<Product>()
            .Property(p => p.Type)
            .HasConversion<string>();

        // [ * ] CHECK Constraints (Requires raw SQL in migrations)

        // ProductListItem must belong to exactly one parent (Receipt OR Request)
        // Add raw SQL for CHECK constraint in migrations:
        /*
        migrationBuilder.AddCheckConstraint(
            name: "CK_ProductListItems_OneParent",
            table: "ProductListItems",
            sql: "\"ParentReceiptID\" IS NOT NULL XOR \"ParentRequestID\" IS NOT NULL"
        );
        */

        // ServiceListItem must belong to exactly one parent (Request OR Visit)
        // Add raw SQL for CHECK constraint in migrations:
        /*
        migrationBuilder.AddCheckConstraint(
            name: "CK_ServiceListItems_OneParent",
            table: "ServiceListItems",
            sql: "\"ParentRequestID\" IS NOT NULL XOR \"ParentVisitID\" IS NOT NULL"
        );
        */
    }
}