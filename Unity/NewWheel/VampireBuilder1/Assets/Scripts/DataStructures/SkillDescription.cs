using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillDescription
{
    public int level;

    public string format;

    public ScriptableObject[] parameters;

    public override string ToString()
    {
        ScriptableObject[] evaluatedParameters = VariableOperation.Evaluate(parameters);
        return string.Format(format, evaluatedParameters);
    }
}