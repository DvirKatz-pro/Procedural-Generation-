# Procedural Generation 
 Details the procedural Generation that i did for a video game project


For a Video game project class i created a procedural world using perlin noise 

### 1. Features & More:

* A 3x3 buffer
* 3 biomes
* Objects such as trees and rocks
* Rivers
* Town generation
* Day/Night cycle

### 2. Implementation:
Several noise maps are used to reliably generate a sudo random world from a seed, these noise maps include

* Biome noise map
* River noise map
* Object noise map
* Town noise map

### 3. The buffer

An initial 3x3 tile set is generated with the player being in the middle, as the player moves through the world, the 3x3 is "shifted" depending on the players movement direction. If the player walks to the north for example, then 3 tiles to the south will be removed and 3 tiles to the north will be added creating a seamless world  


### 4. Gifs:

Note that the camera is intentionally zoomed out so that the workings of the generation can be seen. In actual gameplay the player will not be able to see the world edges 

*Demonstrating the buffer and the savanah biome
![](GIFs/buffer.gif)

*Demonstrating towns
![](GIFs/town.gif)

*Demonstrating snow biome
![](GIFs/snow.gif)

*Demonstrating rivers
![](GIFs/river.gif)


### 7. Requirements:
* Unity version 2020.1.4 or greater 
