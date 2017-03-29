using CoreGraphics;
using Microsoft.HealthVault.Thing;

namespace SandboxIos
{
    public interface IThingCell
    {
        void SetThing(IThing thing);

        CGSize GetSize(IThing thing);
    }
}
