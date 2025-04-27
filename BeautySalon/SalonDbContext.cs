namespace BeautySalon;

using Microsoft.EntityFrameworkCore;
// Make sure you have the Npgsql EF Core package installed: Npgsql.EntityFrameworkCore.PostgreSQL
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure; // For UseNpgsql options
using BeautySalon.DataModels;
using BeautySalon.Entities;
using BeautySalon.Infrastructure;

// IMPORTANT: You will need EF Core Entity classes in MyProject.DataAccess.Entities
// These classes should mirror your Core Data Models but include 'public get; set;' properties,
// 'virtual' navigation properties, and potentially data annotations like [Table] or [Key].
// They will *not* inherit from your Core Data Models in this separate-entity approach.
// Example: MyProject.DataAccess.Entities.Staff, MyProject.DataAccess.Entities.Receipt, etc.

internal class SalonDbContext : DbContext // Use internal as it's a DataAccess detail
{
    // Your template uses IConfigurationDatabase, sticking to that name
    private readonly IConfigurationDatabase? _configurationDatabase;

    // Constructor takes the configuration provider interface
    public SalonDbContext(IConfigurationDatabase configurationDatabase)
    {
        _configurationDatabase = configurationDatabase;
    }

    // OnConfiguring uses the injected connection string provider
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Ensure the provider is not null before using
        if (_configurationDatabase?.ConnectionString != null)
        {
            // Use the connection string from the provider
            optionsBuilder.UseNpgsql(
                _configurationDatabase.ConnectionString,
                o => o.SetPostgresVersion(12, 2)); // Keep your specified PG version
        }
        // Note: base.OnConfiguring(optionsBuilder); is typically NOT called after Use* provider method
        // base.OnConfiguring(optionsBuilder); // Removed as per standard practice
    }

    // OnModelCreating configures schema, relationships, indexes, constraints, etc.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Apply configurations based on your schema and entities ---
        // NOTE: These configurations assume you have corresponding EF Entity classes
        // in MyProject.DataAccess.Entities (e.g., Staff, Customer, Receipt, etc.)
        // that mirror the properties of your Core DMs but are separate classes.

        // Configure Primary Keys (if not named 'Id' or 'ID', or for string keys)
        modelBuilder.Entity<Staff>().HasKey(e => e.ID); // Assuming EF Staff has 'ID'
        modelBuilder.Entity<Customer>().HasKey(e => e.ID); // Assuming EF Customer has 'ID'
        modelBuilder.Entity<Receipt>().HasKey(e => e.ID); // Assuming EF Receipt has 'ID'
        modelBuilder.Entity<Product>().HasKey(e => e.ID); // Assuming EF Product has 'ID'
        modelBuilder.Entity<Service>().HasKey(e => e.ID); // Assuming EF Service has 'ID'
        modelBuilder.Entity<Request>().HasKey(e => e.ID); // Assuming EF Request has 'ID'
        modelBuilder.Entity<Visit>().HasKey(e => e.ID); // Assuming EF Visit has 'ID'
        modelBuilder.Entity<CashBox>().HasKey(e => e.ID); // Assuming EF CashBox has 'ID'
        modelBuilder.Entity<Shift>().HasKey(e => e.ID); // Assuming EF Shift has 'ID'
        modelBuilder.Entity<ProductListItem>().HasKey(e => e.ID); // Assuming EF ProductListItem has its own ID
        modelBuilder.Entity<ServiceListItem>().HasKey(e => e.ID); // Assuming EF ServiceListItem has its own ID
        // Assuming ProductListHeader/ServiceListHeader also have string ID PKs if used

        // --- Configure Indexes ---

        // Customer Index (from your template)
        modelBuilder.Entity<Customer>().HasIndex(x => x.PhoneNumber).IsUnique(); // Assuming EF Customer has 'PhoneNumber'

        // Shift Index (Corrected: Removed incorrect ManufacturerName index from template)
        // Add relevant index for Shift, e.g., on Staff ID and Start Time
        modelBuilder.Entity<Shift>().HasIndex(s => new { s.StaffID, s.DateTimeStart }); // Assuming EF Shift has StaffID and DateTimeStart

        // Staff Index (Corrected: Removed template filters, add relevant index)
        // Assuming EF Staff entity has 'FIO' and 'IsDeleted'
        modelBuilder.Entity<Staff>().HasIndex(s => new { s.FIO, s.IsDeleted }) // Index on FIO + IsDeleted
            .IsUnique() // Assuming FIO is unique for non-deleted staff
            .HasFilter($"\"{nameof(Staff.IsDeleted)}\" = FALSE"); // Filter for unique active staff

        // Product Index (Corrected: Removed template ProductName if EF entity uses Name)
        // Assuming EF Product entity has 'Name' and 'IsDeleted'
        modelBuilder.Entity<Product>().HasIndex(p => new { p.Name, p.IsDeleted }) // Index on Name + IsDeleted
            .IsUnique() // Assuming Name is unique for non-deleted products
            .HasFilter($"\"{nameof(Product.IsDeleted)}\" = FALSE"); // Filter for unique active products

        // --- Configure Relationships ---
        // NOTE: These require defining navigation properties in your EF Entity classes

        // Staff relationships
        // Assuming EF Staff entity has ICollection<Receipt> Receipts and ICollection<Visit> Visits
        modelBuilder.Entity<Staff>()
            .HasMany(s => s.Receipts) // Navigation property in EF Staff
            .WithOne(r => r.Staff)     // Navigation property in EF Receipt (needs to be defined)
            .HasForeignKey(r => r.StaffID); // FK column in Receipt table (matches ReceiptDM field name)

        modelBuilder.Entity<Staff>()
            .HasMany(s => s.Visits)    // Navigation property in EF Staff
            .WithOne(v => v.Staff)     // Navigation property in EF Visit (needs to be defined)
            .HasForeignKey(v => v.WorkerId); // FK column in Visit table (matches your template FK name)

        // Receipt - ProductListItem (1-to-Many)
        // Assuming EF Receipt has ICollection<ProductListItem> Products (matching ReceiptDM property name)
        modelBuilder.Entity<Receipt>()
            .HasMany(r => r.Products)
            .WithOne(pli => pli.Receipt) // Navigation property in EF ProductListItem (needs to be defined)
            .HasForeignKey(pli => pli.ReceiptID); // FK column in ProductListItem table (matches ProductListItemDM field name)

        // Request - ProductListItem (1-to-Many)
        // Assuming EF Request has ICollection<ProductListItem> ProductItems (matching RequestDM property name)
        modelBuilder.Entity<Request>()
            .HasMany(r => r.ProductItems)
            .WithOne(pli => pli.Request)  // Navigation property in EF ProductListItem (needs to be defined)
            .HasForeignKey(pli => pli.RequestID); // FK column in ProductListItem table (needs to be defined/mapped from DM)

        // Request - ServiceListItem (1-to-Many)
        // Assuming EF Request has ICollection<ServiceListItem> ServiceItems (matching RequestDM property name)
        modelBuilder.Entity<Request>()
            .HasMany(r => r.ServiceItems)
            .WithOne(sli => sli.Request)  // Navigation property in EF ServiceListItem (needs to be defined)
            .HasForeignKey(sli => sli.RequestID); // FK column in ServiceListItem table (needs to be defined/mapped from DM)

        // Visit - ServiceListItem (1-to-Many)
        // Based on your VisitDM containing a *required* ServiceListID FK, this mapping is complex.
        // If Visit has a List<ServiceListItem> like Receipt/Request:
        modelBuilder.Entity<Visit>()
            .HasMany(v => v.ServiceItems) // Assuming EF Visit has ICollection<ServiceListItem> ServiceItems
            .WithOne(sli => sli.Visit)   // Nav property in EF ServiceListItem (needs to be defined)
            .HasForeignKey(sli => sli.VisitID); // FK column in ServiceListItem table (needs to be defined/mapped from DM)

        // If Visit truly links via FKs to ServiceListHeader (as your VisitDM struct suggests):
        // You would need EF Entity for ServiceListHeader and configure that relationship instead.
        // Example:
        // modelBuilder.Entity<Visit>().HasOne(v => v.ServiceListHeader).WithMany().HasForeignKey(v => v.ServiceListID);
        // modelBuilder.Entity<Visit>().HasOne(v => v.ProductListHeader).WithMany().HasForeignKey(v => v.ProductListID);

        // --- Configure FKs for list items pointing to Products/Services ---
        // Assuming EF ProductListItem has ProductID and ServiceID FKs
        modelBuilder.Entity<ProductListItem>()
            .HasOne(pli => pli.Product) // Nav property in EF ProductListItem
            .WithMany() // Or WithMany(p => p.ProductListItems) if Product entity links back
            .HasForeignKey(pli => pli.ProductID); // FK column in ProductListItem

        // Assuming EF ServiceListItem has ServiceID and ProductID FKs (ServiceList items should point to Service)
        modelBuilder.Entity<ServiceListItem>()
            .HasOne(sli => sli.Service) // Nav property in EF ServiceListItem
            .WithMany() // Or WithMany(s => s.ServiceListItems) if Service entity links back
            .HasForeignKey(sli => sli.ServiceID); // FK column in ServiceListItem

        // --- Configure decimal precision ---
        modelBuilder.Entity<CashBox>().Property(cb => cb.CurrentCapacity).HasPrecision(18, 2);
        modelBuilder.Entity<Product>().Property(p => p.PricePerOne).HasPrecision(18, 2);
        modelBuilder.Entity<Service>().Property(s => s.BasePrice).HasPrecision(18, 2);
        modelBuilder.Entity<Receipt>().Property(r => r.TotalSumm).HasPrecision(18, 2); // Note TotalSumm
        modelBuilder.Entity<ProductListItem>().Property(pli => pli.Amount).HasColumnName("Amount"); // Assuming column name is 'Amount'
        modelBuilder.Entity<ServiceListItem>().Property(sli => sli.QuantityOrSessions).HasColumnName("QuantityOrSessions"); // Assuming column name
        modelBuilder.Entity<ServiceListItem>().Property(sli => sli.TotalItemPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Request>().Property(r => r.TotalPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Visit>().Property(v => v.TotalPrice).HasPrecision(18, 2); // Note TotalPrice

        // --- Configure Enum Conversions (Store Enums as strings in DB) ---
        modelBuilder.Entity<Staff>().Property(s => s.postType).HasConversion<string>(); // Use the property name on EF Staff
        modelBuilder.Entity<Request>().Property(r => r.Status).HasConversion<string>(); // Assuming EF Request has Status property
        // Add Product Type conversion if applicable to EF Product entity


        // --- Add Check Constraints ---
        // Constraint for ProductListItem: ServiceID IS NULL AND ProductID IS NOT NULL (Assuming ProductListItem is ONLY for Products)
        // Constraint for ServiceListItem: ServiceID IS NOT NULL AND ProductID IS NULL (Assuming ServiceListItem is ONLY for Services)
        modelBuilder.Entity<ProductListItem>()
           .HasCheckConstraint("CK_ProductListItem_OnlyProduct", "\"ServiceID\" IS NULL AND \"ProductID\" IS NOT NULL");

        modelBuilder.Entity<ServiceListItem>()
           .HasCheckConstraint("CK_ServiceListItem_OnlyService", "\"ServiceID\" IS NOT NULL AND \"ProductID\" IS NULL");

        // Constraint for Visit Status (if boolean true/false map to specific values like 0/1 or 'T'/'F' in DB)
        // Assuming 'bool Status' maps to a DB column, ensure its values are consistent if needed
        // EF Core handles bool mapping, no specific constraint needed unless you need to enforce DB level TRUE/FALSE values.
        // If mapping boolean to int 0/1: modelBuilder.Entity<Visit>().Property(v => v.Status).HasConversion<int>();
        // Then add a check constraint on the int column: modelBuilder.Entity<Visit>().HasCheckConstraint("CK_Visit_Status_BoolMap", "\"Status\" IN (0, 1)");


        // --- Configure String Primary Keys/Foreign Keys ---
        // By default, string keys are assumed to be non-nullable unless marked with ?.
        // Ensure your EF entities use 'required string' or 'string?' for keys/FKs as needed.
        // EF Core automatically handles string key mapping.

        // Important: PostgreSQL table and column names are typically lowercase by convention.
        // EF Core Npgsql provider handles this by default.
        // If you need specific case sensitivity or names, configure here using .ToTable("...") or .HasColumnName("...")
        // e.g., modelBuilder.Entity<Staff>().ToTable("staffs"); // Explicit table name
        //       modelBuilder.Entity<Staff>().Property(s => s.FIO).HasColumnName("fio"); // Explicit column name
    }

    // --- Define DbSet properties for your EF Entity classes ---
    // These names should match the template's DbSet names.
    // They point to your EF Entity classes in MyProject.DataAccess.Entities.

    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Request> Requests { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<CashBox> CashBoxes { get; set; } = null!;
    public DbSet<Shift> Shifts { get; set; } = null!;
    public DbSet<Receipt> Recepies { get; set; } = null!; // DbSet name from template, points to Receipt Entity
    public DbSet<Visit> Visits { get; set; } = null!;
    public DbSet<Staff> Workers { get; set; } = null!; // DbSet name from template, points to Staff Entity

    // You will also need DbSets for your list item entities if they have their own PKs:
    public DbSet<ProductListItem> ProductListItems { get; set; } = null!;
    public DbSet<ServiceListItem> ServiceListItems { get; set; } = null!;

    // If using List Headers with FKs, you'd need DbSets for them:
    // public DbSet<ProductListHeader> ProductListHeaders { get; set; } = null!;
    // public DbSet<ServiceListHeader> ServiceListHeaders { get; set; } = null!;
}