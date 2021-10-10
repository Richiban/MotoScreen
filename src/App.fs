module MotoScreen.App

open System

open Fable.Import
open Elmish
open Elmish.React

open MotoScreen.Types
open MotoScreen.Interaction
open MotoScreen.View
open Browser.Dom
open Browser.Types

let init () =
    let initialState = State.Default
    let initialCmd = Cmd.none
    initialState, initialCmd

let goUp quickMenuItems =
    printfn "Going up"
    quickMenuItems

let goDown quickMenuItems = quickMenuItems

type Direction =
    | Left
    | Right

let goLeftRight menu direction =
    let selectedIndex =
        menu.allOptions
        |> Seq.findIndex ((=) menu.currentlySelected)

    let newIndex =
        match direction with
        | Left -> selectedIndex - 1 |> max 0
        | Right ->
            selectedIndex + 1
            |> min (menu.allOptions.Length - 1)

    let newSelected = menu.allOptions |> List.item newIndex

    printfn "%A" newSelected

    { menu with
          currentlySelected = newSelected }

let goLeft quickMenuItems = goLeftRight quickMenuItems Left

let goRight quickMenuItems = goLeftRight quickMenuItems Right

let random = Random()

let randomiseDriveData state =
    let currentSpeed = state.Speed

    // Use even numbers as code for "accelerating" and odd for "decelerating"
    let accelerating = currentSpeed % 2 = 0

    let delta =
        if accelerating then
            random.Next(-1, 16)
        else
            random.Next(-16, 1)

    let newSpeed = max 0 (delta + currentSpeed)

    let newGear =
        match newSpeed / 15 with
        | 0 -> First
        | 1 -> Second
        | 2 -> Third
        | 3 -> Fourth
        | 4 -> Fifth
        | _ -> Sixth

    { state with
          Gear = newGear
          Speed = newSpeed }

let openQuickMenu direction state =
    match direction with
    | KeyUp ->
        { state with
              QuickMenu =
                  { goUp state.QuickMenu with
                        openState = Open TimeSpan.Zero } }
    | KeyDown ->
        { state with
              QuickMenu =
                  { goDown state.QuickMenu with
                        openState = Open TimeSpan.Zero } }
    | KeyLeft ->
        { state with
              QuickMenu =
                  { goLeft state.QuickMenu with
                        openState = Open TimeSpan.Zero } }
    | KeyRight ->
        { state with
              QuickMenu =
                  { goRight state.QuickMenu with
                        openState = Open TimeSpan.Zero } }

let closeQuickMenu state =
    { state with
          QuickMenu =
              { state.QuickMenu with
                    openState = Closed } }

let update (msg: Msg) (state: State) =
    let state =
        let newDriveData = randomiseDriveData state.DriveData
        let stateWithNewDriveData = { state with DriveData = newDriveData }

        match msg with
        | Input direction -> openQuickMenu direction state
        | Tick elapsed ->
            match state.QuickMenu.openState with
            | Closed -> stateWithNewDriveData
            | Open time ->
                let totalOpen = time + elapsed
                let quickMenuHasTimedOut = totalOpen > TimeSpan.FromSeconds 5.0

                if quickMenuHasTimedOut then
                    { stateWithNewDriveData with
                          QuickMenu =
                              { state.QuickMenu with
                                    openState = Closed } }
                else
                    { stateWithNewDriveData with
                          QuickMenu =
                              { state.QuickMenu with
                                    openState = Open(totalOpen) } }

    state, Cmd.none

let timerTick dispatch =
    let interval = TimeSpan.FromMilliseconds(500.0)

    window.setInterval ((fun _ -> dispatch (Tick interval)), int interval.TotalMilliseconds)
    |> ignore

let inputs dispatch =
    let update (e: KeyboardEvent) =
        match e.key with
        | "ArrowUp" -> KeyUp |> Input |> dispatch
        | "ArrowLeft" -> KeyLeft |> Input |> dispatch
        | "ArrowDown" -> KeyDown |> Input |> dispatch
        | "ArrowRight" -> KeyRight |> Input |> dispatch
        | _ -> ()

    document.addEventListener ("keydown", (fun e -> update (e :?> _)))

Program.mkProgram init update render
|> Program.withSubscription
    (fun _ ->
        Cmd.batch [ Cmd.ofSub timerTick
                    Cmd.ofSub inputs ])
|> Program.withReactSynchronous "elmish-app"
|> Program.run
