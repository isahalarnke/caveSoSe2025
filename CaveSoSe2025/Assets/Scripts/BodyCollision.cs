using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCollision : MonoBehaviour
{
    private ParticleSystem particleEffectPrefab;
    private AudioClip collisionSound;

    public void SetParticleEffect(ParticleSystem ps)
    {
        particleEffectPrefab = ps;
    }
    public void SetCollisionSound(AudioClip clip)
    {
        collisionSound = clip;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log($"{gameObject.name} hat {collision.gameObject.name} berührt");
        if (gameObject.tag == "Hand" && collision.gameObject.tag == "Hand"){
           if (collision.contacts.Length > 0)
            {
                Vector3 contactPoint = collision.contacts[0].point;
                CreateBasicParticleEffect(contactPoint);
                PlayCollisionSound(contactPoint);
                Debug.Log("Klatschen");
            }
        }
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
           //rend.material.color = Color.red;
        }

    }
    void CreateBasicParticleEffect(Vector3 position)
    {
        if (particleEffectPrefab == null)
        {
            Debug.LogWarning("Particle system prefab not set.");
            return;
        }
       
        ParticleSystem instance = Instantiate(particleEffectPrefab, position, Quaternion.identity);
        instance.Play();
        Debug.Log("Partikelsystem play");
        Destroy(instance.gameObject, instance.main.startLifetime.constant + 0.2f);

        //Variante mit, wenn eigenes Partikelsystem erstellt wird --> besser von außen zu übergeben, mehr Designfreiheit
        /*GameObject psObj = new GameObject("CollisionParticles");
        psObj.transform.position = position;

        ParticleSystem ps = psObj.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.startColor = Color.blue;
        main.startSize = 0.1f;
        main.startLifetime = 0.5f;
        main.duration = 0.8f;
        main.loop = false;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 30)
        });

        ps.Play();
        //particleSystem.Play();

        Destroy(psObj, main.startLifetime.constant + 0.1f);*/
    }
    void PlayCollisionSound(Vector3 position)
    {
        if (collisionSound == null) return;

        GameObject soundObj = new GameObject("CollisionSound");
        soundObj.transform.position = position;

        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        audioSource.clip = collisionSound;
        audioSource.Play();

        Destroy(soundObj, collisionSound.length + 0.1f);
    }
}
