namespace CrossyIce

type StageMap(definition: StageDefinition) =
    let rows = definition.Layout |> List.toArray

    let width = rows[0].Length
    let height = rows.Length
    
    let mutable startPoint: GridPoint = { X = 0; Y = 0 }
    let mutable goalPoint: GridPoint = { X = 0; Y = 0 }

    let parseCell x y symbol =
        match CellKind.FromSymbol symbol with
        | Some Start -> 
            startPoint <- { X = x; Y = y }
            Start
        | Some Goal -> 
            goalPoint <- { X = x; Y = y }
            Goal
        | Some kind -> kind
        | None -> failwith "Incorrect Symbol"

    let cells =
        Array2D.init width rows.Length (fun x y ->
            let symbol = rows[y][x]
            parseCell x y symbol
        )
        
    member _.Name = definition.Name
    member _.getBombCount = definition.bombCount
    member _.StartPoint = startPoint
    member _.GoalPoint = goalPoint

    member _.Width = width
    member _.Height = height
    member _.IsInside(point: GridPoint) =
        point.X >= 0
        && point.Y >= 0
        && point.X < width
        && point.Y < height
    
    member this.CellAt(point: GridPoint) =
        if this.IsInside(point) then
            cells[point.X, point.Y]
        else
            SolidWall

    member this.BreakFragileWall(point: GridPoint) =
        if this.IsInside(point) && cells[point.X, point.Y].IsBreakable then
            cells[point.X, point.Y] <- Ice