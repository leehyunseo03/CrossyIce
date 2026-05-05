namespace CrossyIce

type Player(init_position: GridPoint) = 

    let mutable position: GridPoint = init_position

    member _.Position = position
