using System;
using System.Collections.Generic;
using System.Text;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public class BindGroupLayout
    {
        private static Dictionary<BindGroupLayoutImpl, BindGroupLayout> instances =
            new Dictionary<BindGroupLayoutImpl, BindGroupLayout>();

        internal BindGroupLayoutImpl Impl;

        private BindGroupLayout(BindGroupLayoutImpl impl)
        {
            Impl = impl;
        }

        internal static BindGroupLayout For(BindGroupLayoutImpl impl)
            => impl.Handle == IntPtr.Zero ? null : instances.GetOrCreate(impl, () => new BindGroupLayout(impl));
    }
}
