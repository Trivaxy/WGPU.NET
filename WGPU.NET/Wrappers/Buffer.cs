using System;
using System.Runtime.InteropServices;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public class Buffer
    {
        private BufferImpl _impl;

        internal BufferImpl Impl 
        {
            get
            {
                if (_impl.Handle == IntPtr.Zero)
                    throw new HandleDroppedOrDestroyedException(nameof(Buffer));

                return _impl;
            }

            private set => _impl = value;
        }

        public ulong SizeInBytes { get; private set; }

        internal Buffer(BufferImpl impl, in BufferDescriptor descriptor)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(Buffer));

            Impl = impl;

            SizeInBytes = descriptor.size;
        }

        public unsafe Span<T> GetConstMappedRange<T>(ulong offset, int size)
            where T : unmanaged
        {
            var structSize = (ulong)Marshal.SizeOf<T>();

            void* ptr = (void*)BufferGetConstMappedRange(Impl, 
                offset * structSize, (ulong)size * structSize);

            return new Span<T>(ptr, size);
        }

        public unsafe Span<T> GetMappedRange<T>(ulong offset, int size)
            where T : unmanaged
        {
            var structSize = (ulong)Marshal.SizeOf<T>();

            void* ptr = (void*)BufferGetMappedRange(Impl,
                offset * structSize, (ulong)size * structSize);

            return new Span<T>(ptr, size);
        }

        public void MapAsync(MapMode mode, ulong offset, ulong size, BufferMapCallback callback)
            => BufferMapAsync(Impl, (uint)mode, offset, size, (s,_) => callback(s), IntPtr.Zero);

        public void Unmap() => BufferUnmap(Impl);

        /// <summary>
        /// Destroys the GPU Resource associated to this <see cref="Buffer"/>
        /// </summary>
        public void DestroyResource()
        {
            BufferDestroy(Impl);
            Impl = default;
        }
        
        /// <summary>
        /// Signals to the underlying rust API that this <see cref="Buffer"/> isn't used anymore
        /// </summary>
        public void FreeHandle()
        {
            BufferDrop(Impl);
            Impl = default;
        }
    }

    public delegate void BufferMapCallback(BufferMapAsyncStatus status);

    public static partial class BufferExtensions
    {
        public static void SetData<T>(this Buffer buffer, ulong offset, ReadOnlySpan<T> span)
            where T : unmanaged
        {
            var dest = buffer.GetMappedRange<T>(offset, span.Length);

            span.CopyTo(dest);
        }
    }
}
