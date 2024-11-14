using System;
using System.Collections;
using System.Collections.Generic;
using Puzzle;
using Unity.VisualScripting;
using UnityEngine;

public class PuzzleGameManager : MonoBehaviour
{
    [SerializeField]private PuzzleFactory _puzzleFactory;
    
    private Dictionary<PuzzlePiece, Sprite> _currentPuzzle = new Dictionary<PuzzlePiece, Sprite>();
    private int _currentMoves;
    private int _numMatches;

    private PuzzlePiece _firstPiece = null;
    
    private event Action OnTwoPiecesChosen;
    private void Awake()
    {
        if (_puzzleFactory == null)
        {
            _puzzleFactory = GetComponent<PuzzleFactory>();
        }

        _puzzleFactory.GetComponent<PuzzleFactory>();
        _puzzleFactory.InitializePuzzle();
    }

    public void StartGame(int difficultyLevel)
    {
        if (_puzzleFactory != null)
        {
            _currentPuzzle = _puzzleFactory.GeneratePuzzle(difficultyLevel);
            foreach (var puzzle in _currentPuzzle)
            {
                puzzle.Key.OnPuzzlePieceSelectedEvent += OnPuzzlePieceSelected;
            }
        }

        if (Camera.main != null && Camera.main.TryGetComponent<CameraScaler>(out var scaler))
        {
            scaler.RepositionCamera((int)(_currentPuzzle.Count * .5f));
        }
    }

    public bool CheckMatch(PuzzlePiece firstSelection, PuzzlePiece secondSelection)
    {
        return firstSelection.Equals(secondSelection);
    }
    
    private void OnPuzzlePieceSelected(PuzzlePiece puzzlePieceRef)
    {
        if (!_currentPuzzle.TryGetValue(puzzlePieceRef, out var content)) return;
        
        
        puzzlePieceRef.RevealSprite(content);
        if (_firstPiece != null)
        {
            if (CheckMatch(_firstPiece, puzzlePieceRef))
            {
                
            }
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
