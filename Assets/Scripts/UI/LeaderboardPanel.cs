using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class LeaderboardPanel : MonoBehaviour
    {
        [SerializeField] private List<LeaderboardContainer> _leaderBoardContainer;
        [SerializeField] private RectTransform _spinner;
        [SerializeField] private float _rotationSpeed = 2f;
        [SerializeField] private Button _closeButton;
        private IFirestoreService _firestoreService;
        
        private void Awake()
        {
            _firestoreService = ServiceLocator.Instance.GetService<IFirestoreService>();
        }

        private async void OnEnable()
        {
            _closeButton.onClick.AddListener(ClosePanel);
            
            _spinner.gameObject.SetActive(true);
            
            foreach (var leaderboardContainer in _leaderBoardContainer)
            {
                leaderboardContainer.gameObject.SetActive(false);
            }

            try
            {
                var results = await _firestoreService.QueryHighScores(1);

                for (var i = 0; i < results.Count; i++)
                {
                    _leaderBoardContainer[i].Init(results[i]);
                }
            }
            finally
            {
                _spinner.gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(ClosePanel);
        }

        private void ClosePanel()
        {
            this.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_spinner == null || _spinner.gameObject.activeSelf)
            {
                _spinner.Rotate(0f, 0f, -_rotationSpeed * Time.deltaTime);
            }
        }
        
    }
}
