# TwitchIntegration

Experimental framework for integrating Twitch in the game.

## How to install

- Move the contents of the `dependencies` folder inside the RoR2 `Managed` folder (note to devs: let me know if there's a better spot for these dlls)
- Move the `TwitchIntegration` folder inside the BepInEx `plugins` folder
- Done!

## How to use

Open the game chat and type `/twitch_connect [channel]`. You can use `tconnect` as alias for the command.

If you don't specify the channel it will connect to the same channel associated to the account you will connect with. Otherwise authorized account and channel will be different (imagine you want to authorize a custom bot while interacting with your own chat channel).

After a few seconds a new tab in your default browser will open asking for authorization (you might need to log in to your Twitch account first if you're not already logged in).

If you authorize the application, the game will automatically get the token and connect the game to Twitch. The Twitch chat and the in game chat are now connected.

## Dev

Classes are in a rough state at the moment, but I hope you can figure out what you can do with this mod. As an example, I made a plugin that extends this mod (https://thunderstore.io/package/OrangeNote/TwitchVotesItems/) and integrates the chat interaction with in game elements. There's plenty of possibilites and I hope new or existing mods integrate with Twitch in the future!
