# Camera
This doc outlines the details behind our custom camera logic.

## Usage
* Make sure the Cam node is updated after moving entities to ensure it correctly tracks the movement of said entities. (Simple way is to make it the last node in the scene).

## Gotchas
**Make sure** the camera is updated after moving objects, or those objects that can potentially move the player. This is because the cameras position is updated based on the player position and if the camera is updated prior to the player position being updated, it can result in terrible **jitter**.
