using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
public class Board : MonoBehaviour
{
    public int _width = 5, _height = 5;
    public GameObject _tilePrefab;
    public GameObject[] _fruidPrefabs;
    public BackgroundTile[,] _allTiles;
    public float _speed = 5f;
    private Camera _camera;
    private bool _isDragging = false;
    private Vector2 _dragStartPos;
    private bool _isRowDrag = false;
    private int _dragIndex = -1;
    private bool _isMoving;
    public bool _isGameOver=false;
    public bool _isFallingObjects = true;
    public GameObject _particleSystem;
    void Start()
    {
        var lvlManager = FindObjectOfType<LevelManager>();
        _allTiles = new BackgroundTile[_width, _height];
        _camera = Camera.main;
        SetUp();
        
    }
    private void Update()
    {
        if (!_isGameOver)
        {
            if(!_isFallingObjects)
            {
                if (_isMoving) return; // Hareket devam ederken yeni komut alma
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                    if (hit.collider != null)
                    {
                        BackgroundTile tile = hit.collider.GetComponent<BackgroundTile>();
                        if (tile != null)
                        {
                            _dragStartPos = mousePos;
                            Vector2 tilePos = tile.transform.position;
                            int col = Mathf.RoundToInt(tilePos.x - transform.position.x);
                            int row = Mathf.RoundToInt(tilePos.y - transform.position.y);
                            float xDiff = Mathf.Abs(mousePos.x - tilePos.x);
                            float yDiff = Mathf.Abs(mousePos.y - tilePos.y);
                            _isRowDrag = xDiff > yDiff;
                            _dragIndex = _isRowDrag ? row : col;
                            _isDragging = true;
                        }
                    }
                }
                if (_isDragging && Input.GetMouseButton(0))
                {
                    Vector2 currentMousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 delta = currentMousePos - _dragStartPos;

                    if (_isRowDrag)
                    {
                        if (Mathf.Abs(delta.x) >= 1f)
                        {
                            int shift = Mathf.RoundToInt(delta.x);
                            if (shift != 0)
                            {
                                StartCoroutine(HandleMovement(() =>
                                {
                                    if (shift > 0)
                                    {
                                        ShiftRowRight(_dragIndex, shift);
                                    }
                                    else
                                    {
                                        ShiftRowLeft(_dragIndex, -shift);
                                    }
                                    _dragStartPos = currentMousePos;
                                }));
                            }
                        }
                    }
                    else
                    {
                        if (Mathf.Abs(delta.y) >= 1f)
                        {
                            int shift = Mathf.RoundToInt(delta.y);
                            if (shift != 0)
                            {
                                StartCoroutine(HandleMovement(() =>
                                {
                                    if (shift > 0)
                                    {
                                        ShiftColumnUp(_dragIndex, shift);
                                    }
                                    else
                                    {
                                        ShiftColumnDown(_dragIndex, -shift);
                                    }
                                    _dragStartPos = currentMousePos;
                                }));
                            }
                        }
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    CheckAndDestroyMatches();
                    _isDragging = false;
                }
                FillBoard();
                CheckAndDestroyMatches();
            }
        }
    }
    private IEnumerator HandleMovement(System.Action movementAction)
    {
        _isMoving = true;
        movementAction.Invoke();
        yield return new WaitForSeconds(0.5f); // Hareketin tamamlanmasý için bekleme süresi
        _isMoving = false;
    }
    private void SetUp()
    {
        var parentPos = transform.position;
        for (int i = 0; i < _allTiles.GetLength(0); i++)
        {
            for (int j = 0; j < _allTiles.GetLength(1); j++)
            {
                Vector2 tempPosition = new Vector2(parentPos.x + i, parentPos.y + j);
                GameObject backgroundTile = Instantiate(_tilePrefab, tempPosition, Quaternion.identity);
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";
                _allTiles[i, j] = backgroundTile.GetComponent<BackgroundTile>();
            }
        }
    }
    private void CheckAndDestroyMatches()
    {
        for (int row = 0; row < _height; row++)
        {
            int matchCount = 1;
            string lastTag = _allTiles[0, row].transform.childCount > 0 ? _allTiles[0, row].transform.GetChild(0).tag : null;

            for (int col = 1; col < _width; col++)
            {
                string currentTag = _allTiles[col, row].transform.childCount > 0 ? _allTiles[col, row].transform.GetChild(0).tag : null;

                if (currentTag == lastTag && currentTag != null)
                {
                    matchCount++;
                    if (col == _width - 1 && matchCount >= 3)
                    {
                        DestroyMatchedTiles(col, row, matchCount, true);
                    }
                }
                else
                {
                    if (matchCount >= 3)
                    {
                        DestroyMatchedTiles(col - 1, row, matchCount, true);
                    }
                    matchCount = 1;
                    lastTag = currentTag;
                }
            }
        }
        for (int col = 0; col < _width; col++)
        {
            int matchCount = 1;
            string lastTag = _allTiles[col, 0].transform.childCount > 0 ? _allTiles[col, 0].transform.GetChild(0).tag : null;
            for (int row = 1; row < _height; row++)
            {
                string currentTag = _allTiles[col, row].transform.childCount > 0 ? _allTiles[col, row].transform.GetChild(0).tag : null;
                if (currentTag == lastTag && currentTag != null)
                {
                    matchCount++;
                    if (row == _height - 1 && matchCount >= 3)
                    {
                        DestroyMatchedTiles(col, row, matchCount, false);
                    }
                }
                else
                {
                    if (matchCount >= 3)
                    {
                        DestroyMatchedTiles(col, row - 1, matchCount, false);
                    }
                    matchCount = 1;
                    lastTag = currentTag;
                }
            }
        }
    }
    public void FillBoard()
    {

        for (int i = 0; i < _allTiles.GetLength(0); i++)
        {
            for (int j = 0; j < _allTiles.GetLength(1); j++)
            {
                if (_allTiles[i, j].transform.childCount == 0)
                {
                    var parentPos = transform.position;
                    Vector3 tempPosition = new(parentPos.x + i, parentPos.y + 10 + j,0);
                    var randomFruit = Random.Range(0, _fruidPrefabs.Length);
                    GameObject fruit = Instantiate(_fruidPrefabs[randomFruit], tempPosition, Quaternion.identity);
                    fruit.name = "( " + i + ", " + j + " )";
                    fruit.transform.parent = _allTiles[i, j].transform;
                    var fruitComp = fruit.GetComponent<Fruit>();
                    fruitComp._x = tempPosition.x;
                    fruitComp._y = tempPosition.y-10;
                }
            }
        }
        //CheckAndDestroyMatches();
    }
    private void ShiftRowLeft(int row, int steps)
    {
        for (int s = 0; s < steps; s++)
        {
            Transform firstChild = _allTiles[0, row].transform.childCount > 0 ? _allTiles[0, row].transform.GetChild(0) : null;
            for (int i = 0; i < _width - 1; i++)
            {
                if (_allTiles[i + 1, row].transform.childCount > 0)
                {
                    Transform child = _allTiles[i + 1, row].transform.GetChild(0);
                    StartCoroutine(SmoothMove(child, _allTiles[i, row].transform.position));
                    child.SetParent(_allTiles[i, row].transform);
                }
            }

            if (firstChild != null)
            {
                StartCoroutine(SmoothMove(firstChild, _allTiles[_width - 1, row].transform.position));
                firstChild.SetParent(_allTiles[_width - 1, row].transform);
            }
        }
    }
    private void ShiftRowRight(int row, int steps)
    {
        for (int s = 0; s < steps; s++)
        {
            Transform lastChild = _allTiles[_width - 1, row].transform.childCount > 0 ? _allTiles[_width - 1, row].transform.GetChild(0) : null;
            for (int i = _width - 1; i > 0; i--)
            {
                if (_allTiles[i - 1, row].transform.childCount > 0)
                {
                    Transform child = _allTiles[i - 1, row].transform.GetChild(0);
                    StartCoroutine(SmoothMove(child, _allTiles[i, row].transform.position));
                    child.SetParent(_allTiles[i, row].transform);
                }
            }

            if (lastChild != null)
            {
                StartCoroutine(SmoothMove(lastChild, _allTiles[0, row].transform.position));
                lastChild.SetParent(_allTiles[0, row].transform);
            }
        }
    }
    private void ShiftColumnUp(int col, int steps)
    {
        for (int s = 0; s < steps; s++)
        {
            Transform lastChild = _allTiles[col, _height - 1].transform.childCount > 0 ? _allTiles[col, _height - 1].transform.GetChild(0) : null;
            for (int j = _height - 1; j > 0; j--)
            {
                if (_allTiles[col, j - 1].transform.childCount > 0)
                {
                    Transform child = _allTiles[col, j - 1].transform.GetChild(0);
                    StartCoroutine(SmoothMove(child, _allTiles[col, j].transform.position));
                    child.SetParent(_allTiles[col, j].transform);
                }
            }

            if (lastChild != null)
            {
                StartCoroutine(SmoothMove(lastChild, _allTiles[col, 0].transform.position));
                lastChild.SetParent(_allTiles[col, 0].transform);
            }
        }
    }
    private void ShiftColumnDown(int col, int steps)
    {
        for (int s = 0; s < steps; s++)
        {
            Transform firstChild = _allTiles[col, 0].transform.childCount > 0 ? _allTiles[col, 0].transform.GetChild(0) : null;
            for (int j = 0; j < _height - 1; j++)
            {
                if (_allTiles[col, j + 1].transform.childCount > 0)
                {
                    Transform child = _allTiles[col, j + 1].transform.GetChild(0);
                    StartCoroutine(SmoothMove(child, _allTiles[col, j].transform.position));
                    child.SetParent(_allTiles[col, j].transform);
                }
            }

            if (firstChild != null)
            {
                StartCoroutine(SmoothMove(firstChild, _allTiles[col, _height - 1].transform.position));
                firstChild.SetParent(_allTiles[col, _height - 1].transform);
            }
        }
    }
    private IEnumerator SmoothMove(Transform objTransform, Vector2 targetPosition)
    {
        Vector2 startPosition = objTransform.position;
        float elapsedTime = 0;
        while (elapsedTime < 1f)
        {
            if (ReferenceEquals(objTransform, null))
            {
                yield break;
            }
            objTransform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime);
            elapsedTime += Time.deltaTime * _speed;
            yield return null;
        }
        objTransform.position = targetPosition;
        CheckAndDestroyMatches();
    }
    private void DestroyMatchedTiles(int col, int row, int matchCount, bool isRow)
    {
        for (int i = 0; i < matchCount; i++)
        {
            int targetCol = isRow ? col - i : col;
            int targetRow = isRow ? row : row - i;

            if (_allTiles[targetCol, targetRow].transform.childCount > 0)
            {
                var objTag = _allTiles[targetCol, targetRow].transform.GetChild(0).tag;
                if (objTag == "Yellow")
                {
                    FindObjectOfType<LevelManager>()._currentYellow++;
                }
                else if (objTag == "Blue")
                {
                    FindObjectOfType<LevelManager>()._currentBlue++;
                }
                else if (objTag == "Red")
                {
                    FindObjectOfType<LevelManager>()._currentRed++;
                }
                else if (objTag == "Green")
                {
                    FindObjectOfType<LevelManager>()._currentGreen++;
                }
                var particle = Instantiate(_particleSystem, _allTiles[targetCol, targetRow].transform.position, Quaternion.identity);
                Destroy(particle, 0.6f);
                Destroy(_allTiles[targetCol, targetRow].transform.GetChild(0).gameObject);
            }
        }
    }
}
