namespace CrossyIce

type Bomb(initPosition: GridPoint) =
    inherit GameObject(initPosition, 10.0f)

    let mutable state = Normal
    let explodeTime = 0.1f
    member _.getState = state

    member _.clearState() =
        state 

    member _.pendingState() =
        state <- Pending

    member _.explodeState() =
        state <- Boom explodeTime

    member _.updateExplosion(frameTime: float32) =
        match state with
        | Boom timeLeft ->
            state <- Boom (timeLeft - frameTime)
        | Normal
        | Pending -> ()

    member _.isExplosionFinished =
        match state with
        | Boom timeLeft -> timeLeft <= 0.0f
        | Normal
        | Pending -> false

    member this.explode () =
        let center = this.getPosition

        [ center
          { center with Y = center.Y - 1 }
          { center with Y = center.Y + 1 }
          { center with X = center.X - 1 }
          { center with X = center.X + 1 } ]