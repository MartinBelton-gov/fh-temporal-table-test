﻿namespace TemporalTableTest.Entities;

public class ReferralStatus : EntityBase<long>
{
    public required string Name { get; set; } = default!;
    public required byte SortOrder { get; set; }

}
