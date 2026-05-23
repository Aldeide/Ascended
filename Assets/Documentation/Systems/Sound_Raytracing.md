# Sound Raytracing System Documentation

The **Sound Raytracing (Acoustic Propagation) System** provides real-time, dynamic sound occlusion, transmission, and environmental reverberation. By using Unity's C# Job System, Burst compiler, and multi-threaded `Physics.RaycastCommand` batching, the system executes high-fidelity physics scans without causing garbage collection (GC) allocations or blocking the main thread.

---

## Architecture Overview

The system consists of three runtime components and two configuration assets:

1.  **`AudioRaycastManager`** *(Monobehaviour)*: The central coordinator. It collects raycast requests from all sound sources and the listener, batches them into a single job, and executes them on worker threads in `LateUpdate`.
2.  **`AudioAcousticMaterial`** *(ScriptableObject)*: Data configuration defining physical sound parameters (absorption, muffling, and transmission loss) of a substance.
3.  **`AcousticPhysicMaterialMap`** *(ScriptableObject)*: A mapping database linking Unity's standard `PhysicMaterial` assets to `AudioAcousticMaterial` profiles.
4.  **`SoundOcclusion`** *(Monobehaviour)*: Attached to `AudioSource` components. Computes obstacle thickness using bidirectional raycasts and applies appropriate volume dampening and low-pass filtering.
5.  **`AcousticReverbEstimator`** *(Monobehaviour)*: Attached to the player's `AudioListener`. Shoots a spherical distribution of rays to dynamically estimate room volume/absorption, configuring the player's `AudioReverbFilter` in real-time.

---

## Setup Guide

Follow these steps to integrate dynamic acoustics into your level:

### Step 1: Create Acoustic Materials
1. In the Project window, right-click and select **Create > Systems > Audio > Acoustic Material**.
2. Create profiles for your scene's materials. Examples:
   *   **Concrete**: Low absorption (0.05), high transmission loss (35dB), low cutoff frequency (300Hz - heavily muffled).
   *   **Wood**: Medium absorption (0.15), medium transmission loss (12dB), medium cutoff (1500Hz).
   *   **Carpet/Fabric**: High absorption (0.6), low transmission loss (5dB), high cutoff (4000Hz).

### Step 2: Create the Physics Material Map
1. Right-click in the Project window and select **Create > Systems > Audio > Physic Material Map**.
2. Assign your **Default Material** (e.g. Concrete, to use as a fallback if a hit collider has no physics material).
3. Expand **Mappings** and match your level's standard Unity `PhysicMaterial` assets to the custom `AudioAcousticMaterial` profiles created in Step 1.

### Step 3: Add the Raycast Manager
* Place the `AudioRaycastManager` script onto an empty GameObject in your boot/persistent scene. (Note: If missing, it will automatically instantiate itself as a persistent DontDestroyOnLoad singleton when queried, but manual placement is preferred for setting the default layer mask).

### Step 4: Configure the Listener Reverb
1. Locate your player camera or whichever GameObject contains the Unity `AudioListener` component.
2. Attach the `AcousticReverbEstimator` component to it.
3. Assign the **Material Map** asset created in Step 2.
4. (Optional) Adjust **Ray Count** (default 32 is recommended for performance; use 16 for mobile/low-end, 64 for high-fidelity).

### Step 5: Enable Sound Occlusion on Audio Sources
1. Attach the `SoundOcclusion` component to any GameObject that has an `AudioSource` (e.g. weapons, environmental hums, enemies).
2. Assign the **Material Map** asset created in Step 2.
3. Set the **Occlusion Layer Mask** override if you wish to ignore specific layers.

---

## Configuration & Parameter Guide

### `SoundOcclusion` Component
*   **Update Interval**: How frequently the occlusion status is queried (default: `0.1s`). Increasing this to `0.2s` or `0.3s` for non-essential or distant sounds saves physics cycles.
*   **Volume Fade Speed**: The rate at which the volume scales from clear to occluded (default: `5.0`). Smooth transitions prevent sudden volume jumps (clicks/pops).
*   **Cutoff Fade Speed**: The rate at which the low-pass filter frequency interpolates (default: `5.0`).
*   **Max Thickness Threshold**: The wall thickness at which a sound becomes fully muffled (default: `5.0m`). If the wall is thicker than this, the sound is clamped to maximum occlusion.
*   **Min Cutoff Frequency**: The lowest frequency the filter will clamp to (default: `150Hz`). Lower values muffle the sound more aggressively.

### `AcousticReverbEstimator` Component
*   **Ray Count**: The number of rays cast per scan (default: `32`). Uses a golden-ratio Fibonacci sphere distribution for uniform coverage.
*   **Max Ray Distance**: The maximum radius of the scan (default: `30.0m`). If a ray misses, it is considered open air, representing sound escaping.
*   **Open Space Absorption**: The absorption coefficient assigned to missed rays (default: `1.0` - total absorption, representing open sky).
*   **Min/Max Decay Time**: Limits the RT60 reverberation length applied to the filter (default: `0.1s` to `7.0s`).

---

## Layer Mask & Optimization Best Practices

> [!IMPORTANT]
> **Use a Dedicated Acoustics Layer**:
> Raycasts check physics geometry to calculate muffling. If rays collide with small dynamic props, particles, grass, or invisible trigger volumes, the audio will fluctuate incorrectly.
> 1. Set up a dedicated physics layer (e.g., `WorldStatic` or `Acoustics`) that only includes heavy structural geometry (floors, walls, ceilings, pillars).
> 2. Exclude physics triggers, ragdolls, debris, and weapons from this layer.
> 3. Assign this layer to the `DefaultLayerMask` field on the `AudioRaycastManager` component, or override it on individual `SoundOcclusion` scripts.

> [!TIP]
> **Throttling distant sound sources**:
> If you have a large number of audio sources active, customize the `UpdateInterval` of distant or quiet sources to updates like `0.5s` or `1.0s` or disable `SoundOcclusion` entirely when the source is beyond its max audibility range.

---

## Technical Details

### Bidirectional Wall Thickness
Standard raycasts only give the entry point of a collision. To find how thick a wall is along the line of sight, the system executes a bidirectional raycast:
1.  **Forward Raycast** (Source $\rightarrow$ Listener): Returns the wall entry point (`hitForward`).
2.  **Backward Raycast** (Listener $\rightarrow$ Source): Returns the wall exit point (`hitBackward`).
3.  **Thickness**: Calculated as $t = \text{distance}(\text{hitForward.point}, \text{hitBackward.point})$.

### Eyring RT60 Reverberation
Dynamic reverb decay is calculated using the **Eyring Formula**, which is mathematically superior to the traditional Sabine formula in highly absorbent (open air) spaces:

$$RT_{60} = \frac{0.0537 \cdot R_{avg}}{-\ln(1 - \bar{\alpha})}$$

Where:
*   $R_{avg}$ is the average distance returned by the 32 spherical rays.
*   $\bar{\alpha}$ is the average absorption coefficient across all hit surfaces (including open-sky leaks).

---

## 🔍 Editor Debug Visualizations

When the Unity Editor is in Play Mode, selecting a GameObject containing either `SoundOcclusion` or `AcousticReverbEstimator` renders visual debug representations directly in the **Scene view**:

### 1. Sound Occlusion & Diffraction Visualizer
Selecting a sound source displays the sample rays extending towards the listener (player camera):
*   **Green lines**: Path is completely clear. No occlusion is applied.
*   **Red lines**: Path is blocked. Shows the forward ray stopping at the wall entry point.
*   **Magenta lines**: Visualizes the backward ray starting at the listener's end and hitting the wall exit point. The gap between the red and magenta hit points illustrates the computed **wall thickness**.
*   **Yellow lines**: Rays are currently queued in the batch and pending worker thread results.

This visual feedback makes it easy to calibrate the **Spread Width** parameter (which broadens the spacing of the left/right rays) to ensure sounds blend around specific door sizes.

### 2. Volumetric Reverb Multi-Bounce Visualizer
Selecting the player's `AudioListener` renders the full **zig-zag paths** of the bounced rays as they reflect off walls:
*   **Reflection Paths**: Renders lines connecting each bounce point. Later bounces fade out in opacity to represent physical sound energy decay.
*   **Ray Color (Red to Cyan)**: Reflects the absorption of the surfaces. Bright **red** represents reflective materials (like concrete/metal), and **cyan** represents absorbent surfaces (like carpet/fabrics) or missed rays escaping into open sky.
*   **Bounce Indicators**: Renders a wireframe sphere at each reflection point to mark the surface hits.

