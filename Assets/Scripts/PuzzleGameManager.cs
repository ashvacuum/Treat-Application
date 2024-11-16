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
    private int _numMatches;
    private int _currentScore;
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
    }   

    private void OnDisable()
    {
        EventBus.Unsubscribe<GameStartEvent>(OnStartGame);
    }

    private void OnStartGame(GameStartEvent evt)
    {
        _firstPiece = null;
        StartGame(evt.Difficulty);
        _currentScore = 0;
        _currentLevelInfo = _levelData.LevelInfos[evt.Difficulty];
        _currentMoves = 0;
        EventBus.Publish(new TimerStartEvent(_currentLevelInfo.timeLeft));
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
        var hasReachedRequiredMatches = _currentScore >= _currentLevelInfo.requiredMatches;
        var isOutOfMoves = _currentMoves >= _currentLevelInfo.numberMoves;

        isWin = hasReachedRequiredMatches;

        return isWin || isOutOfMoves;
    }
    
    private void OnPuzzlePieceSelected(PuzzlePiece puzzlePieceRef)
    {
        if (puzzlePieceRef.IsRevealed) return;
        
        if (_firstPiece != null)
        {
            if (CheckMatch(_firstPiece, puzzlePieceRef))
            {
                _currentScore++;
                EventBus.Publish(new ScoreChangedEvent(_currentScore,1));
                _firstPiece.ToggleCollider(false);
                puzzlePieceRef.ToggleCollider(false);
                
                EventBus.Publish(new ScoreChangedEvent(_currentMoves, 1));
            }
            else
            {
                //add an event bus here for moves made
                _firstPiece.Hide(true);
                puzzlePieceRef.Hide(true);
            }
            
            //only add moves for the second piece
            _currentMoves++;
            
            EventBus.Publish(new MoveChangedEvent(_currentMoves));
            
            if (CheckIfGameEnd(out var isWin))
            {
                EventBus.Publish(new GameEndEvent(_currentScore, 5f, _currentLevelInfo.numberMoves - _currentMoves, isWin));
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
        foreach (var puzzle in _currentPuzzle)
        {
            puzzle.ToggleCollider(false);
            puzzle.OnPuzzlePieceSelectedEvent -= OnPuzzlePieceSelected;
        }
    }
}
