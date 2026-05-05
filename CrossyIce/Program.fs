open Raylib_cs
open CrossyIce

module Program =
    let ScreenWidth = 1024
    let ScreenHeight = 768

    let stageMap = StageMap(StageInfo.stage)
    let renderer = Renderer(ScreenWidth, ScreenHeight)

    let setup () =
        Raylib.InitWindow (ScreenWidth, ScreenHeight, "CrossyIce - 20220557 Hyunseo Lee")
        Raylib.SetTargetFPS (60)

    let render () =
        Raylib.BeginDrawing ()
        Raylib.ClearBackground (Color(226uy, 235uy, 244uy, 255uy))
        renderer.DrawMap (stageMap)
        Raylib.EndDrawing ()

    let cleanup () = Raylib.CloseWindow()

    let rec gameLoop () =
        if not (CBool.op_Implicit(Raylib.WindowShouldClose())) then
            render ()
            gameLoop ()

    setup ()
    gameLoop ()
    cleanup ()
