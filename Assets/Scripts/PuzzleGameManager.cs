using System;
using System.Collections;
using System.Collections.Generic;
using LevelData;
using Puzzle;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PuzzleGameManager : MonoBehaviour
{
    [SerializeField]private PuzzleFactory _puzzleFactory;
    [SerializeField] private LevelData.LevelData _levelData;
    
    private List<PuzzlePiece> _currentPuzzle = new List<PuzzlePiece>();
    private int _currentMoves;
    private int _maxMovesAllowed;
    private int _numMatches;
    private int _requiredNumMatches;
    private int _currentScore;
    private PuzzlePiece _firstPiece = null;
    
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
        
    }

    private void StartGame(int difficultyLevel)
    {
        if (_puzzleFactory != null)
        {
            _currentPuzzle = _puzzleFactory.GeneratePuzzle(_levelData.LevelInfos[difficultyLevel].gridX,_levelData.LevelInfos[difficultyLevel].gridY);
            foreach (var puzzle in _currentPuzzle)
            {
                puzzle.OnPuzzlePieceSelectedEvent += OnPuzzlePieceSelected;
            }
        }

        if (Camera.main != null && Camera.main.TryGetComponent<CameraScaleAdjustment>(out var scaler))
        {
            scaler.RepositionCamera( (int)Mathf.Sqrt(_currentPuzzle.Count));
        }
    }

    private bool CheckMatch(PuzzlePiece firstSelection, PuzzlePiece secondSelection)
    {
        return firstSelection.Equals(secondSelection);
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
                _firstPiece = null;
            }
            else
            {
                //add an event bus here for moves made
                _firstPiece.Hide(true);
                puzzlePieceRef.Hide(true);
                _firstPiece = null;
            }
            //only add moves for the second piece
            _currentMoves++;
        }
        else
        {
            _firstPiece = puzzlePieceRef;
        }

        
        
    }

    private void EndGame()
    {
        
    }
}
