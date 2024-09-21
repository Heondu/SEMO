using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBounds : MonoBehaviour
{
    [SerializeField]
    private Bounds bounds;

    public void SetTargetInBounds(Transform target, Vector3 velocity, TrailRenderer trail)
    {
        if (trail != null)
        {
            trail.gameObject.SetActive(true);
        }

        if (!bounds.Contains(target.position))
        {
            if (trail != null)
            {
                trail.Clear();
                trail.gameObject.SetActive(false);
            }

            Vector3 curPos = target.position;

            float centerX = bounds.center.x;
            float centerY = bounds.center.y;

            float xLeft = centerX - bounds.extents.x;
            float xRight = centerX + bounds.extents.x;
            float yDown = centerY - bounds.extents.y;
            float yUp = centerY + bounds.extents.y;

            //Vector3 newPos = curPos;

            //if (velocity == Vector3.zero)
            //{
            //    newPos.x = (Random.Range(0, 2) == 0 ? xLeft : xRight);
            //    newPos.y = (Random.Range(0, 2) == 0 ? yDown : yUp);
            //}
            //else
            //{
            //    if (velocity.x == 0)
            //    {
            //        //newPos.x = curPos.x;
            //    }
            //    else
            //    {
            //        newPos.x = (velocity.x > 0 ? xLeft : xRight);
            //    }

            //    if (velocity.y == 0)
            //    {
            //        //newPos.x = curPos.x;
            //    }
            //    else
            //    {
            //        newPos.y = (velocity.y > 0 ? yDown : yUp);
            //    }
            //}
            //Vector3 newPos = new Vector3((velocity.x == 0 ? curPos.x : (velocity.x > 0 ? xLeft : xRight)), (velocity.y == 0 ? curPos.y : (velocity.y > 0 ? yDown : yUp)), curPos.z);
            Vector3 newPos = new Vector3(bounds.center.x + (curPos.x > 0 ? -bounds.extents.x : bounds.extents.x), bounds.center.y + Random.Range(-bounds.extents.y, bounds.extents.y));
            target.position = newPos;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(bounds.center.x, bounds.center.y), new Vector3(bounds.extents.x * 2, bounds.extents.y * 2, 0));
    }
}
