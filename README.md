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

