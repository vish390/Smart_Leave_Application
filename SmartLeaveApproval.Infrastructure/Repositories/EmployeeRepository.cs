using Microsoft.EntityFrameworkCore;
using SmartLeaveApproval.Core.Entities;
using SmartLeaveApproval.Core.Interfaces;
using SmartLeaveApproval.Infrastructure.Data;

namespace SmartLeaveApproval.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _db;

    public EmployeeRepository(AppDbContext db) => _db = db;

    public async Task<Employee?> GetByEmailAsync(string email) =>
        await _db.Employees.FirstOrDefaultAsync(x => x.Email == email);

    public async Task<Employee?> GetByIdAsync(int id) =>
        await _db.Employees.FindAsync(id);

    public async Task<Employee> CreateAsync(Employee employee)
    {
        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();
        return employee;
    }
}