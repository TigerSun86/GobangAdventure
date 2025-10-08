using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BuffTracker : MonoBehaviour
{
    [SerializeField, AssignedInCode]
    private List<Buff> activeBuffs;

    [SerializeField]
    private UnityEvent<GameObject, Buff> onTakeBuff;

    public void Add(Buff buff)
    {
        buff.startTime = Time.time;
        activeBuffs.Add(buff);
        StartCoroutine(RemoveBuffAfterDuration(buff));
        onTakeBuff.Invoke(this.gameObject, buff);
    }

    public bool Contains(BuffType buffType)
    {
        return activeBuffs.Any(buff => buff.buffType == buffType);
    }

    public IEnumerable<Buff> Get(BuffType buffType)
    {
        return activeBuffs.Where(buff => buff.buffType == buffType);
    }

    public IEnumerable<Buff> GetAll()
    {
        return activeBuffs;
    }

    public void Remove(Buff buff)
    {
        activeBuffs.Remove(buff);
    }

    private IEnumerator RemoveBuffAfterDuration(Buff buff)
    {
        if (buff.duration != float.PositiveInfinity)
        {
            yield return new WaitForSeconds(buff.duration);
            Remove(buff);
        }
    }
}