module MotoScreen.Types

open System

type QuickMenuItem =
    { id: string
      title: string
      state: string option }
    static member ofList titlePairs =
        titlePairs
        |> Seq.mapi
            (fun i (title, state) ->
                { id = sprintf "mi_%i" i
                  title = title
                  state = state })
        |> Seq.toList

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
        { currentlySelected = None
          allOptions =
              QuickMenuItem.ofList [ "RidingModes", None
                                     "Heated grips", Some "0"
                                     "Heated seat", Some "0"
                                     "Music", None
                                     "Maps", None
                                     "Settings", None ] }

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

type Msg = Msg of unit
