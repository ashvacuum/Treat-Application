using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    public enum PuzzleType
    {
        P1 = 0,
        P2 = 1 << 1,
        P3 = 1 << 2,
        P4 = 1 << 3,
        P5 = 1 << 4
    }
    public class PuzzlePiece : MonoBehaviour, IInteractable, IEquatable<PuzzlePiece>
    {
        private BoxCollider2D _collider;
        private SpriteRenderer _spriteRenderer;
        private PuzzleType _type;
        
        public event Action<PuzzlePiece> OnPuzzlePieceSelectedEvent;
        public bool IsRevealed { get; private set; }
       

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        

        public void Init(PuzzleType type)
        {
            IsRevealed = false;
            Hide();
            _type = type;
        }
        public void SetSpriteBackGround(Sprite backGround)
        {
            _spriteRenderer.sprite = backGround;
        }
        
        public void RevealSprite(Sprite foreground)
        {
            _spriteRenderer.sprite = foreground;
        }


        public void Interact()
        {
            if (IsRevealed) return;
            OnPuzzlePieceSelectedEvent?.Invoke(this);
            IsRevealed = true;
        }

        public void Hide()
        {
            //hide sprite and return background
        }
        
        public bool Equals(PuzzlePiece other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _type == other._type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((PuzzlePiece)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), (int)_type);
        }
    }
}
