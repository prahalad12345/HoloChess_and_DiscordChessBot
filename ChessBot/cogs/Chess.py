import discord 
from discord.ext import commands
import chess
import sys
from fentoboardimage import fenToImage, loadPiecesFolder
from PIL import Image


player=0
flag=0
id1=0 
id2=0
white=None 
black=None
board=None

class Chess(commands.Cog):
    def __init__(self,client):
        self.client=client 
    
    @commands.Cog.listener() 
    async def on_ready(self):
        global flag 
        flag=0
        print('Chess is online')
    
    @commands.command() 
    async def playchess(self,ctx,member:discord.Member=None):
        if (member is None or ctx.author.id == member.id) or str(ctx.author.status)!='online' or str(member.status)!='online':
            await ctx.send("require another member or opponent as a parameter or member must be online")
        else: 
            global id1,id2,flag,board,white,black
            board = chess.Board()
            id1=ctx.author.id 
            id2=member.id 
            white=ctx.author.name
            black=member.name
            embed_message=discord.Embed(title=f'{white} vs {black}',description='Chess Match',color=discord.Color.random())

            embed_message.set_author(name=f"Requested by {white}",icon_url=ctx.author.avatar)
            
            boardImage = fenToImage(
                fen=board.fen(),
                squarelength=50,
                pieceSet=loadPiecesFolder("./pieces"),
                darkColor="#D18B47",
                lightColor="#FFCE9E"
            )
            boardImage.save('chess.png')
            f = discord.File('chess.png', filename="image.png")
            embed_message.set_image(url="attachment://image.png")
            embed_message.add_field(name='White to play',value=white,inline=False)

            await ctx.send(file=f,embed=embed_message)
            flag=1
    
    @commands.command()
    async def w(self,ctx,move:str):
        
        global id1,id2,flag,board,white,black,player
        if(flag==0):
            await ctx.send("start A game to access the command")
            return 
        
        if(player==0 and ctx.author.id==id1):
            try:
                board.push_san(move)
     
                #if you want the color of the author use color=ctx.author.color
                
                embed_message=discord.Embed(title=f'{white} vs {black}',description='Chess Match',color=discord.Color.random())
                
                boardImage = fenToImage(
                    fen=board.fen(),
                    squarelength=50,
                    pieceSet=loadPiecesFolder("./pieces"),
                    darkColor="#D18B47",
                    lightColor="#FFCE9E"
                )
                
                boardImage.save('chess.png')
                f = discord.File('chess.png', filename="image.png")
                embed_message.set_image(url="attachment://image.png")
                embed_message.add_field(name='Black to play',value=black,inline=False)
                await ctx.send(file=f,embed=embed_message)
                player=1
                if(board.is_checkmate()):
                    embed_message=discord.Embed(title=f'{white} vs {black}',description='Chess Match',color=discord.Color.random())
                    embed_message.add_field(name='White has won!',value='1-0',inline=False)
                    await ctx.send(embed=embed_message)
                    player=0 
                    flag=0
                    
                if(board.is_stalemate() or board.can_claim_threefold_repetition() or board.can_claim_fifty_moves()):
                    embed_message=discord.Embed(title=f'{white} vs {black}',description='Chess Match',color=discord.Color.random())
                    embed_message.add_field(name='Draw!',value='0.5-0.5',inline=False)
                    await ctx.send(embed=embed_message)
                    player=0 
                    flag=0
                    
            except: 
                await ctx.send('Play a legal move')  
        else: 
            await ctx.send('Black to Play and only black oppponent types')      
      
    @commands.command()
    async def b(self,ctx,move:str):
        global id1,id2,flag,board,white,black,player
        if(flag==0):
            await ctx.send("start A game to access the command")
            return 
        
        if(player==1 and ctx.author.id==id2):
            try:
                board.push_san(move)
     
                #if you want the color of the author use color=ctx.author.color
                
                embed_message=discord.Embed(title=f'{white} vs {black}',description='Chess Match',color=discord.Color.random())

                boardImage = fenToImage(
                    fen=board.fen(),
                    squarelength=50,
                    pieceSet=loadPiecesFolder("./pieces"),
                    darkColor="#D18B47",
                    lightColor="#FFCE9E"
                )
               
                boardImage.save('chess.png')
                f = discord.File('chess.png', filename="image.png")
                embed_message.set_image(url="attachment://image.png")
                embed_message.add_field(name='White to play',value=black,inline=False)
                await ctx.send(file=f,embed=embed_message)
                player=0
                
                if(board.is_checkmate()):
                    embed_message=discord.Embed(title=f'{white} vs {black}',description='Chess Match',color=discord.Color.random())
                    embed_message.add_field(name='Black has won!',value='0-1',inline=False)
                    await ctx.send(embed=embed_message)
                    player=0 
                    flag=0
                    
                if(board.is_stalemate() or board.can_claim_threefold_repetition() or board.can_claim_fifty_moves()):
                    embed_message=discord.Embed(title=f'{white} vs {black}',description='Chess Match',color=discord.Color.random())
                    embed_message.add_field(name='Draw!',value='0.5-0.5',inline=False)
                    await ctx.send(embed=embed_message)
                    player=0 
                    flag=0
            except: 
                await ctx.send('Play a legal move')  
        else: 
            await ctx.send('White to Play and only white opponent types')
                
async def setup(client):
    await client.add_cog(Chess(client))