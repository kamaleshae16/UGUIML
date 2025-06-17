using System.IO;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Editor utility for exporting Canvas hierarchies to UGUIML XML format
/// </summary>
public class UGUIMLExporter : EditorWindow
{
    private Canvas targetCanvas;
    private string outputPath = "Assets/UGUIML_Export.xml";
    private bool includeInactiveObjects = true;
    private bool preserveBindIds = true;
    private Vector2 scrollPosition;

    [MenuItem("Tools/UGUIML/Export Canvas to UGUIML")]
    public static void ShowWindow()
    {
        GetWindow<UGUIMLExporter>("UGUIML Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Label("UGUIML Canvas Exporter", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Canvas selection
        targetCanvas = (Canvas)EditorGUILayout.ObjectField("Target Canvas", targetCanvas, typeof(Canvas), true);
        
        if (targetCanvas == null)
        {
            EditorGUILayout.HelpBox("Please select a Canvas to export.", MessageType.Info);
            return;
        }

        GUILayout.Space(10);

        // Export options
        GUILayout.Label("Export Options", EditorStyles.boldLabel);
        outputPath = EditorGUILayout.TextField("Output Path", outputPath);
        includeInactiveObjects = EditorGUILayout.Toggle("Include Inactive Objects", includeInactiveObjects);
        preserveBindIds = EditorGUILayout.Toggle("Preserve Bind IDs", preserveBindIds);

        GUILayout.Space(10);

        // Browse button for output path
        if (GUILayout.Button("Browse Output Location"))
        {
            string path = EditorUtility.SaveFilePanel("Save UGUIML File", "Assets", "export", "xml");
            if (!string.IsNullOrEmpty(path))
            {
                // Convert absolute path to relative if it's within the project
                if (path.StartsWith(Application.dataPath))
                {
                    outputPath = "Assets" + path.Substring(Application.dataPath.Length);
                }
                else
                {
                    outputPath = path;
                }
            }
        }

        GUILayout.Space(10);

        // Preview area
        GUILayout.Label("Preview", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
        
        if (targetCanvas != null)
        {
            EditorGUILayout.TextArea(GeneratePreview(), GUILayout.ExpandHeight(true));
        }
        
        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        // Export button
        GUI.enabled = targetCanvas != null && !string.IsNullOrEmpty(outputPath);
        if (GUILayout.Button("Export to UGUIML", GUILayout.Height(30)))
        {
            ExportCanvas();
        }
        GUI.enabled = true;
    }

    private string GeneratePreview()
    {
        if (targetCanvas == null) return "";
        
        try
        {
            XmlDocument doc = GenerateXmlDocument();
            
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings 
                { 
                    Indent = true, 
                    IndentChars = "  ",
                    OmitXmlDeclaration = true
                }))
                {
                    doc.WriteTo(xmlWriter);
                }
                return stringWriter.ToString();
            }
        }
        catch (System.Exception e)
        {
            return $"Error generating preview: {e.Message}";
        }
    }

    private void ExportCanvas()
    {
        try
        {
            XmlDocument doc = GenerateXmlDocument();
            
            // Save to file
            using (var writer = XmlWriter.Create(outputPath, new XmlWriterSettings 
            { 
                Indent = true, 
                IndentChars = "  ",
                Encoding = Encoding.UTF8
            }))
            {
                doc.WriteTo(writer);
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Export Complete", $"UGUIML file exported to:\n{outputPath}", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Export Failed", $"Failed to export UGUIML file:\n{e.Message}", "OK");
        }
    }

    private XmlDocument GenerateXmlDocument()
    {
        XmlDocument doc = new XmlDocument();
        
        // Create XML declaration
        XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        doc.AppendChild(declaration);
        
        // Create root UGUIML element
        XmlElement root = doc.CreateElement("UGUIML");
        doc.AppendChild(root);
        
        // Process all child transforms of the canvas
        for (int i = 0; i < targetCanvas.transform.childCount; i++)
        {
            Transform child = targetCanvas.transform.GetChild(i);
            if (includeInactiveObjects || child.gameObject.activeInHierarchy)
            {
                ProcessTransform(child, root, doc);
            }
        }
        
        return doc;
    }

    private void ProcessTransform(Transform transform, XmlElement parentElement, XmlDocument doc)
    {
        GameObject gameObject = transform.gameObject;
        
        // Determine element type and create appropriate XML node
        XmlElement element = CreateElementFromGameObject(gameObject, doc);
        if (element != null)
        {
            parentElement.AppendChild(element);
            
            // Process children
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (includeInactiveObjects || child.gameObject.activeInHierarchy)
                {
                    // Skip UI components that are structural (like ScrollView Viewport/Content)
                    if (!IsStructuralUIElement(child))
                    {
                        ProcessTransform(child, element, doc);
                    }
                }
            }
        }
    }

    private bool IsStructuralUIElement(Transform transform)
    {
        string name = transform.name.ToLower();
        return name == "viewport" || name == "content" || name == "sliding area" || 
               name == "handle" || name == "background" || name == "checkmark" || 
               name == "text area" || name == "placeholder" || name == "label" || 
               name == "arrow" || name == "template" || name == "item" ||
               name == "item checkmark" || name == "item label" || name == "fill area" ||
               name == "fill" || name == "handle slide area";
    }

    private XmlElement CreateElementFromGameObject(GameObject gameObject, XmlDocument doc)
    {
        XmlElement element = null;
        string elementName = gameObject.name;
        
        // Check for different component types in order of priority
        if (gameObject.GetComponent<TMP_Dropdown>() != null)
        {
            element = CreateDropdownElement(gameObject, doc);
        }
        else if (gameObject.GetComponent<TMP_InputField>() != null)
        {
            element = CreateInputFieldElement(gameObject, doc);
        }
        else if (gameObject.GetComponent<Toggle>() != null)
        {
            element = CreateToggleElement(gameObject, doc);
        }
        else if (gameObject.GetComponent<ToggleGroup>() != null)
        {
            element = CreateToggleGroupElement(gameObject, doc);
        }
        else if (gameObject.GetComponent<Slider>() != null)
        {
            element = CreateProgressBarElement(gameObject, doc);
        }
        else if (gameObject.GetComponent<ScrollRect>() != null)
        {
            element = CreateScrollViewElement(gameObject, doc);
        }
        else if (gameObject.GetComponent<Button>() != null)
        {
            element = CreateButtonElement(gameObject, doc);
        }
        else if (gameObject.GetComponent<TMP_Text>() != null || gameObject.GetComponent<Text>() != null)
        {
            element = CreateTextElement(gameObject, doc);
        }
        else if (gameObject.GetComponent<RawImage>() != null)
        {
            element = CreateImageElement(gameObject, doc);
        }
        else if (gameObject.GetComponent<GridLayoutGroup>() != null)
        {
            element = CreateGridLayoutElement(gameObject, doc);
        }
        else if (gameObject.GetComponent<VerticalLayoutGroup>() != null)
        {
            element = CreateVerticalLayoutElement(gameObject, doc);
        }
        else if (gameObject.GetComponent<HorizontalLayoutGroup>() != null)
        {
            element = CreateHorizontalLayoutElement(gameObject, doc);
        }
        else if (gameObject.GetComponent<CanvasGroup>() != null || gameObject.GetComponent<Image>() != null)
        {
            element = CreatePanelElement(gameObject, doc);
        }
        else
        {
            // Generic panel for any other UI element with RectTransform
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            if (rect != null)
            {
                element = CreatePanelElement(gameObject, doc);
            }
        }

        if (element != null)
        {
            element.SetAttribute("name", elementName);
            AddRectTransformAttributes(element, gameObject.GetComponent<RectTransform>());
            
            // Add bindId if preserving and UGUIMLElement exists
            if (preserveBindIds)
            {
                UGUIMLElement uguimlElement = gameObject.GetComponent<UGUIMLElement>();
                if (uguimlElement != null && uguimlElement.BindingId >= 0)
                {
                    element.SetAttribute("bindId", uguimlElement.BindingId.ToString());
                }
            }
        }

        return element;
    }

    private XmlElement CreatePanelElement(GameObject gameObject, XmlDocument doc)
    {
        XmlElement element = doc.CreateElement("panel");
        
        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            element.SetAttribute("alpha", canvasGroup.alpha.ToString("F2"));
            element.SetAttribute("interactable", canvasGroup.interactable.ToString().ToLower());
            element.SetAttribute("blocksRaycasts", canvasGroup.blocksRaycasts.ToString().ToLower());
        }
        
        Image image = gameObject.GetComponent<Image>();
        if (image != null)
        {
            element.SetAttribute("backgroundColor", ColorToHex(image.color));
            element.SetAttribute("raycastTarget", image.raycastTarget.ToString().ToLower());
        }
        
        return element;
    }

    private XmlElement CreateTextElement(GameObject gameObject, XmlDocument doc)
    {
        XmlElement element = doc.CreateElement("text");
        
        TMP_Text tmpText = gameObject.GetComponent<TMP_Text>();
        Text legacyText = gameObject.GetComponent<Text>();
        
        if (tmpText != null)
        {
            element.SetAttribute("text", tmpText.text);
            element.SetAttribute("fontSize", tmpText.fontSize.ToString("F1"));
            element.SetAttribute("color", ColorToHex(tmpText.color));
            element.SetAttribute("alignment", TextAlignmentToString(tmpText.alignment));
            element.SetAttribute("raycastTarget", tmpText.raycastTarget.ToString().ToLower());
        }
        else if (legacyText != null)
        {
            element.SetAttribute("text", legacyText.text);
            element.SetAttribute("fontSize", legacyText.fontSize.ToString());
            element.SetAttribute("color", ColorToHex(legacyText.color));
            element.SetAttribute("alignment", TextAnchorToString(legacyText.alignment));
            element.SetAttribute("raycastTarget", legacyText.raycastTarget.ToString().ToLower());
        }
        
        return element;
    }

    private XmlElement CreateButtonElement(GameObject gameObject, XmlDocument doc)
    {
        XmlElement element = doc.CreateElement("button");
        
        Button button = gameObject.GetComponent<Button>();
        Image image = gameObject.GetComponent<Image>();
        UGUIMLButton uguimlButton = gameObject.GetComponent<UGUIMLButton>();
        
        if (image != null)
        {
            element.SetAttribute("backgroundColor", ColorToHex(image.color));
            element.SetAttribute("raycastTarget", image.raycastTarget.ToString().ToLower());
        }
        
        if (uguimlButton != null)
        {
            element.SetAttribute("command", uguimlButton.Command);
        }
        
        // Look for text child
        TMP_Text textComponent = gameObject.GetComponentInChildren<TMP_Text>();
        if (textComponent != null)
        {
            element.SetAttribute("text", textComponent.text);
            element.SetAttribute("fontSize", textComponent.fontSize.ToString("F1"));
            element.SetAttribute("textColor", ColorToHex(textComponent.color));
        }
        
        return element;
    }

    private XmlElement CreateImageElement(GameObject gameObject, XmlDocument doc)
    {
        XmlElement element = doc.CreateElement("image");
        
        RawImage rawImage = gameObject.GetComponent<RawImage>();
        if (rawImage != null)
        {
            element.SetAttribute("color", ColorToHex(rawImage.color));
            element.SetAttribute("raycastTarget", rawImage.raycastTarget.ToString().ToLower());
        }
        
        return element;
    }

    private XmlElement CreateScrollViewElement(GameObject gameObject, XmlDocument doc)
    {
        XmlElement element = doc.CreateElement("scrollview");
        
        ScrollRect scrollRect = gameObject.GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            element.SetAttribute("horizontal", scrollRect.horizontal.ToString().ToLower());
            element.SetAttribute("vertical", scrollRect.vertical.ToString().ToLower());
            element.SetAttribute("elasticity", scrollRect.elasticity.ToString("F2"));
            element.SetAttribute("inertia", scrollRect.inertia.ToString().ToLower());
            element.SetAttribute("deceleration", scrollRect.decelerationRate.ToString("F3"));
            element.SetAttribute("scrollSensitivity", scrollRect.scrollSensitivity.ToString("F1"));
            
            element.SetAttribute("showVerticalScrollbar", (scrollRect.verticalScrollbar != null).ToString().ToLower());
            element.SetAttribute("showHorizontalScrollbar", (scrollRect.horizontalScrollbar != null).ToString().ToLower());
        }
        
        Image image = gameObject.GetComponent<Image>();
        if (image != null)
        {
            element.SetAttribute("backgroundColor", ColorToHex(image.color));
        }
        
        return element;
    }

    private XmlElement CreateProgressBarElement(GameObject gameObject, XmlDocument doc)
    {
        XmlElement element = doc.CreateElement("progressbar");
        
        Slider slider = gameObject.GetComponent<Slider>();
        if (slider != null)
        {
            element.SetAttribute("minValue", slider.minValue.ToString("F2"));
            element.SetAttribute("maxValue", slider.maxValue.ToString("F2"));
            element.SetAttribute("value", slider.value.ToString("F2"));
            element.SetAttribute("interactable", slider.interactable.ToString().ToLower());
            element.SetAttribute("showHandle", (slider.handleRect != null).ToString().ToLower());
        }
        
        // Try to find background and fill colors
        Image[] images = gameObject.GetComponentsInChildren<Image>();
        foreach (var img in images)
        {
            if (img.transform.name.ToLower().Contains("background"))
            {
                element.SetAttribute("backgroundColor", ColorToHex(img.color));
            }
            else if (img.transform.name.ToLower().Contains("fill"))
            {
                element.SetAttribute("fillColor", ColorToHex(img.color));
            }
        }
        
        return element;
    }

    private XmlElement CreateToggleGroupElement(GameObject gameObject, XmlDocument doc)
    {
        XmlElement element = doc.CreateElement("togglegroup");
        
        ToggleGroup toggleGroup = gameObject.GetComponent<ToggleGroup>();
        if (toggleGroup != null)
        {
            element.SetAttribute("allowSwitchOff", toggleGroup.allowSwitchOff.ToString().ToLower());
        }
        
        return element;
    }

    private XmlElement CreateToggleElement(GameObject gameObject, XmlDocument doc)
    {
        XmlElement element = doc.CreateElement("toggle");
        
        Toggle toggle = gameObject.GetComponent<Toggle>();
        if (toggle != null)
        {
            element.SetAttribute("isOn", toggle.isOn.ToString().ToLower());
            
            if (toggle.group != null)
            {
                element.SetAttribute("group", toggle.group.name);
            }
        }
        
        // Look for label text
        TMP_Text textComponent = gameObject.GetComponentInChildren<TMP_Text>();
        if (textComponent != null && textComponent.transform.name.ToLower().Contains("label"))
        {
            element.SetAttribute("text", textComponent.text);
            element.SetAttribute("fontSize", textComponent.fontSize.ToString("F1"));
            element.SetAttribute("textColor", ColorToHex(textComponent.color));
        }
        
        return element;
    }

    private XmlElement CreateInputFieldElement(GameObject gameObject, XmlDocument doc)
    {
        XmlElement element = doc.CreateElement("inputfield");
        
        TMP_InputField inputField = gameObject.GetComponent<TMP_InputField>();
        if (inputField != null)
        {
            element.SetAttribute("text", inputField.text);
            element.SetAttribute("characterLimit", inputField.characterLimit.ToString());
            element.SetAttribute("contentType", inputField.contentType.ToString().ToLower());
            
            if (inputField.placeholder != null)
            {
                element.SetAttribute("placeholder", inputField.placeholder.GetComponent<TMP_Text>().text);
            }
            
            if (inputField.textComponent != null)
            {
                element.SetAttribute("fontSize", inputField.textComponent.fontSize.ToString("F1"));
                element.SetAttribute("textColor", ColorToHex(inputField.textComponent.color));
            }
        }
        
        Image image = gameObject.GetComponent<Image>();
        if (image != null)
        {
            element.SetAttribute("backgroundColor", ColorToHex(image.color));
        }
        
        return element;
    }

    private XmlElement CreateDropdownElement(GameObject gameObject, XmlDocument doc)
    {
        XmlElement element = doc.CreateElement("dropdown");
        
        TMP_Dropdown dropdown = gameObject.GetComponent<TMP_Dropdown>();
        if (dropdown != null)
        {
            element.SetAttribute("value", dropdown.value.ToString());
            
            if (dropdown.options.Count > 0)
            {
                StringBuilder options = new StringBuilder();
                for (int i = 0; i < dropdown.options.Count; i++)
                {
                    if (i > 0) options.Append(",");
                    options.Append(dropdown.options[i].text);
                }
                element.SetAttribute("options", options.ToString());
            }
            
            if (dropdown.captionText != null)
            {
                element.SetAttribute("fontSize", dropdown.captionText.fontSize.ToString("F1"));
                element.SetAttribute("textColor", ColorToHex(dropdown.captionText.color));
            }
        }
        
        Image image = gameObject.GetComponent<Image>();
        if (image != null)
        {
            element.SetAttribute("backgroundColor", ColorToHex(image.color));
        }
        
        return element;
    }

    private XmlElement CreateHorizontalLayoutElement(GameObject gameObject, XmlDocument doc)
    {
        XmlElement element = doc.CreateElement("horizontallayout");
        
        HorizontalLayoutGroup layout = gameObject.GetComponent<HorizontalLayoutGroup>();
        if (layout != null)
        {
            element.SetAttribute("spacing", layout.spacing.ToString("F1"));
            element.SetAttribute("childAlignment", layout.childAlignment.ToString());
            element.SetAttribute("reverse", layout.reverseArrangement.ToString().ToLower());
            element.SetAttribute("controlChildWidth", layout.childControlWidth.ToString().ToLower());
            element.SetAttribute("controlChildHeight", layout.childControlHeight.ToString().ToLower());
            element.SetAttribute("forceExpandWidth", layout.childForceExpandWidth.ToString().ToLower());
            element.SetAttribute("forceExpandHeight", layout.childForceExpandHeight.ToString().ToLower());
            element.SetAttribute("padding", $"{layout.padding.left},{layout.padding.right},{layout.padding.top},{layout.padding.bottom}");
        }
        
        ContentSizeFitter fitter = gameObject.GetComponent<ContentSizeFitter>();
        if (fitter != null)
        {
            element.SetAttribute("autoSize", "true");
        }
        
        return element;
    }

    private XmlElement CreateVerticalLayoutElement(GameObject gameObject, XmlDocument doc)
    {
        XmlElement element = doc.CreateElement("verticallayout");
        
        VerticalLayoutGroup layout = gameObject.GetComponent<VerticalLayoutGroup>();
        if (layout != null)
        {
            element.SetAttribute("spacing", layout.spacing.ToString("F1"));
            element.SetAttribute("childAlignment", layout.childAlignment.ToString());
            element.SetAttribute("reverse", layout.reverseArrangement.ToString().ToLower());
            element.SetAttribute("controlChildWidth", layout.childControlWidth.ToString().ToLower());
            element.SetAttribute("controlChildHeight", layout.childControlHeight.ToString().ToLower());
            element.SetAttribute("forceExpandWidth", layout.childForceExpandWidth.ToString().ToLower());
            element.SetAttribute("forceExpandHeight", layout.childForceExpandHeight.ToString().ToLower());
            element.SetAttribute("padding", $"{layout.padding.left},{layout.padding.right},{layout.padding.top},{layout.padding.bottom}");
        }
        
        ContentSizeFitter fitter = gameObject.GetComponent<ContentSizeFitter>();
        if (fitter != null)
        {
            element.SetAttribute("autoSize", "true");
        }
        
        return element;
    }

    private XmlElement CreateGridLayoutElement(GameObject gameObject, XmlDocument doc)
    {
        XmlElement element = doc.CreateElement("gridlayout");
        
        GridLayoutGroup layout = gameObject.GetComponent<GridLayoutGroup>();
        if (layout != null)
        {
            element.SetAttribute("cellSize", $"{layout.cellSize.x},{layout.cellSize.y}");
            element.SetAttribute("spacing", $"{layout.spacing.x},{layout.spacing.y}");
            element.SetAttribute("startCorner", layout.startCorner.ToString());
            element.SetAttribute("startAxis", layout.startAxis.ToString());
            element.SetAttribute("childAlignment", layout.childAlignment.ToString());
            element.SetAttribute("constraint", layout.constraint.ToString());
            element.SetAttribute("constraintCount", layout.constraintCount.ToString());
            element.SetAttribute("padding", $"{layout.padding.left},{layout.padding.right},{layout.padding.top},{layout.padding.bottom}");
        }
        
        ContentSizeFitter fitter = gameObject.GetComponent<ContentSizeFitter>();
        if (fitter != null)
        {
            element.SetAttribute("autoSize", "true");
        }
        
        return element;
    }

    private void AddRectTransformAttributes(XmlElement element, RectTransform rectTransform)
    {
        if (rectTransform == null) return;
        
        element.SetAttribute("position", $"{rectTransform.anchoredPosition.x:F1},{rectTransform.anchoredPosition.y:F1}");
        element.SetAttribute("size", $"{rectTransform.sizeDelta.x:F1},{rectTransform.sizeDelta.y:F1}");
        element.SetAttribute("anchorMin", $"{rectTransform.anchorMin.x:F2},{rectTransform.anchorMin.y:F2}");
        element.SetAttribute("anchorMax", $"{rectTransform.anchorMax.x:F2},{rectTransform.anchorMax.y:F2}");
        element.SetAttribute("pivot", $"{rectTransform.pivot.x:F2},{rectTransform.pivot.y:F2}");
        
        if (rectTransform.localScale != Vector3.one)
        {
            element.SetAttribute("scale", $"{rectTransform.localScale.x:F2},{rectTransform.localScale.y:F2},{rectTransform.localScale.z:F2}");
        }
    }

    private string ColorToHex(Color color)
    {
        return $"#{ColorUtility.ToHtmlStringRGBA(color)}";
    }

    private string TextAlignmentToString(TextAlignmentOptions alignment)
    {
        return alignment switch
        {
            TextAlignmentOptions.Left => "left",
            TextAlignmentOptions.Right => "right",
            TextAlignmentOptions.Center => "center",
            TextAlignmentOptions.Justified => "justified",
            TextAlignmentOptions.TopLeft => "topleft",
            TextAlignmentOptions.TopRight => "topright",
            TextAlignmentOptions.Top => "topcenter",
            TextAlignmentOptions.BottomLeft => "bottomleft",
            TextAlignmentOptions.BottomRight => "bottomright",
            TextAlignmentOptions.Bottom => "bottomcenter",
            _ => "center"
        };
    }

    private string TextAnchorToString(TextAnchor anchor)
    {
        return anchor switch
        {
            TextAnchor.UpperLeft => "topleft",
            TextAnchor.UpperCenter => "topcenter",
            TextAnchor.UpperRight => "topright",
            TextAnchor.MiddleLeft => "left",
            TextAnchor.MiddleCenter => "center",
            TextAnchor.MiddleRight => "right",
            TextAnchor.LowerLeft => "bottomleft",
            TextAnchor.LowerCenter => "bottomcenter",
            TextAnchor.LowerRight => "bottomright",
            _ => "center"
        };
    }
} 