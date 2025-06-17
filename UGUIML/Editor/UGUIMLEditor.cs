using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom editor for UGUIML component with enhanced UI and functionality
/// </summary>
[CustomEditor(typeof(UGUIML))]
public class UGUIMLEditor : UnityEditor.Editor
{
    private UGUIML uguimlComponent;
    private bool showElementDictionaries = false;
    private bool showAnimationTools = false;
    private Vector2 scrollPosition;

    private void OnEnable()
    {
        uguimlComponent = target as UGUIML;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("UGUIML Component - Parses XML markup to create Unity uGUI hierarchies", MessageType.Info);
        
        EditorGUILayout.Space();

        // Draw default inspector
        DrawDefaultInspector();

        EditorGUILayout.Space();

        // Load/Clear buttons
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Load XML", GUILayout.Height(30)))
        {
            if (Application.isPlaying)
            {
                uguimlComponent.LoadXML();
            }
            else
            {
                EditorGUILayout.HelpBox("XML loading is only available during play mode.", MessageType.Warning);
            }
        }

        if (GUILayout.Button("Clear Canvas", GUILayout.Height(30)))
        {
            if (Application.isPlaying)
            {
                uguimlComponent.ClearCanvas();
            }
            else if (EditorUtility.DisplayDialog("Clear Canvas", 
                "This will destroy all child objects of the canvas. This action cannot be undone.", 
                "Clear", "Cancel"))
            {
                // Clear in edit mode
                Transform canvasTransform = uguimlComponent.TargetCanvas.transform;
                for (int i = canvasTransform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(canvasTransform.GetChild(i).gameObject);
                }
            }
        }

        EditorGUILayout.EndHorizontal();

        // Show status
        if (Application.isPlaying)
        {
            EditorGUILayout.Space();
            
            string statusText = uguimlComponent.IsLoaded ? 
                $"Status: Loaded ({uguimlComponent.allElements.Count} elements)" : 
                "Status: Not Loaded";
            
            EditorGUILayout.LabelField(statusText, EditorStyles.boldLabel);
        }

        // Show element dictionaries if loaded
        if (Application.isPlaying && uguimlComponent.IsLoaded)
        {
            EditorGUILayout.Space();
            
            showElementDictionaries = EditorGUILayout.Foldout(showElementDictionaries, "Element Dictionaries", true);
            if (showElementDictionaries)
            {
                DrawElementDictionaries();
            }

            EditorGUILayout.Space();
            
            showAnimationTools = EditorGUILayout.Foldout(showAnimationTools, "Animation Tools", true);
            if (showAnimationTools)
            {
                DrawAnimationTools();
            }
        }

        // Show XML preview if file is assigned
        if (uguimlComponent.XmlFile != null)
        {
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Show XML Content"))
            {
                ShowXMLContentWindow();
            }
        }
    }

    private void DrawElementDictionaries()
    {
        EditorGUI.indentLevel++;
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(200));

        // Show buttons
        if (uguimlComponent.buttons.Count > 0)
        {
            EditorGUILayout.LabelField($"Buttons ({uguimlComponent.buttons.Count}):", EditorStyles.boldLabel);
            foreach (var kvp in uguimlComponent.buttons)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"  {kvp.Key}", GUILayout.Width(150));
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = kvp.Value.gameObject;
                }
                if (GUILayout.Button("Execute", GUILayout.Width(60)) && Application.isPlaying)
                {
                    kvp.Value.ExecuteCommand();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
        }

        // Show panels
        if (uguimlComponent.panels.Count > 0)
        {
            EditorGUILayout.LabelField($"Panels ({uguimlComponent.panels.Count}):", EditorStyles.boldLabel);
            foreach (var kvp in uguimlComponent.panels)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"  {kvp.Key}", GUILayout.Width(150));
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = kvp.Value.gameObject;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
        }

        // Show text elements
        if (uguimlComponent.textElements.Count > 0)
        {
            EditorGUILayout.LabelField($"Text Elements ({uguimlComponent.textElements.Count}):", EditorStyles.boldLabel);
            foreach (var kvp in uguimlComponent.textElements)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"  {kvp.Key}", GUILayout.Width(150));
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = kvp.Value.gameObject;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
        }

        // Show images
        if (uguimlComponent.images.Count > 0)
        {
            EditorGUILayout.LabelField($"Images ({uguimlComponent.images.Count}):", EditorStyles.boldLabel);
            foreach (var kvp in uguimlComponent.images)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"  {kvp.Key}", GUILayout.Width(150));
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = kvp.Value.gameObject;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
        }

        // Show ScrollViews
        if (uguimlComponent.scrollViews.Count > 0)
        {
            EditorGUILayout.LabelField($"ScrollViews ({uguimlComponent.scrollViews.Count}):", EditorStyles.boldLabel);
            foreach (var kvp in uguimlComponent.scrollViews)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"  {kvp.Key}", GUILayout.Width(150));
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = kvp.Value.gameObject;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
        }

        // Show ProgressBars
        if (uguimlComponent.progressBars.Count > 0)
        {
            EditorGUILayout.LabelField($"ProgressBars ({uguimlComponent.progressBars.Count}):", EditorStyles.boldLabel);
            foreach (var kvp in uguimlComponent.progressBars)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"  {kvp.Key}", GUILayout.Width(150));
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = kvp.Value.gameObject;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
        }

        // Show ToggleGroups
        if (uguimlComponent.toggleGroups.Count > 0)
        {
            EditorGUILayout.LabelField($"Toggle Groups ({uguimlComponent.toggleGroups.Count}):", EditorStyles.boldLabel);
            foreach (var kvp in uguimlComponent.toggleGroups)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"  {kvp.Key}", GUILayout.Width(150));
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = kvp.Value.gameObject;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
        }

        // Show Toggles
        if (uguimlComponent.toggles.Count > 0)
        {
            EditorGUILayout.LabelField($"Toggles ({uguimlComponent.toggles.Count}):", EditorStyles.boldLabel);
            foreach (var kvp in uguimlComponent.toggles)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"  {kvp.Key}", GUILayout.Width(150));
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = kvp.Value.gameObject;
                }
                if (GUILayout.Button("Toggle", GUILayout.Width(60)) && Application.isPlaying)
                {
                    kvp.Value.isOn = !kvp.Value.isOn;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
        }

        // Show InputFields
        if (uguimlComponent.inputFields.Count > 0)
        {
            EditorGUILayout.LabelField($"Input Fields ({uguimlComponent.inputFields.Count}):", EditorStyles.boldLabel);
            foreach (var kvp in uguimlComponent.inputFields)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"  {kvp.Key}", GUILayout.Width(150));
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = kvp.Value.gameObject;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
        }

        // Show Dropdowns
        if (uguimlComponent.dropdowns.Count > 0)
        {
            EditorGUILayout.LabelField($"Dropdowns ({uguimlComponent.dropdowns.Count}):", EditorStyles.boldLabel);
            foreach (var kvp in uguimlComponent.dropdowns)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"  {kvp.Key}", GUILayout.Width(150));
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = kvp.Value.gameObject;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
        }

        // Show Layout Groups
        if (uguimlComponent.horizontalLayouts.Count > 0 || uguimlComponent.verticalLayouts.Count > 0 || uguimlComponent.gridLayouts.Count > 0)
        {
            EditorGUILayout.LabelField("Layout Groups:", EditorStyles.boldLabel);
            
            foreach (var kvp in uguimlComponent.horizontalLayouts)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"  [H] {kvp.Key}", GUILayout.Width(150));
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = kvp.Value.gameObject;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            foreach (var kvp in uguimlComponent.verticalLayouts)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"  [V] {kvp.Key}", GUILayout.Width(150));
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = kvp.Value.gameObject;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            foreach (var kvp in uguimlComponent.gridLayouts)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"  [G] {kvp.Key}", GUILayout.Width(150));
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = kvp.Value.gameObject;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndScrollView();
        
        EditorGUI.indentLevel--;
    }

    private void DrawAnimationTools()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Animation tools are only available during play mode.", MessageType.Info);
            return;
        }

        EditorGUI.indentLevel++;

        EditorGUILayout.LabelField("Slide Animations (Root Panels Only):", EditorStyles.boldLabel);
        
        EditorGUILayout.LabelField("Slide In (From Off-Screen):", EditorStyles.miniLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("⬅ From Left"))
        {
            UGUIMLAnimationUtility.SlideInFromLeft(uguimlComponent);
        }
        if (GUILayout.Button("➡ From Right"))
        {
            UGUIMLAnimationUtility.SlideInFromRight(uguimlComponent);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("⬆ From Top"))
        {
            UGUIMLAnimationUtility.SlideInFromTop(uguimlComponent);
        }
        if (GUILayout.Button("⬇ From Bottom"))
        {
            UGUIMLAnimationUtility.SlideInFromBottom(uguimlComponent);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Slide Out (To Off-Screen):", EditorStyles.miniLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("⬅ To Left"))
        {
            UGUIMLAnimationUtility.SlideToLeft(uguimlComponent);
        }
        if (GUILayout.Button("➡ To Right"))
        {
            UGUIMLAnimationUtility.SlideToRight(uguimlComponent);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("⬆ To Top"))
        {
            UGUIMLAnimationUtility.SlideToTop(uguimlComponent);
        }
        if (GUILayout.Button("⬇ To Bottom"))
        {
            UGUIMLAnimationUtility.SlideToBottom(uguimlComponent);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("General Animations:", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🎯 Pop In"))
        {
            UGUIMLAnimationUtility.PopIn(uguimlComponent);
        }
        if (GUILayout.Button("✨ Fade In"))
        {
            UGUIMLAnimationUtility.FadeIn(uguimlComponent);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("👁 Show All"))
        {
            UGUIMLAnimationUtility.ShowAllElements(uguimlComponent);
        }
        if (GUILayout.Button("🚫 Hide All"))
        {
            UGUIMLAnimationUtility.HideAllElements(uguimlComponent);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🌊 Fade Out"))
        {
            UGUIMLAnimationUtility.FadeOut(uguimlComponent);
        }
        if (GUILayout.Button("🔄 Reset Positions"))
        {
            // Reset all elements to original positions
            foreach (var element in uguimlComponent.allElements.Values)
            {
                element.AnimateToOriginalPosition(2f);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Element Type Animations:", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("📋 Animate Panels"))
        {
            UGUIMLAnimationUtility.AnimateElementsByType(uguimlComponent, "panel", AnimationType.FadeIn, Vector3.one);
        }
        if (GUILayout.Button("📝 Animate Texts"))
        {
            UGUIMLAnimationUtility.AnimateElementsByType(uguimlComponent, "text", AnimationType.Scale, Vector3.one);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🔘 Animate Buttons"))
        {
            UGUIMLAnimationUtility.AnimateElementsByType(uguimlComponent, "button", AnimationType.PopIn, Vector3.one);
        }
        if (GUILayout.Button("🖼 Animate Images"))
        {
            UGUIMLAnimationUtility.AnimateElementsByType(uguimlComponent, "image", AnimationType.FadeIn, Vector3.one);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;
    }

    private void ShowXMLContentWindow()
    {
        XMLContentWindow.ShowWindow(uguimlComponent.XmlFile);
    }
}

/// <summary>
/// Window to display XML content with syntax highlighting
/// </summary>
public class XMLContentWindow : EditorWindow
{
    private TextAsset xmlFile;
    private Vector2 scrollPosition;
    private GUIStyle xmlStyle;

    public static void ShowWindow(TextAsset xmlFile)
    {
        XMLContentWindow window = GetWindow<XMLContentWindow>("UGUIML XML Content");
        window.xmlFile = xmlFile;
        window.Show();
    }

    private void OnEnable()
    {
        xmlStyle = new GUIStyle(EditorStyles.textArea)
        {
            wordWrap = false,
            richText = true,
            fontSize = 12
        };
    }

    private void OnGUI()
    {
        if (xmlFile == null)
        {
            EditorGUILayout.HelpBox("No XML file selected.", MessageType.Warning);
            return;
        }

        EditorGUILayout.LabelField($"XML Content: {xmlFile.name}", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        // Display XML content with basic formatting
        string xmlContent = xmlFile.text;
        EditorGUILayout.TextArea(xmlContent, xmlStyle, GUILayout.ExpandHeight(true));
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Copy to Clipboard"))
        {
            EditorGUIUtility.systemCopyBuffer = xmlContent;
            Debug.Log("XML content copied to clipboard");
        }
    }
} 