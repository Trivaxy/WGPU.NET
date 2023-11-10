using System;
using System.Linq;
using System.Runtime.CompilerServices;
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

    public struct RenderPassDepthStencilAttachment
    {
        public TextureView View;

        public LoadOp DepthLoadOp;

        public StoreOp DepthStoreOp;

        public float DepthClearValue;

        public bool DepthReadOnly;

        public LoadOp StencilLoadOp;

        public StoreOp StencilStoreOp;

        public uint StencilClearValue;

        public bool StencilReadOnly;
    }

    public struct ImageCopyTexture
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

    public struct ImageCopyBuffer
    {
        public Buffer Buffer;

        public TextureDataLayout TextureDataLayout;
        
        public static implicit operator Wgpu.ImageCopyBuffer(ImageCopyBuffer t)
        {
            return new Wgpu.ImageCopyBuffer
            {
                layout = t.TextureDataLayout,
                buffer = t.Buffer.Impl
            };
        }
    }


    public class CommandEncoder : IDisposable
    {
        private CommandEncoderImpl _impl;

        internal CommandEncoderImpl Impl 
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(CommandEncoder));

                return _impl;
            }

            private set => _impl = value;
        }

        internal CommandEncoder(CommandEncoderImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(CommandEncoder));

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
            Wgpu.RenderPassDepthStencilAttachment depthStencilAttachmentInner;

            if (depthStencilAttachment != null)
            {
                depthStencilAttachmentInner = new Wgpu.RenderPassDepthStencilAttachment
                {
                    view = depthStencilAttachment.Value.View.Impl,
                    depthLoadOp = depthStencilAttachment.Value.DepthLoadOp,
                    depthStoreOp = depthStencilAttachment.Value.DepthStoreOp,
                    depthClearValue = depthStencilAttachment.Value.DepthClearValue,
                    depthReadOnly = depthStencilAttachment.Value.DepthReadOnly ? 1u : 0u,
                    stencilLoadOp = depthStencilAttachment.Value.StencilLoadOp,
                    stencilStoreOp = depthStencilAttachment.Value.StencilStoreOp,
                    stencilClearValue = depthStencilAttachment.Value.StencilClearValue,
                    stencilReadOnly = depthStencilAttachment.Value.StencilReadOnly ? 1u : 0u
                };
            }

            Span<Wgpu.RenderPassColorAttachment> colorAttachmentsInner =
                stackalloc Wgpu.RenderPassColorAttachment[colorAttachments.Length];

            for (int i = 0; i < colorAttachments.Length; i++)
            {
                RenderPassColorAttachment colorAttachment = colorAttachments[i];

                colorAttachmentsInner[i] = new Wgpu.RenderPassColorAttachment()
                {
                    view = colorAttachment.view.Impl,
                    resolveTarget = colorAttachment.resolveTarget?.Impl ?? default,
                    loadOp = colorAttachment.loadOp,
                    storeOp = colorAttachment.storeOp,
                    clearValue = colorAttachment.clearValue,
                };
            }

            unsafe
            {
                RenderPassEncoder encoder = new RenderPassEncoder(
                    CommandEncoderBeginRenderPass(Impl, new RenderPassDescriptor
                    {
                        label = label,
                        colorAttachments = new IntPtr(Unsafe.AsPointer(ref colorAttachmentsInner.GetPinnableReference())),
                        colorAttachmentCount = (uint)colorAttachments.Length,
                        depthStencilAttachment =
                            depthStencilAttachment != null ? new IntPtr(&depthStencilAttachmentInner) : IntPtr.Zero
                    })
                );

                return encoder;
            }
        }

        public void ClearBuffer(Buffer buffer, ulong offset, ulong size)
            => CommandEncoderClearBuffer(Impl, buffer.Impl, offset, size);

        public void CopyBufferToBuffer(Buffer source, ulong sourceOffset,
            Buffer destination, ulong destinationOffset, ulong size)
            => CommandEncoderCopyBufferToBuffer(Impl, source.Impl, sourceOffset,
                destination.Impl, destinationOffset, size);

        public void CopyBufferToTexture(in ImageCopyBuffer source, in ImageCopyTexture destination,
            in Extent3D copySize)
            => CommandEncoderCopyBufferToTexture(Impl, source, destination, in copySize);

        public void CopyTextureToBuffer(in ImageCopyTexture source, in ImageCopyBuffer destination,
            in Extent3D copySize)
            => CommandEncoderCopyTextureToBuffer(Impl, source, destination, in copySize);

        public void CopyTextureToTexture(in ImageCopyTexture source, in ImageCopyTexture destination,
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
        
        public void Dispose()
        {
            CommandEncoderRelease(Impl);
            Impl = default;
        }
    }
}
