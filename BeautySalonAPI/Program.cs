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

builder.Services.AddScoped<IStaffBLC, StaffBLC>();
builder.Services.AddScoped<ICashBoxBLC, CashBoxBLC>();
builder.Services.AddScoped<ICustomerBLC, CustomerBLC>();
builder.Services.AddScoped<IProductBLC, ProductBLC>();
builder.Services.AddScoped<IReceiptBLC, ReceiptBLC>();
builder.Services.AddScoped<IRequestBLC, RequestBLC>();
builder.Services.AddScoped<IServiceBLC, ServiceBLC>();
builder.Services.AddScoped<IShiftBLC, ShiftBLC>();
builder.Services.AddScoped<IVisitBLC, VisitBLC>();

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
builder.Services.AddScoped<IStaffBLC, StaffBLC>(provider =>
{
    var staffStorage = provider.GetRequiredService<IStaffSC>();
    var mailWorker = provider.GetRequiredService<AbstractMailWork>();
    var logger = provider.GetRequiredService<ILogger<StaffBLC>>();
    var reportGenerator = provider.GetRequiredService<ReportGenerator>();
    var mapper = provider.GetRequiredService<IMapper>();

    return new StaffBLC(staffStorage, (MailKit)mailWorker, logger, reportGenerator, mapper);
});

// Add logging
builder.Services.AddLogging(configure => configure.AddConsole());
builder.Services.AddLogging(configure => configure.AddDebug());

// Register MailKit
builder.Services.AddScoped<MailKit>();

// Configure connection string from appsettings.json
builder.Services.AddDbContext<SalonDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BeautySalonDB")));

// Register ILogger for all categories
builder.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
builder.Services.AddScoped(typeof(ILogger<>), typeof(Logger<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
