using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace Puzzle
{
    public enum PuzzleType
    {
        P1 = 0,
        P2 = 1 << 1,
        P3 = 1 << 2,
        P4 = 1 << 3,
        P5 = 1 << 4,
        P6 = 1 << 5,
        P7 = 1 << 6,
        P8 = 1 << 7
    }
    
    [RequireComponent(typeof(BoxCollider2D))] // require this to be interactable
    public class PuzzlePiece : MonoBehaviour, IInteractable, IEquatable<PuzzlePiece>
    {
        private SpriteRenderer _backgroundSpriteRenderer;
        private SpriteRenderer _hiddenSpriteRenderer;
        private PuzzleType _type;
        private BoxCollider2D _collider;
        
        public event Action<PuzzlePiece> OnPuzzlePieceSelectedEvent;
        public bool IsRevealed { get; private set; }
       

        private void Awake()
        {
            _backgroundSpriteRenderer = GetComponent<SpriteRenderer>();
            _hiddenSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
        }

        public void Init(PuzzleData data)
        {
            IsRevealed = false;
            _type = data.type;
            _hiddenSpriteRenderer.sprite = data.sprite;
            Hide();
            _collider.enabled = true;
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
            _backgroundSpriteRenderer.DOFade(0, .5f);
            _hiddenSpriteRenderer.DOFade(1, .6f);
        }

        public void Hide(bool shouldAnimate = false, float delay = 1)
        {
            if (shouldAnimate)
            {
                StartCoroutine(DelayedAction(delay,() =>
                {
                    _backgroundSpriteRenderer.DOFade(1, .3f);
                    _hiddenSpriteRenderer.DOFade(0, .2f);
                    ToggleCollider(true);
                    IsRevealed = false;
                    Debug.Log("Reenabling Collider with fade");
                }));
                
            }
            else
            {
                _hiddenSpriteRenderer.color = new Color(1, 1, 1, 0);
            }
        }

        public void ToggleCollider(bool isEnabled)
        {
            _collider.enabled = isEnabled;
        }

        private IEnumerator DelayedAction(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
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
