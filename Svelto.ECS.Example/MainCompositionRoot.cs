using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS.Schedulers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svelto.ECS.Example
{
    public class MainCompositionRoot : ICompositionRoot
    {
        public MainCompositionRoot()
        {

        }

        public void OnContextCreated<T>(T contextHolder) { }

        public void OnContextInitialized<T>(T contextHolder)
        {
            CompositionRoot(contextHolder as UnityContext);
        }

        public void OnContextDestroyed(bool hasBeenActivated)
        {
            //final clean up
            _enginesRoot?.Dispose();
        }

        void CompositionRoot(UnityContext contextHolder)
        {
            //the SimpleEntitiesSubmissionScheduler is the scheduler to know when to submit the new entities to the database.
            //Custom ones can be created for special cases. This is the simplest default and it must
            //be ticked explicitly.
            var entitySubmissionScheduler = new EntitiesSubmissionScheduler();
            //The Engines Root is the core of Svelto.ECS. You shouldn't inject the EngineRoot,
            //therefore the composition root class must hold a reference or it will be garbage collected.
            _enginesRoot = new EnginesRoot(entitySubmissionScheduler);
            //The EntityFactory can be injected inside factories (or engines acting as factories) to build new entities
            //dynamically
            var entityFactory = _enginesRoot.GenerateEntityFactory();
            //The entity functions are a set of utility operations on Entities, including removing an entity. 
            var entityFunctions = _enginesRoot.GenerateEntityFunctions();

            //wrap non testable unity static classes, so that can be mocked if needed (or implementation can change in general, without changing the interface).
            //IRayCaster rayCaster = new RayCaster();
            //ITime time = new Time();
            //GameObjectFactory allows to create GameObjects without using the Static method GameObject.Instantiate.
            //While it seems a complication it's important to keep the engines testable and not coupled with hard
            //dependencies
            //var gameObjectResourceManager = new GameObjectResourceManager();
            //IStepEngines are engine that can be stepped (ticked) manually and explicitly with a Step() method
            var orderedEngines = new FasterList<IStepEngine>();
            var unorderedEngines = new FasterList<IStepEngine>();

            //This example has been refactored to show some advanced users of Svelto.ECS in a simple scenario
            //to know more about ECS abstraction layers read: https://www.sebaslab.com/ecs-abstraction-layers-and-modules-encapsulation/

            //Setup all the layers engines
            //OOPLayerContext.Setup(orderedEngines, _enginesRoot, gameObjectResourceManager);
            //DamageLayerContext.Setup(_enginesRoot, orderedEngines);
            //CameraLayerContext.Setup(unorderedEngines, _enginesRoot);
            //PlayerLayerContext.Setup(rayCaster, time, entityFunctions, unorderedEngines, orderedEngines,
            //    _enginesRoot);
            //EnemyLayerContext.Setup(entityFactory, time, entityFunctions,
            //    unorderedEngines, orderedEngines, new WaitForSubmissionEnumerator(entitySubmissionScheduler),
            //    _enginesRoot, gameObjectResourceManager);
            //HudLayerContext.Setup(orderedEngines, _enginesRoot);

            //group engines for order of execution. Ordering and Ticking is 100% user responsibility. This is just one of the possible way to achieve the result desired
            //orderedEngines.Add(new SurvivalUnsortedEnginesGroup(unorderedEngines));
            //orderedEngines.Add(new TickEngine(entitySubmissionScheduler));
            var sortedEnginesGroup = new SortedEnginesGroup(orderedEngines);

            //PlayerSpawner is not an engine, it could have been, but since it doesn't have an update, it's better to be a factory
            var playerSpanwer = new PlayerFactory(gameObjectResourceManager, entityFactory);

            BuildGUIEntitiesFromScene(contextHolder, entityFactory);

            StartMainLoop(sortedEnginesGroup, playerSpanwer);

            //Attach Svelto Inspector: for more info https://github.com/sebas77/svelto-ecs-inspector-unity
#if DEBUG
            //     SveltoInspector.Attach(_enginesRoot);
#endif
        }
        //Svelto ECS doesn't provide a ticking system, the user is responsible for it
        async void StartMainLoop(SortedEnginesGroup enginesToTick, PlayerFactory playerSpanwer)
        {
            await playerSpanwer.StartSpawningPlayerTask();

            RunSveltoUpdateInTheEarlyUpdate(enginesToTick);
        }

        void BuildGUIEntitiesFromScene(UnityContext contextHolder, IEntityFactory entityFactory)
        {
            /// An EntityDescriptorHolder is a special Svelto.ECS hybrid class dedicated to the unity platform.
            /// Once attached to a gameobject it automatically retrieves implementors from the hierarchy.
            /// This pattern is usually useful for guis where complex hierarchy of gameobjects are necessary, but
            /// otherwise you should always create entities in factories. 
            /// The gui of this project is ultra simple and is all managed by one entity only. This way won't do
            /// for a complex GUI.
            /// Note that creating an entity to manage a complex gui like this, is OK only for such a simple scenario
            /// otherwise a widget-like design should be adopted.
            ///
            /// UPDATE: NOTE -> SveltoGUIHelper is now deprecated. Managing GUIs with Entities is not recommended
            /// it's best to use a proper GUI framework and sync models with entity components in sync engines
            /// Building from EntityDescriptorHolders is also sort of unnecessary too (as in there could be better
            /// ways to achieve the same result)
            //SveltoGUIHelper.Create<HUDEntityDescriptorHolder>(
            //    ECSGroups.HUD, contextHolder.transform, entityFactory, true);
        }

        void RunSveltoUpdateInTheEarlyUpdate(SortedEnginesGroup enginesToTick)
        {
            PlayerLoopSystem defaultLoop = PlayerLoop.GetDefaultPlayerLoop();

            // Find the position of the early update in the default loop
            int earlyUpdateIndex = -1;
            for (int i = 0; i < defaultLoop.subSystemList.Length; i++)
            {
                if (defaultLoop.subSystemList[i].type == typeof(EarlyUpdate))
                {
                    earlyUpdateIndex = i + 1;
                    break;
                }
            }

            // Insert a custom update before the early update
            if (earlyUpdateIndex >= 0)
            {
                PlayerLoopSystem[] newSubSystemList = new PlayerLoopSystem[defaultLoop.subSystemList.Length + 1];
                Array.Copy(defaultLoop.subSystemList, newSubSystemList, earlyUpdateIndex);
                newSubSystemList[earlyUpdateIndex] = new PlayerLoopSystem
                {
                    type = typeof(MainCompositionRoot),
                    updateDelegate = Update
                };
                Array.Copy(
                    defaultLoop.subSystemList, earlyUpdateIndex, newSubSystemList, earlyUpdateIndex + 1,
                    defaultLoop.subSystemList.Length - earlyUpdateIndex);
                defaultLoop.subSystemList = newSubSystemList;
            }

            // Set the modified player loop
            PlayerLoop.SetPlayerLoop(defaultLoop);

            void Update()
            {
                if (_enginesRoot.IsValid())
                {
                    enginesToTick.Step();
                }
            }
        }

        EnginesRoot _enginesRoot;
    }
}
