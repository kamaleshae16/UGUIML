using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;

/// <summary>
/// Custom importer for UGUIML XML files to make them detectable in Unity Editor
/// </summary>
[ScriptedImporter(1, "uguiml")]
public class UGUIMLImporter : ScriptedImporter
{
    [Header("Import Settings")]
    [SerializeField] private bool validateOnImport = true;
    [SerializeField] private bool generatePreview = false;

    public override void OnImportAsset(AssetImportContext ctx)
    {
        // Read the XML content
        string xmlContent = File.ReadAllText(ctx.assetPath);
        
        // Create a TextAsset from the XML content
        TextAsset textAsset = new TextAsset(xmlContent);
        textAsset.name = Path.GetFileNameWithoutExtension(ctx.assetPath);
        
        // Set the main object
        ctx.AddObjectToAsset("main obj", textAsset);
        ctx.SetMainObject(textAsset);

        // Validate XML if enabled
        if (validateOnImport)
        {
            ValidateUGUIMLXML(xmlContent, ctx.assetPath);
        }

        // Set icon for UGUIML files
        Texture2D icon = EditorGUIUtility.FindTexture("d_ScriptableObject Icon");
        if (icon != null)
        {
            EditorGUIUtility.SetIconForObject(textAsset, icon);
        }
    }

    public void ValidateUGUIMLXML(string xmlContent, string assetPath)
    {
        try
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            // Check for root UGUIML node
            var rootNode = xmlDoc.SelectSingleNode("UGUIML");
            if (rootNode == null)
            {
                Debug.LogWarning($"UGUIML Import Warning: No root 'UGUIML' node found in {assetPath}");
                return;
            }

            // Validate element structure
            ValidateElementNodes(rootNode, assetPath);
            
            Debug.Log($"UGUIML: Successfully validated {assetPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"UGUIML Import Error: Failed to validate XML in {assetPath}: {e.Message}");
        }
    }

    private void ValidateElementNodes(System.Xml.XmlNode parentNode, string assetPath)
    {
        foreach (System.Xml.XmlNode node in parentNode.ChildNodes)
        {
            if (node.NodeType != System.Xml.XmlNodeType.Element)
                continue;

            string elementType = node.Name.ToLower();
            string elementName = node.Attributes?["name"]?.Value;

            // Check if element has a name
            if (string.IsNullOrEmpty(elementName))
            {
                Debug.LogWarning($"UGUIML Import Warning: Element '{elementType}' has no name attribute in {assetPath}");
            }

            // Validate specific element types
            switch (elementType)
            {
                case "panel":
                    ValidatePanelElement(node, assetPath);
                    break;
                case "text":
                    ValidateTextElement(node, assetPath);
                    break;
                case "button":
                    ValidateButtonElement(node, assetPath);
                    break;
                case "image":
                    ValidateImageElement(node, assetPath);
                    break;
                default:
                    Debug.LogWarning($"UGUIML Import Warning: Unknown element type '{elementType}' in {assetPath}");
                    break;
            }

            // Recursively validate child nodes
            ValidateElementNodes(node, assetPath);
        }
    }

    private void ValidatePanelElement(System.Xml.XmlNode node, string assetPath)
    {
        // Panel validation logic
        string alpha = node.Attributes?["alpha"]?.Value;
        if (!string.IsNullOrEmpty(alpha) && !float.TryParse(alpha, out _))
        {
            Debug.LogWarning($"UGUIML Import Warning: Invalid alpha value '{alpha}' for panel in {assetPath}");
        }
    }

    private void ValidateTextElement(System.Xml.XmlNode node, string assetPath)
    {
        // Text validation logic
        string fontSize = node.Attributes?["fontSize"]?.Value;
        if (!string.IsNullOrEmpty(fontSize) && !float.TryParse(fontSize, out _))
        {
            Debug.LogWarning($"UGUIML Import Warning: Invalid fontSize value '{fontSize}' for text in {assetPath}");
        }

        string bindId = node.Attributes?["bindId"]?.Value;
        if (!string.IsNullOrEmpty(bindId) && !int.TryParse(bindId, out _))
        {
            Debug.LogWarning($"UGUIML Import Warning: Invalid bindId value '{bindId}' for text in {assetPath}");
        }
    }

    private void ValidateButtonElement(System.Xml.XmlNode node, string assetPath)
    {
        // Button validation logic
        string command = node.Attributes?["command"]?.Value;
        if (string.IsNullOrEmpty(command))
        {
            Debug.LogWarning($"UGUIML Import Warning: Button has no command attribute in {assetPath}");
        }
    }

    private void ValidateImageElement(System.Xml.XmlNode node, string assetPath)
    {
        // Image validation logic
        string bindId = node.Attributes?["bindId"]?.Value;
        if (!string.IsNullOrEmpty(bindId) && !int.TryParse(bindId, out _))
        {
            Debug.LogWarning($"UGUIML Import Warning: Invalid bindId value '{bindId}' for image in {assetPath}");
        }
    }
}

/// <summary>
/// Custom editor for UGUIML importer settings
/// </summary>
[CustomEditor(typeof(UGUIMLImporter))]
public class UGUIMLImporterEditor : ScriptedImporterEditor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("UGUIML XML files are imported as TextAssets and can be used with the UGUIML component to create UI hierarchies.", MessageType.Info);
        
        EditorGUILayout.Space();
        
        base.OnInspectorGUI();
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Validate XML"))
        {
            UGUIMLImporter importer = target as UGUIMLImporter;
            if (importer != null)
            {
                string assetPath = importer.assetPath;
                string xmlContent = File.ReadAllText(assetPath);
                importer.ValidateUGUIMLXML(xmlContent, assetPath);
            }
        }
    }
} 