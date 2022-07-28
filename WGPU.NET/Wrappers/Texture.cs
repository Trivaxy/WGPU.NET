using System;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public class Texture
    {
        private TextureImpl _impl;

        private TextureDescriptor _descriptor;

        public string Label => _descriptor.label;

        public uint Usage => _descriptor.usage;

        public TextureDimension Dimension => _descriptor.dimension;

        public Extent3D Size => _descriptor.size;

        public TextureFormat Format => _descriptor.format;

        public uint MipLevelCount => _descriptor.mipLevelCount;

        public uint SampleCount => _descriptor.sampleCount;

        internal Texture(TextureImpl impl, TextureDescriptor descriptor)
        {
            if(impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(Texture));

            Impl = impl;

            _descriptor = descriptor;
        }

        internal TextureImpl Impl
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(Texture));

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

        /// <summary>
        /// Destroys the GPU Resource associated with this <see cref="Texture"/>
        /// </summary>
        public void DestroyResource()
        {
            TextureDestroy(Impl);
            Impl = default;
        }
        
        /// <summary>
        /// Signals to the underlying rust API that this <see cref="Texture"/> isn't used anymore
        /// </summary>
        public void FreeHandle()
        {
            TextureDrop(Impl);
            Impl = default;
        }
    }

    public static partial class TextureExtensions
    {
        public static TextureView CreateTextureView(this Texture texture)
        {
            return texture.CreateTextureView(texture.Label + " View",
                texture.Format,
                texture.Dimension switch
                {
                    TextureDimension.OneDimension => TextureViewDimension.OneDimension,
                    TextureDimension.TwoDimensions => TextureViewDimension.TwoDimensions,
                    TextureDimension.ThreeDimensions => TextureViewDimension.ThreeDimensions,
                    TextureDimension.Force32 => TextureViewDimension.Force32,
                    _ => throw new ArgumentException("Invalid value", nameof(texture.Dimension))
                },
                0,
                texture.MipLevelCount,
                0,
                texture.Size.depthOrArrayLayers,
                TextureAspect.All);
        }
    }
}
