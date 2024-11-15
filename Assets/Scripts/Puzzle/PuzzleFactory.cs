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
        [FormerlySerializedAs("_spritesMatch")] [SerializeField]private List<PuzzleData> _pieceInfo;
        private List<PuzzlePiece> _puzzlePool = new List<PuzzlePiece>();

        private readonly List<int> _puzzleGridSizeToLevel = new List<int>()
        {
            0,2, 2, 3, 3, 3, 4, 4, 5
        };
        
        /// <summary>
        /// Only happens once;
        /// </summary>
        public void GeneratePuzzlePool()
        {
            const int gridSize = 5;
            for (var i = 0; i < gridSize; i++)
            {
                for (var j = 0; j < gridSize; j++)
                {
                    var currentContent = Instantiate(_puzzlePrefab, this.transform);
                    currentContent.gameObject.SetActive(false);
                    currentContent.transform.position = new Vector3(i, j);
                    _puzzlePool.Add(currentContent);
                }
            }
        }
        
        public List<PuzzlePiece> GeneratePuzzle(int puzzleSize)
        {
            var gridSize = _puzzleGridSizeToLevel[puzzleSize] * 2;
            var puzzleData = GeneratePuzzleData(gridSize);
            var puzzleList = new List<PuzzlePiece>();
            
            for (var i = 0; i < gridSize; i++)
            {
                for (var j = 0; j < gridSize; j++)
                {
                    SetupNewPuzzlePiece(puzzleData[i+j], new Vector2(i,j), puzzleList);
                }
            }
            
            return puzzleList;
        }

        private void SetupNewPuzzlePiece(PuzzleData puzzleData, Vector2 pos, List<PuzzlePiece> puzzleList)
        {
            var newPiece = GetFromPool();
            newPiece.gameObject.SetActive(true); //initialize information here
            newPiece.Init(puzzleData);
            newPiece.Hide();
            //newPiece.SetSpriteBackGround(_background);
            newPiece.transform.position = pos;
            puzzleList.Add(newPiece);
        }

        private List<PuzzleData> GeneratePuzzleData(int gridSize)
        {
            var cutOffList = new List<PuzzleData>();
            cutOffList.AddRange(_pieceInfo);
            cutOffList.RemoveRange(gridSize - 1, _pieceInfo.Count - gridSize);
            cutOffList.AddRange(cutOffList);
            cutOffList.Shuffle();
            return cutOffList;
        }

        private PuzzlePiece GetFromPool()
        {
            var piece = _puzzlePool.FirstOrDefault(s => !s.gameObject.activeSelf);
            if (piece == null)
            {
                piece = Instantiate(_puzzlePrefab, this.transform);
            }
            else
            {
                _puzzlePool.Remove(piece);
            }

            return piece;
        }

        private void ReturnToPool(PuzzlePiece piece)
        {
            piece.gameObject.SetActive(false);
            _puzzlePool.Add(piece);
        }

        public void ReturnToPool(List<PuzzlePiece> pieces)
        {
            foreach (var piece in pieces)
            {
                ReturnToPool(piece);
            }
        }
    }

    public interface IInteractable
    {
        void Interact();
        void Hide(bool shouldAnimate = false, float delay = 1);

        void DisableCollider();
    }
    
    [Serializable]
    public struct PuzzleData
    {
        public PuzzleType type;
        public Sprite sprite;
    }
    
    
    
    
}
