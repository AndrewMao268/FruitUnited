public class StabilizedParallax : BackgroundParallax
{

    void Start()
    {
        SetRandomFactor();
        OwnStart();
    }

    override public void SetRandomFactor() {}

    override public float GetRandomFactor()
    {
        return 0.0f;
    }
}