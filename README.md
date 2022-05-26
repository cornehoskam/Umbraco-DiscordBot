# Umbraco Discord Bot
An experimental project I've been working on, combining the [Discord.NET](https://github.com/discord-net/Discord.Net) library with the [Umbraco CMS](https://github.com/umbraco/Umbraco-CMS).

The result (so far!) is an Umbraco project in which we can configure:
 - Which Discord Bots we want to hook into, 
 - Which Discord Servers we want said bots to be able to respond to,
 - Configure custom response Commands for each server, 
 - and all of this from within Umbraco 9!

As this is intended as an experimental & inspirational project, I would not recommend using this as a production-ready platform, but feel free to take inspiration from this!

### Getting Started - Solution

Just clone the site to your local machine, restore the nuget packages, build, and run the solution.

You will be greeted with an Umbraco Installation wizard. Choose whichever type you would like to use!

After the Umbraco installation, head over to Backoffice -> Settings -> uSync, and import all the Settings that are provided in the solution.

Now we're ready to configure our Discord Bot!

### Getting Started - Discord

Now that we have the Umbraco side of thing setup, we need to create a new Discord bot. For information on how to create a bot, and get the bot's access token, feel free to check out [the following blog post](https://cornehoskam.com/posts/creating-a-discord-bot-with-discord-net?utm_source=github&utm_medium=github&utm_campaign=discordbot).

Head back into the Backoffice, and on the root of our Content Tree, create a new Umbraco Discord Client item. Paste the Token in the Token Field, and Save & Publish.

After that, create a child to that Client of type Umbraco Discord Server. Copy the Discord Server ID of the server you wish the bot to respond in, into the Server ID field.

After that, create a child to that Server of type Command Collection. No need for any configuration here.

And lastly, we can start creating custom commands under the Command Collection! Create a new item under the Command Collection set, on which you can configure which command the bot should respond to, and what the response should be!

### Commands

All commands are currently set to respond to the command, prefixed by the '?' symbol. So if you were to add a command called "lorem", the way to call said command would be to enter "?lorem" into the Discord chat!

To reload the commands after adding/changing something in Umbraco, use the "?reload" command in Discord, which will repopulate the bot's command list!

### Example Screenshot
![](https://i.imgur.com/g17QGPc.png)
