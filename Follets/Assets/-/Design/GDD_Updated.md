# GDD - Blade Frenzy (Actualizado)

Este documento resume el GDD original proporcionado por el diseñador y aplica las actualizaciones acordadas por el equipo (22/05/2026).

## 1. Visión general
- Género: Arena de combate 3D local multijugador (2-4 jugadores)
- Plataforma objetivo: PC con soporte para mandos PS4/PS5/Xbox/Switch Pro

## 2. Mecánicas principales

2.1 Movimiento
- Movimiento libre en arena 3D. Sprint.

2.2 Armas
- Arma equipada por el jugador. Puede usarse en melee y lanzarse.
- El arma tiene `Weight` que afecta tiempo de carga y knockback.

2.3 Parry y recogida de arma (ACTUALIZADO)
- El parry es un botón independiente.
- Si el jugador hace un parry mientras su hitbox está activa, podrá recoger el arma volante (si impacta o intercepta) durante la ventana de parry.
- Si el arma lanzada acaba su trayectoria y queda en el suelo, un jugador desarmado la recogerá automáticamente al acercarse.

2.4 Ataque cargado (ACTUALIZADO)
- El ataque cargado se activa manteniendo pulsado el botón de `Attack`.
- Tiempo estándar de carga: 2s para un arma de peso medio. El tiempo de carga varía según `Weight`.
- El jugador puede mantener la carga indefinidamente; si es golpeado mientras carga, será desarmado y la carga se pierde.
- Actualmente, un ataque cargado quita 1 vida (TBD para 2 vidas en futuras iteraciones).

2.5 Golpeado, derribo y roll (ACTUALIZADO)
- Si un jugador es golpeado, entra en estado `KnockedDown` (derribado) y recibe invulnerabilidad temporal.
- Tras un breve periodo de recuperación, podrá ejecutar un `Roll` que desplaza una distancia fija en la dirección del joystick; durante el roll también es invulnerable.
- Los ataques cargados provocan mayor knockback (empujando al jugador más lejos).

2.6 Hitstop
- Implementar hitstop breve al golpear a un jugador, y un hitstop ligeramente mayor en parry/mikiri y en ejecuciones.

2.7 Interact / recoger objetos
- Para recoger una caja o roca, el jugador debe pulsar el botón `Interact` estando desarmado.
- Para recoger un arma en el suelo: si el arma está quieta, se recoge automáticamente si el jugador está desarmado y se acerca.

## 3. Requisitos multijugador local
3.1 Número de jugadores
- Mínimo 2 jugadores locales. Soporte hasta 4.

3.2 Mapeo de mandos
- El proyecto usará `Input System` de Unity. Debe soportar mandos PS4/PS5, Xbox y Switch Pro.
- Los jugadores podrán reasignar sus controles de mando desde la pantalla de selección de personaje pulsando `Select`.

## 4. Primera iteración (MVP)
- Escena de prototipo con placeholders (capsule players), 2 jugadores por defecto.
- PlayerFactory que crea jugadores en runtime con `CharacterController`, `PlayerInput`, `PlayerController`, `PlayerCombat`, `PlayerInventory`, `PlayerState`, `PlayerAnimatorBridge` y `PlayerHUD`.
- Sistema de hitstop básico y recogida de arma en trigger.
- Rebinding básico expuesto desde la UI de selección de personaje.

## 5. Arte y animaciones
- Placeholders en MVP. Preparar `PlayerAnimatorBridge` para conectar parámetros con gameplay.

## 6. Notas técnicas
- Unity Editor: 6000.4.2f1
- Carpeta de scripts propuesta: `Assets/Scripts/Game/` (ya creada)

## 7. Tareas siguientes
- Crear escena `Gameplay.unity` y añadir `SceneInitializer` para spawn de jugadores.
- Crear sistema de selección de jugadores y rebinding desde UI.
- Implementar mejores checks de hitboxes, parry windows y efectos visuales.

---
Documento actualizado por el equipo técnico el 22/05/2026. Para exportar a PDF, abrir este archivo y usar una herramienta de conversión (por ejemplo, Visual Studio Code export, o Word) o indícame si quieres que lo genere en formato PDF aquí (puede requerir conversión externa).
