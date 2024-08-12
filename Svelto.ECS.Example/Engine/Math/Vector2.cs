using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svelto.ECS.Example.Engine.Math
{
    public class Vector2<T> : Vector3<T> where T : unmanaged
    {
        public Vector2(T x, T y)
        {
            this.x = x;
            this.y = y;
            this.z = default(T);
        }
    }

    public class Vector2 : Vector2<float>
    {
        public Vector2(float x, float y) : base(x, y)
        {
        }
    }
}
