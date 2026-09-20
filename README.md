# AI Project Overview

The general idea for this project is to have a small village, something akin to animal crossing, populated with villagers and other creatures. Will feature multiple Agents
and different types of behaviour simulations. The player will be able to explore the village and interact with villagers using a simple character controller.

## Planning

[Trello Board](https://trello.com/invite/b/68ec161569f6ce4c179400b3/ATTI6cc3e6fbf02e3b2426e366961fc32751F0E07F5D/ai-planning)

## Player

Player character will be a cartoony person with minimal features to keep things simple. Again I'm taking a lot of inspiration from the animal crossing characters.
The player will be able to sleep at a house to change the time of day if needed.

Controls:
* Move - WASD
* Look - Mouse
* Interact - E

## AI

### Villagers

Villagers will be cartoony anthropomorphic animals, similar to Animal Crossing, I intend to create a routine for each villager based on the same base AI. Due to
each villager having a varied shedule I'm planning on using behaviour trees as they are more scalable.

The basic shedule for a villager may look like:
* 9am - Leave Home
* 11am - Go Bug Catching
* 12am - Picnic
* 2pm - Head to the shops
* 4pm - Go Home
* 7pm - Leave Home Again
* 8pm - Sit on the beach
* 10pm - Go Home

### Bugs

Bugs will be small and have little detail, just like a sphere with legs or wings, they won't follow a shedule like villagers instead they'll just be active at a
certain time then fade away. For example butterflies won't be around at night so they'll fade away around 6pm. The AI for bugs will operate on a simple FSM, as they
won't do much besides moving around, and interacting with villagers.

### Birds

Birds will be the most simple AI they'll just fly around when day time then go to sleep at night, at this point I'm not planning much interaction with the player or
other Agents.

## Village

The village will be on an island, each villager will have a home that they start and end their day at. The player will also have a home they can visit to sleep at,
this gives the opportunity to the player to change the time of day. There will be areas for bugs such as a little forest. The island isn't going to be super detailed
or anything rather it will just be an environment to showcase the AI.

## References

* (Custom A* Pathing)[https://youtube.com/playlist?list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW&si=r5vXfnK-WsItSKJJ]