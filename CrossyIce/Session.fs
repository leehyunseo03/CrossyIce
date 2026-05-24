namespace CrossyIce

open Raylib_cs

type Session(stageDefinitionlist: StageDefinition list) =
    let mutable gameState = Playing
    let stageClearTime = 2.0f

    let mutable stageIndex = 0
    let mutable stageMap = StageMap(stageDefinitionlist[stageIndex])
    
    let mutable bombs: Bomb list = []
    
    let mutable remainBombCount = stageMap.getBombCount

    let player = Player(stageMap.StartPoint)
    
    let isBombAt point =
        bombs
        |> List.exists ( fun bomb ->
            match bomb.getState with
            | Boom _ -> false
            | Normal
            | Pending -> bomb.getPosition = point
        )

    let isKeyPressed key : bool =
        Raylib.IsKeyPressed(key)
    
    let resetStage () = 
        stageMap <- StageMap(stageDefinitionlist[stageIndex])
        bombs <- []
        remainBombCount <- stageMap.getBombCount
        player.resetPosition stageMap.StartPoint
        player.setDirection Front
        gameState <- Playing

    let stageClear () = 
        if stageIndex + 1 < stageDefinitionlist.Length then
            stageIndex <- stageIndex + 1
            resetStage ()
        else 
            gameState <- GameClear

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
        | Pending when not bomb.isMoving -> true
        | _ -> false

    let explodeBomb (bomb: Bomb) = 
        let explodeRange = bomb.explode()
        explodeRange |> List.iter (fun point -> stageMap.BreakFragileWall point)
        if explodeRange |> List.exists (fun point -> player.getPosition = point) then
            resetStage ()
        else
            bomb.explodeState ()

    let updateBombExplosions frameTime =
        bombs |> List.iter (fun bomb -> bomb.updateExplosion frameTime)

        bombs <-
            bombs
            |> List.filter (fun bomb -> not bomb.isExplosionFinished)
            
    let explodePendingBombs () =
        bombs
        |> List.filter isBombExplode
        |> List.iter explodeBomb

    let canPlaceBombAt point =
        stageMap.IsInside point
        && not (isBombAt point)
        && match stageMap.CellAt point with
           | Dry -> true
           | Ice -> true
           | Start -> false
           | Goal -> false
           | SolidWall -> false
           | FragileWall -> false

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
                bomb.pendingState ()
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
        updateBombExplosions frameTime
        if not player.isMoving && not (isAnyBombMoving ()) then 
            if checkStageClear player.getPosition then
                gameState <- StageClear stageClearTime
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

    member _.GameState = gameState
    member _.StageIndex = stageIndex
    member _.StageTotalCount = stageDefinitionlist.Length
    member _.StageMap = stageMap
    member _.Player = player
    member _.Bombs = bombs

    member _.getRemainBombCount = remainBombCount

    member _.Update(frameTime: float32) =
        if isKeyPressed KeyboardKey.R then
            resetStage ()
        else
            match gameState with
            | Playing -> updatePlayerMovement(frameTime)
            | StageClear remainingTime ->
                updateObjectVisualPositions frameTime
                updateBombExplosions frameTime

                let newRemainingTime = remainingTime - frameTime
                if newRemainingTime <= 0.0f then
                    stageClear ()
                else
                    gameState <- StageClear newRemainingTime
            | GameClear -> ()
