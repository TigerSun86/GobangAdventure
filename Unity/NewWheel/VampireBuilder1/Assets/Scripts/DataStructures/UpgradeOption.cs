using System;

[Serializable]
public class UpgradeOption
{
    public string levelText;

    public string upgradeName;

    public string description;

    public virtual void TriggerUpgrade()
    {
    }
}