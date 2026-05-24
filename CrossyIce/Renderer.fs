namespace CrossyIce

open Raylib_cs

type Renderer(windowWidth: int, windowHeight: int) =
    let boardPadding = 36

    let noDetail _ _ _ = ()
    
    let drawStart x y cellSize =
        let startColor = Color(127uy, 214uy, 161uy, 255uy)
        Raylib.DrawCircle(x + cellSize / 2, y + cellSize / 2, float32 cellSize * 0.12f, startColor)

    let drawGoal x y cellSize =
        let goalColor = Color(255uy, 206uy, 92uy, 255uy)
        let centerX = x + cellSize / 2
        let centerY = y + cellSize / 2
        Raylib.DrawCircleLines(centerX, centerY, float32 cellSize * 0.26f, goalColor)
        Raylib.DrawCircle(centerX, centerY, float32 cellSize * 0.10f, goalColor)

    let drawSolidWall x y cellSize =
        let boxColor = Color(255uy, 255uy, 255uy, 255uy)
        let margin = cellSize / 8
        let innerSize = cellSize - (margin * 2)
        Raylib.DrawRectangleLines(x + margin, y + margin, innerSize, innerSize, boxColor)

    let drawFragileWall x y cellSize =
        let centerX = x + cellSize / 2
        let centerY = y + cellSize / 2
        let lineColor = Color(102uy, 72uy, 52uy, 255uy)
        Raylib.DrawLine(centerX - cellSize / 3, centerY - cellSize / 4, centerX + cellSize / 4, centerY + cellSize / 5, lineColor)
        Raylib.DrawLine(centerX + cellSize / 8, centerY - cellSize / 3, centerX - cellSize / 5, centerY + cellSize / 3, lineColor)
        Raylib.DrawLine(centerX + cellSize / 10, centerY - cellSize / 10, centerX + cellSize / 3, centerY - cellSize / 2, lineColor)
        Raylib.DrawLine(centerX - cellSize / 4, centerY + cellSize / 6, centerX - cellSize / 2, centerY + cellSize / 3, lineColor)

    let cellStyle (cell: CellKind) =
        match cell with
        | Dry -> { BaseColor = Color(248uy, 248uy, 248uy, 255uy); DrawDetail = noDetail }
        | Ice -> { BaseColor = Color(214uy, 239uy, 255uy, 255uy); DrawDetail = noDetail }
        | Start -> { BaseColor = Color(248uy, 248uy, 248uy, 255uy); DrawDetail = drawStart }
        | Goal -> { BaseColor = Color(248uy, 248uy, 248uy, 255uy); DrawDetail = drawGoal }
        | SolidWall -> { BaseColor = Color(82uy, 89uy, 102uy, 255uy); DrawDetail = drawSolidWall }
        | FragileWall -> { BaseColor = Color(177uy, 134uy, 94uy, 255uy); DrawDetail = drawFragileWall }

    let computeCellSize (stageMap: StageMap) =
        let usableWidth = windowWidth - (boardPadding * 2)
        let usableHeight = windowHeight - (boardPadding * 2)
        let widthSize = usableWidth / stageMap.Width
        let heightSize = usableHeight / stageMap.Height
        max 32 (min 72 (min widthSize heightSize))
    
    let computeOrigin (stageMap: StageMap, cellSize: int) =
        let boardWidth = stageMap.Width * cellSize
        let boardHeight = stageMap.Height * cellSize
        let originX = (windowWidth - boardWidth) / 2
        let originY = (windowHeight - boardHeight) / 2
        originX, originY

    let drawGrid (x: int) (y: int) (cellSize: int) = 
        let gridColor = Color(214uy, 220uy, 229uy, 255uy)
        Raylib.DrawRectangleLinesEx(
            Rectangle(float32 x, float32 y, float32 cellSize, float32 cellSize),
            1.0f,
            gridColor
        )

    let drawCell (kind: CellKind) (x: int) (y: int) (cellSize: int) =
        let style = cellStyle kind
        Raylib.DrawRectangle(x, y, cellSize, cellSize, style.BaseColor)
        style.DrawDetail x y cellSize

    let drawMap (stageMap: StageMap) =
        let cellSize = computeCellSize stageMap
        let originX, originY = computeOrigin (stageMap, cellSize)

        for y in 0 .. stageMap.Height - 1 do
            for x in 0 .. stageMap.Width - 1 do
                let point = { X = x; Y = y }
                let kind = stageMap.CellAt(point)
                let px = originX + (x * cellSize)
                let py = originY + (y * cellSize)

                drawCell kind px py cellSize
                drawGrid px py cellSize

        cellSize, originX, originY

    let drawPenguin (x: int) (y: int) (dir: Direction) (cellSize: int) =
        // LLM used
        let bodyColor = Color.Black
        let bellyColor = Color.White
        let beakColor = Color.Orange
        let wingColor = Color.DarkGray
        let scale = float32 cellSize / 112.0f
        let offset value = int (float32 value * scale)
        let radius value = float32 value * scale
        
        match dir with
        | Front ->
            Raylib.DrawEllipse(x - offset 22, y + offset 10, radius 10, radius 25, bodyColor)
            Raylib.DrawEllipse(x + offset 22, y + offset 10, radius 10, radius 25, bodyColor)
            Raylib.DrawEllipse(x - offset 12, y + offset 42, radius 12, radius 8, beakColor)
            Raylib.DrawEllipse(x + offset 12, y + offset 42, radius 12, radius 8, beakColor)
            Raylib.DrawEllipse(x, y, radius 28, radius 45, bodyColor)
            Raylib.DrawEllipse(x, y + offset 12, radius 20, radius 30, bellyColor)
            Raylib.DrawCircle(x - offset 10, y - offset 15, radius 6, Color.White)
            Raylib.DrawCircle(x + offset 10, y - offset 15, radius 6, Color.White)
            Raylib.DrawCircle(x - offset 10, y - offset 15, radius 3, Color.Black)
            Raylib.DrawCircle(x + offset 10, y - offset 15, radius 3, Color.Black)
            Raylib.DrawEllipse(x, y - offset 5, radius 8, radius 5, beakColor)

        | Back ->
            Raylib.DrawEllipse(x - offset 22, y + offset 10, radius 10, radius 25, bodyColor)
            Raylib.DrawEllipse(x + offset 22, y + offset 10, radius 10, radius 25, bodyColor)
            Raylib.DrawEllipse(x - offset 12, y + offset 42, radius 12, radius 8, beakColor)
            Raylib.DrawEllipse(x + offset 12, y + offset 42, radius 12, radius 8, beakColor)
            Raylib.DrawEllipse(x, y, radius 28, radius 45, bodyColor)

        | Right ->
            Raylib.DrawEllipse(x - offset 5, y + offset 42, radius 12, radius 8, beakColor)
            Raylib.DrawEllipse(x + offset 10, y + offset 42, radius 12, radius 8, beakColor)
            Raylib.DrawEllipse(x, y, radius 24, radius 45, bodyColor)
            Raylib.DrawEllipse(x + offset 8, y + offset 12, radius 12, radius 30, bellyColor)
            Raylib.DrawEllipse(x + offset 22, y - offset 10, radius 10, radius 5, beakColor)
            Raylib.DrawCircle(x + offset 8, y - offset 18, radius 6, Color.White)
            Raylib.DrawCircle(x + offset 10, y - offset 18, radius 3, Color.Black)
            Raylib.DrawEllipse(x - offset 2, y + offset 10, radius 8, radius 25, wingColor)

        | Left ->
            Raylib.DrawEllipse(x + offset 5, y + offset 42, radius 12, radius 8, beakColor)
            Raylib.DrawEllipse(x - offset 10, y + offset 42, radius 12, radius 8, beakColor)
            Raylib.DrawEllipse(x, y, radius 24, radius 45, bodyColor)
            Raylib.DrawEllipse(x - offset 8, y + offset 12, radius 12, radius 30, bellyColor)
            Raylib.DrawEllipse(x - offset 22, y - offset 10, radius 10, radius 5, beakColor)
            Raylib.DrawCircle(x - offset 8, y - offset 18, radius 6, Color.White)
            Raylib.DrawCircle(x - offset 10, y - offset 18, radius 3, Color.Black)
            Raylib.DrawEllipse(x + offset 2, y + offset 10, radius 8, radius 25, wingColor)

    let drawExplosionBomb (point: GridPoint) (cellSize: int) (originX: int) (originY: int) =
        let x = originX + point.X * cellSize
        let y = originY + point.Y * cellSize
        let centerX = x + cellSize / 2
        let centerY = y + cellSize / 2

        Raylib.DrawCircle(centerX, centerY, float32 cellSize * 0.45f, Color(255uy, 91uy, 38uy, 220uy))
        Raylib.DrawCircle(centerX, centerY, float32 cellSize * 0.25f, Color(255uy, 232uy, 120uy, 240uy))

    let drawNormalBomb (bomb: Bomb) (cellSize: int) (originX: int) (originY: int) =
        let position = bomb.getVisualPosition

        let centerX = originX + int (position.X * float32 cellSize) + cellSize / 2
        let centerY = originY + int (position.Y * float32 cellSize) + cellSize / 2
        let radius = float32 cellSize * 0.40f
        // LLM used
        Raylib.DrawCircle(centerX, centerY, radius, Color.Black)
        Raylib.DrawCircle(centerX - cellSize / 12, centerY - cellSize / 12, radius * 0.35f, Color.DarkGray)
        Raylib.DrawLine(centerX + cellSize / 8, centerY - cellSize / 5, centerX + cellSize / 4, centerY - cellSize / 3, Color.Orange)
    
    let drawBomb (bomb: Bomb) (stageMap: StageMap) cellSize originX originY =
        match bomb.getState with
        | Normal
        | Pending -> drawNormalBomb bomb cellSize originX originY
        | Boom _ ->
            let exceptSolid = List.filter (fun point -> stageMap.CellAt(point) <> SolidWall) (bomb.explode())
            List.iter (fun point -> drawExplosionBomb point cellSize originX originY) exceptSolid
            
    let drawPlayer (player: Player) (cellSize: int) (originX: int) (originY: int) =
        let position = player.getVisualPosition
        let direction = player.getDirection
        let x = originX + int (position.X * float32 cellSize) + (cellSize / 2)
        let y = originY + int (position.Y * float32 cellSize) + (cellSize / 2)
        drawPenguin x y direction cellSize

    let drawStageCounter (stageMap: StageMap) (stageIndex: int) (cellSize: int) (originX: int) (originY: int) =
        let text = sprintf "Stage : %d " (stageIndex + 1)
        let textColor = Color(82uy, 89uy, 102uy, 255uy)
        Raylib.DrawText(text, originX, originY - 30, 25, textColor)

    let drawBombCounter (bombCount: int) (cellSize: int) (originX: int) (originY: int) (stageMap: StageMap) =
        let boardRight = originX + stageMap.Width * cellSize
        let boardBottom = originY + stageMap.Height * cellSize

        let panelWidth = 130
        let panelHeight = 50
        let margin = 10

        let x = boardRight - panelWidth
        let y = boardBottom + margin

        let backgroundColor = Color(82uy, 89uy, 102uy, 255uy)
        let textColor = Color.White
        Raylib.DrawRectangle(x, y, panelWidth, panelHeight, backgroundColor)

        let iconSize = 40
        let iconX = x + 10
        let iconY = y + (panelHeight - iconSize) / 2
        let iconBomb = Bomb({ X = 0; Y = 0 })

        drawNormalBomb iconBomb iconSize iconX iconY

        Raylib.DrawText(sprintf "x %d" bombCount, x + 60, y + 15, 25, textColor)
    
    let restartButtonRect =
        Rectangle(float32 (windowWidth / 2 - 60), float32 (windowHeight / 2 + 60), 120.0f, 44.0f)

    let exitButtonRect =
        Rectangle(float32 (windowWidth / 2 - 60), float32 (windowHeight / 2+20), 120.0f, 44.0f)

    let drawButton (rect: Rectangle) (text: string) =
        Raylib.DrawRectangleRec(rect, Color(82uy, 89uy, 102uy, 255uy))
        let fontSize = 24
        let textWidth = Raylib.MeasureText(text, fontSize)
        let textX = int rect.X + (int rect.Width - textWidth) / 2
        let textY = int rect.Y + (int rect.Height - fontSize) / 2
        Raylib.DrawText(text, textX, textY, fontSize, Color.White)
        
    let drawRestart () =
        let title = "Restart?"
        let fontSize = 40
        let textWidth = Raylib.MeasureText(title, fontSize)

        Raylib.DrawText(title, (windowWidth - textWidth) / 2, windowHeight / 2 - 80, fontSize, Color(82uy, 89uy, 102uy, 255uy))
        drawButton restartButtonRect "Restart"
        drawButton exitButtonRect "EXIT"

    let drawClear () = 
        let text = "Game Clear"
        let textWidth = Raylib.MeasureText(text, 30)
        Raylib.DrawText(text, (windowWidth - textWidth) / 2, windowHeight / 2 - 20, 30, Color(82uy, 89uy, 102uy, 255uy))
        drawButton exitButtonRect "EXIT"

    member _.Draw (gameState: GameState) (stageMap: StageMap) (stageIndex: int) (player: Player) (bombs: Bomb list) (bombCount: int)=
        match gameState with
        | Playing -> 
            let cellSize, originX, originY = drawMap stageMap
            bombs |> List.iter (fun bomb -> drawBomb bomb stageMap cellSize originX originY)
            drawPlayer player cellSize originX originY
            drawStageCounter stageMap stageIndex cellSize originX originY
            drawBombCounter bombCount cellSize originX originY stageMap
        | Restart ->
            drawRestart ()
        | StageClear time ->
            let text = "Stage Clear!"
            let textWidth = Raylib.MeasureText(text, 30)
            Raylib.DrawText(text, (windowWidth - textWidth) / 2, windowHeight / 2 - 20, 30, Color(82uy, 89uy, 102uy, 255uy))
        | GameClear ->
            drawClear ()
        | Exit -> ()