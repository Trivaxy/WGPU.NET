using System;
using System.Runtime.CompilerServices;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public class RenderBundleEncoder
    {
        private RenderBundleEncoderImpl _impl;

        internal RenderBundleEncoder(RenderBundleEncoderImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(RenderBundleEncoder));

            _impl = impl;
        }

        public void Draw(uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance)
            => RenderBundleEncoderDraw(_impl, vertexCount, instanceCount, firstVertex, firstInstance);

        public void DrawIndexed(uint indexCount, uint instanceCount, uint firstIndex, int baseVertex, uint firstInstance)
            => RenderBundleEncoderDrawIndexed(_impl, indexCount, instanceCount, firstIndex, baseVertex, firstInstance);

        public void DrawIndexedIndirect(Buffer indirectBuffer, ulong indirectOffset)
            => RenderBundleEncoderDrawIndexedIndirect(_impl, indirectBuffer.Impl, indirectOffset);

        public void DrawIndirect(Buffer indirectBuffer, ulong indirectOffset)
            => RenderBundleEncoderDrawIndirect(_impl, indirectBuffer.Impl, indirectOffset);

        public RenderBundle Finish(string label)
        {
            return new RenderBundle(
                RenderBundleEncoderFinish(_impl, new RenderBundleDescriptor
                {
                    label = label
                })
            );
        }

        public void InsertDebugMarker(string markerLabel)
            => RenderBundleEncoderInsertDebugMarker(_impl, markerLabel);

        public void PushDebugGroup(string groupLabel)
            => RenderBundleEncoderPushDebugGroup(_impl, groupLabel);

        public void PopDebugGroup(string groupLabel) => RenderBundleEncoderPopDebugGroup(_impl);

        public unsafe void SetBindGroup(uint groupIndex, BindGroup group, uint[] dynamicOffsets)
        {
            RenderBundleEncoderSetBindGroup(_impl, groupIndex,
                group.Impl,
                (uint)dynamicOffsets.Length,
                ref Unsafe.AsRef<uint>((void*)Util.AllocHArray(dynamicOffsets))
            );
        }

        public void SetIndexBuffer(Buffer buffer, IndexFormat format, ulong offset, ulong size)
            => RenderBundleEncoderSetIndexBuffer(_impl, buffer.Impl, format, offset, size);

        public void SetPipeline(RenderPipeline pipeline) => RenderBundleEncoderSetPipeline(_impl, pipeline.Impl);

        public void SetVertexBuffer(uint slot, Buffer buffer, ulong offset, ulong size)
            => RenderBundleEncoderSetVertexBuffer(_impl, slot, buffer.Impl, offset, size);

    }
}
