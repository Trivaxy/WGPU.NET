using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static WGPU.NET.Wgpu;

namespace WGPU.NET
{
    public struct RequiredLimits
    {
        public Limits Limits;
    }

    public struct DeviceExtras
    {
        public NativeFeature NativeFeatures;
        public string Label;
        public string TracePath;
    }

    public class Adapter
    {
        internal AdapterImpl Impl;

        internal Adapter(AdapterImpl impl)
        {
            Impl = impl;
        }


        public unsafe FeatureName[] EnumerateFeatures()
        {
            FeatureName features = default;

            ulong size = AdapterEnumerateFeatures(Impl, ref features);

            var featuresSpan = new Span<FeatureName>(Unsafe.AsPointer(ref features), (int)size);

            FeatureName[] result = new FeatureName[size];

            featuresSpan.CopyTo(result);

            return result;
        }

        public bool GetLimits(out SupportedLimits limits)
        {
            limits = new SupportedLimits();

            return AdapterGetLimits(Impl, ref limits);
        }

        public void GetProperties(out AdapterProperties properties)
        {
            properties = new AdapterProperties();

            AdapterGetProperties(Impl, ref properties);
        }

        public bool HasFeature(FeatureName feature) => AdapterHasFeature(Impl, feature);

        public void RequestDevice(RequestDeviceCallback callback, QueueDescriptor defaultQueue = default, RequiredLimits ? limits = null, DeviceExtras? deviceExtras = null)
        {
            var _limits = limits.Value;

            AdapterRequestDevice(Impl, new DeviceDescriptor()
            {
                defaultQueue = defaultQueue,
                requiredLimits = Util.Optional(limits),
                nextInChain = deviceExtras==null ? IntPtr.Zero :
                new WgpuStructChain()
                .Add_DeviceExtras(
                    deviceExtras.Value.NativeFeatures, 
                    deviceExtras.Value.Label, 
                    deviceExtras.Value.TracePath)
                .GetPointer()
            }, 
            (s,d,m,_) => callback(s,new Device(d),m), IntPtr.Zero);
        }
    }

    public delegate void RequestDeviceCallback(RequestDeviceStatus status, Device device, string message);
}
