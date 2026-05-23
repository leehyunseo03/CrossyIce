# CrossyIce

A GUI Modified Ice Sliding Puzzle game built with **F# / .NET 10**.

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)  
  Verify with: `dotnet --version` (should show `10.x.x`)

### Run
```bash
# Windows
run.bat

# Unix / macOS
chmod +x run.sh
./run.sh

# Or directly
dotnet run
```

### Build
```bash
dotnet build
```

### Publish Self-Contained Binary

```bash
# Windows x64
dotnet publish -c Release -r win-x64 --self-contained

# Linux x64
dotnet publish -c Release -r linux-x64 --self-contained
```

---

## How to Play
---

## Project Structure
```
CrossyIce/
├── CrossyIce.fsproj    # .NET 10 F# project file
├── run.bat             # Windows run script
├── run.sh              # Unix run script
├── Proposal.pdf        # CrossyIce proposal file
├── README.md           # Project introduction file
└── CrossyIce/
    ├── GameObject.fs   # Base class for movable game objects 
    ├── Bomb.fs         # Bomb object with states
    ├── Player.fs       # Player object with states
    ├── Program.fs      # Entrypoint, Raylib setup, render loop
    ├── Renderer.fs     # Raylib drawing logic
    ├── Session.fs      # Main game logic with input handling, movement, bomb placement, collision, and stage progression
    ├── StageInfo.fs    # Stage definitions with map, bomb limitation
    ├── StageMap.fs     # Stage parsing, cell data
    └── Types.fs        # Shared types for points, directions, cells, movement results, and bomb states
```

### Key Types
```fsharp
// A stage is defined by its name, text-based layout, available bombcount
type StageDefinition =
    { Name: string
      Layout: string list 
      bombCount: int}

// Grid-based coordinates for map position
type Point<'T> = 
    { X: 'T
      Y: 'T }
// Integer coordinates for logical map position
type GridPoint = Point<int>

// Float-based coordinated for smooth visual movement
type VisualPoint = Point<float32>

// Player facing direction
type Direction =
    | Front
    | Back
    | Right
    | Left

// Movement result of bomb after sliding
type MoveResult =
    | Arrived of GridPoint
    | Blocked of GridPoint

// Bomb state : Idle, waiting for explode, or exploding
type BombState =
    | Normal
    | Pending
    | Boom of float32

// Cell Design type
type CellStyle =
    { BaseColor: Color
      DrawDetail: int -> int -> int -> unit }

// Map cell type used to build each stage
type CellKind =
    | Dry
    | Ice
    | SolidWall
    | FragileWall
    | Start
    | Goal
```

---
## Rules Summary


---
## Implementation Details
### LLM Usage
1. Making map grid design    
Used LLM : Gemini 3.1 Pro   
```
Using F# raylib_cs.py design the Board map with grid. each grid cell has type. ice(sky blue), land(white), fragile wall, wall, and start point and goal point. make start point as green dot and goal point as yellow dot. emphasize goal point. make it 2d
```

2. Character Design   
Used LLM : Gemini 3.1 Pro   
```
Using f# and Raylib, draw cute simple penguin in 2d that facing front, left, right, and backward. make simple as possible.
```

3. Bomb Design
Used LLM : Gemini 3.1 Pro
```
Using f# and Raylib, draw Big simple bomb in 2d. make it simple as possible. use cellsize, centerx, centery, radius as given parameter. make as simple as possible. use less than 5 lines. do not use external library
```