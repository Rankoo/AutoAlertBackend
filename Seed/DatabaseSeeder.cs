using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AutoAlertContext context, ILogger logger)
    {
        var adminRole = await GetOrCreateRole(context, "ADMIN", "Administrador del sistema");
        var documentType = await context.DocumentTypes.FirstOrDefaultAsync(d => d.Abbreviation == "CC");
        if (documentType is null)
        {
            documentType = new DocumentTypes { Name = "Cédula de ciudadanía", Abbreviation = "CC" };
            context.DocumentTypes.Add(documentType);
            await context.SaveChangesAsync();
        }

        var permissions = new[]
        {
            "VIEW_STORES", "CREATE_STORES", "EDIT_STORES", "DELETE_STORES",
            "VIEW_SERVICES", "CREATE_SERVICES", "EDIT_SERVICES", "DELETE_SERVICES",
            "VIEW_ALERTS", "CREATE_ALERTS", "EDIT_ALERTS", "DELETE_ALERTS",
            "VIEW_NOTIFICATIONS", "CREATE_NOTIFICATIONS", "EDIT_NOTIFICATIONS", "DELETE_NOTIFICATIONS"
        };
        await EnsurePermissions(context, adminRole, permissions);

        var admin = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@autoalert.local");
        if (admin is null)
        {
            admin = new Users
            {
                Names = "Administrador",
                LastNames = "AutoAlert",
                Email = "admin@autoalert.local",
                DocumentTypeId = documentType.Id,
                DocumentNumber = "1000000000",
                RoleId = adminRole.Id,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123*"),
                IsActive = true,
                ChangePassword = false
            };
            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }

        var centro = await GetOrCreateStore(context, "Tienda Centro", "Bogotá", "Carrera 10 # 20-30");
        var norte = await GetOrCreateStore(context, "Tienda Norte", "Bogotá", "Calle 170 # 8-15");
        var energia = await GetOrCreateService(context, centro.Id, "Energía eléctrica", "Empresa de Energía", "100200300");
        var internet = await GetOrCreateService(context, centro.Id, "Internet", "Proveedor Internet", "IN-4587");
        var agua = await GetOrCreateService(context, norte.Id, "Acueducto", "Empresa de Acueducto", "AG-2025");

        await GetOrCreateAlert(context, admin.Id, energia, DateTime.Today.AddDays(3), 185000m);
        await GetOrCreateAlert(context, admin.Id, internet, DateTime.Today.AddDays(7), 129900m);
        await GetOrCreateAlert(context, admin.Id, agua, DateTime.Today.AddDays(-2), 86000m, "Vencido");
        logger.LogInformation("Datos demo de AutoAlert verificados. Usuario: admin@autoalert.local");
    }

    private static async Task<Roles> GetOrCreateRole(AutoAlertContext context, string name, string description)
    {
        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == name);
        if (role is not null) return role;
        role = new Roles { Name = name, Description = description };
        context.Roles.Add(role); await context.SaveChangesAsync(); return role;
    }

    private static async Task EnsurePermissions(AutoAlertContext context, Roles role, IEnumerable<string> permissions)
    {
        foreach (var permission in permissions)
        {
            var moduleName = permission.Split('_')[1] == "STORES" ? "STORES" : permission.Split('_')[1];
            var module = await context.Modules.FirstOrDefaultAsync(m => m.Name == moduleName);
            if (module is null) { module = new Modules { Name = moduleName }; context.Modules.Add(module); await context.SaveChangesAsync(); }
            var subModule = await context.SubModules.FirstOrDefaultAsync(s => s.Name == permission);
            if (subModule is null) { subModule = new SubModules { ModuleId = module.Id, Name = permission }; context.SubModules.Add(subModule); await context.SaveChangesAsync(); }
            var assignment = await context.RoleSubModules.FirstOrDefaultAsync(r => r.RoleId == role.Id && r.SubModuleId == subModule.Id);
            if (assignment is null) context.RoleSubModules.Add(new RoleSubModules { RoleId = role.Id, SubModuleId = subModule.Id, IsEnabled = true });
            else if (!assignment.IsEnabled) assignment.IsEnabled = true;
        }
        await context.SaveChangesAsync();
    }

    private static async Task<Stores> GetOrCreateStore(AutoAlertContext context, string name, string city, string address)
    {
        var store = await context.Stores.FirstOrDefaultAsync(s => s.Name == name);
        if (store is not null) return store;
        store = new Stores { Name = name, City = city, Address = address };
        context.Stores.Add(store); await context.SaveChangesAsync(); return store;
    }

    private static async Task<Services> GetOrCreateService(AutoAlertContext context, Guid storeId, string name, string provider, string accountNumber)
    {
        var service = await context.Services.FirstOrDefaultAsync(s => s.StoreId == storeId && s.Name == name);
        if (service is not null) return service;
        service = new Services { StoreId = storeId, Name = name, Provider = provider, AccountNumber = accountNumber };
        context.Services.Add(service); await context.SaveChangesAsync(); return service;
    }

    private static async Task GetOrCreateAlert(AutoAlertContext context, Guid userId, Services service, DateTime dueDate, decimal amount, string status = "Pendiente")
    {
        var alert = await context.Alerts.FirstOrDefaultAsync(a => a.ServiceId == service.Id && a.DueDate == dueDate);
        if (alert is null)
        {
            alert = new Alerts { ServiceId = service.Id, DueDate = dueDate, Amount = amount, Status = status };
            context.Alerts.Add(alert); await context.SaveChangesAsync();
        }
        if (!await context.Notifications.AnyAsync(n => n.AlertId == alert.Id && n.UserId == userId))
        {
            context.Notifications.Add(new Notifications { AlertId = alert.Id, UserId = userId, Title = "Alerta de pago", Message = $"{service.Name} vence el {dueDate:dd/MM/yyyy} por {amount:C0}.", Result = "Pendiente" });
            await context.SaveChangesAsync();
        }
    }
}
