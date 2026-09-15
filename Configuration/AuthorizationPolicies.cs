using Microsoft.AspNetCore.Authorization;

namespace AutoAlertBackEnd.Configuration;

/// <summary>
/// Configuración centralizada de políticas de autorización del sistema AutoAlert.
/// Define todas las políticas basadas en permisos que se validan contra la base de datos.
/// </summary>
public static class AuthorizationPolicies
{
    /// <summary>
    /// Lista completa de permisos del sistema organizados por módulo funcional.
    /// Cada permiso se valida mediante PermissionHandler contra RoleSubModules y UserSubmodules.
    /// </summary>
    public static class Permissions
    {
        // Gestión de Usuarios
        public const string ViewUsers = "VIEW_USERS";
        public const string CreateUsers = "CREATE_USERS";
        public const string EditUsers = "EDIT_USERS";
        public const string DeleteUsers = "DELETE_USERS";

        // // Gestión de Roles
        // public const string ViewRoles = "VIEW_ROLES";
        // public const string CreateRoles = "CREATE_ROLES";
        // public const string EditRoles = "EDIT_ROLES";
        // public const string DeleteRoles = "DELETE_ROLES";

        // // Gestión de Empresas
        // public const string ViewCompanies = "VIEW_COMPANIES";
        // public const string CreateCompanies = "CREATE_COMPANIES";
        // public const string EditCompanies = "EDIT_COMPANIES";
        // public const string DeleteCompanies = "DELETE_COMPANIES";

        // // Gestión de Grupos
        // public const string ViewGroups = "VIEW_GROUPS";
        // public const string CreateGroups = "CREATE_GROUPS";
        // public const string EditGroups = "EDIT_GROUPS";
        // public const string DeleteGroups = "DELETE_GROUPS";

        // Gestión de Tiendas
        public const string ViewStores = "VIEW_STORES";
        public const string CreateStores = "CREATE_STORES";
        public const string EditStores = "EDIT_STORES";
        public const string DeleteStores = "DELETE_STORES";

        // // Gestión de Servicios
        public const string ViewServices = "VIEW_SERVICES";
        public const string CreateServices = "CREATE_SERVICES";
        public const string EditServices = "EDIT_SERVICES";
        public const string DeleteServices = "DELETE_SERVICES";

        // // Gestión de Alertas
        public const string ViewAlerts = "VIEW_ALERTS";
        public const string CreateAlerts = "CREATE_ALERTS";
        public const string EditAlerts = "EDIT_ALERTS";
        public const string DeleteAlerts = "DELETE_ALERTS";

        // // Gestión de Notificaciones
        public const string ViewNotifications = "VIEW_NOTIFICATIONS";
        public const string CreateNotifications = "CREATE_NOTIFICATIONS";
        public const string EditNotifications = "EDIT_NOTIFICATIONS";
        public const string DeleteNotifications = "DELETE_NOTIFICATIONS";

        // // Gestión de Módulos
        // public const string ViewModules = "VIEW_MODULES";
        // public const string CreateModules = "CREATE_MODULES";
        // public const string EditModules = "EDIT_MODULES";
        // public const string DeleteModules = "DELETE_MODULES";

        // // Gestión de SubMódulos
        // public const string ViewSubmodules = "VIEW_SUBMODULES";
        // public const string CreateSubmodules = "CREATE_SUBMODULES";
        // public const string EditSubmodules = "EDIT_SUBMODULES";
        // public const string DeleteSubmodules = "DELETE_SUBMODULES";

        // // Gestión de Tipos de Documento
        // public const string ViewDocumentTypes = "VIEW_DOCUMENT_TYPES";
        // public const string CreateDocumentTypes = "CREATE_DOCUMENT_TYPES";
        // public const string EditDocumentTypes = "EDIT_DOCUMENT_TYPES";
        // public const string DeleteDocumentTypes = "DELETE_DOCUMENT_TYPES";

        // // Gestión de Permisos (Roles-SubMódulos y Usuarios-SubMódulos)
        // public const string ViewPermissions = "VIEW_PERMISSIONS";
        // public const string UpdatePermissions = "UPDATE_PERMISSIONS";

        // // Gestión de Relaciones Usuario-Empresa
        // public const string ViewUserCompanies = "VIEW_USER_COMPANIES";
        // public const string CreateUserCompanies = "CREATE_USER_COMPANIES";
        // public const string DeleteUserCompanies = "DELETE_USER_COMPANIES";

        // // Gestión de Relaciones Usuario-Grupo
        // public const string ViewUserGroups = "VIEW_USER_GROUPS";
        // public const string CreateUserGroups = "CREATE_USER_GROUPS";
        // public const string DeleteUserGroups = "DELETE_USER_GROUPS";

        // // Auditoría y Logs (solo lectura)
        // public const string ViewLogs = "VIEW_LOGS";
    }

    /// <summary>
    /// Obtiene la lista completa de todos los permisos del sistema.
    /// </summary>
    public static string[] GetAllPermissions()
    {
        return new[]
        {
            // Usuarios
            Permissions.ViewUsers,
            Permissions.CreateUsers,
            Permissions.EditUsers,
            Permissions.DeleteUsers,

            // // Roles
            // Permissions.ViewRoles,
            // Permissions.CreateRoles,
            // Permissions.EditRoles,
            // Permissions.DeleteRoles,

            // // Empresas
            // Permissions.ViewCompanies,
            // Permissions.CreateCompanies,
            // Permissions.EditCompanies,
            // Permissions.DeleteCompanies,

            // // Grupos
            // Permissions.ViewGroups,
            // Permissions.CreateGroups,
            // Permissions.EditGroups,
            // Permissions.DeleteGroups,

            // Tiendas
            Permissions.ViewStores,
            Permissions.CreateStores,
            Permissions.EditStores,
            Permissions.DeleteStores,

            // // Servicios
            Permissions.ViewServices,
            Permissions.CreateServices,
            Permissions.EditServices,
            Permissions.DeleteServices,

            // // Alertas
            Permissions.ViewAlerts,
            Permissions.CreateAlerts,
            Permissions.EditAlerts,
            Permissions.DeleteAlerts,

            // // Notificaciones
            Permissions.ViewNotifications,
            Permissions.CreateNotifications,
            Permissions.EditNotifications,
            Permissions.DeleteNotifications,

            // // Módulos
            // Permissions.ViewModules,
            // Permissions.CreateModules,
            // Permissions.EditModules,
            // Permissions.DeleteModules,

            // // SubMódulos
            // Permissions.ViewSubmodules,
            // Permissions.CreateSubmodules,
            // Permissions.EditSubmodules,
            // Permissions.DeleteSubmodules,

            // // Tipos de Documento
            // Permissions.ViewDocumentTypes,
            // Permissions.CreateDocumentTypes,
            // Permissions.EditDocumentTypes,
            // Permissions.DeleteDocumentTypes,

            // // Permisos
            // Permissions.ViewPermissions,
            // Permissions.UpdatePermissions,

            // // Usuario-Empresa
            // Permissions.ViewUserCompanies,
            // Permissions.CreateUserCompanies,
            // Permissions.DeleteUserCompanies,

            // // Usuario-Grupo
            // Permissions.ViewUserGroups,
            // Permissions.CreateUserGroups,
            // Permissions.DeleteUserGroups,

            // // Logs
            // Permissions.ViewLogs
        };
    }

    /// <summary>
    /// Configura todas las políticas de autorización en el sistema.
    /// Cada política se asocia con un PermissionRequirement que valida contra la base de datos.
    /// </summary>
    public static void ConfigurePolicies(AuthorizationOptions options)
    {
        var permissions = GetAllPermissions();

        foreach (var permission in permissions)
        {
            options.AddPolicy(permission, policy =>
                policy.Requirements.Add(new PermissionRequirement(permission)));
        }
    }
}
