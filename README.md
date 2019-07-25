# Jet Spaceman

![Jet Spaceman Screenshot](/imgs/jetspaceman1.png)

Jet Spaceman is a space shooter built in Unity by me, Jonathan Fox!

## [Click here to play it in your browser!](https://fishwash.github.io/jet-spaceman/)

## Controls
- Move: WASD or Arrow Keys
- Shoot: Space
- Equip Laser: 1
- Equip Shuriken: 2

## Programming Principles Applied
For this game, I designed around smart usage of memory. Memory leaks can occur when a game is constantly instantiating and destroying game objects. 

Memory is allocated for a game object when it is instantiated, and the memory is cleared when it is destroyed. However, this memory is not always available for reuse. Another game element must be the right size to use the memory, otherwise that portion of memory is essentially useless. If this happens repeatedly, you end up with a large amount of unusable memory.

In this game, enemies and projectiles are constantly being created and destroyed. If I simply created them with Instantiate() and Destroy(), I would end up with memory leaks. Unity handles this with automatic garbage collection, but it can be bad for performance.

My solution uses design pattern called Object Pooling. Instead of creating and destroying objects, it reuses objects from a 'pool'. The objects are pre-instantiated in the pool at the start of the game, and enabled or disabled depending on the need. 

My EnemyPooler creates 50 enemies at the start of the game. Whenever a Spawner object creates an Enemy, it asks the EnemyPooler for an unused Enemy. If there's one available, it uses that and initializes it in the right position. When that Enemy dies, it simply returns itself back to the EnemyPooler.

This allows for the same functionality as doing it a simpler way, but guarantees that there won't be problems with memory. With only 50 objects on the screen you won't notice anything, but in games where there are hundreds of objects it makes a huge difference.
