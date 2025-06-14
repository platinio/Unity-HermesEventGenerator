# Unity-HermesEventGenerator
I did Hermes to fulfill certain requirements which were a deal breaker for me:

- Support for Assembly definitions.
- Modularity so different modules can communicate with simple events without knowing anything about each other.
- The ability to debug these events from an editor window, so I do not need to play the game and follow certain time-consuming steps just to debug an event.
- Don't want to use strings or enums to maintain the different events which might be hard to maintain and prone to error.
- Auto-generated custom event units (Visual Scripting units) for each game event.
- Support for event scopes, I can decide if I want everybody in the scene to know about something, or just let specific GameObjects and their children about an event.

#### Define your custom events using the Hermes Editor Window

![alt text](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/hermesEditorWindow.png?raw=true)

#### Listen to your new event

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

#### Trigger Event

```csharp
sceneGameEvents?.GameEventDispatcher.OnDoDamageGameEvent.Raise(from, to, damage);
```

#### Use Hermes Editor to Debug your events

![alt text](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/hermesEditorWindowDebug.png?raw=true)

# How to install?

# Getting Started

## Create Event Definition

### Open Editor Window

Open the Hermes editor window using Window/General/Hermes Editor

### Create new event definition

![alt text](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/creatEventDefinitionStep.png?raw=true)

### Fill event ddefinition

![alt text](https://github.com/platinio/Unity-HermesEventGenerator/blob/main/ReadmeResources/eventDefinitionDetails.png?raw=true)

Event Details: event name and description, name is the real name you will use in code to listen and trigger this event so it has to be a code friednyl name.

Arguments: event arguments, for each argument you have to write the type and variable name.

Namespaces: Depending in your event arguments you might need to include different namespaces, for example if you use a Rigidbody as an argument, you will need to include UnityEngine namespace.

