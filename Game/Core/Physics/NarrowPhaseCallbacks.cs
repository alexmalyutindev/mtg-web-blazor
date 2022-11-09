using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;

namespace MtgWeb.Core.Physics;

public struct NarrowPhaseCallbacks : INarrowPhaseCallbacks
{
    public void Initialize(Simulation simulation)
    {
        throw new NotImplementedException();
    }

    public bool AllowContactGeneration(
        int workerIndex,
        CollidableReference a,
        CollidableReference b,
        ref float speculativeMargin
    )
    {
        throw new NotImplementedException();
    }

    public bool ConfigureContactManifold<TManifold>(
        int workerIndex,
        CollidablePair pair,
        ref TManifold manifold,
        out PairMaterialProperties pairMaterial
    ) where TManifold : unmanaged, IContactManifold<TManifold>
    {
        throw new NotImplementedException();
    }

    public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
    {
        throw new NotImplementedException();
    }

    public bool ConfigureContactManifold(
        int workerIndex,
        CollidablePair pair,
        int childIndexA,
        int childIndexB,
        ref ConvexContactManifold manifold
    )
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}