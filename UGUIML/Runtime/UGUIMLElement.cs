using System;
using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Base component for all UGUIML-created UI elements that handles animations and bindings
/// </summary>
public class UGUIMLElement : MonoBehaviour
{
    [Header("Element Data")]
    [SerializeField] private string elementName;
    [SerializeField] private string elementType;
    [SerializeField] private int bindingId = -1;
    [SerializeField] private GUIResource currentResource;

    [Header("Animation Settings")]
    [SerializeField] private float animationSpeed = 1f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private UGUIML parentUGUIML;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private XmlNode sourceNode;
    
    // Animation coroutines
    private Coroutine positionAnimation;
    private Coroutine scaleAnimation;
    private Coroutine fadeAnimation;

    // Original values for off-screen animations
    private Vector2 originalPosition;
    private Vector3 originalScale;
    private float originalAlpha;

    public string ElementName => elementName;
    public string ElementType => elementType;
    public int BindingId => bindingId;
    public RectTransform RectTransform => rectTransform;
    public CanvasGroup CanvasGroup => canvasGroup;

    #region Initialization

    public void Initialize(UGUIML uguiml, XmlNode node)
    {
        if (uguiml == null)
        {
            Debug.LogError("UGUIMLElement: UGUIML parent reference is null!");
            return;
        }

        if (node == null)
        {
            Debug.LogError("UGUIMLElement: XML node is null!");
            return;
        }

        parentUGUIML = uguiml;
        sourceNode = node;
        rectTransform = GetComponent<RectTransform>();
        
        // Get or add CanvasGroup for fade animations
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Parse element data from XML
        elementName = GetAttribute("name", "");
        elementType = node.Name.ToLower();
        bindingId = GetIntAttribute("bindId", -1);
        animationSpeed = GetFloatAttribute("animationSpeed", 1f);

        // Store original values
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
        originalAlpha = canvasGroup.alpha;
    }

    #endregion

    #region Animation Methods

    /// <summary>
    /// Animate element to a position
    /// </summary>
    public void AnimateToPosition(Vector2 targetPosition, float? customSpeed = null)
    {
        if (positionAnimation != null)
        {
            StopCoroutine(positionAnimation);
        }
        positionAnimation = StartCoroutine(AnimatePositionCoroutine(targetPosition, customSpeed ?? animationSpeed));
    }

    /// <summary>
    /// Animate element off-screen in specified direction
    /// </summary>
    public void AnimateOffScreen(OffScreenDirection direction, float? customSpeed = null)
    {
        Vector2 targetPosition = CalculateOffScreenPosition(direction);
        AnimateToPosition(targetPosition, customSpeed);
    }

    /// <summary>
    /// Animate element back to original position
    /// </summary>
    public void AnimateToOriginalPosition(float? customSpeed = null)
    {
        AnimateToPosition(originalPosition, customSpeed);
    }

    /// <summary>
    /// Animate element scale
    /// </summary>
    public void AnimateScale(Vector3 targetScale, float? customSpeed = null)
    {
        if (scaleAnimation != null)
        {
            StopCoroutine(scaleAnimation);
        }
        scaleAnimation = StartCoroutine(AnimateScaleCoroutine(targetScale, customSpeed ?? animationSpeed));
    }

    /// <summary>
    /// Animate CanvasGroup alpha
    /// </summary>
    public void AnimateFade(float targetAlpha, float? customSpeed = null)
    {
        if (fadeAnimation != null)
        {
            StopCoroutine(fadeAnimation);
        }
        fadeAnimation = StartCoroutine(AnimateFadeCoroutine(targetAlpha, customSpeed ?? animationSpeed));
    }

    /// <summary>
    /// Fade in element
    /// </summary>
    public void FadeIn(float? customSpeed = null)
    {
        AnimateFade(originalAlpha, customSpeed);
    }

    /// <summary>
    /// Fade out element
    /// </summary>
    public void FadeOut(float? customSpeed = null)
    {
        AnimateFade(0f, customSpeed);
    }

    /// <summary>
    /// Set alpha directly without animation and manage interaction settings
    /// </summary>
    public void SetAlpha(float alpha)
    {
        if (canvasGroup != null)
        {
            bool originalInteractable = canvasGroup.interactable;
            bool originalBlocksRaycasts = canvasGroup.blocksRaycasts;
            canvasGroup.alpha = alpha;
            UpdateCanvasGroupInteraction(alpha, originalInteractable, originalBlocksRaycasts);
        }
    }

    /// <summary>
    /// Set CanvasGroup interactable state
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        if (canvasGroup != null)
        {
            canvasGroup.interactable = interactable;
        }
    }

    /// <summary>
    /// Set CanvasGroup blocks raycasts state
    /// </summary>
    public void SetBlocksRaycasts(bool blocksRaycasts)
    {
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = blocksRaycasts;
        }
    }

    /// <summary>
    /// Get current alpha value
    /// </summary>
    public float GetAlpha()
    {
        return canvasGroup != null ? canvasGroup.alpha : 1f;
    }

    /// <summary>
    /// Check if element is currently interactable
    /// </summary>
    public bool IsInteractable()
    {
        return canvasGroup != null ? canvasGroup.interactable : true;
    }

    /// <summary>
    /// Check if element blocks raycasts
    /// </summary>
    public bool BlocksRaycasts()
    {
        return canvasGroup != null ? canvasGroup.blocksRaycasts : true;
    }

    /// <summary>
    /// Stop all animations
    /// </summary>
    public void StopAllAnimations()
    {
        if (positionAnimation != null)
        {
            StopCoroutine(positionAnimation);
            positionAnimation = null;
        }
        if (scaleAnimation != null)
        {
            StopCoroutine(scaleAnimation);
            scaleAnimation = null;
        }
        if (fadeAnimation != null)
        {
            StopCoroutine(fadeAnimation);
            fadeAnimation = null;
        }
    }

    #endregion

    #region Animation Coroutines

    private IEnumerator AnimatePositionCoroutine(Vector2 targetPosition, float speed)
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        float elapsed = 0f;
        float duration = Vector2.Distance(startPosition, targetPosition) / (speed * 100f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = animationCurve.Evaluate(t);
            
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, curveValue);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
        positionAnimation = null;
    }

    private IEnumerator AnimateScaleCoroutine(Vector3 targetScale, float speed)
    {
        Vector3 startScale = rectTransform.localScale;
        float elapsed = 0f;
        float duration = 1f / speed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = animationCurve.Evaluate(t);
            
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
            yield return null;
        }

        rectTransform.localScale = targetScale;
        scaleAnimation = null;
    }

    private IEnumerator AnimateFadeCoroutine(float targetAlpha, float speed)
    {
        float startAlpha = canvasGroup.alpha;
        bool startInteractable = canvasGroup.interactable;
        bool startBlocksRaycasts = canvasGroup.blocksRaycasts;
        
        float elapsed = 0f;
        float duration = Mathf.Abs(targetAlpha - startAlpha) / speed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = animationCurve.Evaluate(t);
            
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, curveValue);
            canvasGroup.alpha = currentAlpha;
            
            // Auto-manage interactable and blocksRaycasts based on alpha
            UpdateCanvasGroupInteraction(currentAlpha, startInteractable, startBlocksRaycasts);
            
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        UpdateCanvasGroupInteraction(targetAlpha, startInteractable, startBlocksRaycasts);
        fadeAnimation = null;
    }
    
    /// <summary>
    /// Update CanvasGroup interaction settings based on alpha
    /// </summary>
    private void UpdateCanvasGroupInteraction(float alpha, bool originalInteractable, bool originalBlocksRaycasts)
    {
        if (alpha <= 0f)
        {
            // When fully transparent, disable interaction
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            // When visible, restore original interaction settings
            canvasGroup.interactable = originalInteractable;
            canvasGroup.blocksRaycasts = originalBlocksRaycasts;
        }
    }

    #endregion

    #region Binding Methods

    public void SetBinding(int newBinding)
    {
        bindingId = newBinding;
        UGUIMLResources.Singleton.resources[bindingId].bindings.Add(this);
    }

    public void RemoveBinding()
    {
        UGUIMLResources.Singleton.resources[bindingId].bindings.Remove(this);
        bindingId = -1;
    }

    #endregion

    #region Utility Methods

    private Vector2 CalculateOffScreenPosition(OffScreenDirection direction)
    {
        Canvas canvas = parentUGUIML.TargetCanvas;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 canvasSize = canvasRect.sizeDelta;
        
        // Get current position and element size
        Vector2 currentPos = rectTransform.anchoredPosition;
        Vector2 elementSize = rectTransform.sizeDelta;

        return direction switch
        {
            OffScreenDirection.Left => new Vector2(-canvasSize.x / 2 - elementSize.x, currentPos.y),
            OffScreenDirection.Right => new Vector2(canvasSize.x / 2 + elementSize.x, currentPos.y),
            OffScreenDirection.Up => new Vector2(currentPos.x, canvasSize.y / 2 + elementSize.y),
            OffScreenDirection.Down => new Vector2(currentPos.x, -canvasSize.y / 2 - elementSize.y),
            _ => currentPos
        };
    }

    private string GetAttribute(string attributeName, string defaultValue)
    {
        return sourceNode?.Attributes?[attributeName]?.Value ?? defaultValue;
    }

    private float GetFloatAttribute(string attributeName, float defaultValue)
    {
        string value = GetAttribute(attributeName, defaultValue.ToString());
        return float.TryParse(value, out float result) ? result : defaultValue;
    }

    private int GetIntAttribute(string attributeName, int defaultValue)
    {
        string value = GetAttribute(attributeName, defaultValue.ToString());
        return int.TryParse(value, out int result) ? result : defaultValue;
    }

    #endregion

    #region Unity Lifecycle

    private void OnDestroy()
    {
        StopAllAnimations();
        RemoveBinding();
    }

    #endregion
}

/// <summary>
/// Directions for off-screen animations
/// </summary>
public enum OffScreenDirection
{
    Left,
    Right,
    Up,
    Down
} 