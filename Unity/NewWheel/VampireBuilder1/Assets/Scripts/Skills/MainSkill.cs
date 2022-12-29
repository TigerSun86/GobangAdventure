using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class MainSkill : ScriptableObject, ISerializationCallbackReceiver
{
    public string description;

    public float defaultAttack;

    public float defaultCriticalRate;

    public float defaultCriticalAmount;

    public float defaultArea;

    public float defaultAttackDecrease;

    public float attack;

    public float criticalRate;

    public float criticalAmount;

    public float area;

    public float attackDecrease;

    public List<SubSkill> subSkills;

    public void Reset()
    {
        attack = defaultAttack;
        criticalRate = defaultCriticalRate;
        criticalAmount = defaultCriticalAmount;
        area = defaultArea;
        attackDecrease = defaultAttackDecrease;

        foreach (SubSkill subSkill in subSkills)
        {
            subSkill.SetMainSkill(this);
        }
    }

    public void OnAfterDeserialize()
    {
        SceneManager.sceneLoaded += (a, b) => Reset();
        EditorApplication.playModeStateChanged += (a) => Reset();
    }

    public void OnBeforeSerialize()
    {
    }
}