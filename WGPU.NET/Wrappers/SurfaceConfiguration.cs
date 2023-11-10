using System;

namespace WGPU.NET
{
    public struct SurfaceConfiguration
    {
        public IntPtr nextInChain;
            
        public Wgpu.TextureFormat format;
            
        public Wgpu.TextureUsage usage;
            
        public Wgpu.TextureFormat[] viewFormats;
            
        public Wgpu.CompositeAlphaMode alphaMode;
            
        public uint width;
            
        public uint height;
            
        public Wgpu.PresentMode presentMode;
    }
}