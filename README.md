atem [![Build](https://github.com/tyler-m/atem/actions/workflows/ci.yml/badge.svg)](https://github.com/tyler-m/atem/actions/workflows/ci.yml)
===

A cross-platform emulator for the Game Boy Color written in C#

<a href="images/1.gif"><img src="images/1.gif" width="200"></a> 
<a href="images/2.png"><img src="images/2.png" width="200"></a>

<a href="images/3.png"><img src="images/3.png" width="418"></a>

Status
---
- **Processor**
  - [X] Fully implemented Game Boy CPU instruction set
  - [X] Passes Blargg's [cpu_instrs](https://github.com/libretro/testroms/blob/master/blargg-cpu-instrs/) test suite
  
- **Graphics**
  - [X] Per-pixel rendering
  - [X] Passes [cgb-acid2](https://github.com/mattcurrie/cgb-acid2)
  
- **Sound**
  - [X] Synthesizes audio for Pulse, Wave, and Noise channels
  - [X] Audio filters (high-pass, low-pass)
  - [ ] Doesn't fully handle "obscure behavior" yet (see [Pan Docs](https://gbdev.io/pandocs/Audio_details.html#obscure-behavior))

- **Memory**
  - [X] Loads MBC1, MBC3, MBC5 cartridges
  - [X] Boot ROM support
  - [X] Real-time Clock support for MBC3 cartridges
  
- **Input**
  - [X] Keyboard control rebinding
  - [ ] Gamepad support
  - [ ] Serial communication (simulated link cable)
  
- **Saving**
  - [X] Save and load emulator states
  - [X] Battery saves (persistent game saves)
  
- **Debug**
  - [X] Step-through debugging
  - [X] Memory view
  - [X] CPU register view
  - [X] Address-based breakpoints

Developing
---

- SDK for .NET 8.0+ must be installed

Grab the repo
```
git clone https://github.com/tyler-m/atem.git
cd atem
```

Restore dependencies

`dotnet restore`

Build

`dotnet build`

Test

`dotnet test`

Notes
---
This project is made possible by the extensive technical documentation of the Game Boy maintained publicly by retro hardware enthusiasts, most notably, the [Pan Docs](https://github.com/gbdev/pandocs).