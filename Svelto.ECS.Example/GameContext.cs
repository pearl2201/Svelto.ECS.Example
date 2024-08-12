using Svelto.ECS.Schedulers;
using Svelto.ECS.Vanilla.Example.SimpleEntityEngine;
using Svelto.ECS.Vanilla.Example;

namespace Svelto.ECS.Example
{
    public class GameContext
    {
        readonly EnginesRoot _enginesRoot;
        public GameContext()
        {
            //an entity submission scheduler is needed to submit entities to the Svelto database, Svelto is not 
            //responsible to decide when to submit entities, it's the user's responsibility to do so.
            var entitySubmissionScheduler = new SimpleEntitiesSubmissionScheduler();
            //An EnginesRoot holds all the engines and entities created. it needs a EntitySubmissionScheduler to know when to
            //add previously built entities to the Svelto database. Using the SimpleEntitiesSubmissionScheduler
            //is expected as it gives complete control to the user about when the submission happens
            _enginesRoot = new EnginesRoot(entitySubmissionScheduler);

            //an entity factory allows to build entities inside engines
            var entityFactory = _enginesRoot.GenerateEntityFactory();
            //the entity functions allows other operations on entities, like remove and swap
            var entityFunctions = _enginesRoot.GenerateEntityFunctions();

            //Add the Engine to manage the SimpleEntities
            var behaviourForEntityClassEngine = new BehaviourForEntityClassEngine(entityFunctions);
            _enginesRoot.AddEngine(behaviourForEntityClassEngine);

            //build Entity with ID 0 in group0
            entityFactory.BuildEntity<SimpleEntityDescriptor>(new EGID(0, ExclusiveGroups.group0));

            //submit the previously built entities to the Svelto database
            entitySubmissionScheduler.SubmitEntities();

            //as Svelto doesn't provide an engine/system ticking system, it's the user's responsibility to
            //update engines  
            behaviourForEntityClassEngine.Update();
        }
    }

    public class BehaviourForEntityClassEngine : IQueryingEntitiesEngine
    {
        readonly IEntityFunctions _entityFunctions;

        public BehaviourForEntityClassEngine(IEntityFunctions entityFunctions)
        {
            _entityFunctions = entityFunctions;
        }

        public EntitiesDB entitiesDB { get; set; }

        public void Ready() { }

        public void Update()
        {
            var (components, entityIDs, count) = entitiesDB.QueryEntities<EntityComponent>(ExclusiveGroups.group1);

            uint entityID;
            for (var i = 0; i < count; i++)
            {
                components[i].counter++;
                entityID = entityIDs[i];
            }
        }
    }
}
