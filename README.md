# Hermes

A code-generation-based event system for type-safe, decoupled communication between modules, with built-in editor debugging and Visual Scripting support.

---

## Overview

When modules need to communicate — a skill lands a hit, a character dies, a turn ends — the naive approach is direct references. Module A calls Module B directly, which creates hard dependencies and breaks the assembly definition boundaries that keep the project modular.

Hermes solves this with a generated event bus. You define events in an editor window, click Regenerate, and Hermes produces strongly-typed C# classes for each event. Any module can raise or listen to any event without knowing anything about the other module. No strings, no enums, no manual marshaling.

---

## Getting Started

### Create scene event scope

There should always be a scene scope event so you can listen and trigger events in your scenes. Create a new GameObject and add `SceneGameEvents` and `GameEventDispatcher` components.

![Scene Game Events](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/sceneGameEvents.png?raw=true)

### Open the Editor Window

Open the Hermes editor window using **Window → General → Hermes Editor**

![Hermes Editor Window](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/hermesEditorWindow.png?raw=true)

### Create a new event definition

![Create Event Definition](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/creatEventDefinitionStep.png?raw=true)

### Fill the new event details

![Event Definition Details](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/eventDefinitionDetails.png?raw=true)

**Event Details:** event name and description. The name is what you will use in code to listen and trigger this event, so it must be a code-friendly name.

**Arguments:** the typed parameters the event carries. For each argument write the type and variable name.

**Namespaces:** depending on your event arguments you may need to include additional namespaces. For example, if you use a `Rigidbody` as an argument you will need to include the `UnityEngine` namespace.

### Regenerate Events

![Regenerate Events](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/regenerateEvents.png?raw=true)

After defining your events, click **Regenerate Events** and Hermes generates the code for you. It also adds `HERMES_EVENTS_GENERATED` as a scripting define symbol across all build targets.

### Listen to your new event

```csharp
private void Start()
{
#if HERMES_EVENTS_GENERATED
    var sceneGameEvents = ServicesContainer.Resolve<ISceneGameEvents>();
    sceneGameEvents?.GameEventDispatcher.OnDoDamageGameEvent.AddListener(OnDoDamage);
#endif
}

#if HERMES_EVENTS_GENERATED
public void OnDoDamage(OnDoDamageEventArgs args)
{
    // If this damage event is not for me, ignore it
    if (args.to != this) return;

    HP -= args.damage;
    if (HP <= 0)
    {
        HP = 0;
        Destroy(gameObject);
    }
}
#endif
```

Use `#if HERMES_EVENTS_GENERATED` to wrap event-related code. If event generation fails you can remove the scripting symbol by hand and the project still compiles, letting you access the Hermes Editor to fix and regenerate.

### Trigger your new event

```csharp
var sceneGameEvents = ServicesContainer.Resolve<ISceneGameEvents>();
sceneGameEvents?.GameEventDispatcher.OnDoDamageGameEvent.Raise(from, to, damage);
```

### Remove listeners

Always unsubscribe in `OnDestroy` to avoid stale listeners:

```csharp
private void OnDestroy()
{
#if HERMES_EVENTS_GENERATED
    var sceneGameEvents = ServicesContainer.Resolve<ISceneGameEvents>();
    sceneGameEvents?.GameEventDispatcher.OnDoDamageGameEvent.RemoveListener(OnDoDamage);
#endif
}
```

---

## Hermes Settings

Open **Project Settings → Hermes Settings**

![Hermes Settings](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/hermesSettings.png?raw=true)

There are separate paths for Arguments and Events code generation. You can change them by clicking the button on the right — useful when using assembly definitions that require generated code in a specific location. Otherwise the default values work fine.

The **Use Visual Scripting** toggle generates code to raise and listen to events from Visual Scripting graphs. Disable it if you are not using Visual Scripting in your project.

---

## How to Debug Events

Start the game, open the Hermes editor, select the event you want to debug, and click **Add Event Trigger Component**.

![Add Event Trigger Component](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/addEventTriggerComponent.png?raw=true)

If all the arguments of your event can be serialized, Hermes generates an inspector UI where you can set the parameters by hand and trigger the event with custom values.

![Trigger Event Debug](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/triggerEventDebug.png?raw=true)

### Monitoring All Events

`GameEventDispatcher` exposes an `OnGameEventRaisedEvent` callback that fires whenever any event is raised. Use it to log or visualize event traffic during development.

---

## Visual Scripting

When **Use Visual Scripting** is enabled in Hermes Settings, each event gets a generated Visual Scripting unit. These units appear in the visual scripting node search and let you raise or listen to events directly inside Script Graphs without writing C#.

The `ScriptGraphContainer` component manages embedded Script Graph instances on a GameObject when you need to attach Visual Scripting graphs at runtime:

```csharp
// Add a script graph at runtime
scriptGraphContainer.AddScriptGraph(myScriptGraphAsset);

// Remove when done
scriptGraphContainer.RemoveScriptMachines(myScriptGraphAsset);
```

---

## API Reference

### Generated Event Pattern

Every defined event follows the same pattern on `GameEventDispatcher`:

```csharp
// Raise the event
dispatcher.On{EventName}GameEvent.Raise(arg1, arg2, ...);

// Subscribe
dispatcher.On{EventName}GameEvent.AddListener(handler);

// Unsubscribe
dispatcher.On{EventName}GameEvent.RemoveListener(handler);
```

### Key Classes

| Class | Description |
|---|---|
| `GameEventDispatcher` | Central hub that holds all generated event properties |
| `SceneGameEvents` | MonoBehaviour wrapper that exposes the dispatcher and registers with DI |
| `ISceneGameEvents` | Interface resolved via `ServicesContainer` to access the dispatcher |
| `GameEventArgsBase` | Base class for all generated event args classes |
| `EventTriggerBase` | Base class for generated editor trigger components |
| `ScriptGraphContainer` | Runtime manager for embedded Visual Scripting graph instances |

### Resolving the Dispatcher

```csharp
var sceneEvents = ServicesContainer.Resolve<ISceneGameEvents>();
var dispatcher  = sceneEvents?.GameEventDispatcher;
```
