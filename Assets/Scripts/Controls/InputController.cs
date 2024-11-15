using System;
using Puzzle;
using UnityEngine;

namespace Controls
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private LayerMask _interactableLayer;
        [SerializeField]private float _maxRayDistance = 100f;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            // Check for touch input
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);

                // Handle touch began phase
                if (touch.phase == TouchPhase.Began)
                {
                    HandleTouch(touch.position);
                }
            }

            // Optional: Also handle mouse input for testing in editor
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                HandleTouch(Input.mousePosition);
            }
#endif
        }
        
        private void HandleTouch(Vector2 screenPosition)
        {
            // Create ray from camera through touch position
            if (_camera == null) return;
            var ray = _camera.ScreenPointToRay(screenPosition);
            var hit = Physics2D.Raycast(ray.origin, ray.direction, _maxRayDistance, _interactableLayer);

            // Check if we hit something
            if (hit.collider == null) return;
            // Try to get IInteractable component
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Interact();
            }
        }

        // Optional: Draw ray in scene view for debugging
        private void OnDrawGizmos()
        {
            if (Camera.main == null || Input.touchCount <= 0) return;
            
            var ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray.origin, ray.direction * _maxRayDistance);
        }

        private void OnMouseUp()
        {
            if (Camera.main == null) return;
            var pos = Input.mousePosition;
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);
            Debug.Log($"Raycast on {pos}");
            if (hit.collider == null) return;
            Debug.Log ("Target Position: " + hit.collider.gameObject.transform.position);
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Interact();
            }
        }
    }
}
