using System.Numerics;
using BepuPhysics;
using BepuUtilities;

namespace MtgWeb.Core.Physics;

public struct PoseIntegratorCallbacks : IPoseIntegratorCallbacks
{
    public void Initialize(Simulation simulation)
    {
        throw new NotImplementedException();
    }

    public void PrepareForIntegration(float dt)
    {
        throw new NotImplementedException();
    }

    public void IntegrateVelocity(
        Vector<int> bodyIndices,
        Vector3Wide position,
        QuaternionWide orientation,
        BodyInertiaWide localInertia,
        Vector<int> integrationMask,
        int workerIndex,
        Vector<float> dt,
        ref BodyVelocityWide velocity
    )
    {
        throw new NotImplementedException();
    }

    public AngularIntegrationMode AngularIntegrationMode { get; }
    public bool AllowSubstepsForUnconstrainedBodies { get; }
    public bool IntegrateVelocityForKinematics { get; }
}