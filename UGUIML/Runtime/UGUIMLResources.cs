using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class UGUIMLResources : MonoBehaviour
{
    public static UGUIMLResources Singleton;
    public List<GUIResource> resources = new List<GUIResource>(64);

    public Dictionary<string, CanvasGroup> panels = new Dictionary<string, CanvasGroup>();
    
    [Header("Default UI Resources (Used when bindId = -1)")]
    [SerializeField] private Sprite defaultButtonBackground;
    [SerializeField] private Sprite defaultPanelBackground;
    [SerializeField] private Texture2D defaultImageTexture;
    [SerializeField] private TMP_FontAsset defaultFont;
    [SerializeField] private Color defaultTextColor = Color.white;
    [SerializeField] private float defaultFontSize = 14f;
    [SerializeField] private Color defaultButtonColor = Color.white;
    [SerializeField] private Color defaultPanelColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    
    [Header("Rich Component Default Resources")]
    [SerializeField] private Sprite defaultCheckboxBackground;
    [SerializeField] private Sprite defaultCheckboxCheckmark;
    [SerializeField] private Color defaultCheckboxColor = Color.white;
    [SerializeField] private Color defaultCheckmarkColor = new Color(0.2f, 0.6f, 0.9f);
    [SerializeField] private Sprite defaultInputFieldBackground;
    [SerializeField] private Color defaultInputFieldColor = new Color(0.17f, 0.24f, 0.31f);
    [SerializeField] private Sprite defaultDropdownBackground;
    [SerializeField] private Sprite defaultDropdownArrow;
    [SerializeField] private Color defaultDropdownColor = new Color(0.17f, 0.24f, 0.31f);
    [SerializeField] private Sprite defaultScrollbarBackground;
    [SerializeField] private Sprite defaultScrollbarHandle;
    [SerializeField] private Color defaultScrollbarBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    [SerializeField] private Color defaultScrollbarHandleColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);
    [SerializeField] private Color defaultProgressBarBackgroundColor = new Color(0.2f, 0.3f, 0.4f);
    [SerializeField] private Color defaultProgressBarFillColor = new Color(0.2f, 0.6f, 0.9f);
    
    public Sprite DefaultButtonBackground => defaultButtonBackground;
    public Sprite DefaultPanelBackground => defaultPanelBackground;
    public Texture2D DefaultImageTexture => defaultImageTexture;
    public TMP_FontAsset DefaultFont => defaultFont;
    public Color DefaultTextColor => defaultTextColor;
    public float DefaultFontSize => defaultFontSize;
    public Color DefaultButtonColor => defaultButtonColor;
    public Color DefaultPanelColor => defaultPanelColor;
    
    // Rich Component Properties
    public Sprite DefaultCheckboxBackground => defaultCheckboxBackground;
    public Sprite DefaultCheckboxCheckmark => defaultCheckboxCheckmark;
    public Color DefaultCheckboxColor => defaultCheckboxColor;
    public Color DefaultCheckmarkColor => defaultCheckmarkColor;
    public Sprite DefaultInputFieldBackground => defaultInputFieldBackground;
    public Color DefaultInputFieldColor => defaultInputFieldColor;
    public Sprite DefaultDropdownBackground => defaultDropdownBackground;
    public Sprite DefaultDropdownArrow => defaultDropdownArrow;
    public Color DefaultDropdownColor => defaultDropdownColor;
    public Sprite DefaultScrollbarBackground => defaultScrollbarBackground;
    public Sprite DefaultScrollbarHandle => defaultScrollbarHandle;
    public Color DefaultScrollbarBackgroundColor => defaultScrollbarBackgroundColor;
    public Color DefaultScrollbarHandleColor => defaultScrollbarHandleColor;
    public Color DefaultProgressBarBackgroundColor => defaultProgressBarBackgroundColor;
    public Color DefaultProgressBarFillColor => defaultProgressBarFillColor;

    void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void MoveResourceBinding(int from, int to, int index)
    {
        resources[to].bindings.Add(resources[from].bindings[index]);
        resources[from].bindings.RemoveAt(index);
    }

    public void ChangeElementBinding(UGUIML uguiml, string elementName, int newBinding)
    {
        UGUIMLElement element = uguiml.GetUGUIMLElement(elementName);
        if (element == null)
        {
            Debug.LogWarning($"UGUIML Animation: Element '{elementName}' not found!");
            return;
        }

        element.RemoveBinding();
        element.SetBinding(newBinding);
    }
}

[Serializable]
public class GUIResource
{
    public GUIResourceTypes resourceType = GUIResourceTypes.Undefined;

    [Header("Text Resources")]
    public string txt;
    public TMP_ColorGradient gradient;

    [Header("Image Resources")]
    public Texture img;
    public string src;
    public SourceTypes sourceType;
    public bool isLoaded;
    public List<UGUIMLElement> bindings = new List<UGUIMLElement>();

    // [Header("Other Resource")]
    // public Color color;
    // public Sprite sprite;

    public void SyncBindings()
    {
        foreach (UGUIMLElement binding in bindings)
        {
            switch (resourceType)
            {
                case GUIResourceTypes.Button:

                    break;

                case GUIResourceTypes.Image:

                    TMP_Text textComponent = binding.GetComponent<TMP_Text>();
                    if (textComponent != null)
                    {
                        textComponent.text = txt;
                        textComponent.colorGradientPreset = gradient;
                        return;
                    }

                    break;

                case GUIResourceTypes.RawImage:

                    RawImage imageComponent = binding.GetComponent<RawImage>();
                    if (imageComponent != null)
                    {
                        imageComponent.texture = img;
                        return;
                    }

                    break;

                case GUIResourceTypes.Text:

                    break;

                case GUIResourceTypes.Undefined:

                    break;
            }
        }
    }

    public void SetText(string newText)
    {
        txt = newText;
        resourceType = GUIResourceTypes.Text;
        SyncBindings();
    }

    public void SetGradient(string topLeft, string topRight, string bottomLeft, string bottomRight)
    {
        ColorUtility.TryParseHtmlString(topLeft, out gradient.topLeft);
        ColorUtility.TryParseHtmlString(topRight, out gradient.topLeft);
        ColorUtility.TryParseHtmlString(bottomRight, out gradient.bottomRight);
        ColorUtility.TryParseHtmlString(bottomLeft, out gradient.bottomLeft);
        resourceType = GUIResourceTypes.Text;
        SyncBindings();
    }

    public async void LoadSource(string src, SourceTypes sourceTypes)
    {
        isLoaded = false;
        switch (sourceTypes)
        {
            case SourceTypes.Addressable:
                // Load addressable
                break;
            case SourceTypes.AssetBundle:
                // Fetch asset from bundle
                break;
            case SourceTypes.LocalFile:
                // Load local image file
                break;
            case SourceTypes.Resource:
                break;
            case SourceTypes.URL:
                // Load from web request
                break;
        }
        isLoaded = true;
        resourceType = GUIResourceTypes.RawImage;
        SyncBindings();
    }
}
public enum SourceTypes { Addressable, AssetBundle, LocalFile, Resource, URL }
public enum GUIResourceTypes { Text, RawImage, Image, Button, Undefined }