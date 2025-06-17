using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class for animating UGUIML elements programmatically
/// </summary>
public static class UGUIMLAnimationUtility
{
    #region Single Element Animations

    /// <summary>
    /// Animate a single element by name
    /// </summary>
    public static void AnimateElement(UGUIML uguiml, string elementName, AnimationType animationType, Vector3 targetValue, float speed = 1f)
    {
        UGUIMLElement element = uguiml.GetUGUIMLElement(elementName);
        if (element == null)
        {
            Debug.LogWarning($"UGUIML Animation: Element '{elementName}' not found!");
            return;
        }

        switch (animationType)
        {
            case AnimationType.Position:
                element.AnimateToPosition(targetValue, speed);
                break;
            case AnimationType.Scale:
                element.AnimateScale(targetValue, speed);
                break;
            case AnimationType.Fade:
                element.AnimateFade(targetValue.x, speed);
                break;
            case AnimationType.OffScreenLeft:
                element.AnimateOffScreen(OffScreenDirection.Left, speed);
                break;
            case AnimationType.OffScreenRight:
                element.AnimateOffScreen(OffScreenDirection.Right, speed);
                break;
            case AnimationType.OffScreenUp:
                element.AnimateOffScreen(OffScreenDirection.Up, speed);
                break;
            case AnimationType.OffScreenDown:
                element.AnimateOffScreen(OffScreenDirection.Down, speed);
                break;
            case AnimationType.ReturnToOriginal:
                element.AnimateToOriginalPosition(speed);
                break;
            case AnimationType.FadeIn:
                element.FadeIn(speed);
                break;
            case AnimationType.FadeOut:
                element.FadeOut(speed);
                break;
            case AnimationType.PopIn:
                element.RectTransform.localScale = Vector3.zero;
                element.AnimateScale(Vector3.one, speed);
                break;
        }
    }

    /// <summary>
    /// Animate multiple elements at once
    /// </summary>
    public static void AnimateElements(UGUIML uguiml, string[] elementNames, AnimationType animationType, Vector3 targetValue, float speed = 1f, float stagger = 0f)
    {
        for (int i = 0; i < elementNames.Length; i++)
        {
            if (stagger > 0 && i > 0)
            {
                // If staggering, delay each subsequent animation
                UGUIMLElement element = uguiml.GetUGUIMLElement(elementNames[i]);
                if (element != null)
                {
                    uguiml.StartCoroutine(DelayedAnimation(element, animationType, targetValue, speed, stagger * i));
                }
            }
            else
            {
                AnimateElement(uguiml, elementNames[i], animationType, targetValue, speed);
            }
        }
    }

    #endregion

    #region Group Animations

    /// <summary>
    /// Animate all elements of a specific type
    /// </summary>
    public static void AnimateElementsByType(UGUIML uguiml, string elementType, AnimationType animationType, Vector3 targetValue, float speed = 1f)
    {
        List<string> elementNames = new List<string>();

        foreach (var kvp in uguiml.allElements)
        {
            if (kvp.Value.ElementType.Equals(elementType, System.StringComparison.OrdinalIgnoreCase))
            {
                elementNames.Add(kvp.Key);
            }
        }

        AnimateElements(uguiml, elementNames.ToArray(), animationType, targetValue, speed);
    }

    /// <summary>
    /// Animate all panels in sequence
    /// </summary>
    public static void AnimatePanelsSequential(UGUIML uguiml, AnimationType animationType, Vector3 targetValue, float speed = 1f, float delay = 0.2f)
    {
        string[] panelNames = new string[uguiml.panels.Count];
        uguiml.panels.Keys.CopyTo(panelNames, 0);
        AnimateElements(uguiml, panelNames, animationType, targetValue, speed, delay);
    }

    /// <summary>
    /// Show all UI elements with staggered fade-in
    /// </summary>
    public static void ShowAllElements(UGUIML uguiml, float speed = 1f, float stagger = 0.1f)
    {
        string[] elementNames = new string[uguiml.allElements.Count];
        uguiml.allElements.Keys.CopyTo(elementNames, 0);
        AnimateElements(uguiml, elementNames, AnimationType.FadeIn, Vector3.one, speed, stagger);
    }

    /// <summary>
    /// Hide all UI elements with staggered fade-out
    /// </summary>
    public static void HideAllElements(UGUIML uguiml, float speed = 1f, float stagger = 0.1f)
    {
        string[] elementNames = new string[uguiml.allElements.Count];
        uguiml.allElements.Keys.CopyTo(elementNames, 0);
        AnimateElements(uguiml, elementNames, AnimationType.FadeOut, Vector3.zero, speed, stagger);
    }

    #endregion

    #region Preset Animations

    /// <summary>
    /// Slide root panel in from left
    /// </summary>
    public static void SlideInFromLeft(UGUIML uguiml, float speed = 2f, float stagger = 0.1f)
    {
        // Only animate root level panels (direct children of canvas)
        List<string> rootElementNames = new List<string>();
        foreach (var kvp in uguiml.allElements)
        {
            if (kvp.Value.transform.parent == uguiml.TargetCanvas.transform)
            {
                rootElementNames.Add(kvp.Key);
            }
        }

        // First move root elements off-screen left
        foreach (string elementName in rootElementNames)
        {
            UGUIMLElement element = uguiml.GetUGUIMLElement(elementName);
            if (element != null)
            {
                Vector2 offScreenPos = CalculateOffScreenPosition(uguiml, element, OffScreenDirection.Left);
                element.RectTransform.anchoredPosition = offScreenPos;
            }
        }

        // Then animate back to original positions
        AnimateElements(uguiml, rootElementNames.ToArray(), AnimationType.ReturnToOriginal, Vector3.zero, speed, stagger);
    }

    /// <summary>
    /// Slide root panel in from right
    /// </summary>
    public static void SlideInFromRight(UGUIML uguiml, float speed = 2f, float stagger = 0.1f)
    {
        List<string> rootElementNames = new List<string>();
        foreach (var kvp in uguiml.allElements)
        {
            if (kvp.Value.transform.parent == uguiml.TargetCanvas.transform)
            {
                rootElementNames.Add(kvp.Key);
            }
        }

        foreach (string elementName in rootElementNames)
        {
            UGUIMLElement element = uguiml.GetUGUIMLElement(elementName);
            if (element != null)
            {
                Vector2 offScreenPos = CalculateOffScreenPosition(uguiml, element, OffScreenDirection.Right);
                element.RectTransform.anchoredPosition = offScreenPos;
            }
        }

        AnimateElements(uguiml, rootElementNames.ToArray(), AnimationType.ReturnToOriginal, Vector3.zero, speed, stagger);
    }

    /// <summary>
    /// Slide root panel in from top
    /// </summary>
    public static void SlideInFromTop(UGUIML uguiml, float speed = 2f, float stagger = 0.1f)
    {
        List<string> rootElementNames = new List<string>();
        foreach (var kvp in uguiml.allElements)
        {
            if (kvp.Value.transform.parent == uguiml.TargetCanvas.transform)
            {
                rootElementNames.Add(kvp.Key);
            }
        }

        foreach (string elementName in rootElementNames)
        {
            UGUIMLElement element = uguiml.GetUGUIMLElement(elementName);
            if (element != null)
            {
                Vector2 offScreenPos = CalculateOffScreenPosition(uguiml, element, OffScreenDirection.Up);
                element.RectTransform.anchoredPosition = offScreenPos;
            }
        }

        AnimateElements(uguiml, rootElementNames.ToArray(), AnimationType.ReturnToOriginal, Vector3.zero, speed, stagger);
    }

    /// <summary>
    /// Slide root panel in from bottom
    /// </summary>
    public static void SlideInFromBottom(UGUIML uguiml, float speed = 2f, float stagger = 0.1f)
    {
        List<string> rootElementNames = new List<string>();
        foreach (var kvp in uguiml.allElements)
        {
            if (kvp.Value.transform.parent == uguiml.TargetCanvas.transform)
            {
                rootElementNames.Add(kvp.Key);
            }
        }

        foreach (string elementName in rootElementNames)
        {
            UGUIMLElement element = uguiml.GetUGUIMLElement(elementName);
            if (element != null)
            {
                Vector2 offScreenPos = CalculateOffScreenPosition(uguiml, element, OffScreenDirection.Down);
                element.RectTransform.anchoredPosition = offScreenPos;
            }
        }

        AnimateElements(uguiml, rootElementNames.ToArray(), AnimationType.ReturnToOriginal, Vector3.zero, speed, stagger);
    }

    /// <summary>
    /// Pop in animation with scale
    /// </summary>
    public static void PopIn(UGUIML uguiml, float speed = 3f, float stagger = 0.05f)
    {
        string[] elementNames = new string[uguiml.allElements.Count];
        uguiml.allElements.Keys.CopyTo(elementNames, 0);
        
        // Set all elements to scale 0
        foreach (string elementName in elementNames)
        {
            UGUIMLElement element = uguiml.GetUGUIMLElement(elementName);
            if (element != null)
            {
                element.RectTransform.localScale = Vector3.zero;
            }
        }

        // Animate to scale 1
        AnimateElements(uguiml, elementNames, AnimationType.Scale, Vector3.one, speed, stagger);
    }

    /// <summary>
    /// Fade in all elements
    /// </summary>
    public static void FadeIn(UGUIML uguiml, float speed = 2f, float stagger = 0.1f)
    {
        string[] elementNames = new string[uguiml.allElements.Count];
        uguiml.allElements.Keys.CopyTo(elementNames, 0);
        
        foreach (string elementName in elementNames)
        {
            UGUIMLElement element = uguiml.GetUGUIMLElement(elementName);
            if (element != null && element.CanvasGroup != null)
            {
                element.CanvasGroup.alpha = 0f;
            }
        }

        AnimateElements(uguiml, elementNames, AnimationType.FadeIn, Vector3.one, speed, stagger);
    }

    /// <summary>
    /// Fade out all elements
    /// </summary>
    public static void FadeOut(UGUIML uguiml, float speed = 2f, float stagger = 0.1f)
    {
        string[] elementNames = new string[uguiml.allElements.Count];
        uguiml.allElements.Keys.CopyTo(elementNames, 0);
        
        AnimateElements(uguiml, elementNames, AnimationType.FadeOut, Vector3.zero, speed, stagger);
    }

    /// <summary>
    /// Slide root panel to left (off-screen)
    /// </summary>
    public static void SlideToLeft(UGUIML uguiml, float speed = 2f, float stagger = 0.1f)
    {
        // Only animate root level panels (direct children of canvas)
        List<string> rootElementNames = new List<string>();
        foreach (var kvp in uguiml.allElements)
        {
            if (kvp.Value.transform.parent == uguiml.TargetCanvas.transform)
            {
                rootElementNames.Add(kvp.Key);
            }
        }

        // Animate root elements to off-screen left
        foreach (string elementName in rootElementNames)
        {
            UGUIMLElement element = uguiml.GetUGUIMLElement(elementName);
            if (element != null)
            {
                Vector2 offScreenPos = CalculateOffScreenPosition(uguiml, element, OffScreenDirection.Left);
                element.AnimateToPosition(offScreenPos, speed);
            }
        }
    }

    /// <summary>
    /// Slide root panel to right (off-screen)
    /// </summary>
    public static void SlideToRight(UGUIML uguiml, float speed = 2f, float stagger = 0.1f)
    {
        List<string> rootElementNames = new List<string>();
        foreach (var kvp in uguiml.allElements)
        {
            if (kvp.Value.transform.parent == uguiml.TargetCanvas.transform)
            {
                rootElementNames.Add(kvp.Key);
            }
        }

        foreach (string elementName in rootElementNames)
        {
            UGUIMLElement element = uguiml.GetUGUIMLElement(elementName);
            if (element != null)
            {
                Vector2 offScreenPos = CalculateOffScreenPosition(uguiml, element, OffScreenDirection.Right);
                element.AnimateToPosition(offScreenPos, speed);
            }
        }
    }

    /// <summary>
    /// Slide root panel to top (off-screen)
    /// </summary>
    public static void SlideToTop(UGUIML uguiml, float speed = 2f, float stagger = 0.1f)
    {
        List<string> rootElementNames = new List<string>();
        foreach (var kvp in uguiml.allElements)
        {
            if (kvp.Value.transform.parent == uguiml.TargetCanvas.transform)
            {
                rootElementNames.Add(kvp.Key);
            }
        }

        foreach (string elementName in rootElementNames)
        {
            UGUIMLElement element = uguiml.GetUGUIMLElement(elementName);
            if (element != null)
            {
                Vector2 offScreenPos = CalculateOffScreenPosition(uguiml, element, OffScreenDirection.Up);
                element.AnimateToPosition(offScreenPos, speed);
            }
        }
    }

    /// <summary>
    /// Slide root panel to bottom (off-screen)
    /// </summary>
    public static void SlideToBottom(UGUIML uguiml, float speed = 2f, float stagger = 0.1f)
    {
        List<string> rootElementNames = new List<string>();
        foreach (var kvp in uguiml.allElements)
        {
            if (kvp.Value.transform.parent == uguiml.TargetCanvas.transform)
            {
                rootElementNames.Add(kvp.Key);
            }
        }

        foreach (string elementName in rootElementNames)
        {
            UGUIMLElement element = uguiml.GetUGUIMLElement(elementName);
            if (element != null)
            {
                Vector2 offScreenPos = CalculateOffScreenPosition(uguiml, element, OffScreenDirection.Down);
                element.AnimateToPosition(offScreenPos, speed);
            }
        }
    }

    #endregion

    #region Private Utility Methods

    /// <summary>
    /// Calculate proper off-screen position based on canvas size and element properties
    /// </summary>
    private static Vector2 CalculateOffScreenPosition(UGUIML uguiml, UGUIMLElement element, OffScreenDirection direction)
    {
        Canvas canvas = uguiml.TargetCanvas;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        RectTransform elementRect = element.RectTransform;
        
        // Get canvas size in world space
        Vector2 canvasSize = canvasRect.sizeDelta;
        
        // Get element size and current position
        Vector2 elementSize = elementRect.sizeDelta;
        Vector2 currentPos = elementRect.anchoredPosition;
        
        // Calculate off-screen position with proper margin
        float margin = 50f; // Extra margin to ensure element is completely off-screen
        
        return direction switch
        {
            OffScreenDirection.Left => new Vector2(-canvasSize.x / 2 - elementSize.x / 2 - margin, currentPos.y),
            OffScreenDirection.Right => new Vector2(canvasSize.x / 2 + elementSize.x / 2 + margin, currentPos.y),
            OffScreenDirection.Up => new Vector2(currentPos.x, canvasSize.y / 2 + elementSize.y / 2 + margin),
            OffScreenDirection.Down => new Vector2(currentPos.x, -canvasSize.y / 2 - elementSize.y / 2 - margin),
            _ => currentPos
        };
    }

    private static System.Collections.IEnumerator DelayedAnimation(UGUIMLElement element, AnimationType animationType, Vector3 targetValue, float speed, float delay)
    {
        yield return new WaitForSeconds(delay);

        switch (animationType)
        {
            case AnimationType.Position:
                element.AnimateToPosition(targetValue, speed);
                break;
            case AnimationType.Scale:
                element.AnimateScale(targetValue, speed);
                break;
            case AnimationType.Fade:
                element.AnimateFade(targetValue.x, speed);
                break;
            case AnimationType.OffScreenLeft:
                element.AnimateOffScreen(OffScreenDirection.Left, speed);
                break;
            case AnimationType.OffScreenRight:
                element.AnimateOffScreen(OffScreenDirection.Right, speed);
                break;
            case AnimationType.OffScreenUp:
                element.AnimateOffScreen(OffScreenDirection.Up, speed);
                break;
            case AnimationType.OffScreenDown:
                element.AnimateOffScreen(OffScreenDirection.Down, speed);
                break;
            case AnimationType.ReturnToOriginal:
                element.AnimateToOriginalPosition(speed);
                break;
            case AnimationType.FadeIn:
                element.FadeIn(speed);
                break;
            case AnimationType.FadeOut:
                element.FadeOut(speed);
                break;
        }
    }

    #endregion
}

/// <summary>
/// Types of animations available for UGUIML elements
/// </summary>
public enum AnimationType
{
    Position,
    Scale,
    Fade,
    OffScreenLeft,
    OffScreenRight,
    OffScreenUp,
    OffScreenDown,
    ReturnToOriginal,
    FadeIn,
    FadeOut,
    PopIn
} 