using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace WGPU.NET
{
    internal static class Util
    {
		public static IntPtr AllocHStruct<T>(T structure)
			where T : struct
		{
			IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(structure));
			Marshal.StructureToPtr(structure, ptr, false);

			return ptr;
		}

		public unsafe static IntPtr AllocHArray(byte[] arr)
		{
			IntPtr ptr = Marshal.AllocHGlobal(arr.Length);

			Span<byte> span = new Span<byte>((void*)ptr, arr.Length);

			new Span<byte>(arr).CopyTo(span);

			return ptr;
		}

		public unsafe static IntPtr AllocHArray<T>(T[] arr)
			where T : struct
		{
			int size = Marshal.SizeOf<T>();

			IntPtr ptr = Marshal.AllocHGlobal(size * arr.Length);

			Span<T> span = new Span<T>((void*)ptr, arr.Length);

            for (int i = 0; i < arr.Length; i++)
            {
				span[i] = arr[i];
            }

			return ptr;
        }

		public unsafe static IntPtr AllocHArray<T>(int count, IEnumerable<T> items)
			where T : struct
		{
			int size = Marshal.SizeOf<T>();

			IntPtr ptr = Marshal.AllocHGlobal(size * count);

			Span<T> span = new Span<T>((void*)ptr, count);

			int i = 0;

            foreach (var item in items)
            {
				span[i] = item;

				i++;
			}

			if (i != count)
				throw new ArgumentException($"{nameof(count)} is larger then the supplied enumerable");

			return ptr;
		}

		public static IntPtr Optional<T>(T? optional)
			where T : struct
        {
			if (optional == null) return IntPtr.Zero;

			return AllocHStruct(optional.Value);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key)
		where TValue : new()
		{
			if (self.TryGetValue(key, out TValue value))
			{
				return value;
			}

			TValue newVal = new TValue();

			self[key] = newVal;

			return newVal;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key, Func<TValue> creator)
		{
			if (self.TryGetValue(key, out TValue value))
			{
				return value;
			}

			TValue newVal = creator();

			self[key] = newVal;

			return newVal;
		}
	}
}
