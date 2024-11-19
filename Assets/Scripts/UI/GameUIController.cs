using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    // Custom events for game state changes
[Serializable]
public class GameStateEvent : UnityEvent<IGameUIState> { }

public class GameUIController : MonoBehaviour
{
    
    [Header("UI Panels")]
    [SerializeField] private GameObject _preGamePanel;
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private GameObject _postGamePanel;

    
    [Header("Pre-Game UI Elements")]
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Slider _startGameSlider;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TMP_InputField _userNameInput;
    [SerializeField] private LeaderboardPanel _leaderboardPanel;
    [SerializeField] private Button _leaderboardBtn;
    
    [Header("In-Game UI Elements")]
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _quitToMenuButton;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _movesLeftText;
    [SerializeField] private TextMeshProUGUI _timerText;

    
    [Header("Post-Game UI Elements")]
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private TextMeshProUGUI _finalScoreText;
    [SerializeField] private TextMeshProUGUI _moveText;
    [SerializeField] private StarScore[] _starScores;

    
    public GameStateEvent OnStateChanged = new GameStateEvent();

    // State machine
    private GameUIState _currentState;
    private IGameUIState[] _states;
    private int _lastKnownDifficulty;

    private void OnEnable()
    {
        EventBus.Subscribe<TimerUpdateEvent>(UpdateTimer);
        EventBus.Subscribe<ScoreChangedEvent>(UpdateScoreUI);
        EventBus.Subscribe<GameEndEvent>(PopulatePostGameWithData);
    }

    private void OnDisable()
    {
        EventBus.Subscribe<TimerUpdateEvent>(UpdateTimer);
        EventBus.Unsubscribe<ScoreChangedEvent>(UpdateScoreUI);
        EventBus.Unsubscribe<GameEndEvent>(PopulatePostGameWithData);
    }

    private void Awake()
    {
        InitializeStates();
        SetupEventListeners();
    }

    private void InitializeStates()
    {
        _states = new IGameUIState[]
        {
            new PreGameState(this),
            new GameState(this),
            new PostGameState(this)
        };
    }

    private void SetupEventListeners()
    {
        // Pre-game buttons
        if (_startGameButton != null)
            _startGameButton.onClick.AddListener(StartGame);
        if(_levelText != null && _startGameSlider != null)
            _startGameSlider.onValueChanged.AddListener(UpdateLevelValue);
        if(_leaderboardBtn != null)
            _leaderboardBtn.onClick.AddListener(OnLeaderboardButtonPressed);
        

        // In-game buttons
        if (_pauseButton != null)
            _pauseButton.onClick.AddListener(TogglePause);
        if (_quitToMenuButton != null)
            _quitToMenuButton.onClick.AddListener(ReturnToMainMenu);

        // Post-game buttons
        if (_restartButton != null)
            _restartButton.onClick.AddListener(RestartGame);
        if (_mainMenuButton != null)
            _mainMenuButton.onClick.AddListener(ReturnToMainMenu);

    }

    private void Start()
    {
        TransitionTo<PreGameState>();
    }

    private void OnDestroy()
    {
        // Clean up event listeners
        if (_startGameButton != null)
            _startGameButton.onClick.RemoveListener(StartGame);
        if(_startGameSlider != null)
            _startGameSlider.onValueChanged.RemoveListener(UpdateLevelValue);
        if (_pauseButton != null)
            _pauseButton.onClick.RemoveListener(TogglePause);
        if (_quitToMenuButton != null)
            _quitToMenuButton.onClick.RemoveListener(ReturnToMainMenu);
        if (_restartButton != null)
            _restartButton.onClick.RemoveListener(RestartGame);
        if (_mainMenuButton != null)
            _mainMenuButton.onClick.RemoveListener(ReturnToMainMenu);

    }

    protected void TransitionTo<T>() where T : IGameUIState
    {
        _currentState?.OnExit();

        foreach (var state in _states)
        {
            if (state is not T) continue;
            
            _currentState = (GameUIState)state;
            _currentState.OnEnter();
            OnStateChanged?.Invoke(_currentState);
            break;
        }
    }

    // UI Event Handlers
    private void StartGame()
    {
        TransitionTo<GameState>();
        _lastKnownDifficulty = (int)_startGameSlider.value;
        Debug.Log($"Last known {_lastKnownDifficulty}");
        EventBus.Publish(new GameStartEvent(Time.time, _lastKnownDifficulty, _userNameInput.text));
    }

    private void UpdateLevelValue(float value)
    {
        _levelText.text = $"{(int)value}";
    }

    private void OnLeaderboardButtonPressed()
    {
        _leaderboardPanel.gameObject.SetActive(true);
    }

    private void TogglePause()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            EventBus.Publish(new GamePauseEvent(true));
        }
        else
        {
            Time.timeScale = 0;
            EventBus.Publish(new GamePauseEvent(false));
        }
    }

    private void QuitToMenu()
    {
        Time.timeScale = 1;
        TransitionTo<PreGameState>();
        EventBus.Publish(new GameQuitEvent());
    }

    private void RestartGame()
    {
        TransitionTo<GameState>();
        var userName = _userNameInput != null ? _userNameInput.text : string.Empty;
        EventBus.Publish(new GameStartEvent(Time.time, _lastKnownDifficulty, userName));
    }

    private void ReturnToMainMenu()
    {
        TransitionTo<PreGameState>();
        EventBus.Publish(new GameQuitEvent());
    }

    // UI Update Methods
    private void UpdateScoreUI(ScoreChangedEvent evt)
    {
        if (_scoreText != null)
            _scoreText.text = $"Score: {evt.NewScore}";
        if (_finalScoreText != null && _currentState is PostGameState)
            _finalScoreText.text = $"Final Score: {evt.NewScore}";
    }

    private void UpdateTimer(TimerUpdateEvent evt)
    {
        if (_timerText != null)
            _timerText.text = $"Time: {evt.TimeLeft:F1}";
    }
    
    private void PopulatePostGameWithData(GameEndEvent evt)
    {
        TransitionTo<PostGameState>();
        
        if (evt.DidWin)
        {
            foreach (var star in _starScores)
            {
                star.ShowStarSprite(true);
            }

            if (_finalScoreText != null)
                _finalScoreText.text = $"Score: {evt.FinalScore}";

            if (_moveText.text != null)
                _moveText.text = $"Moves Left : {evt.MovesLeft}";
        }
        else
        {
            if (_finalScoreText != null)
                _finalScoreText.text = $"Score: 0";
        }     
    }

    private void OnMovesChanged(MoveChangedEvent evt)
    {
        if (_movesLeftText != null)
            _movesLeftText.text = $"Moves Left: {evt.MoveCount}";
    }

    // Panel control methods
    public void ShowPreGamePanel(bool show) => _preGamePanel.SetActive(show);
    public void ShowGamePanel(bool show) => _gamePanel.SetActive(show);
    public void ShowPostGamePanel(bool show) => _postGamePanel.SetActive(show);
    public void ShowLeaderboardsPanel(bool show) => _leaderboardPanel.gameObject.SetActive(show);


}

    public interface IGameUIState
    {
        void OnEnter();
        void OnExit();
        void Update();
    }
}