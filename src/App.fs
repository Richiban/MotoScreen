module MotoScreen.App

open System

open Elmish
open Elmish.React
open Feliz

open MotoScreen.Types

let init () =
    let initialState = State.Default
    let initialCmd = Cmd.none
    initialState, initialCmd

let update (msg: Msg) (state: State) = state, Cmd.none

let renderError (errorMsg: string) =
    Html.h1 [ prop.style [ style.color.red ]
              prop.text errorMsg ]

let div (classes: string list) (children: ReactElement list) =
    Html.div [ prop.className classes
               prop.children children ]

let renderQuickMenuItem item =
    let tileText =
        match item.state with
        | None -> item.title
        | Some state -> sprintf "%s (%s)" item.title state

    Html.div [ prop.className "tile is-parent"
               prop.children [ Html.div [ prop.key item.id
                                          prop.id item.id
                                          prop.className "tile is-child box"
                                          prop.text tileText ] ] ]

let renderQuickMenuItems items =
    Html.div [ prop.className "tile is-ancestor is-spaced"
               prop.children [ for item in items.allOptions -> renderQuickMenuItem item ] ]

let spinner =
    Html.div [ prop.style [ style.textAlign.center
                            style.marginTop 20 ]
               prop.children [ Html.i [ prop.className "fa fa-cog fa-spin fa-2x" ] ] ]

let renderRpms (rpms: Rpm) =
    Html.progress [ prop.className "progress"
                    prop.text (string rpms)
                    prop.value (rpms.currentRpm)
                    prop.max (rpms.maxRpm) ]

let renderSpeed (speed: Speed) =
    let speedString = sprintf "%i mph" speed
    Html.div [ prop.text speedString ]

let renderGear gear =
    let s =
        match gear with
        | First -> "1"
        | Neutral -> "N"
        | Second -> "2"
        | Third -> "3"
        | Fourth -> "4"
        | Fifth -> "5"
        | Sixth -> "6"

    Html.div [ prop.text s ]

let renderFuel { level = level; range = range } =
    Html.div [ prop.children [ Html.text "Fuel "
                               Html.progress [ prop.value level
                                               prop.max 100 ]
                               Html.text (sprintf " (%i mi)" range) ] ]

// let renderItems =
//     function
//     | HasNotStartedYet -> Html.none
//     | InProgress -> spinner
//     | Resolved (Error errorMsg) -> renderError errorMsg
//     | Resolved (Ok items) -> Html.fragment [ for item in items -> renderItem item ]

let render (state: State) (dispatch: Msg -> unit) =
    Html.div [ prop.style [ style.padding 20 ]
               prop.children [ Html.h1 [ prop.className "title"
                                         prop.text "Moto Screen" ]

                               renderSpeed state.Speed

                               renderRpms state.Rpm

                               renderGear state.Gear

                               renderFuel state.Fuel

                               renderQuickMenuItems state.QuickMenuItems ] ]

Program.mkProgram init update render
|> Program.withReactSynchronous "elmish-app"
|> Program.run
