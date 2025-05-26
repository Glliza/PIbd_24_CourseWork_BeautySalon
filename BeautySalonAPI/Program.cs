using Microsoft.Extensions.DependencyInjection;
using BeautySalon.SCImplementations;
using BeautySalon.BusinessLogicContracts;
using BeautySalon.BLImplementations;
using BeautySalon.StorageContracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore; // Added for DbContext
using BeautySalon.MailWork;
using BeautySalon.AdapterContracts;
using BeautySalonAPI.AdaptersImplementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Or AddControllersWithViews if it's an MVC app
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// MailWorker DI configuration
builder.Services.AddScoped<AbstractMailWork, MailKit>();

// Add logging
builder.Services.AddLogging(configure => configure.AddConsole());
builder.Services.AddLogging(configure => configure.AddDebug());

// Configure connection string from appsettings.json
builder.Services.AddDbContext<SalonDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BeautySalonDB")));

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
