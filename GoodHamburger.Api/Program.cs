using GoodHamburger.Api.Configurations;
using GoodHamburger.Api.Configurations.Seed;
using GoodHamburger.Api.Configurations.Seed.Abstraction;
using GoodHamburger.Api.Context;
using GoodHamburger.Api.Middleware;
using GoodHamburger.Api.Services;
using GoodHamburger.Api.Services.Abstractions;
using GoodHamburger.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging.Console;
 
var builder = WebApplication.CreateBuilder(args);



// services
builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<IOrderService, OrderService>();

//seeds
if (builder.Environment.IsStaging() || builder.Environment.IsEnvironment(ConfigApp.FistStartEnvironment))
{
    builder.Services.AddScoped<IAppSeed, MenuItemSeed>();
}

// cors
builder.Services.AddCors(options =>
{
    options.AddPolicy(ConfigApp.CorsDevelopmentPolicy, policy => policy
        .WithOrigins(ConfigApp.WebDevelopmentHttpUrl, ConfigApp.WebDevelopmentHttpsUrl)
        .AllowAnyHeader()
        .AllowAnyMethod());

    options.AddPolicy(ConfigApp.CorsStagingPolicy, policy => policy
        .WithOrigins(ConfigApp.WebStagingUrl)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// logs
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
    options.IncludeScopes = false;
    options.ColorBehavior = LoggerColorBehavior.Enabled;
});
builder.Logging.SetMinimumLevel(LogLevel.Information);

// context
builder.Services.AddDbContext<GoodHamburgerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// options
builder.Services.AddControllers()
    .AddNewtonsoftJson(options => {
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
            .SelectMany(entry => entry.Errors)
            .Select(error => error.ErrorMessage);

        return CreateBadRequest(errors);
    };
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.SwaggerDoc("v1", new()
    {
        Title = "Good Hamburger",
        Description = ""
    });
});

var app = builder.Build();

var menuImagesPath = Path.GetFullPath(Path.Combine(
    builder.Environment.ContentRootPath,
    "..",
    "GoodHamburger",
    "wwwroot",
    "images",
    "menu"));

if (Directory.Exists(menuImagesPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(menuImagesPath),
        RequestPath = "/images/menu"
    });
}

app.UseStaticFiles();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment(ConfigApp.FistStartEnvironment))
{
    app.UseCors(ConfigApp.CorsDevelopmentPolicy);
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Good Hamburger v1");
        c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
    });
}

// Staging para testes em conteiner local
if (app.Environment.IsStaging())
{
    app.UseCors(ConfigApp.CorsStagingPolicy);
}

if (app.Environment.IsStaging() || app.Environment.IsEnvironment(ConfigApp.FistStartEnvironment))
{
    await AppSeedRunner.TryRunAsync(app.Services, app.Logger);
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();

static ObjectResult CreateBadRequest(IEnumerable<string?> errors)
{
    return new ObjectResult(CreateErrorResponse(errors))
    {
        StatusCode = StatusCodes.Status400BadRequest
    };
}

static Response<object?> CreateErrorResponse(IEnumerable<string?> errors)
{
    const string invalidRequestMessage = "Os dados da requisicao sao invalidos.";

    var normalizedErrors = errors
        .Where(error => !string.IsNullOrWhiteSpace(error))
        .Select(error => error!.Trim())
        .Distinct()
        .ToList();

    if (normalizedErrors.Count == 0)
    {
        normalizedErrors.Add(invalidRequestMessage);
    }

    return new Response<object?>(null, string.Join(" | ", normalizedErrors), normalizedErrors);
}
