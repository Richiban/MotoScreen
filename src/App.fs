module MotoScreen.App

open System

open Fable.Import
open Elmish
open Elmish.React

open MotoScreen.Types
open MotoScreen.Interaction
open MotoScreen.Render
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

let goLeftRight quickMenuItems direction =
    match quickMenuItems.currentlySelected with
    | None ->
        let selected = Some quickMenuItems.allOptions.Head

        printfn "%A" selected

        { quickMenuItems with
              currentlySelected = selected }
    | Some opt ->
        let selectedIndex =
            quickMenuItems.allOptions
            |> Seq.findIndex ((=) opt)

        let newIndex =
            match direction with
            | Left -> selectedIndex - 1 |> max 0
            | Right ->
                selectedIndex + 1
                |> min (quickMenuItems.allOptions.Length - 1)

        let newSelected =
            quickMenuItems.allOptions |> List.item newIndex

        printfn "%A" newSelected

        { quickMenuItems with
              currentlySelected = Some newSelected }

let goLeft quickMenuItems = goLeftRight quickMenuItems Left

let goRight quickMenuItems = goLeftRight quickMenuItems Right

let update (msg: Msg) (state: State) =
    let state =
        match msg with
        | Input KeyUp ->
            { state with
                  QuickMenuItems = goUp state.QuickMenuItems }
        | Input KeyDown ->
            { state with
                  QuickMenuItems = goDown state.QuickMenuItems }
        | Input KeyLeft ->
            { state with
                  QuickMenuItems = goLeft state.QuickMenuItems }
        | Input KeyRight ->
            { state with
                  QuickMenuItems = goRight state.QuickMenuItems }
        | Tick tickData -> state

    state, Cmd.none

let timerTick dispatch =
    window.setInterval ((fun _ -> dispatch (Tick())), 1000)
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
