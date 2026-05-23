open Raylib_cs
open CrossyIce

module Program =
    let ScreenWidth = 1024
    let ScreenHeight = 768

    let gameSession = Session(StageInfo.stages)
    let renderer = Renderer(ScreenWidth, ScreenHeight)
    
    let setup () =
        Raylib.InitWindow (ScreenWidth, ScreenHeight, "CrossyIce - 20220557 Hyunseo Lee")
        Raylib.SetTargetFPS (60)

    let render () =
        Raylib.BeginDrawing ()
        Raylib.ClearBackground (Color(226uy, 235uy, 244uy, 255uy))
        renderer.Draw gameSession.GameState gameSession.StageMap gameSession.Player gameSession.Bombs gameSession.getRemainBombCount  
        Raylib.EndDrawing ()

    let cleanup () = Raylib.CloseWindow()

    let rec gameLoop () =
        if not (CBool.op_Implicit(Raylib.WindowShouldClose())) then
            let frameTime = Raylib.GetFrameTime ()
            gameSession.Update(frameTime)
            render ()
            gameLoop ()

    setup ()
    gameLoop ()
    cleanup ()
