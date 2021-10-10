module MotoScreen.View

open Elmish
open Elmish.React
open Feliz

open MotoScreen.Types

let renderError (errorMsg: string) =
    Html.h1 [ prop.style [ style.color.red ]
              prop.text errorMsg ]

let div (classes: string list) (children: ReactElement list) =
    Html.div [ prop.className classes
               prop.children children ]

let renderQuickMenuItem item isSelected menuIsOpen =
    let tileText =
        match item.state with
        | None -> item.title
        | Some state -> sprintf "%s (%s)" item.title state

    let className =
        [ "tile"
          "is-child"
          if menuIsOpen then "box" ]

    let styles =
        [ if isSelected && menuIsOpen then
              style.backgroundColor "lightblue" ]

    Html.div [ prop.className "tile is-parent"
               prop.children [ Html.div [ prop.key item.id
                                          prop.id item.id
                                          prop.style styles
                                          prop.className className
                                          prop.text tileText ] ] ]

let renderQuickMenuItems menu =
    let menuIsOpen =
        match menu.openState with
        | Open _ -> true
        | Closed -> false

    Html.div [ prop.className "tile is-ancestor is-spaced"
               prop.children [ for item in menu.allOptions ->
                                   renderQuickMenuItem item (menu.currentlySelected = item) menuIsOpen ] ]

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
    Html.div [ prop.style [ style.padding 20
                            style.width 500
                            style.custom ("margin", "auto") ]
               prop.children [ Html.h1 [ prop.className "title"
                                         prop.text "Moto Screen" ]

                               renderSpeed state.DriveData.Speed

                               renderRpms state.DriveData.Rpm

                               renderGear state.DriveData.Gear

                               renderFuel state.DriveData.Fuel

                               renderQuickMenuItems state.QuickMenu ] ]
