using System.Collections;
using UnityEngine;

public class reaction : MonoBehaviour
{
    private const float AttackLockDuration = 0.42f;
    private const float BossBounceSpeed = 5f;

    public GameObject player;
    public Rigidbody rb;
    public Animator anim;

    private int _hitsRemainingForBounce = 2;
    private Player _playerScript;

    private void Awake()
    {
        if (player != null)
        {
            _playerScript = player.GetComponent<Player>();
        }
        else
        {
            _playerScript = GetComponentInParent<Player>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("graund"))
        {
            if (_playerScript != null)
            {
                _playerScript.StopMovement();
            }
            else
            {
                if (anim != null) anim.Play("stop");
                Player.canGo = true;
            }
        }
        else if (other.CompareTag("mob"))
        {
            if (anim != null) anim.Play("attack");
            StartCoroutine(LockCameraDuringAttack());
            Player.last = 0;
        }
        else if (other.CompareTag("dead"))
        {
            if (anim != null) anim.Play("dead");
            if (rb != null) rb.linearVelocity = Vector3.zero;
        }
        else if (other.CompareTag("boss") && killboss.rast)
        {
            HandleBossBounce();
        }
    }

    private void HandleBossBounce()
    {
        _hitsRemainingForBounce--;
        if (_hitsRemainingForBounce > 0) return;

        Vector3 bounceVelocity = Vector3.zero;
        int nextDirection = Player.last;

        switch (Player.last)
        {
            case 1:
                bounceVelocity = new Vector3(0f, 0f, -BossBounceSpeed);
                nextDirection = 2;
                break;
            case 2:
                bounceVelocity = new Vector3(0f, 0f, BossBounceSpeed);
                nextDirection = 1;
                break;
            case 3:
                bounceVelocity = new Vector3(BossBounceSpeed, 0f, 0f);
                nextDirection = 4;
                break;
            case 4:
                bounceVelocity = new Vector3(-BossBounceSpeed, 0f, 0f);
                nextDirection = 3;
                break;
        }

        _hitsRemainingForBounce = 2;

        if (_playerScript != null)
        {
            _playerScript.OnRedirected(bounceVelocity, nextDirection);
        }
        else if (rb != null && bounceVelocity != Vector3.zero)
        {
            rb.linearVelocity = bounceVelocity;
            Player.last = nextDirection;
            if (player != null)
            {
                player.transform.rotation = Quaternion.LookRotation(bounceVelocity);
            }
        }
    }

    private IEnumerator LockCameraDuringAttack()
    {
        Player.cam = false;
        yield return new WaitForSeconds(AttackLockDuration);
        Player.cam = true;
    }
}