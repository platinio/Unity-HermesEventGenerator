# Unity-HermesEventGenerator
I did Hermes to fulfill certain requirements which were a deal breaker for me:

- Support for Assembly definitions.
- Modularity so different modules can communicate with simple events without knowing anything about each other.
- The ability to debug these events from an editor window, so I do not need to play the game and follow certain time-consuming steps just to debug an event.
- Don't want to use strings or enums to maintain the different events which might be hard to maintain and prone to error.
- Auto-generated custom event units (Visual Scripting units) for each game event.
- Support for event scopes, I can decide if I want everybody in the scene to know about something, or just let specific GameObjects and their children about an event.

### Define your custom events using the Hermes Editor Window

![alt text](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/hermesEditorWindow.png?raw=true)

### Listen to your new event

```csharp
private void Start()
{
  //register listener
  sceneGameEvents?.GameEventDispatcher.OnDoDamageGameEvent.AddListener(OnDoDamage);
}

public void OnDoDamage(OnDoDamageEventArgs args)
{
  //if this damage event is not for me ignore
  if (args.to != this) return;
            
  HP -= args.damage;
  if (HP <= 0)
  {
    HP = 0;
    Destroy(gameObject);
  }
}
```

### Trigger Event

```csharp
sceneGameEvents?.GameEventDispatcher.OnDoDamageGameEvent.Raise(from, to, damage);
```

### Use Hermes Editor to Debug your events

Each event has a generated custom editor to trigger the events using the editor window.

![alt text](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/hermesEditorWindowDebug.png?raw=true)

# How to install?

# Getting Started

### Create scene event scope

There should be always a scene scope event so you can listen and trigger events in your scenes, create a new GameObject and add SceneGameEvents and GameEventDispatcher components.

![alt text](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/sceneGameEvents.png?raw=true)

### Open the Editor Window

Open the Hermes editor window using Window/General/Hermes Editor

### Create a new event definition

![alt text](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/creatEventDefinitionStep.png?raw=true)

### Fill the new event details

![alt text](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/eventDefinitionDetails.png?raw=true)

**Event Details:** event name and description, name is the real name you will use in code to listen and trigger this event so it has to be a code friednyl name.

**Arguments:** event arguments, for each argument you have to write the type and variable name.

**Namespaces:** Depending in your event arguments you might need to include different namespaces, for example if you use a Rigidbody as an argument, you will need to include UnityEngine namespace.

### Regenerate Events

![alt text](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/regenerateEvents.png?raw=true)

After defining your events Hermes will generate the code for you.

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
  //if this damage event is not for me ignore
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
Notice we are using #if HERMES_EVENTS_GENERATED to encapsulate events related code, Hermes will add this scripting symbol if the events were generated, this way if you have problems with the event generation you can remove this scripting symbol by hand and still compile, this is mostly so you can still access Hermes Editor window and fix events and regenerate.


### Trigger your new event

```csharp
var sceneGameEvents = ServicesContainer.Resolve<ISceneGameEvents>();
sceneGameEvents?.GameEventDispatcher.OnDoDamageGameEvent.Raise(from, to, damage);
```

### Remove listeners

```csharp
private void OnDestroy()
{
#if HERMES_EVENTS_GENERATED
  var sceneGameEvents = ServicesContainer.Resolve<ISceneGameEvents>();
  sceneGameEvents?.GameEventDispatcher.OnDoDamageGameEvent.RemoveListener(OnDoDamage);
#endif
}
```





