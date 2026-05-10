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

    let updatePlayerMovement (frameTime: float32) =
        player.setVisualPosition frameTime
        
        if not player.isMoving then 
            let playerPos = player.getPosition

            let newPos = 
                if isKeyPressed KeyboardKey.W then
                    player.setDirection Back
                    Some {playerPos with Y = playerPos.Y - 1}
                elif isKeyPressed KeyboardKey.S then
                    player.setDirection Front
                    Some {playerPos with Y = playerPos.Y + 1}
                elif isKeyPressed KeyboardKey.A then
                    player.setDirection Left
                    Some {playerPos with X = playerPos.X - 1}
                elif isKeyPressed KeyboardKey.D then
                    player.setDirection Right
                    Some {playerPos with X = playerPos.X + 1}
                else
                    None

            match newPos with
            | Some pos -> 
                if checkCollision pos then
                    ()
                else 
                    player.setPosition pos
            | None -> ()

    member _.StageMap = stageMap
    member _.Player = player

    member _.Update(frameTime: float32) =
        updatePlayerMovement(frameTime)
