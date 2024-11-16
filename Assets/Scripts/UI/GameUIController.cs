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

    
    [Header("In-Game UI Elements")]
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _quitToMenuButton;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _timerText;

    
    [Header("Post-Game UI Elements")]
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private TextMeshProUGUI _finalScoreText;

    
    public GameStateEvent OnStateChanged = new GameStateEvent();

    // State machine
    private GameUIState currentState;
    private IGameUIState[] states;
    private int _lastKnownDifficulty;

    private void OnEnable()
    {
        EventBus.Subscribe<TimerUpdateEvent>(UpdateTimer);
        EventBus.Subscribe<ScoreChangedEvent>(UpdateScoreUI);
    }

    private void OnDisable()
    {
        EventBus.Subscribe<TimerUpdateEvent>(UpdateTimer);
        EventBus.Unsubscribe<ScoreChangedEvent>(UpdateScoreUI);
    }

    private void Awake()
    {
        InitializeStates();
        SetupEventListeners();
    }

    private void InitializeStates()
    {
        states = new IGameUIState[]
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
        

        // In-game buttons
        if (_pauseButton != null)
            _pauseButton.onClick.AddListener(TogglePause);
        if (_quitToMenuButton != null)
            _quitToMenuButton.onClick.AddListener(QuitToMenu);

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
            _quitToMenuButton.onClick.RemoveListener(QuitToMenu);
        if (_restartButton != null)
            _restartButton.onClick.RemoveListener(RestartGame);
        if (_mainMenuButton != null)
            _mainMenuButton.onClick.RemoveListener(ReturnToMainMenu);

    }

    protected void TransitionTo<T>() where T : IGameUIState
    {
        currentState?.OnExit();

        foreach (var state in states)
        {
            if (state is not T) continue;
            
            currentState = (GameUIState)state;
            currentState.OnEnter();
            OnStateChanged?.Invoke(currentState);
            break;
        }
    }

    // UI Event Handlers
    private void StartGame()
    {
        TransitionTo<GameState>();
        _lastKnownDifficulty = (int)_startGameSlider.value;
        EventBus.Publish(new GameStartEvent(Time.time, _lastKnownDifficulty, _userNameInput.text));
    }

    private void UpdateLevelValue(float value)
    {
        _levelText.text = $"{(int)value}";
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
    }

    // UI Update Methods
    private void UpdateScoreUI(ScoreChangedEvent evt)
    {
        if (_scoreText != null)
            _scoreText.text = $"Score: {evt.NewScore}";
        if (_finalScoreText != null && currentState is PostGameState)
            _finalScoreText.text = $"Final Score: {evt.NewScore}";
    }

    private void UpdateTimer(TimerUpdateEvent evt)
    {
        if (_timerText != null)
            _timerText.text = $"Time: {evt.TimeLeft:F1}";
    }

    // Panel control methods
    public void ShowPreGamePanel(bool show) => _preGamePanel.SetActive(show);
    public void ShowGamePanel(bool show) => _gamePanel.SetActive(show);
    public void ShowPostGamePanel(bool show) => _postGamePanel.SetActive(show);

    // Public methods for external systems
    public void EndGame(int finalScore)
    {
        TransitionTo<PostGameState>();
        
        
    }
}

    public interface IGameUIState
    {
        void OnEnter();
        void OnExit();
        void Update();
    }
}