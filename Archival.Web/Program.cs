using Application.Services;
using Archival.Application.Services;
using Archival.Application.Util;
using Archival.Core.Configuration;
using Archival.Core.Interfaces;
using Archival.Core.Model;
using DB;
using DB.Data;
using DB.Data.Repositories;
using DB.File;
using Oracle.ManagedDataAccess.Client;

IConfiguration Configuration = new ConfigurationBuilder()
  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
  .AddEnvironmentVariables()
  .AddCommandLine(args)
  .Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. 
builder.Services.AddControllersWithViews(); 
builder.Services.AddHttpClient(); 
builder.Services.AddScoped(typeof(AppRepository<NAS>));
builder.Services.AddScoped(typeof(AppRepository<Daily>));
builder.Services.AddScoped(typeof(AppRepository<PDF>));
builder.Services.AddScoped(typeof(ProcessRecords<NAS>));
builder.Services.AddScoped(typeof(ProcessRecords<Daily>));
builder.Services.AddScoped(typeof(ProcessRecords<PDF>));
builder.Services.AddScoped(typeof(CSAccess<NAS>));
builder.Services.AddScoped(typeof(CSAccess<Daily>));
builder.Services.AddScoped(typeof(CSAccess<PDF>));
builder.Services.AddScoped(typeof(Util<NAS>));
builder.Services.AddScoped(typeof(Util<Daily>));
builder.Services.AddScoped(typeof(Util<PDF>));
builder.Services.AddScoped<NASConversion>();
builder.Services.AddScoped<DailyConversion>();
builder.Services.AddScoped<PDFConversion>();
builder.Services.AddScoped<Email>();
builder.Services.AddScoped<CryptographyCore>();
builder.Services.AddScoped<SecureInfo>();
builder.Services.AddScoped<IAuthenticate, Authenticate>();
builder.Services.AddScoped<IEmail, Email>();
builder.Services.AddScoped<ILogManagerCustom, LogManagerCustom>();
builder.Services.AddScoped(typeof(ICSAccess<NAS>), typeof(CSAccess<NAS>));
builder.Services.AddScoped(typeof(ICSAccess<Daily>), typeof(CSAccess<Daily>));
builder.Services.AddScoped(typeof(ICSAccess<PDF>), typeof(CSAccess<PDF>));
builder.Services.AddScoped(typeof(IUtil<NAS>), typeof(Util<NAS>));
builder.Services.AddScoped(typeof(IUtil<Daily>), typeof(Util<Daily>));
builder.Services.AddScoped(typeof(IUtil<PDF>), typeof(Util<PDF>));
builder.Services.AddScoped(typeof(IPDFService<NAS>), typeof(PDFService<NAS>));
builder.Services.AddScoped(typeof(IPDFService<Daily>), typeof(PDFService<Daily>));
builder.Services.AddScoped(typeof(IPDFService<PDF>), typeof(PDFService<PDF>));
builder.Services.AddScoped<INASConversion, NASConversion>();
builder.Services.AddScoped<IDailyConversion, DailyConversion>();
builder.Services.AddScoped<IPDFConversion, PDFConversion>();
builder.Services.AddScoped<ICryptography, CryptographyCore>();
builder.Services.AddScoped<ISecureInfo, SecureInfo>();
builder.Services.AddScoped(typeof(ICSVReport<NAS>), typeof(CSVReport<NAS>));
builder.Services.AddScoped(typeof(ICSVReport<Daily>), typeof(CSVReport<Daily>));
builder.Services.AddScoped(typeof(ICSVReport<PDF>), typeof(CSVReport<PDF>));
builder.Services.AddScoped(typeof(IProcessRecords<NAS>), typeof(ProcessRecords<NAS>));
builder.Services.AddScoped(typeof(IProcessRecords<Daily>), typeof(ProcessRecords<Daily>));
builder.Services.AddScoped(typeof(IProcessRecords<PDF>), typeof(ProcessRecords<PDF>));
builder.Services.AddScoped(typeof(IFilterRecord<NASItem>), typeof(FilterRecord<NASItem>));
builder.Services.AddScoped(typeof(IFilterRecord<DailyItem>), typeof(FilterRecord<DailyItem>));
builder.Services.AddScoped(typeof(IFilterRecord<PDFItem>), typeof(FilterRecord<PDFItem>));
builder.Services.AddScoped(typeof(IRepository<NAS>), typeof(AppRepository<NAS>));
builder.Services.AddScoped(typeof(IRepository<Daily>), typeof(AppRepository<Daily>));
builder.Services.AddScoped(typeof(IRepository<PDF>), typeof(AppRepository<PDF>));
builder.Services.AddScoped(typeof(IRepository<NASItem>), typeof(AppRepository<NASItem>));
builder.Services.AddScoped(typeof(IRepository<DailyItem>), typeof(AppRepository<DailyItem>));
builder.Services.AddScoped(typeof(IRepository<PDFItem>), typeof(AppRepository<PDFItem>));
builder.Services.Configure<LogConfiguration>(Configuration.GetSection(nameof(LogConfiguration.Logger)));
builder.Services.Configure<BaseConfiguration>(Configuration.GetSection(nameof(BaseConfiguration.APIConnection)));
builder.Services.Configure<CSVConfiguration>(Configuration.GetSection(nameof(CSVConfiguration.CSVPath)));
builder.Services.Configure<ConversionConfiguration>(Configuration.GetSection(nameof(ConversionConfiguration.Conversion)));
builder.Services.Configure<EmailConfiguration>(Configuration.GetSection(nameof(EmailConfiguration.Email)));
//string connectionString = Configuration.GetConnectionString("DBConnection");
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//builder.Services.AddTransient<OracleConnection>(e => new OracleConnection(connectionString));

builder.Services.AddSingleton<DapperContext>();
var app = builder.Build();

//Connection.ConfigureServices(builder.Configuration, builder.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.Configuration.GetRequiredSection(DBConfig.ConnectionStrings);
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
