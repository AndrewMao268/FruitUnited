public class RandomizedParallax : BackgroundParallax
{
    // lower = less random, higher = more random
    public int randomness;
    private int randomFactor;

    void Start()
    {
        SetRandomFactor();
        OwnStart();
    }

    override public void SetRandomFactor()
    {
        System.Random random = new System.Random();
        randomFactor = random.Next(-randomness, randomness);
    }

    public override float GetRandomFactor()
    {
        return randomFactor;
    }
}