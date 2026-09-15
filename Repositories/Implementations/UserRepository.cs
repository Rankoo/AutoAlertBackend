using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Dtos;
using AutoAlertBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoAlertBackEnd.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AutoAlertContext _context;

    public UserRepository(AutoAlertContext context)
    {
        _context = context;
    }

    public async Task<PagedUsersDto> GetAllUsersAsync(
        int page,
        int pageSize,
        Guid? roleId = null,
        string? search = null,
        bool? isActive = null)
    {
        var usersQuery = _context.Users.AsNoTracking();

        if (roleId.HasValue)
            usersQuery = usersQuery.Where(u => u.RoleId == roleId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = $"%{search.Trim()}%";
            usersQuery = usersQuery.Where(u =>
                EF.Functions.Like(u.Names, searchTerm) ||
                (u.LastNames != null && EF.Functions.Like(u.LastNames, searchTerm)) ||
                EF.Functions.Like(u.Email, searchTerm));
        }

        if (isActive.HasValue)
            usersQuery = usersQuery.Where(u => u.IsActive == isActive.Value);

        var projectedUsers = usersQuery
            .OrderBy(u => u.Id)
            .Select(u => new UserListDto
            {
                Id = u.Id,
                RoleName = u.Role != null ? u.Role.Name : null,
                Names = u.Names,
                LastNames = u.LastNames,
                Email = u.Email,
                IsActive = u.IsActive,
                LastLoginAt = u.LastLoginAt
            });
        var totalItems = await projectedUsers.CountAsync();
        var users = await projectedUsers
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedUsersDto
        {
            Users = users,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public async Task<Users?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }
    public async Task<Users> CreateUserAsync(CreateUserDto newUser)
    {
        var user = new Users()
        {  
            Names = newUser.Names,
            LastNames = newUser.LastNames,
            Email = newUser.Email,
            PhoneNumber = newUser.PhoneNumber,
            Address = newUser.Address,
            DocumentNumber = newUser.DocumentNumber,
            RoleId = newUser.RoleId,
            DocumentTypeId = newUser.DocumentTypeId,
            Position = newUser.Position,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.TemporalPassword),
            ChangePassword = newUser.ChangePassword,
            IsActive = newUser.IsActive
        };

        _context.Users.Add(user);

        if (newUser.CompanyId.HasValue && newUser.CompanyId.Value != Guid.Empty)
        {
            var companyAssociation = new UserCompanies()
            {
                UserId = user.Id,
                CompanyId = newUser.CompanyId.Value
            };

            _context.UserCompanies.Add(companyAssociation);
        }
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<Users?> UpdateUserAsync(Guid id, UpdateUserDto user)
    {
        var existingUser = await _context.Users.FindAsync(id);
        
        if (existingUser == null)
            return null;

        existingUser.Names = user.Names;
        existingUser.LastNames = user.LastNames;
        existingUser.Email = user.Email;
        existingUser.PhoneNumber = user.PhoneNumber;
        existingUser.Address = user.Address;
        existingUser.DocumentNumber = user.DocumentNumber;
        existingUser.DocumentTypeId = user.DocumentTypeId;
        existingUser.RoleId = user.RoleId;
        existingUser.Position = user.Position;
        existingUser.ChangePassword = user.ChangePassword;
        existingUser.IsActive = user.IsActive;

        if (!string.IsNullOrWhiteSpace(user.Password))
            existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

        await _context.SaveChangesAsync();
        
        return existingUser;
    }

    public async Task<Users?> UpdateOwnProfileAsync(Guid id, UpdateOwnProfileDto profile)
    {
        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser is null)
            return null;

        existingUser.Names = profile.Names.Trim();
        existingUser.LastNames = profile.LastNames?.Trim();
        existingUser.PhoneNumber = profile.PhoneNumber?.Trim();
        existingUser.Address = profile.Address?.Trim();
        existingUser.DocumentNumber = profile.DocumentNumber?.Trim();
        existingUser.DocumentTypeId = profile.DocumentTypeId;
        existingUser.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return existingUser;
    }

    public async Task<bool> UpdateOwnPasswordAsync(Guid id, string password)
    {
        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser is null)
            return false;

        existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        existingUser.ChangePassword = false;
        existingUser.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Users?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task UpdateLastLoginAsync(Guid userId)
    {
        await _context.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.LastLoginAt, DateTimeOffset.UtcNow));
    }

    public async Task<UserQuantitiesDto> GetUserQuantitiesAsync()
    {
        var now = DateTime.Now;
        var sevenDaysAgo = now.AddDays(-7);

        return new UserQuantitiesDto
        {
            TotalUsers = await _context.Users.CountAsync(),
            ActiveUsers = await _context.Users.CountAsync(u => u.IsActive),
            Administrators = await _context.Users
                .CountAsync(u => u.Role != null &&
                    (u.Role.Name == "Administrador" || u.Role.Name == "Admin")),
            UsersCreatedLastSevenDays = await _context.Users
                .CountAsync(u => u.CreatedAt >= sevenDaysAgo && u.CreatedAt <= now)
        };
    }
}
