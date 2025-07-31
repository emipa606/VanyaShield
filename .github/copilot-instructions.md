# GitHub Copilot Instructions for RimWorld Mod Development

## Mod Overview and Purpose
This project aims to introduce and enhance shield mechanics in RimWorld through the creation of a new type of apparel called the "Vanya Shield Belt." This mod provides players with advanced defensive equipment that can absorb damage, display shield status, and add dynamic interactions when characters are in combat.

## Key Features and Systems
- **Vanya Shield Belt**: A new apparel item that provides a protective energy shield for the wearer.
- **Damage Absorption**: The shield belt can absorb damage, preventing it from affecting the wearer.
- **Status Display**: Provides a visual indicator (through gizmos) of the shield's current status.
- **Dynamic Effects**: Includes smoke bursts and visual feedback when the shield absorbs damage or breaks.
- **Simulated Killing**: Implements custom behavior for simulating the death mechanics of characters interacting with the shield.

## Coding Patterns and Conventions
- **Class Structure**: The project follows a modular class-based structure for clarity and reusability, with specific classes dedicated to managing components and properties of the Vanya Shield Belt.
- **Method Naming**: Uses descriptive method names such as `AbsorbedDamage` and `FreshBubble` to clearly indicate the purpose of each function.
- **Access Modifiers**: Public classes and private/protected methods help ensure that only necessary components are exposed, maintaining encapsulation and reducing errors.

## XML Integration
- **Configurable Properties**: Leverages XML files to define and configure properties related to the shield belt, such as durability, absorption capacity, and visual elements.
- **Mod Metadata**: Ensures that all mod-related information is properly documented within XML files to support easy mod management and user interaction.

## Harmony Patching
- **Patch Implementation**: Utilizes Harmony to inject new functionality into RimWorld's existing codebase without altering the original game files.
- **Modular Patches**: Classes like `PatchMain` help organize and manage patches, ensuring they are applied consistently and efficiently across game updates.

## Suggestions for Copilot
- **Consistency in Naming**: Encourage Copilot to suggest names that are consistent with existing methods and classes in the codebase for harmony and clarity (e.g., `FreshBubble`, `Reset`).
- **XML Suggestions**: Provide auto-completion for XML configurations specific to RimWorld to help streamline the integration of new mod features.
- **Error Prevention**: Suggest logic that includes error checking and validation, especially when dealing with game states and complex interactions such as damage calculations.
- **Reusable Code**: Encourage suggestions for reusable code blocks to optimize repetitive tasks related to shield effects or status updates.

By adhering to these guidelines, developers can ensure that the mod remains robust, easy to understand, and aligned with the evolving needs of the RimWorld community.
