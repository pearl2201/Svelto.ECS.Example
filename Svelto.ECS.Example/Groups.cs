using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svelto.ECS.Example
{
    public class Dead : GroupTag<Dead>
    {

        static Dead()
        {
            bitmask = ExclusiveGroupBitmask.DISABLED_BIT;
        }
    }

    public class Damageable : GroupTag<Damageable> { };

    public static class FilterIDs
    {
        static readonly FilterContextID DamageLayerFilterContext = FilterContextID.GetNewContextID();

        public static CombinedFilterID DamagedEntitiesFilter = new CombinedFilterID(0, DamageLayerFilterContext);
        public static CombinedFilterID DeadEntitiesFilter = new CombinedFilterID(1, DamageLayerFilterContext);
    }
}
