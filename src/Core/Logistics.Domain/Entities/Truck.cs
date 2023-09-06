﻿namespace Logistics.Domain.Entities;

public class Truck : AuditableEntity, ITenantEntity
{
    public string? TruckNumber { get; set; }
    public string? DriverId { get; set; }

    public virtual Employee? Driver { get; set; }
    public virtual IList<Load> Loads { get; } = new List<Load>();
}
