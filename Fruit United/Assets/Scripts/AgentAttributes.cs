public class AgentAttributes
{
    public float height = 1;
    public float width = 0.3f;

    public float jumpHeight = 3.0f;
    public float minAccel = 1.0f;
    public float jumpA = -4.89161183078935f;
    public float jumpB = 7.71168716787364f;

    public AgentAttributes()
    {
        this.height = 1.0f;
        this.width = 1.0f;

        this.jumpHeight = 3.0f;
        this.minAccel = 10.0f;
        this.jumpA = -5.0f;
        this.jumpB = 8.0f;
    }
    public AgentAttributes(float height, float width, float jumpHeight, float minAccel, float jumpA, float jumpB)
    {
        this.height = height;
        this.width = width;
        this.jumpHeight = jumpHeight;
        this.minAccel = minAccel;
        this.jumpA = jumpA;
        this.jumpB = jumpB;
    }

    public bool isSame(AgentAttributes other)
    {
        if (other == null) return false;

        return this.height == other.height &&
            this.width == other.width &&
            this.jumpHeight == other.jumpHeight &&
            this.minAccel == other.minAccel &&
            this.jumpA == other.jumpA &&
            this.jumpB == other.jumpB;
    }
}