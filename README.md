# 🚀 Mars Rover Simulation with A* Pathfinding in Unity

## 🎯 Overview

This Unity project simulates a **Mars rover** navigating through a Mars-like terrain using the **A\*** pathfinding algorithm and basic **obstacle avoidance**. The simulation demonstrates autonomous navigation between customizable waypoints while detecting and avoiding obstacles in real time.

---

## 🧠 Key Features

- 🛰️ Mars rover simulation in a rocky terrain
- 🌌 Uses **A\*** pathfinding algorithm to find the shortest route between waypoints
- 🧱 **Obstacle detection** using raycasting
- 🤖 Smooth navigation and dynamic route adjustment
- 🧾 **Performance tracking** (distance & time) logged in Unity's console
- 🔁 Rover travels from start → goal1 → end → back to start

---

## 🌍 Environment & Assets

- Mars terrain built using **Unity’s Terrain tools**
- Mars textures from free Unity Asset Store pack:
  - 🪐 [Mars Landscape 3D](https://assetstore.unity.com/packages/3d/environments/landscapes/mars-landscape-3d-175814)
- Rover model: *Mongo Sci-Fi Car*
- Core scripts adapted from Unity workshops:
  - `PathfindingTester.cs`
  - `AStarManager.cs`
  - `ObstacleAvoidance.cs`

---

## 📂 How to Use

1. **Clone this repo** or download the ZIP
2. Open the project in **Unity Hub**
3. Open the main scene (e.g. `MarsScene`)
4. In the **Hierarchy**, click the `Rover`
5. In the **Inspector**, select a `Waypoint` for the rover to travel to
6. Click ▶️ **Play** to run the simulation

---
