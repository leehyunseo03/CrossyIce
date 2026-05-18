namespace CrossyIce

type Player(initPosition: GridPoint) =
    inherit GameObject(initPosition, 6.0f)

    let mutable direction: Direction = Front

    member _.getDirection = direction

    member _.setDirection(nextDir: Direction) =
        direction <- nextDir