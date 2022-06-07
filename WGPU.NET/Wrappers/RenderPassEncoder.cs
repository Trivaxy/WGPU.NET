using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public class RenderPassEncoder
    {
        internal RenderPassEncoderImpl Impl;

        internal RenderPassEncoder(RenderPassEncoderImpl impl)
        {
            Impl = impl;
        }

        public void BeginOcclusionQuery(uint queryIndex)
            => RenderPassEncoderBeginOcclusionQuery(Impl, queryIndex);

        public void BeginPipelineStatisticsQuery(QuerySet querySet, uint queryIndex)
            => RenderPassEncoderBeginPipelineStatisticsQuery(Impl, querySet.Impl, queryIndex);

        public void Draw(uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance)
            => RenderPassEncoderDraw(Impl, vertexCount, instanceCount, firstVertex, firstInstance);

        public void DrawIndexed(uint indexCount, uint instanceCount, uint firstIndex, int baseVertex, uint firstInstance)
            => RenderPassEncoderDrawIndexed(Impl, indexCount, instanceCount, firstIndex, baseVertex, firstInstance);

        public void DrawIndexedIndirect(Buffer indirectBuffer, ulong indirectOffset)
            => RenderPassEncoderDrawIndexedIndirect(Impl, indirectBuffer.Impl, indirectOffset);

        public void DrawIndirect(Buffer indirectBuffer, ulong indirectOffset)
            => RenderPassEncoderDrawIndirect(Impl, indirectBuffer.Impl, indirectOffset);

        public void End() => RenderPassEncoderEnd(Impl);

        public void EndOcclusionQuery() => RenderPassEncoderEndOcclusionQuery(Impl);

        public void EndPipelineStatisticsQuery() => RenderPassEncoderEndPipelineStatisticsQuery(Impl);

        public unsafe void ExecuteBundles(RenderBundle[] bundles)
        {
            RenderPassEncoderExecuteBundles(Impl, (uint)bundles.Length,
                ref Unsafe.AsRef<RenderBundleImpl>(
                    (void*)Util.AllocHArray(bundles.Length, bundles.Select(x => x.Impl))
                )
            );
        }

        public void InsertDebugMarker(string markerLabel)
            => RenderPassEncoderInsertDebugMarker(Impl, markerLabel);

        public void PushDebugGroup(string groupLabel)
            => RenderPassEncoderPushDebugGroup(Impl, groupLabel);

        public void PopDebugGroup(string groupLabel) => RenderPassEncoderPopDebugGroup(Impl);

        public unsafe void SetBindGroup(uint groupIndex, BindGroup group, uint[] dynamicOffsets)
        {
            RenderPassEncoderSetBindGroup(Impl, groupIndex,
                group.Impl,
                (uint)dynamicOffsets.Length,
                ref Unsafe.AsRef<uint>((void*)Util.AllocHArray(dynamicOffsets))
            );
        }

        public void SetBlendConstant(in Color color) => RenderPassEncoderSetBlendConstant(Impl, color);

        public void SetIndexBuffer(Buffer buffer, IndexFormat format, ulong offset, ulong size)
            => RenderPassEncoderSetIndexBuffer(Impl, buffer.Impl, format, offset, size);

        public void SetPipeline(RenderPipeline pipeline) => RenderPassEncoderSetPipeline(Impl, pipeline.Impl);

        public unsafe void SetPushConstants<T>(ShaderStage stages, uint offset, ReadOnlySpan<T> data)
            where T : unmanaged
        {
            RenderPassEncoderSetPushConstants(
                           Impl, (uint)stages, offset, (uint)(data.Length * sizeof(T)),
                           (IntPtr)Unsafe.AsPointer(ref MemoryMarshal.GetReference(data))
                       );
        }

        public void SetScissorRect(uint x, uint y, uint width, uint height)
            => RenderPassEncoderSetScissorRect(Impl, x, y, width, height);

        public void SetStencilReference(uint reference) => RenderPassEncoderSetStencilReference(Impl, reference);

        public void SetVertexBuffer(uint slot, Buffer buffer, ulong offset, ulong size)
            => RenderPassEncoderSetVertexBuffer(Impl, slot, buffer.Impl, offset, size);

        public void SetViewport(float x, float y, float width, float height, float minDepth, float maxDepth)
            => RenderPassEncoderSetViewport(Impl, x, y, width, height, minDepth, maxDepth);
    }
}
