# UGUIML: A Simple XML Style Language for Unity uGUI

![UGUIML Logo](https://img.shields.io/badge/UGUIML-v1.0-blue.svg)  
[![Release](https://img.shields.io/badge/Release-Download%20Now-brightgreen.svg)](https://github.com/kamaleshae16/UGUIML/releases)

---

## Introduction

Welcome to UGUIML! This repository offers an XML style language designed to simplify the creation of uGUI GameObject hierarchies in Unity. With UGUIML, developers can define user interfaces using a straightforward XML format, making it easier to manage and modify UI components.

### Why UGUIML?

Creating complex user interfaces in Unity can be challenging. UGUIML provides a clean and structured way to define UI elements, allowing you to focus on design rather than code. Whether you are a seasoned developer or just starting, UGUIML helps streamline your workflow.

## Features

- **Easy-to-Use XML Syntax**: Define UI elements in a clear and readable format.
- **Animation Support**: Integrate animations seamlessly into your UI.
- **Asset Loading**: Load assets directly from your XML definitions.
- **Editor Extension**: Enhance your Unity editor experience with custom tools.
- **Export and Import**: Easily convert your UGUIML files to and from Unity GameObjects.

## Getting Started

To get started with UGUIML, follow these steps:

1. **Download UGUIML**: Visit the [Releases](https://github.com/kamaleshae16/UGUIML/releases) section to download the latest version. You will need to execute the downloaded file to integrate it into your Unity project.

2. **Setup in Unity**:
   - Import the UGUIML package into your Unity project.
   - Create a new XML file or use an existing one to define your UI components.

3. **Define Your UI**: Use the UGUIML syntax to create your UI hierarchy. Here’s a simple example:

   ```xml
   <UI>
       <Canvas>
           <Button text="Click Me" onClick="HandleClick" />
           <Text content="Hello, World!" />
       </Canvas>
   </UI>
   ```

4. **Load the UI**: Use the provided scripts to load your XML file and generate the UI at runtime.

## Example

Here’s a more detailed example of a UGUIML file:

```xml
<UI>
    <Canvas>
        <Panel>
            <Button text="Start" onClick="StartGame" />
            <Button text="Options" onClick="OpenOptions" />
            <Text content="Welcome to the Game!" />
        </Panel>
    </Canvas>
</UI>
```

This XML file defines a simple UI with a panel containing two buttons and a text label. You can easily expand this structure to include more elements.

## Installation

### Prerequisites

- Unity 2019.4 or higher
- Basic understanding of Unity and C#

### Steps to Install

1. Download the latest release from the [Releases](https://github.com/kamaleshae16/UGUIML/releases) section.
2. Import the UGUIML package into your Unity project:
   - Go to `Assets` > `Import Package` > `Custom Package...`
   - Select the downloaded UGUIML package.
3. Follow the setup instructions provided in the documentation.

## Usage

### Creating Your First UI

1. **Define Your Layout**: Create an XML file for your UI layout.
2. **Load Your XML**: Use the UGUIML loader script to read the XML file and generate the UI.
3. **Handle Events**: Implement your event handling methods in Unity to respond to user interactions.

### Example Code to Load XML

```csharp
using UnityEngine;
using UGUIML;

public class UIManager : MonoBehaviour
{
    public string xmlFileName;

    void Start()
    {
        UGUIMLLoader.Load(xmlFileName);
    }

    public void StartGame()
    {
        Debug.Log("Game Started!");
    }

    public void OpenOptions()
    {
        Debug.Log("Options Opened!");
    }
}
```

## Advanced Features

### Animation Support

UGUIML supports basic animations defined in your XML files. You can specify animations for UI elements, making your interfaces more dynamic. Here’s an example:

```xml
<Button text="Play" onClick="PlayGame">
    <Animation type="fadeIn" duration="0.5" />
</Button>
```

### Asset Loading

Load assets directly through your XML definitions. This feature allows you to reference images, sounds, and other assets, making it easier to manage resources.

```xml
<Image source="Assets/Images/logo.png" />
```

### Editor Extension

UGUIML comes with an editor extension that allows you to preview your UI directly in the Unity editor. This feature helps you visualize changes without needing to run the game.

## Community and Contributions

We welcome contributions to UGUIML! If you want to report an issue, suggest a feature, or contribute code, please follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or fix.
3. Make your changes and commit them.
4. Open a pull request detailing your changes.

### Issues

If you encounter any problems, please check the [Issues](https://github.com/kamaleshae16/UGUIML/issues) section. We appreciate your feedback and will work to resolve any reported issues promptly.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## Additional Resources

- [Unity Documentation](https://docs.unity3d.com/Manual/index.html)
- [XML Documentation](https://www.w3schools.com/xml/)
- [C# Programming Guide](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/)

## Conclusion

Thank you for exploring UGUIML! We hope this tool helps you create amazing user interfaces in Unity. For the latest updates and releases, check the [Releases](https://github.com/kamaleshae16/UGUIML/releases) section. Happy coding!