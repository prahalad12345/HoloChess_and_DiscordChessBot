import discord 
from discord.ext import commands,tasks
import random
from itertools import cycle
import os 
import asyncio
import json
import chess


client = commands.Bot(command_prefix="*/",intents=discord.Intents.all())


#client event this is were we add the bot stuff
@client.event 

async def on_ready():
    #tree sync is for the slash commands
    await client.tree.sync()
    print("Success:Bot is connected to Discord")

@client.hybrid_command(name='description',description='describe the bot')
async def description(ctx):
    info_embed=discord.Embed(title=f"ChessBot's information",description='information of the user',color=discord.Color.random())
    info_embed.add_field(name='Name:',value='ChessBot',inline=False)
    info_embed.add_field(name='Description:',value='Moderates only a single chess match . Warning: on accessing the playchess command again will start a new chess game',inline=False)
    info_embed.add_field(name='commands:(*/playchess <playername>):',value='initiates a game with the author of command and playername',inline=False)
    info_embed.add_field(name='commands:(*/w <move>):',value='make the <move> by white',inline=False)
    info_embed.add_field(name='commands:(*/b <move>):',value='make the <move> by black',inline=False)
    info_embed.add_field(name='Status:',value='online',inline=False)
    info_embed.add_field(name='Bot user:',value='yes',inline=False)
    await ctx.send(embed=info_embed)

async def load():
    for filename in os.listdir('./cogs'):
        if filename.endswith('.py'):
            await client.load_extension(f'cogs.{filename[:-3]}')
            print(f'{filename[:-3]} is loaded')
#missing argument error making it in general . check moderation.py for seperate error for seperate commands
@client.event 
async def on_command_error(ctx,error):
    if isinstance(error,commands.MissingRequiredArgument):
        await ctx.send('General:Error missing required argument!')


async def main():
    async with client:
        await load() 
        await client.start('MTExMzAwNzI3MjI4MTUyNjI5Mg.GZIKZi.hEVLBoiPdlIinTY8yNl3WLsgY6Xy1M4K0NhwpU')

asyncio.run(main())