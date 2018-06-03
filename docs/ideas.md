# Ideas

This document contains random ideas / ramblings that we can keep track of as we are inspired.

## Coats of Paint
* Particle effects
    * Glowing effects (fireflies) / Glowy whisps
    * Water droplets
    * Leaf rustles as player walks (on impact with walking animation)
    * Bugs and little bits floating around in the background
        * When you get near, the bugs disperse
        * Sound design
    * Random jungle sounds
* Visual flare
    * Vines hanging from platforms
    * Sprite skewing when touching vines (create illusion of interactivity)
    * Wood / trunk sprites holding up the platforms

## How to track level metrics

We can use this system to make sure levels are balanced and fun.

* Each level has individual obstacles
* Obstacles are deemed "fun" when it takes `T` tries to clear them
* `T` is defined as:
    * The number of tries a player, who has experience clearing all past levels, takes until she successfully passes an obstacle.
* Each level has an `A` ammount of obstacles. 

This `T` number is key, and it's curve accross level will determine the kind of game we want. A constant `T` will mean the levels get harder, but progress is consistent. A slowly growing `T` means there might be a potential for player frustration, but greater reward.

`T` can be measured by placing invisible checkpoints on levels after obstacles. Levels need to be designed around the ammount of obstacles they have.

`A` will specify how long a level with a constant `T` will be cleared in average. Like `T`, `A` can increase or remain constant over time.

`A` and `T` values can only be gathered during play testing.

# Assets we can use

* Pixel particles
https://untiedgames.itch.io/wills-magic-pixel-particle-effects

* Mechanical fortress + other cool tilesets by the same author:

https://untiedgames.itch.io/mechanical-fortress-tileset
