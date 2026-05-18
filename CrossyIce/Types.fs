namespace CrossyIce
open Raylib_cs

type StageDefinition =
    { Name: string
      Layout: string list }

[<Struct>]
type Point<'T> = 
    { X: 'T
      Y: 'T }

type GridPoint = Point<int>
type VisualPoint = Point<float32>

type Direction =
    | Front
    | Back
    | Right
    | Left

type Bomb =
    { Position: GridPoint }
    
type CellStyle =
    { BaseColor: Color
      DrawDetail: int -> int -> int -> unit }

type CellKind =
    | Dry
    | Ice
    | SolidWall
    | FragileWall
    | Start
    | Goal

    static member FromSymbol =
        function
        | '#' -> Some SolidWall
        | '_' -> Some Dry
        | '~' -> Some Ice
        | 'S' -> Some Start
        | 'G' -> Some Goal
        | 'X' -> Some FragileWall
        | _ -> None

    member this.IsWalkable =
        match this with
        | Dry
        | Ice
        | Start
        | Goal -> true
        | SolidWall
        | FragileWall -> false

    member this.Slides =
        match this with
        | Ice -> true
        | _ -> false

    member this.IsBreakable =
        match this with
        | FragileWall -> true
        | _ -> false

