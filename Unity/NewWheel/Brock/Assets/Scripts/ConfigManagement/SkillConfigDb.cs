using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class SkillConfigDb
{

    [SerializeField]
    private StringToSkillConfigDictionary skillConfigMap;

    public SkillConfigDb(List<SkillConfig> skillConfigs)
    {
        this.skillConfigMap = new StringToSkillConfigDictionary();
        foreach (SkillConfig skill in skillConfigs)
        {
            if (this.skillConfigMap.ContainsKey(skill.GetId()))
            {
                Debug.LogWarning($"Duplicate skill config found: {skill.GetId()}. Skipping.");
                continue;
            }

            LinkResources(skill);

            this.skillConfigMap.Add(skill.GetId(), skill);

            // JsonSerializerSettings settings = new JsonSerializerSettings
            // {
            //     Converters = { new StringEnumConverter() }
            // };
            // string json = JsonConvert.SerializeObject(skill, Formatting.Indented, settings);
            // System.IO.Directory.CreateDirectory("SerializedSkills");
            // System.IO.File.WriteAllText($"SerializedSkills\\{key}.json", json);
        }
    }

    public SkillConfig Get(string id)
    {
        if (skillConfigMap.TryGetValue(id, out SkillConfig skill))
        {
            return skill;
        }

        Debug.LogWarning($"Skill config not found for id: {id}");
        return null;
    }

    private void LinkResources(SkillConfig skillConfig)
    {
        // Link skill event's modifiers.
        foreach (ActionConfig actionConfig in GetAllActionConfigs(skillConfig))
        {
            switch (actionConfig)
            {
                case ApplyAuraModifierActionConfig aac:
                    AuraModifierConfig auraModifierConfig = GetModifierConfig(skillConfig, aac.modifierId) as AuraModifierConfig;
                    if (auraModifierConfig == null)
                    {
                        Debug.LogWarning($"AuraModifierConfig wrong type for id: {aac.modifierId} in skill: {skillConfig.GetId()}");
                    }

                    aac.modifierConfig = auraModifierConfig;
                    break;
                case ApplyModifierActionConfig amc:
                    amc.modifierConfig = GetModifierConfig(skillConfig, amc.modifierId);
                    break;
                case RemoveModifierActionConfig config:
                    config.modifierConfig = GetModifierConfig(skillConfig, config.modifierId);
                    break;
                case LinearProjectileActionConfig lpc:
                    lpc.sprite = ParserUtility.ParseSpriteSafe(lpc.spritePath, "spritePath");
                    break;
                default:
                    break;
            }
        }

        // Link modifiers' modifiers.
        foreach (AuraModifierConfig amc in skillConfig.modifierConfigs?.Values
            .OfType<AuraModifierConfig>()
            ?? Enumerable.Empty<AuraModifierConfig>())
        {
            if (string.IsNullOrEmpty(amc.childModifierId))
            {
                Debug.LogWarning($"AuraModifierConfig {amc.id} in skill: {skillConfig.GetId()} has empty childModifierId");
                continue;
            }

            amc.childModifierConfig = GetModifierConfig(skillConfig, amc.childModifierId);
        }

        // Preload buff icons.
        foreach (ModifierConfig mc in skillConfig.modifierConfigs?.Values
            .Where(m => m.buffType != BuffType.None)
            ?? Enumerable.Empty<ModifierConfig>())
        {
            mc.buffIconSprite = ParserUtility.ParseSpriteSafe(mc.buffIcon, "buffIcon");
        }
    }

    private ModifierConfig GetModifierConfig(SkillConfig skillConfig, string modifierId)
    {
        if (skillConfig.modifierConfigs?.TryGetValue(modifierId, out ModifierConfig config) == true)
        {
            return config;
        }

        Debug.LogWarning($"Modifier config not found for id: {modifierId} in skill: {skillConfig.GetId()}");
        return null;
    }

    private IEnumerable<ActionConfig> GetAllActionConfigs(SkillConfig skillConfig)
    {
        return skillConfig.events?.Values
            .SelectMany(a => a)
            ?? Enumerable.Empty<ActionConfig>();
    }
}
