----------------------------------------
SampleVariation-Alien-Level1.1-RobustJumpOnHead
----------------------------------------

The standard alien sample includes a jump on head mecanic that relies on Unity physics collissions. However sometimes the Unity physics 
system will miss a collission particualrly on low end devices. This alternative sample uses the standard raycast mecanism to calculate 
head collissions and thus guarantees you never 'miss'.

Its a little more complex to set up and slightly ymore performance intensive which is why its not the standard.

----------------------------------------
SampleVariation-Alien-Level1.1-WithHealth
----------------------------------------

Adds health and collectable health items to the alien sample... nuff said.

------------------------------------------------
SampleVariation-Alien-Level1.1-PersistItemState
SampleVariation-Alien-Level1.3-PersistItemState
------------------------------------------------

This sample saves item state, door state and enemy state using PersistableObjects.

Once you collect an item it wont reset until GameOver (even if you load other scenes). 

Note you must die or exit the level to save, it doesn't save if you use the editors stop control.

------------------------------------------------
SampleVariation-Alien-Menu-WithCharacterSelect
SampleVariation-Alien-Level1.1-WithCharacterSelect
------------------------------------------------

These saples use the menu system and Character Loader to allow user to pick between different characters.