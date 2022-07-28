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
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(Surface));

            Impl = impl;
        }

        public TextureFormat GetPreferredFormat(Adapter adapter)
        {
            return SurfaceGetPreferredFormat(Impl, adapter.Impl);
        }
    }
}
