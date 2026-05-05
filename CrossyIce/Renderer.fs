namespace CrossyIce

open Raylib_cs

type CellStyle =
    { BaseColor: Color
      DrawDetail: int -> int -> int -> unit }

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

    member _.DrawMap(stageMap: StageMap) =
        drawMap stageMap |> ignore
