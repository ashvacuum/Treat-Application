using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StarScore : MonoBehaviour
    {
        [SerializeField] private Image _starSprite;


        public void Init()
        {
            _starSprite.color = new Color(1, 1, 1, 0);
        }
        
        public void ShowStarSprite(bool withAnim)
        {
            _starSprite.DOFade(1, .9f);
        }
    }
}
