# BRC-PhotoStorage
BepInEx plugin that increases the phone camera storage in Bomb Rush Cyberfunk.
![Screen](https://user-images.githubusercontent.com/42678262/278137634-31b175dc-3439-4c11-b33d-b3a434683aaa.png)


# Configuration
A documented BepInEx configuration file will be created the first time you run the mod, called **"com.LazyDuchess.BRC.PhotoStorage.cfg"**. You can use r2modman to more easily edit it. Here you can edit the amount of photos you can save, set to 1000 by default.

# Building from source
You will need to provide a publicized version of the **"Assembly-CSharp.dll"** file which can be found in your "Bomb Rush Cyberfunk_Data/Managed" folder. To publicize it, you can use the [BepInEx.AssemblyPublicizer](https://github.com/BepInEx/BepInEx.AssemblyPublicizer) tool, and paste the result into **"lib/Assembly-CSharp-publicized.dll"** in this project's root directory.

You will also need to provide the **Unity.TextMeshPro.dll** assembly in the **"/lib"** folder, which can also be found in "Bomb Rush Cyberfunk_Data/Managed".
