# GorillaLocomotion
🦍 GorillaLocomotion – Summary of the Entire Repo

📌 What the Repository Is

A Unity C# codebase containing movement, physics, utilities, and support systems used for Gorilla Tag–style locomotion.
It includes locomotion helpers, time systems, object pooling, bone offsets, gameplay utilities, and core framework scripts.

🧩 Major Systems + Their Roles

Below is a consolidated and simplified breakdown of the important classes and their functions.

1️⃣ Time System
GTTime

Custom wrapper around Unity time.

Gives consistent timing for physics, VR updates, and tick-based systems.

Helps avoid desync between Update and FixedUpdate.

2️⃣ Tick System
TickSystemTimer
TickSystemTimerAbstract

Custom “tick loop” separate from Unity’s update loop.

Runs logic at fixed intervals (good for stable physics).

Helps locomotion feel consistent even on unstable framerates.

3️⃣ Locomotion / Player Helpers

(This repo version doesn’t contain one giant PlayerMovement class, but it includes supporting components)

BoneOffset

Adjusts bones on the VR avatar (hands, arms).

Keeps avatar limbs aligned with VR controllers.

Prevents stretching/breaking animation during movement.

RigidbodyHighlighter

A debug tool that visually shows physics bodies.

Helps developers test collisions and locomotion physics.

4️⃣ Asset and Prefab Management
GTAssetRef<T>
GTDirectAssetRef<T>

Strongly-typed references to game assets (prefabs, scriptable objects).

Helps load or link assets safely at runtime or in the editor.

AssignInCorePrefabAttribute

Custom attribute for MonoBehaviours.

Automatically assigns referenced prefabs or core objects.

5️⃣ Lifecycle / Object Behavior Interfaces
IRefreshable

Object can update/refresh itself.

IResettableItem

The object can return to its default state.

ISpawnable

An object can be spawned dynamically by game systems.

These interfaces create consistent behavior across many scripts.

6️⃣ Object Pooling
ObjectPool<T>
ObjectPoolEvents

Recycles objects to improve performance.

Common in physics-heavy VR games where objects spawn/despawn frequently.

7️⃣ Logging Utilities
GTLogErrorLimiter

Limits how often errors are printed.

Prevents console spam in VR (important for performance).

8️⃣ Gameplay / Environment
InfectionLavaController

Controls the “lava/infection” mechanic.

Changes behavior when a player touches lava or infected surfaces.

9️⃣ Misc Utility Systems
DelegateListProcessor

Manages lists of callbacks/delegates.

Used for custom events, listeners, or internal signaling.

🎯 High-Level Summary for AI

Here is the entire repo summarized in an AI-friendly block:

The GorillaLocomotion repository is a Unity C# project that provides core infrastructure for VR locomotion, timing, physics utilities, bone adjustments, game object management, and event handling.
It does not contain one single “PlayerMovement” class, but instead breaks the locomotion support into modular systems: GTTime for timing, TickSystem for fixed-interval logic, BoneOffset for VR avatar alignment, object pooling for performance, asset reference types for prefab management, and interfaces for object lifecycle control.
Additional scripts handle logging, debugging, and gameplay features like infection/lava interactions.
Altogether, the repo forms the backbone that a Gorilla Tag–style locomotion system would plug into.
