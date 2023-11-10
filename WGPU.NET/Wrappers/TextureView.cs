using System;
using System.Collections.Generic;

using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public class TextureView : IDisposable
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

            set => _impl = value;
        }

        /// <summary>
        /// The Texture this TextureView belongs to. If this TextureView belongs to the SwapChain, then this is null.
        /// </summary>
        public Texture Texture { get; }

        public static TextureView FromHandle(IntPtr ptr) => new TextureView(new TextureViewImpl(ptr), null);

        public IntPtr GetTextureViewHandle() => new IntPtr(_impl.Handle.ToInt64());

        private TextureView(TextureViewImpl impl, Texture texture)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(TextureView));

            Impl    = impl;
            Texture = texture;
        }

        internal static TextureView Create(TextureViewImpl impl, Texture texture)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new HandleDroppedOrDestroyedException(nameof(TextureView));

            TextureView view = new TextureView(impl, texture);
            instances.Add(impl, view);

            return view;
        }

        internal static TextureView CreateUntracked(TextureViewImpl impl) => new TextureView(impl, null);

        internal static TextureView For(TextureViewImpl impl) => impl.Handle == IntPtr.Zero ? null : instances[impl];

        internal static void Forget(TextureViewImpl impl) => instances.Remove(impl);

        /// <summary>
        /// This function will be called automatically when this TextureView's associated Texture is disposed.
        /// If you dispose the TextureView yourself, 
        /// </summary>
        public void Dispose()
        {
            Texture?.RemoveTextureView(this);
            Forget(Impl);
            TextureViewRelease(Impl);
            Impl = default;
        }
    }
}
