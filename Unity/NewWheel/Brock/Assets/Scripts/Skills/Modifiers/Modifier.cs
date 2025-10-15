using System;

[Serializable]
public class Modifier
{
    public ModifierConfig config;

    public float duration;

    public float interval;

    public Modifier(ModifierConfig config)
    {
        this.config = config;
        this.duration = config.duration;
        this.interval = config.interval;
    }

    // Returns true if the modifier is still active.
    public bool Tick(float deltaTime)
    {
        if (!this.config.isPermanent)
        {
            // Decrease duration only if the modifier is not permanent.
            this.duration -= deltaTime;
            if (this.duration <= 0)
            {
                return false;
            }
        }

        if (this.config.interval > 0)
        {
            this.interval -= deltaTime;
            if (this.interval <= 0)
            {
                this.interval = this.config.interval;
                // TODO: Trigger interval event.
            }
        }

        return true;
    }
}