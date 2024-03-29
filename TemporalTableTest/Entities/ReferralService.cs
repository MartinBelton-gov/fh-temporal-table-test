﻿namespace TemporalTableTest.Entities;

public class ReferralService : EntityBase<long>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public virtual required ReferralOrganisation ReferralOrganisation { get; set; }
}
