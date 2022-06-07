using System;
using System.Collections.Generic;
using System.Text;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public class TextureView
    {
        private static Dictionary<TextureViewImpl, TextureView> instances = 
            new Dictionary<TextureViewImpl, TextureView>();

        internal TextureViewImpl Impl;

        private TextureView(TextureViewImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(TextureView));

            Impl = impl;
        }

        internal static TextureView For(TextureViewImpl impl)
            => impl.Handle==IntPtr.Zero ? null : instances.GetOrCreate(impl, () => new TextureView(impl));
    }
}
