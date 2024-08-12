using Svelto.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svelto.ECS.Example
{
    public abstract class UnityContext
    {
    }

    public class UnityContext<T> : UnityContext, IDisposable where T : class, ICompositionRoot, new()
    {
        T _applicationRoot;
        bool _hasBeenInitialised;
        public UnityContext() {
            _applicationRoot = new T();

            _applicationRoot.OnContextCreated(this);
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                await Task.Yield();
                _hasBeenInitialised = true;

                _applicationRoot.OnContextInitialized(this);
            }    
        }
        public void Dispose()
        {
            _applicationRoot?.OnContextDestroyed(_hasBeenInitialised);
        }
    }
}
