using UnityEngine;
public class LevelManager : MonoBehaviour
{
    public int _yellow, _currentYellow;
    public int _blue, _currentBlue;
    public int _red, _currentRed;
    public int _green, _currentGreen;
    public GameObject _nextLevelButton;
    public GameObject _particleSystem;
    public Board _board;
    public TMPro.TextMeshProUGUI _yellowText;
    public TMPro.TextMeshProUGUI _redText;
    public TMPro.TextMeshProUGUI _greenText;
    public TMPro.TextMeshProUGUI _blueText;
    private void Start()
    {
        _board = FindObjectOfType<Board>();
    }
    private void Update()
    {
        if (!ReferenceEquals(_yellowText,null))
            _yellowText.text = _currentYellow.ToString() + "/" +_yellow.ToString();
        if (!ReferenceEquals(_redText, null))
            _redText.text = _currentRed.ToString() + "/" + _red.ToString();
        if (!ReferenceEquals(_greenText, null))
            _greenText.text = _currentGreen.ToString() + "/" + _green.ToString();
        if (!ReferenceEquals(_blueText, null))
            _blueText.text = _currentBlue.ToString() + "/" + _blue.ToString();
        CheckIfLevelCompleted();
    }
    public void CheckIfLevelCompleted()
    {
        var sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex == 1)
        {
            if(_yellow<=_currentYellow && _red <= _currentRed)
            {
                NextLevelButton();
            }
        }
        if (sceneIndex == 2)
        {
            if (_yellow <= _currentYellow && _red <= _currentRed && _green <= _currentGreen)
            {
                NextLevelButton();
            }
        }
        if (sceneIndex == 3)
        {
            if (_red <= _currentRed && _green <= _currentGreen && _blue <= _currentBlue)
            {
                NextLevelButton();
            }
        }
    }
    public void NextLevelButton()
    {
        _board._isGameOver = true;
        if (_nextLevelButton.activeInHierarchy==false)
        {
            Vector2 vector2 = new(0, 0);
            Instantiate(_particleSystem, vector2, Quaternion.identity);
            Instantiate(_particleSystem, vector2, Quaternion.identity);
            Instantiate(_particleSystem, vector2, Quaternion.identity); 
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 3)
            {
                _nextLevelButton.gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "You have completed the game!";
            }
            _nextLevelButton.SetActive(true);
        }
    }

}
