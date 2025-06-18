# UGUIML - WIP
XML style language for creating uGUI GameObject hierachies in Unity.

This library was created since I found Unity's uxml & uss system to be to cumbersome and it's animation system to rigid. This allows for text files to be created manually or by LLM as I needed a solution where GUI and GUI assets could be loaded during runtime from on external source. However the GameObject uGUI system is still used allowing the familiar development patterns developed over the years without retraining designers.

## Features: 

- Automatic uGUI opimization practices.
- Custom animation system for moving UI GameObjects.
- Custom resource system for loading images and text from external sources.
- Load/edit uGUI with XML style markup files.
- Export existing canvas hierarchies to UGUIML files.

# UGUIML Documentation

## Table of Contents
1. [Getting Started](#getting-started)
2. [File Structure](#file-structure)
3. [Core Components](#core-components)
4. [Layout System](#layout-system)
5. [Event System](#event-system)
6. [Canvas System](#canvas-system)
7. [Styling and Appearance](#styling-and-appearance)
8. [Animation System](#animation-system)
9. [Best Practices](#best-practices)
10. [Examples](#examples)
11. [Troubleshooting](#troubleshooting)

---

## Getting Started

### Setup
1. Import the UGUIML package into your Unity project
2. Add the `UGUIML` component to a GameObject in your scene
3. Set the `xmlFileName` property to your `.uguiml` file name
4. Place your `.uguiml` files in the `Assets/UGUIML/Examples/` folder

### Creating Your First UI

Create a new file called `MyFirstUI.uguiml`:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<UGUIML>
    <Panel name="MainPanel" 
           position="0,0" 
           size="400,300" 
           backgroundColor="#2C3E50">
        
        <Text name="WelcomeText" 
              text="Welcome to UGUIML!" 
              position="0,50" 
              size="350,40" 
              fontSize="24" 
              color="#FFFFFF" 
              alignment="center" />
        
        <Button name="ClickMeBtn" 
                text="Click Me!" 
                position="0,-50" 
                size="150,40" 
                backgroundColor="#3498DB" 
                textColor="#FFFFFF" 
                command="HandleClick" />
    </Panel>
</UGUIML>
```

---

## File Structure

### XML Declaration
Every UGUIML file must start with the XML declaration:
```xml
<?xml version="1.0" encoding="UTF-8"?>
```

### Root Element
The root element must be `<UGUIML>`:
```xml
<UGUIML>
    <!-- Your UI components go here -->
</UGUIML>
```

### Component Hierarchy
Components can be nested to create parent-child relationships:
```xml
<Panel name="Parent">
    <Panel name="Child">
        <Text name="Grandchild" />
    </Panel>
</Panel>
```

---

## Core Components

### Panel
Creates a UI Panel component for grouping and layout.

**Attributes:**
- `name` (required): Unique identifier
- `position`: X,Y position relative to parent
- `size`: Width,Height dimensions
- `backgroundColor`: Hex color code (#RRGGBB)
- `anchorMin`: Minimum anchor point (X,Y)
- `anchorMax`: Maximum anchor point (X,Y)
- `animationSpeed`: Animation duration multiplier

**Example:**
```xml
<Panel name="MainContainer" 
       position="0,0" 
       size="800,600" 
       backgroundColor="#34495E"
       anchorMin="0.5,0.5"
       anchorMax="0.5,0.5" />
```

### Text
Displays text content.

**Attributes:**
- `name` (required): Unique identifier
- `text`: Text content to display
- `position`: X,Y position
- `size`: Width,Height dimensions
- `fontSize`: Text size in points
- `color`: Text color (#RRGGBB)
- `alignment`: Text alignment (left, center, right)

**Example:**
```xml
<Text name="Title" 
      text="My Application" 
      position="0,100" 
      size="400,50" 
      fontSize="28" 
      color="#FFFFFF" 
      alignment="center" />
```

### Button
Interactive button component.

**Attributes:**
- `name` (required): Unique identifier
- `text`: Button label
- `position`: X,Y position
- `size`: Width,Height dimensions
- `backgroundColor`: Button background color
- `textColor`: Text color
- `fontSize`: Text size
- `command`: Event command to execute on click

**Example:**
```xml
<Button name="SaveBtn" 
        text="Save" 
        position="100,0" 
        size="120,40" 
        backgroundColor="#27AE60" 
        textColor="#FFFFFF" 
        fontSize="16"
        command="SaveData" />
```

### InputField
Text input component.

**Attributes:**
- `name` (required): Unique identifier
- `placeholder`: Placeholder text
- `text`: Initial text value
- `position`: X,Y position
- `size`: Width,Height dimensions
- `backgroundColor`: Background color
- `textColor`: Text color
- `fontSize`: Text size
- `contentType`: Input type (standard, password, email, etc.)

**Example:**
```xml
<InputField name="UsernameField" 
            placeholder="Enter username..." 
            position="0,50" 
            size="300,35"
            backgroundColor="#FFFFFF" 
            textColor="#2C3E50" 
            fontSize="14" />
```

### Toggle
Checkbox/toggle component.

**Attributes:**
- `name` (required): Unique identifier
- `text`: Label text
- `position`: X,Y position
- `size`: Width,Height dimensions
- `isOn`: Initial state (true/false)

**Example:**
```xml
<Toggle name="EnableSoundToggle" 
        text="Enable Sound Effects" 
        position="0,0" 
        size="200,25" 
        isOn="true" />
```

### Dropdown
Selection dropdown component.

**Attributes:**
- `name` (required): Unique identifier
- `position`: X,Y position
- `size`: Width,Height dimensions
- `backgroundColor`: Background color
- `textColor`: Text color
- `fontSize`: Text size
- `options`: Comma-separated list of options
- `value`: Selected index (0-based)

**Example:**
```xml
<Dropdown name="QualityDropdown" 
          position="0,0" 
          size="200,35"
          backgroundColor="#FFFFFF" 
          options="Low,Medium,High,Ultra"
          value="2" 
          fontSize="14" 
          textColor="#2C3E50" />
```

### ProgressBar
Progress indicator component.

**Attributes:**
- `name` (required): Unique identifier
- `position`: X,Y position
- `size`: Width,Height dimensions
- `minValue`: Minimum value
- `maxValue`: Maximum value
- `value`: Current value
- `backgroundColor`: Background color
- `fillColor`: Fill color

**Example:**
```xml
<ProgressBar name="LoadingBar" 
             position="0,0" 
             size="300,20"
             minValue="0" 
             maxValue="100" 
             value="75" 
             backgroundColor="#ECF0F1" 
             fillColor="#3498DB" />
```

### Slider
Value slider component.

**Attributes:**
- `name` (required): Unique identifier
- `position`: X,Y position
- `size`: Width,Height dimensions
- `minValue`: Minimum value
- `maxValue`: Maximum value
- `value`: Current value
- `backgroundColor`: Background color
- `fillColor`: Fill color

**Example:**
```xml
<Slider name="VolumeSlider" 
        position="0,0" 
        size="200,20"
        minValue="0" 
        maxValue="100" 
        value="50" 
        backgroundColor="#95A5A6" 
        fillColor="#3498DB" />
```

---

## Layout System

### Positioning
UGUIML supports multiple positioning methods:

#### Absolute Positioning
```xml
<Panel position="100,50" size="200,100" />
```

#### Anchor-based Positioning
```xml
<Panel anchorMin="0,0" anchorMax="1,1" />
```

#### Alternative Attribute Format
For backward compatibility, you can also use:
```xml
<Panel anchoredPosition="0,0" width="200" height="100" />
```

### Layout Patterns

#### Horizontal Layout
```xml
<Panel name="HorizontalContainer">
    <Button position="-100,0" size="80,40" />
    <Button position="0,0" size="80,40" />
    <Button position="100,0" size="80,40" />
</Panel>
```

#### Vertical Layout
```xml
<Panel name="VerticalContainer">
    <Button position="0,60" size="200,40" />
    <Button position="0,0" size="200,40" />
    <Button position="0,-60" size="200,40" />
</Panel>
```

#### Grid Layout
```xml
<Panel name="GridContainer">
    <Button position="-50,25" size="40,40" />
    <Button position="0,25" size="40,40" />
    <Button position="50,25" size="40,40" />
    <Button position="-50,-25" size="40,40" />
    <Button position="0,-25" size="40,40" />
    <Button position="50,-25" size="40,40" />
</Panel>
```

---

## Event System

UGUIML provides a comprehensive event system for handling user interactions.

### Command-based Events
The simplest way to handle events is using the `command` attribute:

```xml
<Button command="SaveGame" />
<Button command="LoadGame|level1" />
<Button command="SetVolume|0.8" />
```

### Event Handler Script
Create a script that implements event handlers:

```csharp
public class UIEventHandler : MonoBehaviour
{
    public void SaveGame()
    {
        // Handle save game
    }
    
    public void LoadGame(string level)
    {
        // Handle load game with parameter
    }
    
    public void SetVolume(float volume)
    {
        // Handle volume change
    }
}
```

### Supported Events by Component

#### Button Events
- `onClick`: Triggered when button is clicked

#### InputField Events
- `onValueChanged`: Triggered when text changes
- `onEndEdit`: Triggered when editing ends
- `onSubmit`: Triggered when Enter is pressed

#### Toggle Events
- `onValueChanged`: Triggered when toggle state changes

#### Dropdown Events
- `onValueChanged`: Triggered when selection changes

#### Slider Events
- `onValueChanged`: Triggered when value changes

### Event Parameters
Events can include parameters separated by pipe (`|`) characters:

```xml
<Button command="PlaySound|buttonClick|0.5" />
<Button command="ChangeScene|MainMenu" />
<Button command="SpawnEnemy|Goblin|10,20" />
```

---

## Canvas System

UGUIML supports nested canvas creation for complex UI hierarchies.

### Canvas Component
```xml
<Canvas name="MainCanvas"
        renderMode="ScreenSpaceOverlay"
        sortingOrder="0"
        pixelPerfect="true"
        referenceResolution="1920,1080"
        scaleMode="ScaleWithScreenSize"
        matchWidthOrHeight="0.5">
    
    <!-- Canvas content -->
    
</Canvas>
```

### Canvas Attributes
- `renderMode`: Rendering mode (ScreenSpaceOverlay, ScreenSpaceCamera, WorldSpace)
- `sortingOrder`: Sorting order for multiple canvases
- `pixelPerfect`: Enable pixel perfect rendering
- `referenceResolution`: Reference screen resolution (Width,Height)
- `scaleMode`: UI scale mode
- `matchWidthOrHeight`: Match ratio (0=width, 1=height, 0.5=balanced)

### Nested Canvas Example
```xml
<UGUIML>
    <Panel name="MainUI">
        <!-- Main UI content -->
        
        <Canvas name="ModalOverlay" sortingOrder="10">
            <Panel name="ModalDialog" backgroundColor="#000000AA">
                <Text text="Are you sure?" />
                <Button text="Yes" command="ConfirmAction" />
                <Button text="No" command="CancelAction" />
            </Panel>
        </Canvas>
    </Panel>
</UGUIML>
```

---

## Styling and Appearance

### Color System
Colors are specified using hex codes:
```xml
<Panel backgroundColor="#2C3E50" />  <!-- Dark blue-gray -->
<Text color="#FFFFFF" />             <!-- White -->
<Button backgroundColor="#E74C3C" />  <!-- Red -->
```

### Common Color Palette
```xml
<!-- Material Design Colors -->
<Panel backgroundColor="#2196F3" />  <!-- Blue -->
<Panel backgroundColor="#4CAF50" />  <!-- Green -->
<Panel backgroundColor="#FF9800" />  <!-- Orange -->
<Panel backgroundColor="#9C27B0" />  <!-- Purple -->
<Panel backgroundColor="#F44336" />  <!-- Red -->
<Panel backgroundColor="#607D8B" />  <!-- Blue Gray -->
```

### Typography
```xml
<Text fontSize="12" />   <!-- Small -->
<Text fontSize="16" />   <!-- Normal -->
<Text fontSize="20" />   <!-- Large -->
<Text fontSize="24" />   <!-- Extra Large -->
<Text fontSize="32" />   <!-- Heading -->
```

### Text Alignment
```xml
<Text alignment="left" />
<Text alignment="center" />
<Text alignment="right" />
```

---

## Animation System

UGUIML includes built-in animation support for smooth UI transitions.

### Animation Speed
Control animation timing with the `animationSpeed` attribute:
```xml
<Panel animationSpeed="1.0" />   <!-- Normal speed -->
<Panel animationSpeed="0.5" />   <!-- Slower -->
<Panel animationSpeed="2.0" />   <!-- Faster -->
```

### Animated Components
Most components support animation:
```xml
<Panel name="AnimatedPanel" 
       position="0,0" 
       size="300,200" 
       backgroundColor="#3498DB"
       animationSpeed="1.5">
    
    <Text name="FadeInText" 
          text="Animated Text" 
          animationSpeed="2.0" />
</Panel>
```

---

## Best Practices

### File Organization
- Use descriptive file names: `MainMenu.uguiml`, `InventoryPanel.uguiml`
- Group related UI files in folders
- Keep files focused on single UI screens or components

### Naming Conventions
- Use PascalCase for component names: `MainPanel`, `SaveButton`
- Include component type in names: `UsernameInputField`, `VolumeSlider`
- Use descriptive names: `PlayerHealthBar` instead of `Bar1`

### Layout Design
- Design for multiple screen resolutions
- Use relative positioning when possible
- Maintain consistent spacing and alignment
- Group related elements in panels

### Performance
- Minimize deeply nested hierarchies
- Use appropriate canvas render modes
- Avoid excessive animation speeds
- Reuse common UI patterns

### Code Organization
```xml
<!-- Good: Well-organized structure -->
<UGUIML>
    <!-- Header Section -->
    <Panel name="Header">
        <!-- Header content -->
    </Panel>
    
    <!-- Main Content -->
    <Panel name="MainContent">
        <!-- Main content -->
    </Panel>
    
    <!-- Footer Section -->
    <Panel name="Footer">
        <!-- Footer content -->
    </Panel>
</UGUIML>
```

---

## Examples

### Login Screen
```xml
<?xml version="1.0" encoding="UTF-8"?>
<UGUIML>
    <Panel name="LoginPanel" 
           position="0,0" 
           size="400,500" 
           backgroundColor="#34495E"
           anchorMin="0.5,0.5"
           anchorMax="0.5,0.5">
        
        <Text name="LoginTitle" 
              text="Login" 
              position="0,150" 
              size="300,50" 
              fontSize="32" 
              color="#FFFFFF" 
              alignment="center" />
        
        <InputField name="UsernameField" 
                    placeholder="Username" 
                    position="0,50" 
                    size="300,40"
                    backgroundColor="#FFFFFF" 
                    textColor="#2C3E50" 
                    fontSize="16" />
        
        <InputField name="PasswordField" 
                    placeholder="Password" 
                    position="0,0" 
                    size="300,40"
                    backgroundColor="#FFFFFF" 
                    textColor="#2C3E50" 
                    fontSize="16"
                    contentType="password" />
        
        <Button name="LoginButton" 
                text="Login" 
                position="0,-50" 
                size="150,40" 
                backgroundColor="#3498DB" 
                textColor="#FFFFFF" 
                fontSize="16"
                command="AttemptLogin" />
        
        <Button name="RegisterButton" 
                text="Register" 
                position="0,-100" 
                size="150,40" 
                backgroundColor="#95A5A6" 
                textColor="#FFFFFF" 
                fontSize="16"
                command="ShowRegister" />
    </Panel>
</UGUIML>
```

### Settings Menu
```xml
<?xml version="1.0" encoding="UTF-8"?>
<UGUIML>
    <Panel name="SettingsPanel" 
           position="0,0" 
           size="600,700" 
           backgroundColor="#2C3E50">
        
        <Text name="SettingsTitle" 
              text="Settings" 
              position="0,300" 
              size="500,50" 
              fontSize="28" 
              color="#FFFFFF" 
              alignment="center" />
        
        <!-- Audio Settings -->
        <Panel name="AudioSection" position="0,150" size="550,120" backgroundColor="#34495E">
            <Text name="AudioLabel" text="Audio Settings" position="0,40" size="500,30" fontSize="18" color="#FFFFFF" alignment="center" />
            
            <Text name="VolumeLabel" text="Master Volume:" position="-150,0" size="120,25" fontSize="14" color="#FFFFFF" alignment="left" />
            <Slider name="VolumeSlider" position="50,0" size="200,25" minValue="0" maxValue="100" value="75" backgroundColor="#95A5A6" fillColor="#3498DB" />
            
            <Toggle name="MuteToggle" text="Mute All Sounds" position="0,-25" size="200,25" isOn="false" />
        </Panel>
        
        <!-- Graphics Settings -->
        <Panel name="GraphicsSection" position="0,0" size="550,120" backgroundColor="#34495E">
            <Text name="GraphicsLabel" text="Graphics Settings" position="0,40" size="500,30" fontSize="18" color="#FFFFFF" alignment="center" />
            
            <Text name="QualityLabel" text="Quality:" position="-150,0" size="80,25" fontSize="14" color="#FFFFFF" alignment="left" />
            <Dropdown name="QualityDropdown" position="50,0" size="150,30" backgroundColor="#FFFFFF" textColor="#2C3E50" fontSize="14" options="Low,Medium,High,Ultra" value="2" />
            
            <Toggle name="FullscreenToggle" text="Fullscreen" position="0,-25" size="150,25" isOn="true" />
        </Panel>
        
        <!-- Control Buttons -->
        <Panel name="ButtonSection" position="0,-200" size="550,80" backgroundColor="#34495E">
            <Button name="ApplyButton" text="Apply" position="-75,0" size="100,40" backgroundColor="#27AE60" textColor="#FFFFFF" fontSize="16" command="ApplySettings" />
            <Button name="ResetButton" text="Reset" position="75,0" size="100,40" backgroundColor="#E67E22" textColor="#FFFFFF" fontSize="16" command="ResetSettings" />
            <Button name="BackButton" text="Back" position="0,-40" size="100,40" backgroundColor="#95A5A6" textColor="#FFFFFF" fontSize="16" command="BackToMainMenu" />
        </Panel>
    </Panel>
</UGUIML>
```

### Inventory Grid
```xml
<?xml version="1.0" encoding="UTF-8"?>
<UGUIML>
    <Panel name="InventoryPanel" 
           position="0,0" 
           size="800,600" 
           backgroundColor="#2C3E50">
        
        <Text name="InventoryTitle" 
              text="Inventory" 
              position="0,250" 
              size="700,50" 
              fontSize="24" 
              color="#FFFFFF" 
              alignment="center" />
        
        <!-- Inventory Grid -->
        <Panel name="InventoryGrid" position="0,50" size="600,400" backgroundColor="#34495E">
            <!-- Row 1 -->
            <Button name="Slot1" position="-225,150" size="60,60" backgroundColor="#95A5A6" command="SelectItem|1" />
            <Button name="Slot2" position="-150,150" size="60,60" backgroundColor="#95A5A6" command="SelectItem|2" />
            <Button name="Slot3" position="-75,150" size="60,60" backgroundColor="#95A5A6" command="SelectItem|3" />
            <Button name="Slot4" position="0,150" size="60,60" backgroundColor="#95A5A6" command="SelectItem|4" />
            <Button name="Slot5" position="75,150" size="60,60" backgroundColor="#95A5A6" command="SelectItem|5" />
            <Button name="Slot6" position="150,150" size="60,60" backgroundColor="#95A5A6" command="SelectItem|6" />
            <Button name="Slot7" position="225,150" size="60,60" backgroundColor="#95A5A6" command="SelectItem|7" />
            
            <!-- Row 2 -->
            <Button name="Slot8" position="-225,75" size="60,60" backgroundColor="#95A5A6" command="SelectItem|8" />
            <Button name="Slot9" position="-150,75" size="60,60" backgroundColor="#95A5A6" command="SelectItem|9" />
            <Button name="Slot10" position="-75,75" size="60,60" backgroundColor="#95A5A6" command="SelectItem|10" />
            <Button name="Slot11" position="0,75" size="60,60" backgroundColor="#95A5A6" command="SelectItem|11" />
            <Button name="Slot12" position="75,75" size="60,60" backgroundColor="#95A5A6" command="SelectItem|12" />
            <Button name="Slot13" position="150,75" size="60,60" backgroundColor="#95A5A6" command="SelectItem|13" />
            <Button name="Slot14" position="225,75" size="60,60" backgroundColor="#95A5A6" command="SelectItem|14" />
            
            <!-- Additional rows... -->
        </Panel>
        
        <!-- Item Details Panel -->
        <Panel name="ItemDetails" position="0,-150" size="700,100" backgroundColor="#34495E">
            <Text name="ItemName" text="Select an item" position="0,25" size="600,30" fontSize="18" color="#FFFFFF" alignment="center" />
            <Text name="ItemDescription" text="Item details will appear here" position="0,-10" size="600,25" fontSize="14" color="#BDC3C7" alignment="center" />
            
            <Button name="UseButton" text="Use" position="-75,-40" size="80,30" backgroundColor="#27AE60" textColor="#FFFFFF" fontSize="14" command="UseItem" />
            <Button name="DropButton" text="Drop" position="75,-40" size="80,30" backgroundColor="#E74C3C" textColor="#FFFFFF" fontSize="14" command="DropItem" />
        </Panel>
    </Panel>
</UGUIML>
```

---

## Troubleshooting

### Common Issues

#### XML Parsing Errors
**Problem**: "Root element is missing" or XML parsing fails
**Solution**: 
- Ensure file starts with `<?xml version="1.0" encoding="UTF-8"?>`
- Check that root element is `<UGUIML>`
- Verify all tags are properly closed
- Escape special characters in text content

#### Component Not Appearing
**Problem**: UI components don't show up in game
**Solution**:
- Check component name is unique
- Verify position and size attributes
- Ensure parent panel is large enough
- Check anchor settings

#### Event Not Triggering
**Problem**: Button clicks or other events don't work
**Solution**:
- Verify command name matches method name
- Ensure UIEventHandler script is attached to appropriate GameObject
- Check method signature matches parameter types
- Verify method is public

#### Layout Issues
**Problem**: Components overlapping or positioned incorrectly
**Solution**:
- Check Canvas Scaler settings (should be 1920x1080 reference)
- Verify anchor points are set correctly
- Ensure position values are appropriate for screen size
- Use appropriate canvas render mode

#### Performance Issues
**Problem**: UI is slow or laggy
**Solution**:
- Reduce animation speeds
- Minimize nested canvas hierarchies
- Use appropriate canvas render modes
- Optimize complex layouts

### Debugging Tips

1. **Use Unity Console**: Check for error messages and warnings
2. **Verify File Path**: Ensure `.uguiml` files are in correct directory
3. **Test Incrementally**: Start with simple UI and add complexity gradually
4. **Check Component Names**: Ensure all names are unique within the hierarchy
5. **Validate XML**: Use XML validator to check syntax

### Error Messages

#### "Root 'UGUIML' node not found"
- File doesn't contain proper `<UGUIML>` root element
- Check file encoding and XML declaration

#### "Component with name 'X' already exists"
- Duplicate component names in hierarchy
- Use unique names for all components

#### "Failed to parse color 'X'"
- Invalid hex color format
- Use format: `#RRGGBB` (e.g., `#FF0000` for red)

#### "Method 'X' not found for command"
- Event handler method doesn't exist
- Check method name spelling and public access

---
