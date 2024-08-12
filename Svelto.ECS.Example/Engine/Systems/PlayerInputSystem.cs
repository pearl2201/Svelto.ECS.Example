using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svelto.ECS.Example.Engine.Systems
{
    public class PlayerInputSystem : IQueryingEntitiesEngine, IStepEngine
    {
        public EntitiesDB entitiesDB { private get; set; }
        IEnumerator _readInput;
        public void Ready()
        {
            _readInput = ReadInput();
        }

        public void Step()
        {
            _readInput.MoveNext();
        }

        IEnumerator ReadInput()
        {
            void IteratePlayersInput()
            {

            }
            while (true)
            {
                IteratePlayersInput();

                yield return null;
            }
        }
        public string name => nameof(PlayerInputSystem);
    }
}
