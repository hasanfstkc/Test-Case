using UnityEngine;

public class Fruit : MonoBehaviour
{
    public float _x, _y;
    private bool _isFalling = true;
    private Board _board;
    private void Start()
    {
        _board = FindObjectOfType<Board>();
    }
    void Update()
    {
        if (_isFalling)
        {
            _board._isFallingObjects = true;
            if (Vector3.Distance(transform.position, new Vector3(_x, _y, 0)) < 0.01f)
            {
                _board._isFallingObjects = false;
                _isFalling = false;
            }
            transform.position = Vector3.Lerp(transform.position, new Vector3(_x, _y, 0), Time.deltaTime * 3f);
        }
    }
}
