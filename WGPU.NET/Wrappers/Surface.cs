using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public class Surface
    {
        internal SurfaceImpl Impl;

        internal Surface(SurfaceImpl impl)
        {
            Impl = impl;
        }

        public TextureFormat GetPreferredFormat(Adapter adapter)
        {
            return SurfaceGetPreferredFormat(Impl, adapter.Impl);
        }
    }
}
