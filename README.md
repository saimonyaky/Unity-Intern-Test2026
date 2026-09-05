# Unity Gameplay Test

## Overview
This is a Unity technical test focused on modifying and enhancing the gameplay of the original project.
---

## What I've worked on

### 1. Item Interaction & Movement
* **Description:** Move items from the board to the bottom cells by tapping on them. Once an item moves to a bottom cell, you can’t move it back to the board.
* **Relevant Scripts:** `Board.cs`, `BoradController.cs`

### 2. Matching System & Cell Management
* **Description:** Developed the logic to manage the bottom cells. The system evaluates the items currently in the cells; if there are **exactly three identical items**, they are automatically cleared, freeing up capacity for new items.
* **Relevant Scripts:** `Board.cs`, `BoradController.cs`, `Utils.cs`

### 3. Win & Loss Conditions
* **Description:** Integrated the end-game logic based on the player's progression.
    *   **Victory:** Triggered when the player successfully clears all items from the board.
    *   **Defeat:** Triggered if the player fills up all the bottom cells without making a match (no more space to move items).
* **Relevant Scripts:** `LevelCondition.cs`, `LevelTime.cs`, `GameSettings.cs`, `LevelTime.cs`
