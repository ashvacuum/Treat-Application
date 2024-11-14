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

    private void Awake()
    {
        if (_puzzleFactory == null)
        {
            _puzzleFactory.GetComponent<PuzzleFactory>();
        }
        _puzzleFactory.InitializePuzzle();
    }

    public void StartGame(int difficultyLevel)
    {
        if (_puzzleFactory != null)
        {
            _puzzleFactory.GeneratePuzzle(difficultyLevel);
        }
    }

    public bool CheckMatch(PuzzlePiece firstSelection, PuzzlePiece secondSelection)
    {
        //TODO compare first and second selection
        return true;
    }
    
    private void OnPuzzlePieceSelected(PuzzlePiece puzzlePieceRef)
    {
        if (_currentPuzzle.TryGetValue(puzzlePieceRef, out var content))
        {
            puzzlePieceRef.RevealSprite(content);
            //call check match if the 2nd puzzle was selected
        }
    }
}
