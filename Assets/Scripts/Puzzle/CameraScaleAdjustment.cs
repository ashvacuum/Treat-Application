using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Puzzle
{
    public class CameraScaleAdjustment : MonoBehaviour
    {
        [SerializeField] private float _cameraOffset = 3;
        [SerializeField] private float _padding = 2;
        
        private float _screenWidth;
        private float _screenHeight;
        private Camera cam;

        private Action<GameStartEvent> gameStarthandler;
        // Use this for initialization
        private void Awake() {
            cam = Camera.main;
            _screenHeight = Screen.height;
            _screenWidth = Screen.width;

            gameStarthandler = OnGameStart;
        }

        private void OnEnable()
        {
            EventBus.Subscribe(gameStarthandler);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe(gameStarthandler);
        }

        private void OnGameStart(GameStartEvent start)
        {
            //RepositionCamera();
        }
        

        /// <summary>
        /// Manages camera positioning when generating boards
        /// </summary>
        /// <param name="gridSize"></param>
        public void RepositionCamera(int gridSize) {
            var tempPos = new Vector3((gridSize-1) * .5f, (gridSize-1) * .5f, -_cameraOffset);
            transform.position = tempPos;
            //Camera.main.orthographicSize = (board.width >= board.height) ? (board.width / 2 + padding) / aspectRatio : board.height / 2 + padding;

            if (cam != null)
            {
                cam.orthographicSize = gridSize * .5f + (_padding % 7);
            }

        }
    }
}
