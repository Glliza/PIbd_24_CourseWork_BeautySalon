using Microsoft.Extensions.FileProviders;
using BeautySalonAPI.AdaptersImplementations;
using BeautySalon.BusinessLogicContracts;
using Microsoft.EntityFrameworkCore;
using BeautySalon.SCImplementations;
using BeautySalon.BLImplementations;
using BeautySalon.StorageContracts;
using BeautySalon.AdapterContracts;
using BeautySalon.MailWork;
using AutoMapper;
using BeautySalonAPI.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Or AddControllersWithViews if it's an MVC app
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Configure dependency injection
builder.Services.AddScoped<IStaffSC, StaffSC>();
builder.Services.AddScoped<ICashBoxSC, CashBoxSC>();
builder.Services.AddScoped<ICustomerSC, CustomerSC>();
builder.Services.AddScoped<IProductSC, ProductSC>();
builder.Services.AddScoped<IReceiptSC, ReceiptSC>();
builder.Services.AddScoped<IRequestSC, RequestSC>();
builder.Services.AddScoped<IServiceSC, ServiceSC>();
builder.Services.AddScoped<IShiftSC, ShiftSC>();
builder.Services.AddScoped<IVisitSC, VisitSC>();

builder.Services.AddScoped<IStaffBLC>(provider =>
{
    var staffStorage = provider.GetRequiredService<IStaffSC>();
    var mailWorker = provider.GetRequiredService<MailKit>(); // Use MailKit directly
    var logger = provider.GetRequiredService<ILogger<StaffBLC>>();
    var reportGenerator = provider.GetRequiredService<ReportGenerator>();
    var mapper = provider.GetRequiredService<IMapper>();

    return new StaffBLC(staffStorage, mailWorker, logger, reportGenerator, mapper);
});
builder.Services.AddScoped<ICashBoxBLC>(provider =>
{
    var cashBoxStorage = provider.GetRequiredService<ICashBoxSC>();
    var logger = provider.GetRequiredService<ILogger<CashBoxBLC>>();

    return new CashBoxBLC(cashBoxStorage, logger);
});
builder.Services.AddScoped<ICustomerBLC>(provider =>
{
    var customerStorage = provider.GetRequiredService<ICustomerSC>();
    var logger = provider.GetRequiredService<ILogger<CustomerBLC>>();

    return new CustomerBLC(customerStorage, logger);
});
builder.Services.AddScoped<IProductBLC>(provider =>
{
    var productStorage = provider.GetRequiredService<IProductSC>();
    var logger = provider.GetRequiredService<ILogger<ProductBLC>>();

    return new ProductBLC(productStorage, logger);
});
builder.Services.AddScoped<IReceiptBLC>(provider =>
{
    var receiptStorage = provider.GetRequiredService<IReceiptSC>();
    var logger = provider.GetRequiredService<ILogger<ReceiptBLC>>();

    return new ReceiptBLC(receiptStorage, logger);
});
builder.Services.AddScoped<IRequestBLC>(provider =>
{
    var requestStorage = provider.GetRequiredService<IRequestSC>();
    var logger = provider.GetRequiredService<ILogger<RequestBLC>>();

    return new RequestBLC(requestStorage, logger);
});
builder.Services.AddScoped<IServiceBLC>(provider =>
{
    var serviceStorage = provider.GetRequiredService<IServiceSC>();
    var logger = provider.GetRequiredService<ILogger<ServiceBLC>>();

    return new ServiceBLC(serviceStorage, logger);
});
builder.Services.AddScoped<IShiftBLC>(provider =>
{
    var shiftStorage = provider.GetRequiredService<IShiftSC>();
    var logger = provider.GetRequiredService<ILogger<ShiftBLC>>();

    return new ShiftBLC(shiftStorage, logger);
});
builder.Services.AddScoped<IVisitBLC>(provider =>
{
    var visitStorage = provider.GetRequiredService<IVisitSC>();
    var logger = provider.GetRequiredService<ILogger<VisitBLC>>();

    return new VisitBLC(visitStorage, logger);
});

builder.Services.AddScoped<ICashBoxAdapter, CashBoxAdapter>();
builder.Services.AddScoped<ICustomerAdapter, CustomerAdapter>();
builder.Services.AddScoped<IProductAdapter, ProductAdapter>();
builder.Services.AddScoped<IReceiptAdapter, ReceiptAdapter>();
builder.Services.AddScoped<IRequestAdapter, RequestAdapter>();
builder.Services.AddScoped<IServiceAdapter, ServiceAdapter>();
builder.Services.AddScoped<IShiftAdapter, ShiftAdapter>();
builder.Services.AddScoped<IStaffAdapter, StaffAdapter>();
builder.Services.AddScoped<IVisitAdapter, VisitAdapter>();

builder.Services.AddScoped<ReportGenerator>();
builder.Services.AddScoped<ReportController>();

// MailWorker DI configuration
builder.Services.AddScoped<AbstractMailWork, MailKit>();
builder.Services.AddScoped<MailKit>(); // Register MailKit separately

builder.Services.AddScoped<IStaffBLC>(provider =>
{
    var staffStorage = provider.GetRequiredService<IStaffSC>();
    var mailWorker = provider.GetRequiredService<MailKit>(); // Use MailKit directly
    var logger = provider.GetRequiredService<ILogger<StaffBLC>>();
    var reportGenerator = provider.GetRequiredService<ReportGenerator>();
    var mapper = provider.GetRequiredService<IMapper>();

    return new StaffBLC(staffStorage, mailWorker, logger, reportGenerator, mapper);
});

// Add logging
builder.Services.AddLogging(configure => configure.AddConsole());
builder.Services.AddLogging(configure => configure.AddDebug());

// Register generic logger service
builder.Services.AddSingleton<ILoggerFactory, LoggerFactory>();

// Register MailWorker services
builder.Services.AddScoped<AbstractMailWork, MailKit>();

// Configure connection string from appsettings.json
builder.Services.AddDbContext<SalonDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("beautysalonDB")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Configure static file middleware to serve files from the html directory
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "html")),
    RequestPath = "/html"
});

// Set up a default route to serve index.html at the root URL
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/html/index.html");
    }
    else
    {
        await next();
    }
});

app.UseAuthorization();

app.MapControllers();

app.Run();
