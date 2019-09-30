_________________________________________________________________________________________________________

Package "Tail Animator"
Version 1.2.6

Made by FImpossible Games - Filip Moeglich
https://www.FilipMoeglich.pl
FImpossibleGames@Gmail.com or Filip.Moeglich@Gmail.com

_________________________________________________________________________________________________________

Unity Connect: https://connect.unity.com/u/5b2e9407880c6425c117fab1
Youtube: https://www.youtube.com/channel/UCDvDWSr6MAu1Qy9vX4w8jkw
Facebook: https://www.facebook.com/FImpossibleGames
Twitter (@FimpossibleG): https://twitter.com/FImpossibleG
Google+: https://plus.google.com/u/3/115325467674876785237

_________________________________________________________________________________________________________
Description:

Tail Animator is package of behaviours simulating elastic tail movement.
Animation is procedural so it reacts with changes on object's position / rotation / scale.
Just add component to object you want to animate elastic, define start transform (or bone) and play with 
parameters, but remember that bones rotations orientation should be correct with unity axes.
Scripts are commented in detailed way.
You also can put component on animated meshes and blend procedural animation with animator's movement.

Main features:
-Includes interactive demo scenes
-Includes full source code
-Scripts are commented in detailed way
-Visual friendly inspector window

Main Components:
- FTail_Animator - Component to animate transforms like tail
- FTail_AnimatorBlending - Component dedicated for animated objects with possibility to blend animation with tail animator
- FTail_Animator2D / UI - Components to animate tail behaviour in 2D space
- FTail_Editor_Skinner - Components to create skinned model ready to be animated from static mesh, all inside unity

_________________________________________________________________________________________________________
Version history:

V1.0 - 18/07/2018:
Initial release

V1.0.1
- Added overrides for Start() method because in some cases it wasn't executed for some reason, probably different .net targets
- Updated fSimpleAssets resources to V1.1

V1.1.0
- Added new examples and components to animate transforms with tail animator in 2D space- UI and Sprites
- Now you can put one transform to bones list and it will be root bone, so you can have component attached to much other object than tail bone
- Custom inspector to see all parameters more clear
- New example scenes
- School of fish example scene

V1.1.1
- Added "Automatic" tuning option for fixing orientation axes automatically by default

V1.1.2
- Added another auto-tuning wrong rotations options, making Tail Animator more universal to cooperate with different skeleton structures

V1.2.0 [Big Update]
- Removed some scripts because they are not needed anymore, they're replaced by more efficient ones
(FTail_FixedUpdate etc. because now you can choose which update clock should be used inside inspector)
- Animator components are renamed to be more intuitive
(FTail_Sinus to FTail_Animator, FTail_MecanimBones to FTail_AnimatorBlending, FTail_2D to FTail_Animator2D etc.)
- Upgraded custom inspector and added rendering gizmos in scene view
- Added icons for individual components so you find them easier
- Added FTail_Editor_Skinner component to skin static meshes onto skinned mesh renderers with tail bone structure inside unity editor
- Now bones hierarchy will not be changed at all in order to animate tail

V1.2.1
- Option "Auto go through all bones" under "Tuning Parameters" is renamed to "Full correction" and is upgraded 
to calculate correction for each bone individually, makes it match initial pose at lazy state 
also added new option "Animate Corrections" when you toggle "Full correction" which is matching keyframed animation's rotations

V1.2.2
- Small tweaks for inspector window
- Added Button "Connect with animator" which is changing variables 'Refresh Helpers', 'Full Correction' and 'Animate Corrections' so you switch to newest feature with one click and more intuitivity
- Added toggle 'Refresh Helpers' which is refreshing helper variables in next frame, to use when your model's T-pose is much different from animations of tail chain you want to animate (for example arms)
this option allows you to add tail animator to character's arms, pelvis etc. enable 'Full Correction' and 'Animate Corrections' so your model starts to be elastic and you can adjust stiffness
- Added manual pdf file with visual friendly description to help you use Tail Animator features in most effetive way

V1.2.3
- Added toggle "Queue To Last Update" which is putting component update order to be last, helpful for integrating other components which affecting bones hierarchy from code like Spine Animator
So when you have this option toggles, tail animator will work on bones positions and rotations dictated by spine animator, not by unity animator
- Few small polishes

V1.2.4
- Added component "FTail_Animator_MassUpdater" which handles tail animators update method in one Update tick, it can boost performance of tail animator when you are using a lot of tail animators (from more than 100 it can give some boost, but when more tail animators, then difference more noticable)
- Added new example scenes:
	Fockatrice: Quadroped creature with tail, wings, long neck and feather like elements, all of that enchanced by tail animator to give you some ideas
	Flime: Not animated slime model with some bones, animated only by tail animator components
	Hair performance tests: Scene with different hairstyles using lots of tail animator components !read provided info on canvases!
	Furry Fiped: Rouch example showing how many tails you can compute in the same time with low cpu overload in reference to components count

V1.2.5
- Added support for one and two - length bone chains
- Added variable "Root To Parent" to make first bone in chain be affected by parent transform (sometimes you will have to toggle "Full Correction" variable to make it work)

V1.2.6
- Added experimental collision detection feature
- New example scene with collision feature
- Added parameter "Gravity Power" simulating weight of tail
- Some fixes inside editor stuff
