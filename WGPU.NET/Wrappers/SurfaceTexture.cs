namespace WGPU.NET
{
    public class SurfaceTexture
    {
        public Texture texture;
        public uint suboptimal;
            
        public Wgpu.SurfaceGetCurrentTextureStatus status;
    }
}