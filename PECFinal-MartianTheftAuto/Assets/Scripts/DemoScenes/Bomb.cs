using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public void Start()
    {
        Explode();
    }

    void Explode()
    {
        ParticleSystem explosion = GetComponent<ParticleSystem>();
        explosion.Play();
        Destroy(this.gameObject, explosion.main.duration);
    }

}
