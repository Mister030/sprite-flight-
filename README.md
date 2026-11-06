# Sprite Flight

Een 2D-ruimtespel gemaakt in Unity. Je bestuurt een ruimteschip met de muis en ontwijkt rondvliegende obstakels. Hoe langer je overleeft, hoe hoger je score.

## Features

- Besturing met de muis via het nieuwe Unity Input System, met een vlam en motorgeluid tijdens het stuwen
- Obstakels met een willekeurige grootte, richting, snelheid en draaiing; kleinere obstakels gaan sneller
- Score en high score in beeld via UI Toolkit, met een restart-knop
- Explosie- en bounce-effecten met geluid bij botsingen

## Code

| Script | Wat het doet |
| --- | --- |
| [`PlayerController.cs`](Assets/Scripts/PlayerController.cs) | Beweging, score, high score en wat er gebeurt bij een botsing |
| [`Obstacle.cs`](Assets/Scripts/Obstacle.cs) | Willekeurige eigenschappen van een obstakel en het bounce-effect |

## Openen

Open de map in Unity 6 (`6000.0.78f1`) en start de scène `Assets/Scenes/Game.unity`.

Stack: C#, Unity 6, Input System, UI Toolkit.
