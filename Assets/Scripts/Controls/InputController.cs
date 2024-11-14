using Puzzle;
using UnityEngine;

namespace Controls
{
    public class InputController : MonoBehaviour
    {
        private void OnMouseUp()
        {
            if (Camera.main == null) return;
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider == null) return;
            Debug.Log ("Target Position: " + hit.collider.gameObject.transform.position);
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Interact();
            }
        }
    }
}
