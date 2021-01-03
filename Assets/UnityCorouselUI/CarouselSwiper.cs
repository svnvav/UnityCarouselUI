
using System.Collections;
using System.Linq;
using UnityCarouselUI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityCarouselUI
{

    public class CarouselSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform _pagesContainer;
        [SerializeField] private Transform _indicatorsContainer;
        [SerializeField] private float _percentThreshold = 0.3f;
        [SerializeField] private float _easing = 0.3f;

        private CarouselIndicator[] _indicators;
        private float _locationAnchorMinX;
        private float _locationAnchorMaxX;
        private int _pageCount;
        private int _currentPage;

        private float PageWidth => _pagesContainer.rect.width;

        void Start()
        {
            _locationAnchorMinX = _pagesContainer.anchorMin.x;
            _locationAnchorMaxX = _pagesContainer.anchorMax.x;
            _pageCount = _pagesContainer.childCount;
            
            _indicators = _indicatorsContainer.GetComponentsInChildren<CarouselIndicator>();
            foreach (var indicator in _indicators)
            {
                indicator.SetTransparency(0);
            }
            _indicators[_currentPage].SetTransparency(1);
        }

        public void OnDrag(PointerEventData eventData)
        {
            var difference = (eventData.pressPosition.x - eventData.position.x) / PageWidth;
            _pagesContainer.anchorMin = new Vector2(_locationAnchorMinX - difference, _pagesContainer.anchorMin.y);
            _pagesContainer.anchorMax = new Vector2(_locationAnchorMaxX - difference, _pagesContainer.anchorMax.y);
            
            if (difference > 0 && _currentPage < _pageCount - 1)
            {
                _indicators[_currentPage].SetTransparency(1 - difference);
                _indicators[_currentPage + 1].SetTransparency(difference);
            }
            else if (difference < 0 && _currentPage > 0)
            {
                _indicators[_currentPage].SetTransparency(1 + difference);
                _indicators[_currentPage - 1].SetTransparency(-difference);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var percentage = (eventData.pressPosition.x - eventData.position.x) / PageWidth;
            if (Mathf.Abs(percentage) >= _percentThreshold)
            {
                if (percentage > 0 && _currentPage < _pageCount - 1)
                {
                    _currentPage++;
                }
                else if (percentage < 0 && _currentPage > 0)
                {
                    _currentPage--;
                }

                _locationAnchorMinX = -_currentPage;
                _locationAnchorMaxX = -_currentPage + 1;
            }

            StartCoroutine(SmoothMove(_pagesContainer.anchorMin.x, _pagesContainer.anchorMax.x, _easing));
        }

        private IEnumerator SmoothIndicatorsChange(float seconds)
        {
            var t = 0f;
            var startTransparency = _indicators.Select(item => item.CurrentTransparency).ToArray();
            while (t <= 1.0f)
            {
                t += Time.deltaTime / seconds;

                for (int i = 0; i < _indicators.Length; i++)
                {
                    if (i == _currentPage)
                    {
                        //_indicators[i].SetTransparency();
                    }
                }
                
                yield return null;
            }
        }
        
        private IEnumerator SmoothMove(float startAnchorMinX, float startAnchorMaxX, float seconds)
        {
            var t = 0f;
            while (t <= 1.0f)
            {
                t += Time.deltaTime / seconds;
                _pagesContainer.anchorMin = new Vector2(Mathf.Lerp(startAnchorMinX, _locationAnchorMinX, t),
                    _pagesContainer.anchorMin.y);
                _pagesContainer.anchorMax = new Vector2(Mathf.Lerp(startAnchorMaxX, _locationAnchorMaxX, t),
                    _pagesContainer.anchorMax.y);
                yield return null;
            }
        }
    }
}
