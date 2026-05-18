namespace CrossyIce

open Raylib_cs

type Session(stageDefinitionlist: StageDefinition list) =
    let mutable stageIndex = 0
    let mutable stageMap = StageMap(stageDefinitionlist[stageIndex])
    let player = Player(stageMap.StartPoint)

    let mutable bombs: Bomb list = []

    let isKeyPressed key : bool =
        Raylib.IsKeyPressed(key)
    
    let stageClear () = 
        stageIndex <- stageIndex + 1
        stageMap <- StageMap(stageDefinitionlist[stageIndex])
        bombs <- []
        player.resetPosition stageMap.StartPoint

    let checkStageClear (point: GridPoint) = 
        let cellType = stageMap.CellAt point 
        match cellType with
        | Goal-> true
        | _ -> false

    let checkCollision (point: GridPoint) =
        let cellType = stageMap.CellAt point 
        match cellType with
        | SolidWall -> true
        | FragileWall -> true
        | _ -> false

    let frontPoint (pos: GridPoint) (direction: Direction) =
        match direction with
        | Back -> { pos with Y = pos.Y - 1 }
        | Front -> { pos with Y = pos.Y + 1 }
        | Left -> { pos with X = pos.X - 1 }
        | Right -> { pos with X = pos.X + 1 }
        
    let rec getDestination (pos: GridPoint) (direction: Direction) = 
        let nextPos = frontPoint pos direction
        let nextCell = stageMap.CellAt nextPos

        if checkCollision nextPos then
            pos
        elif nextCell.Slides then
            getDestination nextPos direction
        else
            nextPos
    
    let isBombAt point =
        bombs |> List.exists (fun bomb -> bomb.Position = point)
    
    let canPlaceBombAt point =
        stageMap.IsInside point
        && not (checkCollision point)
        && not (isBombAt point)

    let placeBomb () =
        let target = frontPoint player.getPosition player.getDirection

        if canPlaceBombAt target then
            bombs <- { Position = target } :: bombs
            
    let updatePlayerMovement (frameTime: float32) =
        player.setVisualPosition frameTime
        
        if not player.isMoving then 
            if checkStageClear player.getPosition then
              stageClear ()
            elif isKeyPressed KeyboardKey.Space then
                placeBomb ()
            else
                let playerPos = player.getPosition
                
                let newDir = 
                    if isKeyPressed KeyboardKey.W then Some Back
                    elif isKeyPressed KeyboardKey.S then Some Front
                    elif isKeyPressed KeyboardKey.A then Some Left
                    elif isKeyPressed KeyboardKey.D then Some Right
                    else None

                match newDir with
                | Some dir -> 
                    player.setDirection dir
                    let finalPos = getDestination playerPos dir

                    if finalPos <> playerPos then 
                        player.setPosition finalPos
                | None -> ()

    member _.StageMap = stageMap
    member _.Player = player
    member _.Bombs = bombs

    member _.Update(frameTime: float32) =
        updatePlayerMovement(frameTime)
