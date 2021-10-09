module MotoScreen.Types

open System

open MotoScreen.Interaction

type Msg =
    | Input of UserInteraction
    | Tick of Unit

type QuickMenuItem =
    { id: string
      title: string
      state: string option }

type Percentage = int
type Speed = int

type Rpm = { currentRpm: int; maxRpm: int }

type FuelInfo =
    { level: Percentage
      range: int }
    static member Default = { level = 50; range = 0 }

type Gear =
    | First
    | Neutral
    | Second
    | Third
    | Fourth
    | Fifth
    | Sixth

type QuickMenuItems =
    { currentlySelected: QuickMenuItem option
      allOptions: QuickMenuItem list }
    static member Default =
        let items =
            [ "RidingModes", None
              "Heated grips", Some "0"
              "Heated seat", Some "0"
              "Music", None
              "Maps", None
              "Trip computer", None
              "Settings", None ]
            |> Seq.mapi
                (fun i (title, state) ->
                    { id = sprintf "mi_%i" i
                      title = title
                      state = state })
            |> Seq.toList

        { currentlySelected = None
          allOptions = items }

type State =
    { QuickMenuItems: QuickMenuItems
      Speed: Speed
      Rpm: Rpm
      Fuel: FuelInfo
      Time: DateTimeOffset
      Gear: Gear }
    static member Default =
        { QuickMenuItems = QuickMenuItems.Default
          Speed = 0
          Rpm = { currentRpm = 0; maxRpm = 10_000 }
          Fuel = FuelInfo.Default
          Time = DateTimeOffset.Now
          Gear = Neutral }

type TickData = { gear: string }
