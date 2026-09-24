module FantomasTools.Client.SkillBanner

open Fable.Core
open Fable.React
open Fable.React.Props
open Feliz

/// Set once the banner is closed, so it stays closed on the next visit.
[<Literal>]
let private DismissedKey = "fantomasReportSkillBannerDismissed"

[<Literal>]
let private Command =
    "npx skills add fsprojects/fantomas --skill fantomas-report -g"

[<Literal>]
let private DocsUrl =
    "https://fsprojects.github.io/fantomas/docs/end-users/ReportingBugs.html#With-a-coding-agent"

[<Emit("navigator.clipboard.writeText($0)")>]
let private writeText (_text: string) : JS.Promise<unit> = jsNative

/// Points people who work with a coding agent to the `fantomas-report` skill, which shrinks a
/// failing file to a sample and fills in the issue the same way the Fantomas tab does.
///
/// Whether it was closed is kept in local storage rather than the model: it is about this browser,
/// not about the code on screen, and nothing else in the app needs to know.
[<ReactComponent>]
let SkillBanner () =
    let dismissed, setDismissed =
        React.useState (fun () -> not (isNull (Browser.WebStorage.localStorage.getItem DismissedKey)))

    let copied, setCopied = React.useState false

    let dismiss _ =
        Browser.WebStorage.localStorage.setItem (DismissedKey, "true")
        setDismissed true

    let copy _ =
        writeText Command
        |> Promise.iter (fun () ->
            setCopied true
            JS.setTimeout (fun () -> setCopied false) 2000 |> ignore
        )

    let copyIcon =
        if copied then
            "fa-solid fa-check"
        else
            "fa-regular fa-copy"

    if dismissed then
        null
    else
        div [
            Id "skill-banner"
            Role "region"
            HTMLAttr.Custom("aria-label", "Report bugs with a coding agent")
        ] [
            i [ ClassName "fa-solid fa-robot" ] []
            p [] [
                str "Found a Fantomas bug? Let your coding agent report it with the "
                a [ Href DocsUrl; Target "_blank" ] [ str "fantomas-report skill" ]
                str "."
            ]
            div [] [
                code [] [ str Command ]
                button [ Title "Copy to clipboard"; OnClick copy ] [ i [ ClassName copyIcon ] [] ]
            ]
            button [ Id "close-skill-banner"; Title "Dismiss"; OnClick dismiss ] [
                i [ ClassName "fa-solid fa-xmark" ] []
            ]
        ]
