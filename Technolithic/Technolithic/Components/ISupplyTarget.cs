using System;

namespace Technolithic
{
    public interface ISupplyTarget : IApproachable
    {
        Inventory Inventory { get; }

        Action<ISupplyTarget> OnRemovedCallback { get; set; }
    }
}
