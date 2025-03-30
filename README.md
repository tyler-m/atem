atem [![Build](https://github.com/tyler-m/atem/actions/workflows/ci.yml/badge.svg)](https://github.com/tyler-m/atem/actions/workflows/ci.yml)
===

A cross-platform emulator for the Game Boy Color written in C# with .NET and MonoGame.

![demo](https://github.com/tyler-m/atem/assets/7759273/3ac33f7a-4caa-4ef3-81b6-41bc6b482adf)

Progress
---
- [X] Processor
  - [X] Fully implemented Game Boy CPU instruction set
  - [X] Passes Blargg's cpu_instrs test suite
- [X] Graphics
  - [X] Per-pixel rendering
  - [X] Passes [cgb-acid2](https://github.com/mattcurrie/cgb-acid2)
  - [X] Renders a number of "torture test" games like [_Prehistorik Man_](https://eldred.fr/blog/prehistorik/)
- [ ] Sound
  - [X] Synthesizes audio for Pulse, Wave and Noise channels
  - [X] Audio filters (e.g. high pass and low pass)
  - [ ] Handles "obscure behavior" as noted in the Pan Docs
- [X] Memory
  - [X] Loads MBC1, MBC3, MBC5 cartridges
  - [X] Boot ROM support
  - [X] Real-time Clock support for MBC3 cartridges
- [ ] Input
  - [ ] Serial communication
  - [ ] Control rebinding
  - [ ] Gamepad support
- [X] Save
  - [X] Save and load emulator states
  - [X] Battery saves
- [ ] Debug
  - [X] Step-through debugging
  - [X] Memory view
  - [X] CPU register view
  - [X] Address based breakpoints
  - [ ] Conditional breakpoints

Notes
---
This project is made possible by the extensive technical documentation of the Game Boy maintained publicly by retro hardware enthusiasts, most notably, the [Pan Docs](https://github.com/gbdev/pandocs).