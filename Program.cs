using EventManagementSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC + API controllers
builder.Services.AddControllersWithViews();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // IMPORTANT: Only include API endpoints (attribute-routed) under /api/*
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        var path = apiDesc.RelativePath; // e.g. "api/EventsApi"
        return !string.IsNullOrWhiteSpace(path) &&
               path.StartsWith("api/", StringComparison.OrdinalIgnoreCase);
    });
});

// Your services
builder.Services.AddSingleton<SupabaseService>();

// Session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Swagger only in dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EventManagementSystem API v1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

// Map attribute-routed controllers (your /api/* controllers)
app.MapControllers();

app.MapControllerRoute(
    name: "categories_root",
    pattern: "Categories",
    defaults: new { controller = "Categories", action = "Index" });

app.MapControllerRoute(
    name: "locations_root",
    pattern: "Locations",
    defaults: new { controller = "Locations", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
