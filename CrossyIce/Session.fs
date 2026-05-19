namespace CrossyIce

open Raylib_cs

type Session(stageDefinitionlist: StageDefinition list) =
    let mutable stageIndex = 0
    let mutable stageMap = StageMap(stageDefinitionlist[stageIndex])
    
    let mutable bombs: Bomb list = []
    
    let mutable remainBombCount = stageMap.getBombCount

    let player = Player(stageMap.StartPoint)
    
    let isBombAt point =
        bombs |> List.exists (fun bomb -> bomb.getPosition = point)

    let isKeyPressed key : bool =
        Raylib.IsKeyPressed(key)
    
    let stageClear () = 
        stageIndex <- stageIndex + 1
        stageMap <- StageMap(stageDefinitionlist[stageIndex])
        bombs <- []
        remainBombCount <- stageMap.getBombCount
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
        | _ when isBombAt point -> true
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
            Blocked pos
        elif nextCell.Slides then
            getDestination nextPos direction
        else
            Arrived nextPos
    
    let isBombExplode (bomb: Bomb) =
        match bomb.getState with
        | Boom when not bomb.isMoving -> true
        | _ -> false

    let explodeBomb (bomb: Bomb) = 
        let explodeRange = bomb.explode()
        List.iter stageMap.BreakFragileWall explodeRange
        bombs <- bombs |> List.filter (fun b -> b <> bomb)

    let explodePendingBombs () =
        bombs
        |> List.filter isBombExplode
        |> List.iter explodeBomb

    let canPlaceBombAt point =
        stageMap.IsInside point
        && not (checkCollision point)

    let tryGetBombAt point =
        bombs |> List.tryFind (fun bomb -> bomb.getPosition = point)

    let tryPushBomb (bomb:Bomb) direction =
        let finalBombPos = getDestination bomb.getPosition direction

        match finalBombPos with
        | Arrived finalBombPos ->
            if finalBombPos = bomb.getPosition then
                false
            else
                bomb.setPosition finalBombPos
                true
        | Blocked finalBombPos ->
            if finalBombPos = bomb.getPosition then
                false
            else
                bomb.setPosition finalBombPos
                bomb.explodeState ()
                true
    
    
        
    let placeBomb () =
        let target = frontPoint player.getPosition player.getDirection

        if canPlaceBombAt target && remainBombCount > 0 then
            bombs <- Bomb(target) :: bombs
            remainBombCount <- remainBombCount - 1
    
    let isAnyBombMoving () =
        bombs |> List.exists (fun bomb -> bomb.isMoving)

    let updateObjectVisualPositions frameTime =
        player.setVisualPosition frameTime
        bombs |> List.iter (fun bomb -> bomb.setVisualPosition frameTime)
        
    let updatePlayerMovement (frameTime: float32) =
        updateObjectVisualPositions frameTime
        explodePendingBombs ()

        if not player.isMoving && not (isAnyBombMoving ()) then 
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
                    let nextPos = frontPoint playerPos dir

                    match tryGetBombAt nextPos with
                    | Some bomb -> 
                        if tryPushBomb bomb dir then
                            player.setPosition nextPos
                    | None -> 
                        let finalPos = getDestination playerPos dir
                        match finalPos with
                        | Arrived finalPos
                        | Blocked finalPos -> 
                            if finalPos <> playerPos then
                                player.setPosition finalPos

                | None -> ()

    member _.StageMap = stageMap
    member _.Player = player
    member _.Bombs = bombs

    member _.getRemainBombCount = remainBombCount

    member _.Update(frameTime: float32) =
        updatePlayerMovement(frameTime)
