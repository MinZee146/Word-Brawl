using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private float _x, _y, _titleJiggleDistance = 5f;
    [SerializeField] private GameObject _title;
    private RawImage _background;
    private RectTransform _titleRectTransform;

    private void Start()
    {
        _background = GetComponent<RawImage>();
        _titleRectTransform = _title.GetComponent<RectTransform>();

        StartJiggleAnimation();
    }

    private void Update()
    {
        _background.uvRect = new Rect(_background.uvRect.position + new Vector2(_x, _y) * Time.deltaTime,
            _background.uvRect.size);
    }

    private void StartJiggleAnimation()
    {
        float originalY = _titleRectTransform.position.y;

        _titleRectTransform
            .DOMoveY(originalY + _titleJiggleDistance, 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
