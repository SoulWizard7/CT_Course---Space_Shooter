# CT_Course---Space_Shooter

Made as part of assignment for Computer Technology course at FutureGames, class of FG21GP-BOD.
Space Shooter project made in Unity 2021.3.13f1 and DOTS (Entities 0.51.1)

In this project I mainly tried to learn how to use Unitys DOTS functionality and syntax. Meanwhile trying to understand what Data Oriented Programming means.

I have code from a tutorial https://dots-tutorial.moetsi.com/ which I have used for a base and tried to expand and explore DOTS from thereon. Some of the code from the tutorial is still there, while I have created original coded, modified and added functionality as well.

In the project most things work from with entities, such as player controller, spawning, destroying. The worst part of the project is the UI. I had issues with connecting Unitys UI/Canvas/Monobehaviours and Entities for the build, so there is working UI for some player feedback but with a bad workaround.

Collision works with distance checks since all objects are spheres and I managed to optimize the algorithm to stay over 100fps (120-140 average) with 60000 asteroids and 20-30 bullets spawned. This part is what I focused on a lot for performance, to have large amount of moving objects with some collision checking. I did not get multithreading to work for the collision, although multithreding is used in many systems for optimization purposes.

I wanted to use pooling for bullets but it is not in the project, but I want to add that later to see if I can gain performance.

To test a working build of the project, unzip the ExecitableBuild_Windows.zip and run the .exe file.
