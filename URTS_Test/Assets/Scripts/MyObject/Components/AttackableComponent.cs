using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MyObjectType { Light, Heavy, VHeavy };
public class AttackableComponent : MonoBehaviour
{
    public float currHealth;
    public float maxHealth = 5;

    public Renderer[] renderers;

    public GameObject OnDeathParticlesAndSound;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        currHealth = maxHealth;
        if (renderers != null) {
            float t = 1 - (float)currHealth / (float)maxHealth;

            if (renderers == null || renderers.Length <= 0)
                return;

            foreach (Renderer renderer in renderers) {
                renderer.material.color = Color.Lerp(Color.green, Color.red, t);
            }
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void ReceiveDamage(float damage) {
        if (currHealth <= 0)
            return;

        currHealth -= damage;
        ChangeColorOnHealth();

        if (currHealth <= 0) {
            foreach (Renderer renderer in renderers) {
                renderer.enabled = false;
            }
            if (OnDeathParticlesAndSound != null)
                Instantiate(OnDeathParticlesAndSound, transform.position, Quaternion.identity);
            if(audioSource != null) {
                audioSource.Play();
            }
            Destroy(gameObject, 0.1f);
        }
    }

    private void ChangeColorOnHealth() {
        float t = 1 - (float)currHealth / (float)maxHealth;

        //Debug.Log(Color.Lerp(Color.green, Color.green, t));
        if (renderers == null || renderers.Length <= 0)
            return;

        foreach (Renderer renderer in renderers) {
            renderer.material.color = Color.Lerp(Color.green, Color.red, t);
        }
    }
}
