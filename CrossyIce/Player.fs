namespace CrossyIce

type Player(init_position: GridPoint) = 

    let mutable position: GridPoint = init_position
    let mutable direction: Direction = Front

    member _.getPosition = position
    member _.setPosition (nextPos: GridPoint) = 
        position <- nextPos

    member _.getDirection = direction
    member _.setDirection (nextDir: Direction) = 
        direction <- nextDir

