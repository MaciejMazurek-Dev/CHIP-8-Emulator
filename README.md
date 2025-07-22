# CHIP-8 Emulator
A simple and functional CHIP-8 emulator written in **C#**. 
It interprets and runs classic CHIP-8 ROMs with graphical and input support.


## What is CHIP-8?
CHIP-8 is a virtual machine designed in the 1970s to run simple games. 
It has a small instruction set (35 opcodes), a 64x32 monochrome display, and is a great target for emulator projects.

## Demo
![CHIP-8 Emulator Demo](Readme.gif)


## Features
- Full CHIP-8 instruction set support  
- Display rendering using **MonoGame**  
- ROM loading via command line  


## Technologies

- C# 13
- .NET 9.0
- [MonoGame 3.8.3](https://www.monogame.net/)


## Installation
```
git clone https://github.com/MaciejMazurek-Dev/CHIP-8-Emulator.git
cd CHIP-8-Emulator/src/Chip8_MonoGame
dotnet build
dotnet run -- "<full path to a CHIP-8 ROM file>"
```
Example:
```
dotnet run -- "C:\Downloads\Chip8Game.ch8"
```


## Controls
|CHIP-8 Keypad	|Keyboard|
|---------------|--------|
|0123           |ABCD    |
|4567           |EFGH    |
|89AB           |IJKL    |
|CDEF           |MNOP    |


### Future Improvements
- Debugging tools (step mode, opcode log, registers states)
- Custom keyboard input mapping
- Sound emulation
