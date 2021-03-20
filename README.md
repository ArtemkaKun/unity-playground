# unity-playground
My playground project in Unity to create and test features, that are too small to be separated in specified project. Feel free to use any assets/code from it.

## Content

### ChanceRange - edtior tool to control the probability of getting object in term of ranges.

[Presentation video](https://youtu.be/eu4oFNvJjLs)

This component is used generic types, so you can use it not only with GameObjects. Just inherit the RangeChancesController<T> and define the type instead of T.
![Code sample 0](https://user-images.githubusercontent.com/36485221/111867108-444b6180-8972-11eb-89f4-b9b6673ace1a.png)

Also, make sure you created the separate class for UI presentation of your inherited component in the Editor folder.
![Code sample 1](https://user-images.githubusercontent.com/36485221/111867151-7f4d9500-8972-11eb-92d3-edc138ba4ec1.png)

I created the TestObjectSpawner as an example, you can find it on the scene.

You can control ranges with:

* Slider in the Editor
* Changing the RawChance value in the Editor
* From code (ATTENTION! In code you can change object's chance range only with the function SetUnitRange(Vector2 newRange)).
