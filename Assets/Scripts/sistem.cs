using UnityEngine;
using UnityEngine.UI;

public class sistem : MonoBehaviour
{
    public Text Svipe;
    private int _cachedMoves = -1;

    private void Update()
    {
        if (Svipe != null && Player.MoveCounter != _cachedMoves)
        {
            _cachedMoves = Player.MoveCounter;
            Svipe.text = $"шагов : {_cachedMoves}";
        }
    }
}