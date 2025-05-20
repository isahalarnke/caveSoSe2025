using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCollision : MonoBehaviour
{
   
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{gameObject.name} hat {collision.gameObject.name} berührt");
        if (gameObject.tag == "Hand" && collision.gameObject.tag == "Hand"){
           if (collision.contacts.Length > 0)
            {
                Vector3 contactPoint = collision.contacts[0].point;
                CreateBasicParticleEffect(contactPoint);
            }
        }
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.red;
        }

    }
    void CreateBasicParticleEffect(Vector3 position)
    {
        GameObject psObj = new GameObject("CollisionParticles");
        psObj.transform.position = position;

        ParticleSystem ps = psObj.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.startColor = Color.red;
        main.startSize = 0.2f;
        main.startLifetime = 0.5f;
        main.duration = 0.5f;
        main.loop = false;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 30)
        });

        ps.Play();

        Destroy(psObj, main.startLifetime.constant + 0.1f);
    }
}
