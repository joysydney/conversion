using Application.Services;
using Archival.Application.Services; 
using Archival.Core.Interfaces;
using Archival.Core.Model;
using DB.Data;
using DB.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oracle.ManagedDataAccess.Client; 
using Microsoft.EntityFrameworkCore;
using Archival.Application.Util;
using Archival.Core.Configuration;
using DB.File; 
//this Program.cs for testing only
IConfiguration Configuration = new ConfigurationBuilder() 
  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) 
  .AddEnvironmentVariables()
  .AddCommandLine(args)
  .Build();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, configuration) =>
    {
        var env = context.HostingEnvironment; 
        var sharedFolder = Path.Combine(env.ContentRootPath, "..", "Shared");

        //configuration.AddJsonFile("appsettings.json"); 
        var path = Path.Combine(sharedFolder, "appsettings.json");
        configuration.AddJsonFile(path, optional: true);  
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHttpClient();
        services.AddScoped(typeof(AppRepository<NAS>)); 
        services.AddScoped(typeof(ProcessRecords<NAS>)); 
        services.AddScoped(typeof(ProcessRecords<Daily>));  
        services.AddScoped(typeof(ProcessRecords<PDF>));  
        services.AddScoped<NASConversion>(); 
        services.AddScoped<DailyConversion>();  
        services.AddScoped<PDFConversion>();  
        services.AddScoped<Email>();  
        services.AddScoped<IAuthenticate, Authenticate>();  
        services.AddScoped<IDailyConversion, DailyConversion>();  
        services.AddScoped<IEmail, Email>();  
        services.AddScoped<ILogManagerCustom, LogManagerCustom>(); 
        services.AddScoped(typeof(ICSAccess<NAS>), typeof(CSAccess<NAS>)); 
        services.AddScoped(typeof(ICSAccess<Daily>), typeof(CSAccess<Daily>)); 
        services.AddScoped(typeof(IUtil<NAS>), typeof(Util<NAS>)); 
        services.AddScoped(typeof(IUtil<Daily>), typeof(Util<Daily>)); 
        services.AddScoped(typeof(IPDFService<NAS>), typeof(PDFService<NAS>)); 
        services.AddScoped(typeof(IPDFService<Daily>), typeof(PDFService<Daily>)); 
        services.AddScoped<INASConversion, NASConversion>();
        services.AddScoped<IDailyConversion, DailyConversion>();
        services.AddScoped(typeof(ICSVReport<NAS>), typeof(CSVReport<NAS>));  
        services.AddScoped(typeof(ICSVReport<Daily>), typeof(CSVReport<Daily>));   
        services.AddScoped(typeof(IProcessRecords<NAS>), typeof(ProcessRecords<NAS>));  
        services.AddScoped(typeof(IProcessRecords<Daily>), typeof(ProcessRecords<Daily>));  
        services.AddScoped(typeof(IProcessRecords<PDF>), typeof(ProcessRecords<PDF>));  
        services.AddScoped(typeof(IFilterRecord<NASItem>), typeof(FilterRecord<NASItem>));
        services.AddScoped(typeof(IFilterRecord<DailyItem>), typeof(FilterRecord<DailyItem>));
        services.AddScoped(typeof(IFilterRecord<PDFItem>), typeof(FilterRecord<PDFItem>)); 
        services.Configure<LogConfiguration>(context.Configuration.GetSection(nameof(LogConfiguration.Logger)));
        services.Configure<BaseConfiguration>(context.Configuration.GetSection(nameof(BaseConfiguration.APIConnection)));
        services.Configure<CSVConfiguration>(context.Configuration.GetSection(nameof(CSVConfiguration.CSVPath)));
        services.Configure<ConversionConfiguration>(context.Configuration.GetSection(nameof(ConversionConfiguration.Conversion)));
        services.Configure<EmailConfiguration>(context.Configuration.GetSection(nameof(EmailConfiguration.Email)));
        string connectionString = Configuration.GetConnectionString("DBConnection");
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddTransient<OracleConnection>(e => new OracleConnection(connectionString));
        /*services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });*/
    })
    .Build();
/*   
NASConversion c = host.Services.GetRequiredService<NASConversion>();
await c.Execute();

 */
DailyConversion c = host.Services.GetRequiredService<DailyConversion>();
await c.Execute();

await host.RunAsync();