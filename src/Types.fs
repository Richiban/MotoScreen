module MotoScreen.Types

open System

open MotoScreen.Interaction

type Msg =
    | Input of UserInteraction
    | Tick of TimeSpan

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

type OpenState =
    | Closed
    | Open of openFor: TimeSpan

type QuickMenu =
    { currentlySelected: QuickMenuItem
      allOptions: QuickMenuItem list
      openState: OpenState }
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

        { currentlySelected = items |> Seq.head
          allOptions = items
          openState = Closed }

type DriveData =
    { Speed: Speed
      Rpm: Rpm
      Fuel: FuelInfo
      Gear: Gear }
    static member Default =
        { Speed = 0
          Rpm = { currentRpm = 0; maxRpm = 10_000 }
          Fuel = FuelInfo.Default
          Gear = Neutral }

type State =
    { QuickMenu: QuickMenu
      Time: DateTimeOffset
      DriveData: DriveData }
    static member Default =
        { QuickMenu = QuickMenu.Default
          Time = DateTimeOffset.Now
          DriveData = DriveData.Default }
