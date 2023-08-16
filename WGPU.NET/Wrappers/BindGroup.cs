using System;
using System.Collections.Generic;
using System.Text;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public class BindGroup : IDisposable
    {
        private BindGroupImpl _impl;

        internal BindGroupImpl Impl 
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(BindGroup));

                return _impl;
            }

            private set => _impl = value;
        }

        internal BindGroup(BindGroupImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(BindGroup));

            Impl = impl;
        }

        public void Dispose()
        {
            BindGroupRelease(Impl);
            Impl = default;
        }
    }
}
