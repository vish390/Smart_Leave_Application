using SmartLeaveApproval.Core.Entities;
using SmartLeaveApproval.Core.Interfaces;

namespace SmartLeaveApproval.Application.Services;

public class AuthService
{
    private readonly IEmployeeRepository _empRepo;

    public AuthService(IEmployeeRepository empRepo) => _empRepo = empRepo;

    public async Task<Employee?> LoginAsync(string email, string password)
    {
        var employee = await _empRepo.GetByEmailAsync(email);
        if (employee == null) return null;

        bool valid = BCrypt.Net.BCrypt.Verify(password, employee.PasswordHash);
        return valid ? employee : null;
    }

    public async Task<(bool success, string message)> RegisterAsync(string fullName, string email, string password, string role)
    {
        var existing = await _empRepo.GetByEmailAsync(email);
        if (existing != null) return (false, "Email already registered.");

        var employee = new Employee
        {
            FullName = fullName,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role
        };

        await _empRepo.CreateAsync(employee);
        return (true, "Registration successful.");
    }
}