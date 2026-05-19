namespace CrossyIce

type Bomb(initPosition: GridPoint) =
    inherit GameObject(initPosition, 10.0f)

    member this.explode () =
        let center = this.getPosition

        [ center;
        { center with Y = center.Y - 1 };
        { center with Y = center.Y + 1 };
        { center with X = center.X - 1 };
        { center with X = center.X + 1 };]