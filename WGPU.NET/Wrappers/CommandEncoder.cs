using System.Linq;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public struct RenderPassColorAttachment
    {
        public TextureView view;

        public TextureView resolveTarget;

        public LoadOp loadOp;

        public StoreOp storeOp;

        public Color clearValue;
    }

    public partial struct ImageCopyTexture
    {
        public Texture Texture;

        public uint MipLevel;

        public Origin3D Origin;

        public TextureAspect Aspect;

        public static implicit operator Wgpu.ImageCopyTexture(ImageCopyTexture t)
        {
            return new Wgpu.ImageCopyTexture
            {
                texture = t.Texture.Impl,
                mipLevel = t.MipLevel,
                origin = t.Origin,
                aspect = t.Aspect
            };
        }
    }


    public class CommandEncoder
    {
        internal CommandEncoderImpl Impl;

        internal CommandEncoder(CommandEncoderImpl impl)
        {
            Impl = impl;
        }

        public ComputePassEncoder BeginComputePass(string label)
        {
            return new ComputePassEncoder(
                CommandEncoderBeginComputePass(Impl, new ComputePassDescriptor
                {
                    label = label
                })
            );
        }

        public RenderPassEncoder BeginRenderPass(string label, 
            RenderPassColorAttachment[] colorAttachments,
            RenderPassDepthStencilAttachment? depthStencilAttachment
            )
        {
            return new RenderPassEncoder(
                CommandEncoderBeginRenderPass(Impl, new RenderPassDescriptor
                {
                    label = label,
                    colorAttachments = Util.AllocHArray(colorAttachments.Length, 
                        colorAttachments.Select(x=>new Wgpu.RenderPassColorAttachment
                        {
                            view = x.view.Impl,
                            resolveTarget = x.resolveTarget?.Impl ?? default,
                            loadOp = x.loadOp,
                            storeOp = x.storeOp,
                            clearValue = x.clearValue,
                            
                        })
                    ),
                    colorAttachmentCount = (uint)colorAttachments.Length,
                    depthStencilAttachment = Util.Optional(depthStencilAttachment)
                })
            );
        }

        public void ClearBuffer(Buffer buffer, ulong offset, ulong size)
            => CommandEncoderClearBuffer(Impl, buffer.Impl, offset, size);

        public void CopyBufferToBuffer(Buffer source, ulong sourceOffset,
            Buffer destination, ulong destinationOffset, ulong size)
            => CommandEncoderCopyBufferToBuffer(Impl, source.Impl, sourceOffset,
                destination.Impl, destinationOffset, size);

        public void CopyBufferToTexure(in ImageCopyBuffer source, in ImageCopyTexture destination,
            in Extent3D copySize)

            => CommandEncoderCopyBufferToTexture(Impl, in source, new Wgpu.ImageCopyTexture
            {

            }, in copySize);

        public void CopyTexureToBuffer(in ImageCopyTexture source, in ImageCopyBuffer destination,
            in Extent3D copySize)

            => CommandEncoderCopyTextureToBuffer(Impl, source, in destination, in copySize);

        public void CopyTexureToTexture(in ImageCopyTexture source, in ImageCopyTexture destination,
            in Extent3D copySize)

            => CommandEncoderCopyTextureToTexture(Impl, source, destination, in copySize);

        public CommandBuffer Finish(string label) => new CommandBuffer(CommandEncoderFinish(Impl,
            new CommandBufferDescriptor
            {
                label = label
            }));

        public void InsertDebugMarker(string markerLabel)
            => CommandEncoderInsertDebugMarker(Impl, markerLabel);

        public void PushDebugGroup(string groupLabel)
            => CommandEncoderPushDebugGroup(Impl, groupLabel);

        public void PopDebugGroup(string groupLabel) => CommandEncoderPopDebugGroup(Impl);

        public void ResolveQuerySet(QuerySet querySet, uint firstQuery, uint queryCount, Buffer destination, ulong destinationOffset)
            => CommandEncoderResolveQuerySet(Impl, querySet.Impl, firstQuery, queryCount, destination.Impl, destinationOffset);

        public void WriteTimestamp(QuerySet querySet, uint queryIndex)
            => CommandEncoderWriteTimestamp(Impl, querySet.Impl, queryIndex);
    }
}
