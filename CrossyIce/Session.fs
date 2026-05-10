namespace CrossyIce

open Raylib_cs

type Session(stageDefinition: StageDefinition) =
    let stageMap = StageMap(stageDefinition)
    let player = Player(stageMap.StartPoint)

    let isKeyPressed key : bool =
        Raylib.IsKeyPressed(key)

    let checkCollision (point: GridPoint) =
        let cellType = stageMap.CellAt point 
        match cellType with
        | SolidWall -> true
        | FragileWall -> true
        | _ -> false

    let rec getDestination (pos: GridPoint) (direction: Direction) = 
        let nextPos = 
            match direction with
                | Back -> { pos with Y = pos.Y - 1 }
                | Front -> { pos with Y = pos.Y + 1 }
                | Left -> { pos with X = pos.X - 1 }
                | Right -> { pos with X = pos.X + 1 }
        
        let nextCell = stageMap.CellAt nextPos

        if checkCollision nextPos then
            pos
        elif nextCell.Slides then
            getDestination nextPos direction
        else
            nextPos
            

    let updatePlayerMovement (frameTime: float32) =
        player.setVisualPosition frameTime
        
        if not player.isMoving then 
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

    member _.Update(frameTime: float32) =
        updatePlayerMovement(frameTime)
