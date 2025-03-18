public class TrajectoryBundle
{
    public JumpTrajectory tilemap;
    public JumpTrajectory world;

    public TrajectoryBundle(JumpTrajectory tilemap, JumpTrajectory world)
    {
        this.tilemap = tilemap;
        this.world = world;
    }
}