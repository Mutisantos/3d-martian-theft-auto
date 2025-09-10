using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/** Define un asistente para apuntar al enemigo que esté mas cerca del rango de visión del jugador
 * Esteban.Hernandez
 */
public class AimHelper : MonoBehaviour
{
    public float AimSpeed = 10.0f;
    [SerializeField]
    private List<GameObject> _collidedObjects;


    // Start is called before the first frame update
    void Awake()
    {
        _collidedObjects = new List<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {
        if (_collidedObjects.Count > 0)
        {
            LookAtClosestTargetInSight();
        }
    }

    private void LookAtClosestTargetInSight()
    {
        var closestPlayerCollider = GetClosestPlayerCollider();
        if (closestPlayerCollider != null)
        {
            var direction = closestPlayerCollider.transform.position - transform.position;
            var lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * AimSpeed);
        }
    }


    private GameObject GetClosestPlayerCollider()
    {
       
        var closestCollider = _collidedObjects[0];
        if(closestCollider == null)
        {
            _collidedObjects.RemoveAt(0);
            return null;
        }
        var closestDistance = Vector3.Distance(transform.position, closestCollider.transform.position);

        var tempPlayerColliderList = new List<GameObject>(_collidedObjects);

        foreach (var playerCollider in _collidedObjects)
        {
            if (!playerCollider.gameObject.activeSelf)
            {
                tempPlayerColliderList.Remove(playerCollider);
                continue;
            }
            var distance = Vector3.Distance(transform.position, playerCollider.transform.position);
            if (!(distance < closestDistance))
                continue;
            closestDistance = distance;
            closestCollider = playerCollider;
        }
        _collidedObjects = tempPlayerColliderList;
        return closestCollider;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Enter: {other.name} + {other.tag}");
        if (other.CompareTag("Enemy"))
        {
            _collidedObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log($"Exit: {other.name} + {other.tag}");
        if (other.CompareTag("Enemy"))
        {
            _collidedObjects.Remove(other.gameObject);
        }
    }


}
