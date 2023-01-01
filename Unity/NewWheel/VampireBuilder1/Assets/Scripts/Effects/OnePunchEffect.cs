using UnityEngine;

public class OnePunchEffect : MonoBehaviour
{
    [SerializeField] float moveSpeed;

    private void Update()
    {
        this.transform.position += Vector3.right * moveSpeed;

        Color tmp = GetComponent<SpriteRenderer>().color;
        tmp.a -= Time.deltaTime;
        GetComponent<SpriteRenderer>().color = tmp;
        if (tmp.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
