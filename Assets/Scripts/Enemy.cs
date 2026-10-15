using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private const float DeathAnimationDelay = 0.42f;
    private const float DespawnDelay = 4f;

    public Collider[] col;
    public Animator anim;

    private bool _isDead;

    private void Start()
    {
        col = GetComponents<Collider>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isDead && other.CompareTag("Player"))
        {
            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DeathSequence()
    {
        _isDead = true;

        if (col != null)
        {
            foreach (Collider c in col)
            {
                if (c != null) Destroy(c);
            }
        }

        yield return new WaitForSeconds(DeathAnimationDelay);

        if (anim != null) anim.Play("dead");
        Finish.vrags = Mathf.Max(0, Finish.vrags - 1);
        killboss.vrags = Mathf.Max(0, killboss.vrags - 1);

        yield return new WaitForSeconds(DespawnDelay);
        Destroy(gameObject);
    }
}