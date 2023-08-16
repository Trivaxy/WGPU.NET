using System;
using System.Collections.Generic;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public class Texture : IDisposable
    {
        private TextureImpl _impl;

        private TextureDescriptor _descriptor;

        private HashSet<TextureViewImpl> createdViews;

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
            createdViews = new HashSet<TextureViewImpl>();
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
            TextureAspect aspect)
        {
            TextureView view = TextureView.Create(TextureCreateView(Impl, new TextureViewDescriptor
            {
                label = label,
                format = format,
                dimension = dimension,
                baseMipLevel = baseMipLevel,
                mipLevelCount = mipLevelCount,
                baseArrayLayer = baseArrayLayer,
                arrayLayerCount = arrayLayerCount,
                aspect = aspect
            }), this);

            createdViews.Add(view.Impl);

            return view;
        }

        internal void RemoveTextureView(TextureView view)
        {
            if (view.Texture != this)
                throw new TextureDoesNotOwnViewException(Label);
                    
            createdViews.Remove(view.Impl);
        }

        public void Dispose()
        {
            foreach (TextureViewImpl impl in createdViews)
            {
                TextureView.For(impl).Impl = default;
                TextureView.Forget(impl);
                TextureViewRelease(impl);
            }
            
            createdViews.Clear();
            
            TextureDestroy(Impl);
            TextureRelease(Impl);
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
