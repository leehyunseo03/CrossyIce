namespace CrossyIce

type StageMap(definition: StageDefinition) =
    let rows = definition.Layout |> List.toArray

    let width = rows[0].Length
    let height = rows.Length

    let parseCell x y symbol =
        match CellKind.FromSymbol symbol with
        | Some kind -> kind
        | None -> failwith "Incorrect Symbol"

    let cells =
        Array2D.init width rows.Length (fun x y ->
            let symbol = rows[y][x]
            parseCell x y symbol
        )
        
    member _.Name = definition.Name
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
