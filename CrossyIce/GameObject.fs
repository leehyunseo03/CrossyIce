namespace CrossyIce

[<AbstractClass>]
type GameObject(initPosition: GridPoint, initMoveSpeed: float32) =

    let toVisual (p: GridPoint) : VisualPoint =
        { X = float32 p.X; Y = float32 p.Y }

    let mutable position: GridPoint = initPosition
    let mutable visualPosition: VisualPoint = toVisual initPosition
    let mutable moveSpeed: float32 = initMoveSpeed

    member _.getPosition = position

    member _.setPosition(nextPos: GridPoint) =
        position <- nextPos

    member _.resetPosition(nextPos: GridPoint) =
        position <- nextPos
        visualPosition <- toVisual nextPos

    member _.getVisualPosition = visualPosition

    member _.setVisualPosition(frameTime: float32) =
        let targetVisPos = toVisual position

        let dx = targetVisPos.X - visualPosition.X
        let dy = targetVisPos.Y - visualPosition.Y
        let distance = sqrt (dx * dx + dy * dy)
        let moveParam = moveSpeed * frameTime

        if distance <= moveParam then
            visualPosition <- targetVisPos
        else
            let dX = dx / distance
            let dY = dy / distance

            visualPosition <-
                { X = visualPosition.X + dX * moveParam
                  Y = visualPosition.Y + dY * moveParam }

    member _.isMoving =
        let target = toVisual position
        let dx = target.X - visualPosition.X
        let dy = target.Y - visualPosition.Y
        sqrt (dx * dx + dy * dy) > 0.001f

    member _.MoveSpeed
        with get () = moveSpeed
        and set value = moveSpeed <- value