using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackData
{
    public float attackBase;

    public AttackOption attackOption;

    public AttackData(float attackBase, AttackOption attackOption = AttackOption.None)
    {
        this.attackBase = attackBase;
        this.attackOption = attackOption;
    }
}