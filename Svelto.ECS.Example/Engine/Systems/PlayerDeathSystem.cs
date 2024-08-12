using Svelto.Common;
using Svelto.ECS.Example.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Svelto.ECS.EntitiesDB;

namespace Svelto.ECS.Example.Engine.Systems
{
    [Sequenced(nameof(PlayerDeathSystem))]
    public class PlayerDeathSystem : IStepEngine, IQueryingEntitiesEngine
    {
        public EntitiesDB entitiesDB { get; set; }
        public string name => nameof(PlayerDeathSystem);

        readonly IEntityFunctions _DBFunctions;

        EntitiesDB.SveltoFilters _sveltoFilters;
        public void Ready()
        {
            _sveltoFilters = entitiesDB.GetFilters();
        }

        public PlayerDeathSystem(IEntityFunctions dbFunctions)
        {
            _DBFunctions = dbFunctions;
        }

        public void Step()
        {
            var deadEntitiesFilter =
                   _sveltoFilters.GetTransientFilter<HealthComponent>(FilterIDs.DeadEntitiesFilter);

            foreach (var (filteredIndices, group) in deadEntitiesFilter)
            {
                //if (PlayerAliveGroup.Includes(group)) //is it a player to be damaged? 
                //{
                //    var (sounds, anim, ids, _) = entitiesDB.QueryEntities<SoundComponent, AnimationComponent>(group);

                //    for (int i = 0; i < filteredIndices.count; i++)
                //    {
                //        anim[filteredIndices[i]].animationState = new AnimationState(PlayerAnimations.Die);
                //        sounds[filteredIndices[i]].playOneShot = (int)AudioType.death;

                //        var egid = new EGID(ids[filteredIndices[i]], group);
                //        //not removing the player, only swapping to dead state, so audio can be played
                //        _DBFunctions.SwapEntityGroup<PlayerEntityDescriptor>(egid, PlayerDeadGroup.BuildGroup);
                //        //remove the gun entity so the player engines will stop processing it
                //        _DBFunctions.RemoveEntity<PlayerGunEntityDescriptor>(entitiesDB
                //               .QueryEntity<WeaponComponent>(egid).weapon.ToEGID(entitiesDB));
                //    }
                //}
            }
        }
    }
}
