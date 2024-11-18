using System;
using UnityEngine;

namespace Firebase
{
    public class GameSessionManager : MonoBehaviour
    {
        private IFirestoreService firestoreService;

        private GameSessionData _currentGameSession;

        private void Awake()
        {
            firestoreService = ServiceLocator.Instance.GetService<IFirestoreService>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameStartEvent>(OnGameStart);
            EventBus.Subscribe<GameEndEvent>(OnGameEnd);
            EventBus.Subscribe<GameQuitEvent>(OnGameQuit);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStartEvent>(OnGameStart);
            EventBus.Unsubscribe<GameEndEvent>(OnGameEnd);
            EventBus.Unsubscribe<GameQuitEvent>(OnGameQuit);
        }

        private void OnGameStart(GameStartEvent evt)
        {
            _currentGameSession = new GameSessionData()
            {
                playerId = evt.Username,
                score = 0
            };
        }

        private void OnGameEnd(GameEndEvent evt)
        {
            _currentGameSession.score = evt.FinalScore;

            if (firestoreService != null)
            {
                firestoreService.SavePlayerScore(_currentGameSession.playerId, _currentGameSession.playerId,
                    _currentGameSession.score);
            }
        }

        private void OnGameQuit(GameQuitEvent evt)
        {
            _currentGameSession = null;
        }
    }
    
    [Serializable]
    public class GameSessionData
    {
        public string playerId;
        public int score;
    }
}
