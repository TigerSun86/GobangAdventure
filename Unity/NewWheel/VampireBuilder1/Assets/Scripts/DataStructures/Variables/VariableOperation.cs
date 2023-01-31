using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/VariableOperation")]
public class VariableOperation : ScriptableObject
{
    public OperationType operationType;

    public static ScriptableObject[] Evaluate(ScriptableObject[] parameters)
    {
        OperationEvaluator operationEvaluator = new OperationEvaluator(parameters);
        return operationEvaluator.Evaluate();
    }

    private class OperationEvaluator
    {
        private ScriptableObject[] parameters;

        private int index;

        public OperationEvaluator(ScriptableObject[] parameters)
        {
            this.parameters = parameters;
            this.index = 0;
        }

        public ScriptableObject[] Evaluate()
        {
            List<ScriptableObject> result = new List<ScriptableObject>();
            while (index < parameters.Length)
            {
                ScriptableObject obj = parameters[index];
                index++;

                if (obj is VariableOperation)
                {
                    result.Add(EvaluateOperation((obj as VariableOperation).operationType));
                }
                else
                {
                    result.Add(obj);
                }
            }

            return result.ToArray();
        }

        private FloatVariable EvaluateOperation(OperationType operationType)
        {
            List<FloatVariable> tempVariables = new List<FloatVariable>();
            try
            {
                FloatVariable num1 = null;
                while (index < parameters.Length)
                {
                    ScriptableObject obj = parameters[index];
                    index++;

                    FloatVariable currentNum;
                    if (obj is VariableOperation)
                    {
                        currentNum = EvaluateOperation((obj as VariableOperation).operationType);
                    }
                    else if (obj is FloatVariable)
                    {
                        currentNum = obj as FloatVariable;
                    }
                    else if (obj is IntVariable)
                    {
                        currentNum = ScriptableObject.CreateInstance<FloatVariable>();
                        tempVariables.Add(currentNum);
                        currentNum.SetValue((obj as IntVariable).value);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Invalid type {obj.GetType()}");
                    }

                    if (num1 == null)
                    {
                        num1 = currentNum;
                    }
                    else
                    {
                        return CalculateTwoNums(operationType, num1, currentNum);
                    }
                }

                throw new InvalidOperationException($"The input has not sufficient variables to evaluate operations");
            }
            finally
            {
                foreach (FloatVariable variable in tempVariables)
                {
                    Destroy(variable);
                }
            }
        }

        private FloatVariable CalculateTwoNums(OperationType operationType, FloatVariable num1, FloatVariable num2)
        {
            FloatVariable result = ScriptableObject.CreateInstance<FloatVariable>();
            try
            {
                switch (operationType)
                {
                    case OperationType.ADDITION:
                        result.SetValue(num1.value + num2.value);
                        break;
                    case OperationType.SUBSTRACTION:
                        result.SetValue(num1.value - num2.value);
                        break;
                    case OperationType.MUTIPLICATION:
                        result.SetValue(num1.value * num2.value);
                        break;
                    case OperationType.DIVISION:
                        if (num2.value == 0)
                        {
                            throw new DivideByZeroException();
                        }

                        result.SetValue(num1.value / num2.value);
                        break;
                    default:
                        throw new InvalidEnumArgumentException(nameof(operationType), (int)operationType, typeof(OperationType));
                }

                return result;
            }
            finally
            {
                Destroy(result);
            }
        }
    }
}