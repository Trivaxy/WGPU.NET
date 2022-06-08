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

        private TextureViewImpl _impl;

        internal TextureViewImpl Impl 
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(TextureView));

                return _impl;
            }

            private set => _impl = value;
        }

        private TextureView(TextureViewImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(TextureView));

            Impl = impl;
        }

        internal static TextureView For(TextureViewImpl impl)
            => impl.Handle==IntPtr.Zero ? null : instances.GetOrCreate(impl, () => new TextureView(impl));
        
        /// <summary>
        /// Signals to the underlying rust API that this <see cref="TextureView"/> isn't used anymore
        /// </summary>
        public void FreeHandle()
        {
            TextureViewDrop(Impl);
            Impl = default;
        }
    }
}
