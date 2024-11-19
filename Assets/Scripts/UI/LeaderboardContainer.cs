using Firebase;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LeaderboardContainer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _difficultyText;
        
        
        public void Init(GameSessionData data)
        {
            _nameText.text = data.playerId;
            _scoreText.text = $"{data.score}";
            this.gameObject.SetActive(true);
        }
    }
}
