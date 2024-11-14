using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Puzzle
{
    public class PuzzleFactory : MonoBehaviour
    {
        [SerializeField]private PuzzlePiece _puzzlePrefab;
        [SerializeField] private Sprite _background;
        [SerializeField] private Sprite _actual;
        [FormerlySerializedAs("_spritesMatch")] [SerializeField]private List<PuzzleData> _pieceInfo;
        private List<PuzzlePiece> _puzzlePieces = new List<PuzzlePiece>();

        
        /// <summary>
        /// Only happens once;
        /// </summary>
        public void InitializePuzzle()
        {
            const int gridSize = 4;
            for (var i = 0; i < gridSize; i++)
            {
                for (var j = 0; j < gridSize; j++)
                {
                    var currentContent = Instantiate(_puzzlePrefab, this.transform);
                    currentContent.gameObject.SetActive(false);
                    currentContent.transform.position = new Vector3(i, j);
                    _puzzlePieces.Add(currentContent);
                }
            }
        }
        
        public Dictionary<PuzzlePiece, Sprite> GeneratePuzzle(int puzzleSize)
        {
            var gridSize = puzzleSize * 2;
            
            _puzzlePieces.Shuffle();
            foreach (var piece in _puzzlePieces)
            {
                piece.gameObject.SetActive(false);
            }
            

            var puzzleDictionary = new Dictionary<PuzzlePiece, Sprite>();

            var puzzleData = GeneratePuzzleData(gridSize);
            
            for (var i = 0; i < gridSize; i++)
            {
                _puzzlePieces[i].gameObject.SetActive(true); //initialize information here
                _puzzlePieces[i].Init(puzzleData[i]);
                _puzzlePieces[i].Hide();
                _puzzlePieces[i].SetSpriteBackGround(_background);
                
                puzzleDictionary[_puzzlePieces[i]] = puzzleData[i].sprite;
            }

            return puzzleDictionary;
        }

        private List<PuzzleData> GeneratePuzzleData(int gridSize)
        {
            var cutOffList = _pieceInfo;
            cutOffList.RemoveRange(gridSize, _pieceInfo.Count - gridSize);
            cutOffList.AddRange(cutOffList);
            cutOffList.Shuffle();
            return cutOffList;
        }
    }

    public interface IInteractable
    {
        void Interact();
        void Hide(bool shouldAnimate = false);
    }
    
    [Serializable]
    public struct PuzzleData
    {
        public PuzzleType type;
        public Sprite sprite;
    }
    
    
    
    
}
