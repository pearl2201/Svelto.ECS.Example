using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svelto.ECS.Example.Engine.Math
{
    public class Vector3<T> where T: unmanaged
    {
        public T x;
        public T y;
        public T z;
    }

    public class Vector3: Vector3<float>
    {

    }

    public class Vector3Int: Vector3<int> { }
}
