using System;

namespace WGPU.NET
{
    public struct SurfaceCapabilities
    {
        public IntPtr nextInChain;
            
        public Wgpu.TextureFormat[] formats;
            
        public Wgpu.PresentMode[] presentModes;
            
        public Wgpu.CompositeAlphaMode[] alphaModes;
    }
}