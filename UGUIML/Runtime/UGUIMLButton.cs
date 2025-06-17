using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Button component that integrates with CommandSystem for command execution
/// </summary>
[RequireComponent(typeof(Button))]
public class UGUIMLButton : MonoBehaviour
{
    [Header("Command Settings")]
    [SerializeField] private string command;
    [SerializeField] private bool logCommandExecution = true;

    private Button button;
    private CommandSystem commandSystem;

    #region Properties

    public string Command
    {
        get => command;
        set => command = value;
    }

    public Button Button => button;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        button = GetComponent<Button>();
        commandSystem = CommandSystem.Singleton;

        if (commandSystem == null)
        {
            Debug.LogError($"UGUIML Button '{gameObject.name}': CommandSystem singleton not found!");
        }
    }

    private void Start()
    {
        // Ensure event listener is properly assigned on start
        if (!string.IsNullOrEmpty(command))
        {
            AddCommandListener();
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Set the command to be executed when button is clicked
    /// </summary>
    /// <param name="newCommand">Command string to execute</param>
    public void SetCommand(string newCommand)
    {
        command = newCommand;
        
        // Update the button listener when command changes
        if (button != null)
        {
            RemoveCommandListener();
            if (!string.IsNullOrEmpty(command))
            {
                AddCommandListener();
            }
        }
    }

    /// <summary>
    /// Execute the assigned command using CommandSystem
    /// </summary>
    public void ExecuteCommand()
    {
        if (string.IsNullOrEmpty(command))
        {
            Debug.LogWarning($"UGUIML Button '{gameObject.name}': No command assigned!");
            return;
        }

        if (commandSystem == null)
        {
            Debug.LogError($"UGUIML Button '{gameObject.name}': CommandSystem not available!");
            return;
        }

        if (logCommandExecution)
        {
            Debug.Log($"UGUIML Button '{gameObject.name}': Executing command: {command}");
        }

        // Execute command using CommandSystem
        commandSystem.RunCommand(command);
    }

    /// <summary>
    /// Add click listener for command execution
    /// </summary>
    public void AddCommandListener()
    {
        if (button != null)
        {
            button.onClick.AddListener(ExecuteCommand);
        }
    }

    /// <summary>
    /// Remove click listener for command execution
    /// </summary>
    public void RemoveCommandListener()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(ExecuteCommand);
        }
    }

    /// <summary>
    /// Enable or disable command logging
    /// </summary>
    /// <param name="enabled">Whether to log command execution</param>
    public void SetCommandLogging(bool enabled)
    {
        logCommandExecution = enabled;
    }

    /// <summary>
    /// Enable or disable the button
    /// </summary>
    /// <param name="enabled">Whether the button should be interactable</param>
    public void SetButtonEnabled(bool enabled)
    {
        if (button != null)
        {
            button.interactable = enabled;
        }
    }

    /// <summary>
    /// Check if the button is currently enabled
    /// </summary>
    /// <returns>True if button is interactable</returns>
    public bool IsButtonEnabled()
    {
        return button != null && button.interactable;
    }

    #endregion

    #region Editor Methods

#if UNITY_EDITOR
    /// <summary>
    /// Validate command in editor
    /// </summary>
    private void OnValidate()
    {
        if (!string.IsNullOrEmpty(command))
        {
            // Basic validation - check if command contains pipe character (parameter separator)
            if (command.Contains("|"))
            {
                // Command has parameters - this is good
            }
            else
            {
                // Command without parameters - might be valid for some commands
            }
        }
    }
#endif

    #endregion
} 