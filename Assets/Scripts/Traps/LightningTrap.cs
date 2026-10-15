using System.Collections;
using UnityEngine;

public class LightningTrap : MonoBehaviour
{
    private const float VfxLifetime = 2f;
    private const float AnimationCrossFade = 0.1f;

    [Header("Visuals & UI")]
    public GameObject lightningVFXPrefab;
    public GameObject loseScreen;
    public float vfxOffset_Y = 0.5f;

    [Header("Timing")]
    public float strikeDelay = 0.5f;

    private int _lastProcessedMove = -1;
    private bool _isPlayerInside;
    private GameObject _playerRef;

    private void Start()
    {
        _lastProcessedMove = Player.MoveCounter;
    }

    private void Update()
    {
        if (Player.MoveCounter != _lastProcessedMove)
        {
            _lastProcessedMove = Player.MoveCounter;

            if (Player.MoveCounter > 0 && Player.MoveCounter % 2 == 0)
            {
                StartCoroutine(StrikeRoutine());
            }
        }
    }

    private IEnumerator StrikeRoutine()
    {
        yield return new WaitForSeconds(strikeDelay);

        SpawnLightningVfx();

        if (_isPlayerInside && _playerRef != null)
        {
            KillPlayer(_playerRef);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInside = true;
            _playerRef = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInside = false;
            _playerRef = null;
        }
    }

    private void SpawnLightningVfx()
    {
        if (lightningVFXPrefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.up * vfxOffset_Y;
        GameObject vfx = Instantiate(lightningVFXPrefab, spawnPos, Quaternion.identity);
        Destroy(vfx, VfxLifetime);
    }

    private void KillPlayer(GameObject playerObj)
    {
        if (playerObj.TryGetComponent(out Player playerScript))
        {
            playerScript.enabled = false;

            if (playerScript.rb != null)
            {
                playerScript.rb.linearVelocity = Vector3.zero;
                playerScript.rb.isKinematic = true;
            }

            if (playerScript._anim != null)
            {
                playerScript._anim.CrossFadeInFixedTime("dead", AnimationCrossFade);
            }
        }

        Player.cam = false;

        if (loseScreen != null)
        {
            loseScreen.SetActive(true);
        }
    }
}