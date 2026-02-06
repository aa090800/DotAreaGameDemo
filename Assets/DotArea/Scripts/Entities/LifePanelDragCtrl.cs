using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LifePanelDragCtrl : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    Vector2 OrignalPos;
    RectTransform rectTransform;
    Canvas canvas;
    public Toggle toggle;
    DotAreaUICtrl UICtrl;
    DotAreaGameManager game;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        OrignalPos = rectTransform.anchoredPosition;
        canvas = GetComponentInParent<Canvas>();
        UICtrl = canvas.GetComponent<DotAreaUICtrl>();
    }
    private void Start()
    {
        game = DotAreaGameManager.Instance;

        toggle.onValueChanged.AddListener((isOn) => 
        {
            rectTransform.anchoredPosition = OrignalPos;
            UICtrl.GlowLiftPanel(isOn);
            game.isCheating = isOn;
        });
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
       
    }

    bool back;
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        float dist = Vector2.Distance(rectTransform.anchoredPosition, OrignalPos);

        if (dist >110f) back = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!back) rectTransform.anchoredPosition = OrignalPos;
        back = false;
    }
}
