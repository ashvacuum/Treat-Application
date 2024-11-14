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
        [SerializeField]private List<PuzzleData> _spritesMatch;
        private readonly List<PuzzlePiece> _puzzlePieces = new List<PuzzlePiece>();
        private Dictionary<PuzzlePiece, Sprite> _puzzleContents = new Dictionary<PuzzlePiece, Sprite>();

        private void Awake()
        {
            
        }
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
                    _puzzlePrefab.gameObject.SetActive(false);
                    _puzzlePieces.Add(_puzzlePrefab);
                    currentContent.transform.position = new Vector3(i, j);
                    
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

            var sprites = GetRandomSprites(gridSize);
            
            for (int i = 0; i < gridSize; i++)
            {
                _puzzlePieces[i].gameObject.SetActive(true); //initialize information here
                _puzzlePieces[i].Hide();
                puzzleDictionary[_puzzlePieces[i]] = sprites[i];
            }

            return puzzleDictionary;
        }

        

        private List<Sprite> GetRandomSprites(int gridSize)
        {
            //TODO: change this to actually provide sprites from a list
            return new List<Sprite>();
        }
    }

    public interface IInteractable
    {
        void Interact();
        void Hide();
        
    }
    
    [Serializable]
    public struct PuzzleData
    {
        public PuzzleType _type;
        public Sprite _sprite;

    }
    
    
    
    
}
