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
    
    [RequireComponent(typeof(BoxCollider2D))] // require this to be interactable
    public class PuzzlePiece : MonoBehaviour, IInteractable, IEquatable<PuzzlePiece>
    {
        private SpriteRenderer _backgroundSpriteRenderer;
        private SpriteRenderer _hiddenSpriteRenderer;
        private PuzzleType _type;
        
        public event Action<PuzzlePiece> OnPuzzlePieceSelectedEvent;
        public bool IsRevealed { get; private set; }
       

        private void Awake()
        {
            _backgroundSpriteRenderer = GetComponent<SpriteRenderer>();
            _hiddenSpriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        }
        
        
        

        public void Init(PuzzleData data)
        {
            IsRevealed = false;
            Hide();
            _type = data.type;
            _hiddenSpriteRenderer.sprite = data.sprite;
        }
        
        public void SetSpriteBackGround(Sprite backGround)
        {
            _backgroundSpriteRenderer.sprite = backGround;
        }

        public void Interact()
        {
            if (IsRevealed) return;
            OnPuzzlePieceSelectedEvent?.Invoke(this);
            IsRevealed = true;
            
            //TODO make a small animation lerping the alpha values of the background sprite
        }

        public void Hide(bool shouldAnimate = false)
        {
            if (shouldAnimate)
            {
                //create coding animation here
            }
            else
            {
                _backgroundSpriteRenderer.color = new Color(1, 1, 1, 0);
            }
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
