namespace CrossyIce

open Raylib_cs

type Session(stageDefinition: StageDefinition) =
    let stageMap = StageMap(stageDefinition)
    let player = Player(stageMap.StartPoint)

    let isKeyPressed key : bool =
        Raylib.IsKeyPressed(key)

    let updatePlayerDirection () =
        if isKeyPressed KeyboardKey.W then
            player.setDirection Back
        elif isKeyPressed KeyboardKey.S then
            player.setDirection Front
        elif isKeyPressed KeyboardKey.A then
            player.setDirection Left
        elif isKeyPressed KeyboardKey.D then
            player.setDirection Right

    member _.StageMap = stageMap
    member _.Player = player

    member _.Update() =
        updatePlayerDirection ()
