using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MyObjectType { Light, Heavy, VHeavy };
public class AttackableComponent : MonoBehaviour
{
    public float currHealth;
    public float maxHealth = 5;

    public Renderer[] renderers;

    // Start is called before the first frame update
    void Start()
    {
        currHealth = maxHealth;
        if (renderers != null) {
            float t = 1 - (float)currHealth / (float)maxHealth;
            foreach (Renderer renderer in renderers) {
                renderer.material.color = Color.Lerp(Color.green, Color.red, t);
            }
        }
    }

    public void ReceiveDamage(float damage) {
        currHealth -= damage;
        ChangeColorOnHealth();

        if (currHealth <= 0) {
            Destroy(gameObject);
        }
    }

    private void ChangeColorOnHealth() {
        float t = 1 - (float)currHealth / (float)maxHealth;

        //Debug.Log(Color.Lerp(Color.green, Color.green, t));
        foreach (Renderer renderer in renderers) {
            renderer.material.color = Color.Lerp(Color.green, Color.red, t);
        }
    }
}
