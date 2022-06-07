using System;
using System.Runtime.CompilerServices;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public class RenderBundleEncoder
    {
        internal RenderBundleEncoderImpl Impl;

        internal RenderBundleEncoder(RenderBundleEncoderImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(RenderBundleEncoder));

            Impl = impl;
        }

        public void Draw(uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance)
            => RenderBundleEncoderDraw(Impl, vertexCount, instanceCount, firstVertex, firstInstance);

        public void DrawIndexed(uint indexCount, uint instanceCount, uint firstIndex, int baseVertex, uint firstInstance)
            => RenderBundleEncoderDrawIndexed(Impl, indexCount, instanceCount, firstIndex, baseVertex, firstInstance);

        public void DrawIndexedIndirect(Buffer indirectBuffer, ulong indirectOffset)
            => RenderBundleEncoderDrawIndexedIndirect(Impl, indirectBuffer.Impl, indirectOffset);

        public void DrawIndirect(Buffer indirectBuffer, ulong indirectOffset)
            => RenderBundleEncoderDrawIndirect(Impl, indirectBuffer.Impl, indirectOffset);

        public RenderBundle Finish(string label)
        {
            return new RenderBundle(
                RenderBundleEncoderFinish(Impl, new RenderBundleDescriptor
                {
                    label = label
                })
            );
        }

        public void InsertDebugMarker(string markerLabel)
            => RenderBundleEncoderInsertDebugMarker(Impl, markerLabel);

        public void PushDebugGroup(string groupLabel)
            => RenderBundleEncoderPushDebugGroup(Impl, groupLabel);

        public void PopDebugGroup(string groupLabel) => RenderBundleEncoderPopDebugGroup(Impl);

        public unsafe void SetBindGroup(uint groupIndex, BindGroup group, uint[] dynamicOffsets)
        {
            RenderBundleEncoderSetBindGroup(Impl, groupIndex,
                group.Impl,
                (uint)dynamicOffsets.Length,
                ref Unsafe.AsRef<uint>((void*)Util.AllocHArray(dynamicOffsets))
            );
        }

        public void SetIndexBuffer(Buffer buffer, IndexFormat format, ulong offset, ulong size)
            => RenderBundleEncoderSetIndexBuffer(Impl, buffer.Impl, format, offset, size);

        public void SetPipeline(RenderPipeline pipeline) => RenderBundleEncoderSetPipeline(Impl, pipeline.Impl);

        public void SetVertexBuffer(uint slot, Buffer buffer, ulong offset, ulong size)
            => RenderBundleEncoderSetVertexBuffer(Impl, slot, buffer.Impl, offset, size);

    }
}
