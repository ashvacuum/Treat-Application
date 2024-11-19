using UnityEngine;

namespace UI
{
    public class TimeManager : MonoBehaviour
    {
        private bool _isTimerRunning;
        private float _timeElapsed;
        private float _targetTime;

        private void OnEnable()
        {
            EventBus.Subscribe<TimerStartEvent>(OnTimerStart);
            EventBus.Subscribe<GameEndEvent>(OnGameEnded);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<TimerStartEvent>(OnTimerStart);
            EventBus.Unsubscribe<GameEndEvent>(OnGameEnded);
        }

        private void OnTimerStart(TimerStartEvent evt)
        {
            _targetTime = evt.TimeLeft;
        }
    
        private void OnGameEnded(GameEndEvent evt)
        {
            StopTimer();
        }

        private void StopTimer()
        {
            _isTimerRunning = false;
            _timeElapsed = 0;
        
        }

        void FixedUpdate()
        {
            if (!_isTimerRunning) return;
        
            _timeElapsed += Time.fixedDeltaTime;
        
            if (_timeElapsed >= _targetTime)
            {
                StopTimer();
            }
            else
            {
                EventBus.Publish(new TimerUpdateEvent(_targetTime - _timeElapsed));
            }
        }
    }
}