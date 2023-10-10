﻿using Logistics.Domain.Core;
using Logistics.Shared.Enums;

namespace Logistics.Domain.Entities;

public class Employee : AuditableEntity, ITenantEntity
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    
    public decimal Salary { get; set; }
    public SalaryType SalaryType { get; set; }
    
    /// <summary>
    /// When employee joined to this tenant
    /// </summary>
    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// User's device token generated by the Firebase
    /// </summary>
    public string? DeviceToken { get; set; }
    
    public string? TruckId { get; set; }
    public virtual Truck? Truck { get; set; }

    /// <summary>
    /// Dispatched loads by dispatchers
    /// </summary>
    public virtual List<Load> DispatchedLoads { get; } = new();
    public virtual List<PayrollPayment> PayrollPayments { get; set; } = new();

    /// <summary>
    /// User tenant roles
    /// </summary>
    public virtual HashSet<TenantRole> Roles { get; } = new(new TenantRoleComparer());

    public virtual List<EmployeeTenantRole> EmployeeRoles { get; } = new();

    public string GetFullName() => $"{FirstName} {LastName}";

    public static Employee CreateEmployeeFromUser(User user)
    {
        var newEmployee = new Employee
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber
        };

        return newEmployee;
    }
}
