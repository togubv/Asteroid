using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    [SerializeField] private Transform opposite;
    [SerializeField] private BorderAxis borderAxis;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("UFO"))
        {
            return;
        }

        if (borderAxis == BorderAxis.x)
        {
            if (transform.position.x > 0)
            {
                collider.gameObject.transform.position = new Vector2(opposite.position.x + (collider.bounds.size.x + 0.5f), collider.gameObject.transform.position.y);
                return;
            }

            collider.gameObject.transform.position = new Vector2(opposite.position.x - (collider.bounds.size.x + 0.5f), collider.gameObject.transform.position.y);
            return;
        }

        if (transform.position.y > 0)
        {
            collider.gameObject.transform.position = new Vector2(collider.gameObject.transform.position.x, opposite.position.y + (collider.bounds.size.y + 0.5f));
            return;
        }

        collider.gameObject.transform.position = new Vector2(collider.gameObject.transform.position.x, opposite.position.y - (collider.bounds.size.y + 0.5f));
        return;
    }
}