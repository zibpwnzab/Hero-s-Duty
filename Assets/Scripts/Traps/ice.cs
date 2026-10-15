using UnityEngine;

public class ice : MonoBehaviour
{
    private const float CenterSnapRadius = 0.25f;

    public Vector3 up;
    public int lastUp;
    public Vector3 down;
    public int lastDown;
    public Vector3 right;
    public int lastRight;
    public Vector3 left;
    public int lastLeft;

    private Player _playerScript;
    private Rigidbody _playerRb;
    private bool _hasDeflectedCurrentPass;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerRb = other.GetComponent<Rigidbody>();
            _playerScript = other.GetComponent<Player>();
            _hasDeflectedCurrentPass = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _hasDeflectedCurrentPass = false;
        }
    }

    private void FixedUpdate()
    {
        if (_hasDeflectedCurrentPass) return;

        if (_playerScript == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _playerScript = playerObj.GetComponent<Player>();
                _playerRb = playerObj.GetComponent<Rigidbody>();
            }
        }

        if (_playerScript == null || _playerRb == null) return;

        float deltaX = Mathf.Abs(_playerScript.transform.position.x - transform.position.x);
        float deltaZ = Mathf.Abs(_playerScript.transform.position.z - transform.position.z);

        if (deltaX <= CenterSnapRadius && deltaZ <= CenterSnapRadius)
        {
            TryDeflectPlayer();
        }
    }

    private void TryDeflectPlayer()
    {
        Vector3 newVelocity = Vector3.zero;
        int newLast = Player.last;

        if (Player.last == 2)
        {
            newVelocity = new Vector3(up.x, 0f, up.z);
            newLast = lastUp;
        }
        else if (Player.last == 1)
        {
            newVelocity = new Vector3(down.x, 0f, down.z);
            newLast = lastDown;
        }
        else if (Player.last == 3)
        {
            newVelocity = new Vector3(right.x, 0f, right.z);
            newLast = lastRight;
        }
        else if (Player.last == 4)
        {
            newVelocity = new Vector3(left.x, 0f, left.z);
            newLast = lastLeft;
        }
        else
        {
            return;
        }

        _hasDeflectedCurrentPass = true;

        Vector3 snappedPos = _playerScript.transform.position;
        snappedPos.x = transform.position.x;
        snappedPos.z = transform.position.z;
        _playerScript.transform.position = snappedPos;

        _playerScript.OnRedirected(newVelocity, newLast);
    }
}