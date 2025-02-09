# PlatformFramework
Modified movement, platforming, and dash framework from [>_TER]RAFORM. Full game is still under development, but a demo is playable here: https://skyenxt.itch.io/terraform

Common controls are below (found in `PlayerMovement.cs`):
 - Movement: Left/Right Arrow
 - Jump: Up Arrow
 - Dash: Space

Controls specific to [>_TER]RAFORM are below (found in `Interpreter.cs`):
 - Temporary Platform: Left Shift
 - Destroy: X (you won't be able to see this one in action in this framework)

Background pulsing and music are also included, but that was mainly because I was too lazy to take it out. Open the scene `StoryMode` to get started (`Assets/Scenes/StoryMode`). Note that all platforms should be under EffectsLayer as well as have a collider component to be detected--to make things easy, just copy paste existing platforms and change dimensions accordingly.
