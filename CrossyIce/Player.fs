namespace CrossyIce

type Player(initPosition: GridPoint) = 

    let mutable position: GridPoint = initPosition
    let mutable direction: Direction = Front

    let toVisual (p: GridPoint) : VisualPoint = 
        { X = float32 p.X; Y = float32 p.Y }

    let mutable visualPosition: VisualPoint = toVisual initPosition

    member _.getPosition = position
    member _.setPosition (nextPos: GridPoint) = 
        position <- nextPos

    member _.getDirection = direction
    member _.setDirection (nextDir: Direction) = 
        direction <- nextDir
    
    member _.getVisualPosition = visualPosition
    member _.setVisualPosition (frameTime: float32) = 
        let targetVisPos = toVisual position

        let dx = targetVisPos.X - visualPosition.X
        let dy = targetVisPos.Y - visualPosition.Y
        let distance = sqrt (dx * dx + dy * dy)
        let moveParam = 5.0f * frameTime

        if distance <= moveParam then
            visualPosition <- targetVisPos
        else    
            let dX = dx / distance
            let dY = dy / distance            
            visualPosition <- { 
                X = visualPosition.X + (dX * moveParam)
                Y = visualPosition.Y + (dY * moveParam) 
            }

    member _.isMoving =
        let target = toVisual position
        let dx = target.X - visualPosition.X
        let dy = target.Y - visualPosition.Y
        (sqrt (dx * dx + dy * dy)) > 0.001f