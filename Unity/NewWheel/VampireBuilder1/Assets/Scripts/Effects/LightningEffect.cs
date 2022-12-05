using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LightningEffect : MonoBehaviour
{
    [SerializeField] float timeToLive = 0.5f;

    Vector2 start;

    Vector2 end;

    int zigzagCount = 30;

    float maxStrength = 0.05f;

    float minStrength = 0.01f;

    LineRenderer lineRenderer;

    public void SetPositions(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
    }

    // Start is called before the first frame update
    void Start()
    {
        List<Vector2> positions = new List<Vector2>();
        Vector2 direction = end - start;
        Vector2 normalizedPerpendicular = direction.normalized.Perpendicular1();
        Vector2 directionSection = direction / (zigzagCount + 1);
        positions.Add(start);
        for (int i = 0; i < zigzagCount; i++)
        {
            float factor = Random.Range(minStrength, maxStrength);
            factor = Random.value > 0.5 ? factor : -factor;
            Vector2 sideOffset = normalizedPerpendicular * factor;
            Vector2 position = start + (directionSection * (i + 1)) + sideOffset;
            positions.Add(position);
        }

        positions.Add(end);
        Vector3[] positionArray = positions.Select(o => (Vector3)o).ToArray();
        lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.positionCount = positionArray.Length;
        lineRenderer.SetPositions(positionArray);
    }

    // Update is called once per frame
    void Update()
    {
        timeToLive -= Time.deltaTime;
        if (timeToLive <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
