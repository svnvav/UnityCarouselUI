
using UnityEngine;
using UnityEngine.UI;

namespace UnityCarouselUI
{
    public class CarouselIndicator : MonoBehaviour
    {
        [SerializeField] private Image _dot;

        private float _initialTransparency;

        public float CurrentTransparency => _dot.color.a;

        private void Start()
        {
            _initialTransparency = _dot.color.a;
        }

        public void SetTransparency(float value)
        {
            var color = _dot.color;
            color.a = value * _initialTransparency;
            _dot.color = color;
        }
    }
}