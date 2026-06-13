# Project Snapshot
- **Unity**: 6000.2.1f1
- **Build Target**: StandaloneWindows64
- **Render Pipeline**: PC_RPAsset

## Time & Physics
- Time.fixedDeltaTime: 0.01999999
- Physics.defaultSolverIterations: 12
- Physics.defaultSolverVelocityIterations: 12
- Physics.defaultContactOffset: 0.01
- Physics.reuseCollisionCallbacks: True

## Quality
- Current Quality Level: PC
- VSync: 0, AntiAliasing: 0, Shadows: All

## Layers & Tags
- Layers: Default, TransparentFX, Ignore Raycast, player, Water, UI, Jugadores, Manos, Agarrables, Peligros, Suelo
- Tags: Untagged, Respawn, Finish, EditorOnly, MainCamera, Player, GameController

### Layer Collision Matrix (pairs ignored)
- Jugadores ✕ Manos
- Manos ✕ Manos

## Build Settings Scenes
- [x] Assets/Escenas/Ice.unity
- [x] Assets/Escenas/Arena.unity

## Input System
- ControlesJugador.inputactions (ruta: Assets/Controles/ControlesJugador.inputactions)
- InputSystem_Actions.inputactions (ruta: Assets/InputSystem_Actions.inputactions)
- CinemachineDefaultInputActions.inputactions (ruta: Packages/com.unity.cinemachine/Runtime/Input/CinemachineDefaultInputActions.inputactions)
- DefaultInputActions.inputactions (ruta: Packages/com.unity.inputsystem/InputSystem/Plugins/PlayerInput/DefaultInputActions.inputactions)

## Candidate Player Prefabs
- Assets/Prefabs/Jugador Variant.prefab [PlayerInput] [Animator Humanoid]
- Assets/Prefabs/Jugador.prefab [PlayerInput] [Animator Humanoid]

## Render Pipeline Asset
- GraphicsSettings RP Asset: PC_RPAsset
