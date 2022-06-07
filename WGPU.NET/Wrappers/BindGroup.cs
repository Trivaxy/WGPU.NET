using System;
using System.Collections.Generic;
using System.Text;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public class BindGroup
    {
        internal BindGroupImpl Impl;

        internal BindGroup(BindGroupImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(BindGroup));

            Impl = impl;
        }
    }
}
