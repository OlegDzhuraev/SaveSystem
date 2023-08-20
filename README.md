# Save System
World state save system for Unity engine. 

**Note:** This is very early version, not for production use.

## Dependencies
- InsaneOne.Core
- Newtonsoft.JSON

## How to install
Download and unpack to your project.

Package Manager actually not fully supported.

## How to use

### Concepts
- **SaveSystemData** - ScriptableObject, which store all prefabs with Saveable components data.
- **Saveable** - abstract MonoBehaviour-class, which you should derive for all your "saveable" object types. One prefab/object should have only one Saveable component.
- **SaveDTO** - DTO-class, which can be serialized (it should include no reference types, no complex types which can not be serialized, etc).
- **WorldSaver** - utility static class, handles world state save.

### How to start
First of all, create **SaveSystemData** asset from Project Window by right-click and selecting **Create/InsaneOne/Save System Data**.

Next, you need to make a new **Saveable**-deriven components for all objects in your game, which should be saved (dont forget, you need DTO-class for each Saveable).

Look at **BasicSaveableObject** and **BasicObjectDTO** for example.

### How to save and load world state
```cs
var save = WorldSaver.Collect(); // will collect all Saveable objects from scene and return serializable list. You can serialize it to JSON or something else and save the way you want.

// var jsonString = JsonConvert.Serialize(save); // get serialized save string, etc
// <...> 

WorldSaver.Load(save); // will load scene state from this save.
```

### Scene objects
If you have objects on scene which should be saved and **will not** be spawned as prefab, you need check **Is Scene Objec**t toggle in the **Saveable** component. These objects will not be placed in the **Save System Data**, since they don't need prefab hash.

Now, all is ready to be saved. Only one step is left:

After you're placed all your objects on scene, use **Save System Tool** from the top-menu. Here you need to click **Update scene Saveables IDs** button.

In current version, you need to do it **every time you add/remove objects** with **Saveable** component on scene. Prefer to do it when your scene is ready and will be rarely changed in future.

## License
Apache License, Version 2.0
