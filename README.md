# Readme.md
To have the best view:
1) please don't modify the window size after it opens
2) please don't zoom in/out, rotate view
3) you can still try 1. 2., at least it won't crush...

- Work done by me

Based on the planet model, I implement a ray tracing, and a cone hit test, add new vertices, new time-scheduling parameters and other effects you could see. The ray is obtained by transforming the fragment coordinate into wolrd coornidate. The user can modify the resolution by editing the values of 'width' and 'height' in main.cpp (and REBUILD the exe). There is not triangle meshes representing the cones. Instead, I use a huge ball at the back as a screen and present cone intersection on the screen. I also, by modifying the ray's direction, create a background.

- Aknowledgement

ConvertToEigenMatrix in main.cpp adapted from https://stackoverflow.com/a/40512883

line 121-131 in main.cpp (read vertex from stl file) adapted from TA Darren

sepUpLights in planet.fs structure adapted from: https://www.shadertoy.com/view/XstSRX
(Note that it is for setting up the rotations and radius only)

The derivation of cone intersection formula adapted from http://lousodrome.net/blog/light/2017/01/03/intersection-of-a-ray-and-a-cone/

 - Idea of this?

This project is named *imagination*. The idea came up when I was debugging my shaderpipeline at 4 am. The moon falls onto the planet right after civilivation begin(city lights) - 1980s, the era of disco music. The abstruct aftermath of planet explosion has transformed the moon into a giant red skull, which live countless home-less souls that dance with disco music forever and ever...
One could watch my work while playing some disco musics. At least I did ;)

Enjoy!


