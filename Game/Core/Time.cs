namespace MtgWeb.Core;

public static class Time
{
    public static float CurrentTime { get; private set; }
    public static float DeltaTime { get; private set; }
    public static float LastUpdateTime { get; private set; }

    public static void Tick(float deltaTime)
    {
        DeltaTime = deltaTime;
        LastUpdateTime = CurrentTime;
        CurrentTime += DeltaTime;
    }
    
}