namespace CrossyIce

open Raylib_cs

type Session(stageDefinition: StageDefinition) =
    let stageMap = StageMap(stageDefinition)
    let player = Player(stageMap.StartPoint)

    let isKeyPressed key : bool =
        Raylib.IsKeyPressed(key)

    let updatePlayerMovement () =
        let playerPos = player.getPosition

        if isKeyPressed KeyboardKey.W then
            player.setDirection Back
            player.setPosition {playerPos with Y = playerPos.Y - 1}
        elif isKeyPressed KeyboardKey.S then
            player.setDirection Front
            player.setPosition {playerPos with Y = playerPos.Y + 1}
        elif isKeyPressed KeyboardKey.A then
            player.setDirection Left
            player.setPosition {playerPos with X = playerPos.X - 1}
        elif isKeyPressed KeyboardKey.D then
            player.setDirection Right
            player.setPosition {playerPos with X = playerPos.X + 1}

    member _.StageMap = stageMap
    member _.Player = player

    member _.Update() =
        updatePlayerMovement ()
