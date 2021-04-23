Welcome to the Quadtree Target System, if you've got lots of entities that need to search distance checks over the entire scene, 
and you're currently running through an entire list, this will greatly improve the performance of your game. 

Everything you need is in the four scripts found in the scripts folder,
one of which (QTTSQuadTreeNode) you do not need to touch.

----------------------------------------------------------------------------------------------------------------------------------------------------------

First, in the QTTSTarget script, you can find the TargetTypes enum at the bottom. You can add or remove any target types you need
for your game without breaking anything.
(Note: Some scripts in the Example folder will give you errors if you remove the current types, but you can freely delete the folder)

for QTTSQuadtree: 
1) Place it in the scene
2) Use the editor window to set the center and radius so that it 
covers the entire area you need, Its okay if it extends further out than what you need.

As for the max targets, you can edit it to fit your needs but keeping it as is will generally suit you better.

for QTTSTarget(Yep, its a target):
1)Set it on a prefab or in scene and pick what target type you want it to be in the editor, or do it at runtime with the GetTargetType method
2)If it starts in the scene, set QTTSQuadtree in the editor, otherwise inject the data with the SetQuadTree method
3)If the position of the target changes, update it with the UpdatePosition method

for QTTSTargetFinder(It finds ^that):
1)Set it on a prefab or in scene and pick the target types you need it to find in the editor, or do it at runtime with the SetNewTypesToTarget method
2)If it starts in the scene, set QTTSQuadtree in the editor, otherwise inject the data with the SetQuadTree method
3)If you want a maximum search radius, set the _searchRadius variable in the editor or with the ChangeSearchRadius method
3)Call the SetTargetClosestToCurrentPosition or SetTargetClosestToPoint methods to automatically find an applicable target, if any
4)Access the target with your code using the CurrentTarget parameter, or clear the target with the ClearTarget method

----------------------------------------------------------------------------------------------------------------------------------------------------------
And thats about it, everything else will

As said before, everything in the example folder can be deleted. 
But you can feel free to go into the profiler with it to test the results yourself, or mess with the numbers using it.
Or you can check the images in the profiler images folder to see what my results were

