using Microsoft.EntityFrameworkCore;
using PetClinic.Components;
using PetClinic.Data;
using PetClinic.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<PetClinicContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=petclinic.db"));

builder.Services.AddMemoryCache();
builder.Services.AddScoped<OwnerService>();
builder.Services.AddScoped<PetService>();
builder.Services.AddScoped<VisitService>();
builder.Services.AddScoped<VetService>();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PetClinicContext>();
    DbInitializer.Initialize(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// JSON API for vets (mirrors Spring's @ResponseBody)
app.MapGet("/api/vets", async (VetService vetService) =>
{
    var vets = await vetService.GetAllAsync();
    return Results.Ok(vets.Select(v => new
    {
        v.Id,
        v.FirstName,
        v.LastName,
        specialties = v.Specialties.Select(s => new { s.Id, s.Name })
    }));
});

app.Run();
