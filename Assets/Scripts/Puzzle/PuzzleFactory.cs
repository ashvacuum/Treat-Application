using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        
        public List<PuzzlePiece> GeneratePuzzle(int puzzleSizeX, int puzzleSizeY)
        {
            ReturnToPool(_puzzlePool);
            
            var totalPieces = puzzleSizeX * puzzleSizeY;
            var puzzleData = GeneratePuzzleData(totalPieces);
            var puzzleList = new List<PuzzlePiece>();
            var sb = new StringBuilder();
            for (var i = 0; i < puzzleSizeX; i++)
            {
                for (var j = 0; j < puzzleSizeY; j++)
                {
                    sb.AppendLine($"Type for {i},{j}, {i+j}, {puzzleData[i + j].type}");
                    SetupNewPuzzlePiece(puzzleData[i+j], new Vector2(i,j), puzzleList);
                }
            }
            
            Debug.Log(sb.ToString());
            
            return puzzleList;
        }

        private void SetupNewPuzzlePiece(PuzzleData puzzleData, Vector2 pos, List<PuzzlePiece> puzzleList)
        {
            var newPiece = GetFromPool();
            newPiece.gameObject.SetActive(true);
            newPiece.Init(puzzleData);
            newPiece.Hide();
            newPiece.transform.position = pos;
            puzzleList.Add(newPiece);
        }

        private List<PuzzleData> GeneratePuzzleData(int gridSize)
        {
            var actualRequiredSize = gridSize / 2;
            var cutOffList = new List<PuzzleData>();
            cutOffList.AddRange(_pieceInfo);
            cutOffList.Shuffle();
            cutOffList.RemoveRange( actualRequiredSize - 1, _pieceInfo.Count - actualRequiredSize);
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

        void ToggleCollider(bool isEnabled);
    }
    
    [Serializable]
    public struct PuzzleData
    {
        public PuzzleType type;
        public Sprite sprite;
    }
    
    
    
    
}
