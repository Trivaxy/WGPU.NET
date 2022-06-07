using System;
using System.Collections.Generic;
using System.Text;

namespace WGPU.NET
{
    public class HandleDestroyedException : Exception
    {
        public HandleDestroyedException(string resourceType)
            : base($"Handle of this {resourceType} has been destroyed" +
                  $"therefore it can't be used anymore")
        {

        }
    }

    public class ResourceCreationError : Exception
    {
        public ResourceCreationError(string resourceType)
            : base($"There has been an error creating this {resourceType}, handle was null")
        {

        }
    }
}
