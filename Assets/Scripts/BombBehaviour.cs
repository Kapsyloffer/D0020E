using UnityEngine;

public class BombBehaviour : MonoBehaviour
{
    public int explosionRadius;
    public int explosionForce;
    public GameObject explosionEffect;
    void OnTriggerEnter2D(Collider2D col)
    {
        Explode();
    }

    void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach(Collider2D obj in colliders)
        {
            Rigidbody2D rigidbody = obj.GetComponent<Rigidbody2D>();
            if(rigidbody != null)
            {
                AddExplosionForce(rigidbody, explosionForce, transform.position, explosionRadius);
            }
        }

        Destroy(gameObject);
    }

    void AddExplosionForce(Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, int explosionRadius, ForceMode2D mode = ForceMode2D.Force)
    {
        var explosionDir = rb.position - explosionPosition;
        var explosionDistance = explosionDir.magnitude;

        Debug.Log("Before: " + explosionDir);
        explosionDir.Normalize();
        Debug.Log("After: " + explosionDir);

        rb.AddForce(Mathf.Lerp(0, explosionForce, explosionRadius - explosionDistance) * explosionDir, mode);
    }
}
