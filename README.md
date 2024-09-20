# EditorTools

## Attribution

Quite a few of the components here are not by my design. They're all Open Source, and where possible I tried to attribute to the original author. If you see something that you believe is yours, please let me know and I'll be happy to add you to the list of contributors.

## Overview

This is a collection of Editor Tools that I've found useful in my day-to-day work. They're all designed to be as unobtrusive as possible, and to be as easy to use as possible. I've tried to keep the number of dependencies to a minimum, and to keep the code as clean as possible.

## Installation
1. Open the Unity project you want to install this package in.
2. Open the Package Manager window.
3. Click on the `+` button and select `Add package from git URL...`.
4. Paste the URL of this repo into the text field and press `Add`.

## Tools - Attributes
### BoxRangeAttribute
The BoxRangeDrawer is a custom property drawer for Unity that provides a user-friendly way to input and restrict values for fields marked with `[BoxRange]`. It supports int, float, Vector2, Vector3, and Vector3Int types, displaying a slider in the Unity Inspector to enforce that values remain within a specified range. This tool enhances the editor experience by visually representing the allowed range for properties, ensuring that values stay within defined bounds without additional validation logic. The drawer is especially useful for developers who need to constrain numeric or vector inputs directly within the Unity editor.

### DecimalAttribute
The DecimalsDrawer is a custom property drawer for Unity that rounds float properties to a specified number of decimal places in the Unity Inspector, based on the DecimalsAttribute. Mark a value with `[Decimal]` It displays the allowed precision and ensures that the values entered are automatically rounded to the defined decimal precision, using MidpointRounding.AwayFromZero. Currently, it supports rounding for float properties and provides placeholder messages for unsupported types like Vector2 and Vector3. This drawer simplifies the process of managing decimal precision in float values directly within the Unity editor. 

MidpointRounding.AwayFromZero is used to ensure that values are rounded to the nearest even number when they fall exactly between two integers. This rounding method is commonly used in financial applications and is the default behavior for the Math.Round method in C#.


### DisableEditingAttribute
The DisableEditingPropertyDrawer is a custom property drawer for Unity that disables editing of fields marked with `[DisableEditing]`. When applied, it renders the property as read-only, preventing users from modifying its value while still displaying it in the Inspector. This drawer is useful for situations where a property needs to be visible but should not be editable, such as when the value is controlled by other systems or scripts.

### InfoTextAttribute
The InfoTextDrawer is a custom property drawer for Unity that displays informational text in the Unity Inspector for fields marked with `[InfoText]`. When applied, it shows a help box with a message specified by the attribute, without rendering the actual property field. This drawer is useful for providing additional context or instructions to developers or designers using the Inspector, improving clarity and guidance in complex systems or settings.

### PreviewDrawer
The PreviewDrawer is a custom property drawer for Unity that allows fields marked with `[Preview]` to display a visual preview of certain object types in the Unity Inspector. It supports previews for Texture, Material, Sprite, and GameObject types, displaying the associated texture or material on the Inspector when these objects are assigned to the field. The drawer automatically adjusts the height of the preview based on the attribute's specified height, making it useful for providing immediate visual feedback for assets like textures or materials without the need to open them separately.

### ReadOnlyAttribute (BeginReadOnlyGroup, EndReadOnlyGroup)
The ReadOnlyDrawer, BeginReadOnlyGroupDrawer, and EndReadOnlyGroupDrawer are custom property drawers for Unity that allow you to mark individual properties or groups of properties as read-only in the Inspector. The ReadOnlyDrawer disables a single property marked with `[ReadOnly]`, ensuring that the field remains visible but uneditable. The `[BeginReadOnlyGroup]` and `[EndReadOnlyGroup]` are used to define a block of properties that are rendered as read-only within a group, disabling editing for all properties between these two markers. This is useful when you want to prevent users from modifying certain properties while still allowing them to view the values.


### TagSelectorAttribute
The TagSelectorPropertyDrawer is a custom property drawer for Unity that allows you to select tags from a dropdown menu for fields marked with `[TagSelector]`. If the UseDefaultTagFieldDrawer option is enabled, it uses Unity's default tag selector. Otherwise, it generates a custom tag list, including a "<NoTag>" option, allowing the user to assign or clear tags from the property. This drawer simplifies tag selection by offering a dropdown of all available tags, ensuring that string properties can only be assigned valid tag values, which is especially useful in managing tagging systems within Unity projects.

## TimeDrawerAttribute

The TimeDrawer is a custom property drawer for Unity that formats an integer value representing time (in seconds) into a human-readable format, such as hours, minutes, and seconds, for fields marked with the TimeAttribute. If the DisplayHours option in the TimeAttribute is enabled, the time is shown in a hh:mm:ss format; otherwise, it is displayed in a mm:ss format. It displays both the raw integer input field and the formatted time underneath it. If the property is not an integer, it displays an error message. This drawer is useful for managing and visualizing time-based properties in the Unity Inspector.


## Tools - Others
### AddToggleButtonInHierarchy

The AddToggleButtonInHierarchy class adds a custom toggle button to the Unity Hierarchy window, allowing you to enable or disable GameObjects directly from the Hierarchy view. The toggle button appears next to each GameObject, letting you quickly change its active state. When the toggle state is changed, the script records the change using Unity's undo system and marks the scene as dirty to ensure that the change is saved. This functionality, based on a solution from Warped Imagination, streamlines the process of managing GameObject states without having to access their properties through the Inspector.

### ContextProperties

The ContextProperties class enhances Unity's Inspector by adding custom context menu options for Vector3 properties. When you right-click on a Vector3 property in the Inspector, two additional menu items appear: "Zero Out" and "One". Selecting "Zero Out" sets the Vector3 property to Vector3.zero, while "One" sets it to Vector3.one. This functionality, inspired by a solution from Warped Imagination, provides a quick way to reset or set Vector3 properties to common values directly from the context menu, streamlining the process of adjusting these properties.


### CreateMaterialsForTextures

The CreateMaterialsForTextures class is a Unity ScriptableWizard that facilitates the creation of materials for selected textures. When invoked from the Unity menu, it opens a wizard allowing users to create materials using a specified shader (defaulting to "SimpleLit"). The wizard processes selected textures in the Unity Editor's Project view, creating a new material for each texture if a material does not already exist at the same path. The script ensures that asset editing is handled properly, logging warnings if materials already exist and saving all changes at the end of the operation. This tool simplifies the batch creation of materials, streamlining the workflow for applying textures to materials.


### EditorGUIHelpers

The EditorGUIHelpers class is an abstract base class for creating custom Unity editor windows and inspectors with improved styling and functionality. It provides methods for setting up various GUI styles, creating common editor controls like buttons, sliders, and fields, and managing inspector layouts. Key features include:

- Custom Inspector Toggles: Methods for switching between default and custom inspector views.
- GUI Style Initialization: Predefined styles for labels, boxes, and buttons to ensure consistent appearance.
- Inspector Fields: Helper methods to create and manage fields for different data types, including floats, ints, vectors, colors, and more.
- Layout Management: Utility methods for creating spaced and boxed layouts to organize inspector elements effectively.

This class is designed to streamline the creation of custom inspectors and editor windows by encapsulating common tasks and providing reusable components.


### GameObjectUtilityExtensions

The GameObjectUtilityExtension class provides a utility function to remove missing scripts from selected GameObjects in Unity. It includes:

- RemoveMissingScripts() Method: This method, accessible from the Unity editor menu under "SOSXR/DANGER/Remove Missing Scripts", iterates over the selected GameObjects and removes any components that are missing from their respective GameObjects. It logs the number of GameObjects processed.
- RemoveMissingScripts(GameObject gameObject) Method: This method handles the removal of missing scripts from a single GameObject. It uses SerializedObject to find and delete elements in the components array that are null.

This script is useful for cleaning up GameObjects in a scene by removing references to non-existent scripts, which can help maintain a clean project and avoid potential errors.


### HierarchyIconDisplay
The HierarchyIconDisplay class customizes the Unity hierarchy window by displaying icons for game objects based on their components.
From Warp Imagination, this script enhances the hierarchy view by:

- Displaying icons for the most significant component (excluding Transform).
- Optionally keeping icons for prefabs.
- Drawing background color based on selection and hover state.


### MissingMonoBehaviourDetector
The MissingMonoBehaviourDetector class is a Unity editor script that detects and logs GameObjects with missing MonoBehaviours in the scene. When run, it iterates over all GameObjects in the scene and checks for any components that are null or missing. If a GameObject contains a missing MonoBehaviour, it logs a warning message in the console, indicating the GameObject name and the missing component type. This script helps identify and address issues related to missing scripts in the scene, ensuring that GameObjects are properly configured and functional.


### ReadmeShower
The ReadmeShower class is a Unity editor script that displays a README file in the Inspector window when attached to a GameObject. When found, it reads the contents of a specified README file and displays them in a scrollable text area within the Inspector upon starting Unity. This script is useful for providing documentation, instructions, or additional information directly within the Unity Editor, making it easily accessible to developers or users working on the project.


### SetIconWindow
The SetIconWindow class is a Unity editor window that allows you to set custom icons for selected GameObjects in the scene. When opened, it displays a grid of icons that can be assigned to GameObjects, allowing you to visually differentiate objects in the scene hierarchy. The window provides a simple interface for selecting and applying icons to GameObjects, enhancing the organization and visual representation of the scene contents. This tool is useful for customizing the appearance of GameObjects in the Unity Editor, making it easier to identify and manage objects within the scene. Icons need to be tagged with 'scriptIcon' to be displayed in the grid. Select all scripts you want to change the icon of, and hit Cmd+I to open the window.
