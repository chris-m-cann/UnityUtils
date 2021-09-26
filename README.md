# UnityUtils
A project for me to dump in all the reusable utilities I use in my other projects

#### Current Dependencies (other than unity ones)
- DoTween
- Odin

##Contents

There are a number of test scenes showcasing the partiular utilities. These are:

### TweenBehaviour
This is a component used to setup tweens in the inspector. For example having a UI panel move to center from off screen. 
The compoennt holds a list of Tweens of various types that can be kicked off via TweenBehaviour.Play(x) where x is the index of the tween in the list.
Currently supported tween types are:
- Position
- Rotation
- Local Scale
- Rect Position (anchored)
- Sprite Alpha
- CanvasGroup Alpha
- Renderer material floats (as in properties in the materials shader)
- Image/RawImage material floats

There are a number of settings to customise such as:
- Duration
- PlayType
- Whether it uses scaled or unscaled time
- play on enable
- the easing curve to ues
- initial delays
- use absolte values or relative ones
- a unity event kicked off when the tween is complete

See the scene TweenBehaviour for details and examples.

### GameEvent
Scriptable object based variables and events based off of this talk [here](https://www.youtube.com/watch?v=raQ3iHhE_Kk&t=1226s)

It basically just stores either a value, an event or both (in observable variable) as an asset that can be passed to various objects around the scene.
This removes coupling between scene objects. Allowing you to work on prefabs with no scene references.
It also helps build event based systems so when event X fires it is easy to add a new listener to do something new at that point.

The main parts are:
- Variable<T>: The  asset representation of a value
- GameEvent<T>: The asset representation of an event
- ObservableVariable<T>: a combination of both of the above
- EventReference : An either type that can be either a GameEvent or an ObservableVariable. Used by components that just want to subscribe to an event without caring about which asset type it is.
- VariableReference: Similar to EventReference. This is a type that can be either a constant, a Variable<T> or an ObservableVariable<T>
- VariableTypeBuilderWindow: An EditorWindow built to automate making new Variables, Events and such. To add a new supported type this way just got to the asset menu under Custom click "new variable...". This will open up the editor window that can help automate this. It is also available under "Tools/VariableTypeBuilder"

For more details and example uses see the GameEvent scene  

