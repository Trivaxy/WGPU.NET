using System;
using System.Collections.Generic;
using System.Text;

namespace WGPU.NET
{
    public class HandleDroppedOrDestroyedException : Exception
    {
        public HandleDroppedOrDestroyedException(string resourceType)
            : base($"Handle of this {resourceType} has been dropped " +
                  $"or it's resources have been destroyed" +
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

    public class TextureDoesNotOwnViewException : Exception
    {
        public TextureDoesNotOwnViewException(string textureName)
            : base($"The texture {textureName} does not own the texture view")
        {
            
        }
    }
}
