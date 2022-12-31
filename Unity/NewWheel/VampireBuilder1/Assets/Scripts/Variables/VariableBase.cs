using System;
using UnityEngine;

public class VariableBase : ScriptableObject
{
    [NonSerialized] public VariableChangeEvent variableChangeEvent = new VariableChangeEvent();
}