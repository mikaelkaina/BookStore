using BookStore.API.Middleware;
using BookStore.Application;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure;
using BookStore.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "BookStore API";
        options.Theme = ScalarTheme.Purple;
        options.WithHttpBearerAuthentication(bearer =>
        {
            bearer.Token = "seu-token-aqui";
        });
    });

    using var scope = app.Services.CreateScope();
    await SeedRolesAsync(scope.ServiceProvider);
    await SeedRoleAsync(scope.ServiceProvider);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

static async Task SeedRolesAsync(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = ["Admin", "Customer"];
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

}

static async Task SeedRoleAsync(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var customerRepository = services.GetRequiredService<ICustomerRepository>();
    var unitOfWork = services.GetRequiredService<IUnitOfWork>();

    string[] roles = ["Admin", "Customer"];
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    const string adminEmail = "admin@bookstore.com";
    var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

    if (existingAdmin is null)
    {
        var customerResult = Customer.Create(
            "Admin", "BookStore", adminEmail,
            phone: null, document: "529.982.247-25");

        if (customerResult.IsSuccess)
        {
            await customerRepository.AddAsync(customerResult.Value);
            await unitOfWork.SaveChangesAsync();

            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "BookStore",
                EmailConfirmed = true,
                CustomerId = customerResult.Value.Id,
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123456");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

