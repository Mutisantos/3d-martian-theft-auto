using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Script para que los enemigos hagan drops de objetos consumibles con cierto valor.
 * Esteban.Hernandez
 */
public class EnemyDropper : MonoBehaviour
{

    public Consumable DropPrefab;
    public int CustomAmount;

    private void OnDestroy()
    {
        Consumable instance = Instantiate(DropPrefab);
        instance.transform.position = gameObject.transform.position + Vector3.up * 5;
        instance.name = instance.name.Replace("(Clone)", "");
        instance.amount = CustomAmount;
    }
}
