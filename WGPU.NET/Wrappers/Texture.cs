using System;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public class Texture
    {
        private TextureImpl _impl;

        internal Texture(TextureImpl impl)
        {
            Impl = impl;
        }

        internal TextureImpl Impl
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDestroyedException(nameof(Texture));

                return _impl;
            }

            private set => _impl = value;
        }

        public TextureView CreateTextureView(string label, TextureFormat format, TextureViewDimension dimension,
            uint baseMipLevel, uint mipLevelCount, uint baseArrayLayer, uint arrayLayerCount,
            TextureAspect aspect) =>
            TextureView.For(TextureCreateView(Impl, new TextureViewDescriptor
            {
                label = label,
                format = format,
                dimension = dimension,
                baseMipLevel = baseMipLevel,
                mipLevelCount = mipLevelCount,
                baseArrayLayer = baseArrayLayer,
                arrayLayerCount = arrayLayerCount,
                aspect = aspect
            }));

        public void DestroyHandle()
        {
            TextureDestroy(Impl);
            Impl = default;
        }
    }
}
