using UnityEngine;

[CreateAssetMenu]
public class MainSkillRuntimeSet : RuntimeSet<MainSkill>
{
    public void Clear()
    {
        for (int i = Items.Count - 1; i >= 0; i--)
        {
            MainSkill mainSkill = Items[i];
            mainSkill.Disable();
        }

        NotifyChanged();
    }
}
