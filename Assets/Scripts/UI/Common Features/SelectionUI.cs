using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class SelectionUI : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private static float duration = 3f;

    [SerializeField]
    protected Image highlightImage;

    private const float MAXALPHA = 0.75f;

    private const float MINALPHA = 0.15f;

    private bool stop = false;

    private Coroutine currentCoroutine;

    public SelectorManager ParentSelector { get; private set; }

    protected virtual void Start()
    {
        ParentSelector = transform.parent.GetComponent<SelectorManager>();
    }

    public virtual void Highlight()
    {
        if (highlightImage != null && currentCoroutine == null)
        {
            stop = false;
            highlightImage.color = new Color(1, 1, 1, MINALPHA);
            StartFlashing();
        }
    }

    public virtual void UnHighlight()
    {
        if (highlightImage != null)
        {
            stop = true;
            StopFlashing();
            highlightImage.color = new Color(1, 1, 1, MINALPHA);
        }
    }

    public void StartFlashing()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(FlashRoutine());
    }

    public void StopFlashing()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }

    private IEnumerator FlashRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(Flash(MAXALPHA));
            //yield return new WaitForSeconds(delay);
            yield return StartCoroutine(Flash(MINALPHA));
        }
    }

    private IEnumerator Flash(float targetAlpha)
    {
        Color startColor = highlightImage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
        float time = 0f;

        while (time < duration && !stop)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            highlightImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
    }

    private void OnUIHover()
    {
        ParentSelector?.UIHover(this);
    }

    private void OnUIClicked()
    {
        ParentSelector?.UIClicked(this);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        OnUIHover();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        OnUIClicked();
    }
}
