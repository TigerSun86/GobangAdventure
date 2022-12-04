using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class PositionUtilities
{
    public static T FindClosest<T>(Vector3 position, IEnumerable<T> positionHolders, Func<T, bool> isValidFunc, Func<T, Vector3> getPositionFunc)
    {
        T closest = default;
        float minDistance = float.MaxValue;
        foreach (T positionHolder in positionHolders)
        {
            if (!isValidFunc.Invoke(positionHolder))
            {
                continue;
            }

            float currentDistance = Vector3.Distance(position, getPositionFunc.Invoke(positionHolder));
            if (currentDistance < minDistance)
            {
                closest = positionHolder;
                minDistance = currentDistance;
            }
        }

        return closest;
    }

    public static T FindRandom<T>(IEnumerable<T> positionHolders, Func<T, bool> isValidFunc, Func<T, Vector3> getPositionFunc)
    {
        List<T> list = positionHolders.Where(o => isValidFunc.Invoke(o)).ToList();
        if (!list.Any())
        {
            return default;
        }

        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }
}
