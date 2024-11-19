using System;
using System.Collections;
using System.Collections.Generic;
using LevelData;
using Puzzle;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PuzzleGameManager : MonoBehaviour
{
    [SerializeField]private PuzzleFactory _puzzleFactory;
    [SerializeField] private LevelData.LevelData _levelData;
    
    private List<PuzzlePiece> _currentPuzzle = new List<PuzzlePiece>();
    private int _currentMoves;
    private int _currentMatches;
    private int _currentScore;
    private float _timeLeft;
    private PuzzlePiece _firstPiece = null;
    private LevelInformation _currentLevelInfo;
    
    private void Awake()
    {
        if (_puzzleFactory == null)
        {
            _puzzleFactory = GetComponent<PuzzleFactory>();
        }

        _puzzleFactory.GetComponent<PuzzleFactory>();
        _puzzleFactory.GeneratePuzzlePool();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<GameStartEvent>(OnStartGame);
        EventBus.Subscribe<GameQuitEvent>(OnQuitGame);
        EventBus.Subscribe<TimerUpdateEvent>(OnTimerHitZero);
    }   

    private void OnDisable()
    {
        EventBus.Unsubscribe<GameStartEvent>(OnStartGame);
        EventBus.Unsubscribe<GameQuitEvent>(OnQuitGame);
        EventBus.Unsubscribe<TimerUpdateEvent>(OnTimerHitZero);
    }

    private void OnTimerHitZero(TimerUpdateEvent evt)
    {
        _timeLeft = evt.TimeLeft;
        Debug.Log($"Time Left: {_timeLeft}");
        if (!(_timeLeft <= 0)) return;
        
        EventBus.Publish(new GameEndEvent(_currentScore, _timeLeft, _currentLevelInfo.numberMoves - _currentMoves,
            false));

        EndGame();
    }

    private void OnStartGame(GameStartEvent evt)
    {
        _firstPiece = null;
        _currentScore = 0;
        _currentLevelInfo = _levelData.LevelInfos[evt.Difficulty];
        Debug.Log($"Difficulty {evt.Difficulty}");
        _currentMatches = 0;
        _timeLeft = 0;
        _currentMoves = 0;
        StartGame(evt.Difficulty);
        EventBus.Publish(new TimerStartEvent(_currentLevelInfo.timeLeft));
        EventBus.Publish(new MoveChangedEvent(_currentLevelInfo.numberMoves));
    }

    private void OnQuitGame(GameQuitEvent evt)
    {
        _firstPiece = null;
        _puzzleFactory.ReturnToPool(_currentPuzzle);
    }

    private void StartGame(int difficultyLevel)
    {
        
        if (_puzzleFactory != null)
        {
            _currentPuzzle = _puzzleFactory.GeneratePuzzle(_currentLevelInfo.gridX,_currentLevelInfo.gridY);
            foreach (var puzzle in _currentPuzzle)
            {
                puzzle.OnPuzzlePieceSelectedEvent += OnPuzzlePieceSelected;
            }
        }

        if (Camera.main != null && Camera.main.TryGetComponent<CameraScaleAdjustment>(out var cameraScaleAdjuster))
        {
            cameraScaleAdjuster.RepositionCamera(_currentLevelInfo.gridX,_currentLevelInfo.gridY);
        }
    }

    private bool CheckMatch(PuzzlePiece firstSelection, PuzzlePiece secondSelection)
    {
        return firstSelection.Equals(secondSelection);
    }

    private bool CheckIfGameEnd(out bool isWin)
    {
        var hasReachedRequiredMatches = _currentMatches >= _currentLevelInfo.requiredMatches;
        var isOutOfMoves = _currentMoves >= _currentLevelInfo.numberMoves;
        isWin = hasReachedRequiredMatches;
        return isWin || isOutOfMoves;
    }
    
    private void OnPuzzlePieceSelected(PuzzlePiece puzzlePieceRef)
    {
        if (puzzlePieceRef.IsRevealed) return;

        var movesLeft = _currentLevelInfo.numberMoves - _currentMoves;
        
        if (_firstPiece != null)
        {
            if (CheckMatch(_firstPiece, puzzlePieceRef))
            {
                var newScore = Mathf.CeilToInt(movesLeft * _timeLeft);
                Debug.Log($"{_currentLevelInfo.numberMoves} {_currentMoves} {_timeLeft}");
                _currentScore += newScore;
                EventBus.Publish(new ScoreChangedEvent(_currentScore,newScore));
                
                _currentMatches++;
            }
            else
            {
                //add an event bus here for moves made
                _firstPiece.Hide(true);
                puzzlePieceRef.Hide(true);
            }
            
            //only add moves for the second piece
            _currentMoves++;
            
            EventBus.Publish(new MoveChangedEvent(_currentLevelInfo.numberMoves - _currentMoves));
 
            if (CheckIfGameEnd(out var isWin))
            {
                EventBus.Publish(new GameEndEvent(_currentScore, _timeLeft, _currentLevelInfo.numberMoves - _currentMoves,
                    isWin));

                EndGame();
            }

            _firstPiece = null;
        }
        else
        {
            _firstPiece = puzzlePieceRef;
        }
    }   

    private void OnPauseGame(GamePauseEvent evt)
    {
        foreach (var puzzle in _currentPuzzle)
        {
            if (!puzzle.IsRevealed)
            {
                puzzle.ToggleCollider(!evt.IsPaused);
            }
        }
    }
    
    private void OnUnpauseGame()
    {
        foreach (var puzzle in _currentPuzzle)
        {
            if (!puzzle.IsRevealed)
            {
                puzzle.ToggleCollider(true);
            }
        }
    }

    private void EndGame()
    {
        _puzzleFactory.ReturnToPool(_currentPuzzle);
        _currentPuzzle.Clear();
                
        foreach (var puzzle in _currentPuzzle)
        {
            puzzle.OnPuzzlePieceSelectedEvent -= OnPuzzlePieceSelected;
        }
    }
}
