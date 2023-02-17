using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillStage1 : MonoBehaviour
{
    public SkillId skillId;

    private void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = ColorUtility.GetColorForString(skillId.ToString());
    }
}
