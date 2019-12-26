using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Controls global Game Interactions, update values on UI and disparate events;
/// </summary>
public class GameManager : MonoBehaviour
{
    //singleton
    public static GameManager Instance { get; set; }

    [Header("UI Fields")]
    [SerializeField, Tooltip("Reference to player score text in the UI")]
    private TextMeshProUGUI _scoreText = null;
    [SerializeField, Tooltip("Reference to coin score text in the UI")]
    private TextMeshProUGUI _coinText = null;
    [SerializeField, Tooltip("Reference to modifier text in the UI")]
    private TextMeshProUGUI _modifierText = null;
    [SerializeField, Tooltip("Reference to animator of death Screen Menu")]
    private Animator _deathMenuAnim = null;
    [SerializeField, Tooltip("Reference to score text on death Screen")]
    private TextMeshProUGUI _deathScoreText = null;
    [SerializeField, Tooltip("Reference to coin score text on death Screen")]
    private TextMeshProUGUI _deathCoinScoreText = null;
    [SerializeField, Tooltip("Reference to high score text on death Screen")]
    private TextMeshProUGUI _highScoreText = null;

    [Space(), SerializeField, Tooltip("Reference to the player controller script")]
    private PlayerController _playerController = null;
    [SerializeField] private int _coinScoreAmount = 5;
    [SerializeField] private GlacierSpawner _glacierSpawner = null;
    [SerializeField] private CameraController _cameraController = null;
    [SerializeField] private Animator _gameMenuAnim = null;
    [SerializeField] private Animator _menuAnim = null;
    [SerializeField] private Animator _coinIconAnim = null;

    private float _score, _coinScore, _modifierScore;
    private bool _isGamePlaying;
    private int _lastScore;

    public int Score { get { return (int)_score; }}
    public int CoinScore { get { return (int)_coinScore; } }
    public float ModifierScore { get { return _modifierScore; } }

    public bool IsDead { get; set; }

    

    private void Awake()
    {
        Instance = this;
        _modifierScore = 1;
        
    }

    private void Start()
    {
        UpdateAllScores();
        _highScoreText.text = PlayerPrefs.GetInt("HighScore").ToString();
    }

    private void Update()
    {
        if(MobileInputManager.Instance.Tap && !_isGamePlaying)
        {
            OnGameEnter();
        }
        if (_isGamePlaying && !IsDead)
        {
            _score += (Time.deltaTime * _modifierScore);
            if(_lastScore != (int) _score)
            {
                _lastScore = (int)_score;
                UpdateScoreText();
            }
            
        }
    }

    public void UpdateAllScores()
    {
        UpdateScoreText();
        UpdateCoinText();
        _modifierText.text = _modifierScore.ToString("0.00");
    }

    public void UpdateScoreText()
    {
        _scoreText.text = _score.ToString("0");
    }

    public void UpdateCoinText()
    {
        _coinText.text = _coinScore.ToString();
    }

    public void UpdateModifier(float amount)
    {
        _modifierScore = 1 + amount;
        _modifierText.text = _modifierScore.ToString("0.00");
    }

    public void GetCoin()
    {
        _coinIconAnim.SetTrigger("Collect");
        _coinScore ++; 
        UpdateCoinText();
        _score += _coinScoreAmount;
        UpdateScoreText();
    }

    public void OnGameEnter()
    {
        _isGamePlaying = true;
        _playerController.StartRunning();
        _glacierSpawner.IsScrolling = true;
        _cameraController.IsMoving = true;
        _menuAnim.SetBool("Show", false);
        _gameMenuAnim.SetBool("Show", true);
    }


    public void OnReplayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void OnDeath()
    {
        _glacierSpawner.IsScrolling = false;
        IsDead = true;
        _deathScoreText.text = _score.ToString("0");
        _deathCoinScoreText.text = _coinScore.ToString("0");
        _gameMenuAnim.SetBool("Show", false);
        _deathMenuAnim.SetTrigger("PlayerDead");

        if(_score > PlayerPrefs.GetInt("HighScore"))
        {
            float s = _score;
            if (s % 1 == 0) s += 1;
            PlayerPrefs.SetInt("HighScore", (int)_score);
        }
    }

}
