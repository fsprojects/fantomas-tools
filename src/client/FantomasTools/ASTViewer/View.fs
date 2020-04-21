module FantomasTools.Client.ASTViewer.View

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open FantomasTools.Client
open FantomasTools.Client.ASTViewer.Model
open FantomasTools.Client.Editor
open Reactstrap

let private isJsonView =
    function
    | JsonViewer -> true
    | _ -> false

let private isEditorView =
    function
    | Editor -> true
    | _ -> false

let private isRawView =
    function
    | Raw -> true
    | _ -> false

let private isGraph =
    function
    | Graph -> true
    | _ -> false

let private results model dispatch =
    match model.Parsed with
    | Ok (Some parsed) ->
        match model.View with
        | Raw ->
            FantomasTools.Client.Editor.editorInTab
                [ EditorProp.Language "fsharp"
                  EditorProp.IsReadOnly true
                  EditorProp.Value parsed.String ]
        | Editor ->
            FantomasTools.Client.Editor.editorInTab
                [ EditorProp.Language "fsharp"
                  EditorProp.IsReadOnly true
                  EditorProp.Value(Fable.Core.JS.JSON.stringify (parsed.Node, space = 4)) ]
        | JsonViewer ->
            ReactJsonView.viewer
                [ ReactJsonView.Src(parsed.Node)
                  ReactJsonView.Name null
                  ReactJsonView.DisplayDataTypes false
                  ReactJsonView.DisplayObjectSize false
                  ReactJsonView.IndentWidth 2
                  ReactJsonView.OnLookup(fun o ->
                      let range: FantomasTools.Client.Editor.HighLightRange =
                          { StartLine = !!(o.value?StartLine)
                            StartColumn = !!(o.value?StartCol)
                            EndLine = !!(o.value?EndLine)
                            EndColumn = !!(o.value?EndCol) }

                      dispatch (HighLight range))
                  ReactJsonView.ShouldLookup(fun o -> o.key = "Range")
                  ReactJsonView.ShouldCollapse(fun x -> x?name = "Range") ]
        | Graph -> div [] [ str "graph view has not been ported yet" ]
    // Please extract this to its own file

    //           let node = parsed.Node
    //           let rec propertiesToHtml inArray p =
    //               match p with
    //               | JNumber x -> sprintf "%f" x
    //               | JString x -> x
    //               | JBool x -> sprintf "%b" x
    //               | JNull -> "null"
    //               | JArray [JString k; v] when inArray ->
    //                   propertiesToHtml false (JObject (Map.ofSeq[k,v]))
    //               | JArray xs -> xs |> Seq.map (propertiesToHtml true >> sprintf "<li>%s</li>") |> String.concat "" |> sprintf "<ul>%s</ul>"
    //               | JObject o ->
    //                   o |> Map.filter (fun k _ -> not (k.ToLower().Contains "range")) |> Map.toSeq
    //                   |> Seq.map (fun (k, v) -> sprintf "<li><b>%s: </b>%s</li>" k (propertiesToHtml false v))
    //                   |> String.concat "" |> sprintf "<ul>%s</ul>"
    //
    //           let simpleType (t: string) = t.Split([|'.'|]) |> Seq.last
    //           let rec buildTree isRoot n = {
    //               Label = simpleType n.Type
    //               Color = if isRoot then Some "lime" else None
    //               Tooltip =
    //                   Thoth.Json.Encode.Auto.toString(0, n.Properties) |> SimpleJson.parse
    //                   |> fun x -> JArray[JString (sprintf "<b><i>%s</i></b>" n.Type); x] |> propertiesToHtml false
    //               Childrens = List.map (buildTree false) n.Childs
    //               Original = n
    //               }
    //           let buildTree n = buildTree true n
    //           let limitTree = memoize2 <| fun allowedSet n ->
    //               let rec f n =
    //                   let childs = n.Childrens |> List.filter (fun c -> List.contains c allowedSet)
    //                   let limit = not(List.isEmpty n.Childrens) && List.isEmpty childs
    //                   { n with Childrens = childs |> List.map f
    //                            Color = if limit then Some "cyan" else None}
    //               f n
    //           let limitTreeByNodes = memoize2 <| fun maxNodes n ->
    //               let q = Queue.empty
    //               let rec loop q acc i =
    //                    if i >= maxNodes then acc else
    //                    match Queue.dequeue q with
    //                    | None -> acc
    //                    | Some (x, q2) ->
    //                        let q3 = x.Childrens |> List.fold Queue.enqueue q2
    //                        loop q3 (x::acc) (i+1)
    //               let allowedNodes = loop (Queue.enqueue Queue.empty n) [] 1
    //               limitTree allowedNodes n
    //
    //           let rec childsRange = memoize <| fun n ->
    //               match n.Range with
    //               | Some r -> Some r
    //               | None ->
    //               let rs = n.Childs |> Seq.choose childsRange
    //               if Seq.isEmpty rs then None else Some (rs |> Seq.reduce (fun r1 r2 -> {
    //                   StartLine = min r1.StartLine r2.StartLine
    //                   StartCol = min r1.StartCol r2.StartCol
    //                   EndLine = max r1.EndLine r2.EndLine
    //                   EndCol = max r1.EndCol r2.EndCol
    //                   }))
    //           let onHover (n: Node) =
    //               //Browser.Dom.console.log("hoverNode Event", n, editor)
    //               editor |> Option.iter (fun editor ->
    //                   let empty = createEmpty<Monaco.IRange>
    //                   let range = empty
    //                   n |> childsRange |> Option.iter (fun r ->
    //                       range.endColumn <- r.EndCol + 1
    //                       range.endLineNumber <- r.EndLine
    //                       range.startColumn <- r.StartCol + 1
    //                       range.startLineNumber <- r.StartLine
    //                       //Browser.Dom.console.log("set range", range)
    //                       editor.setSelection(range)
    //                       editor.revealRangeInCenter(range, ScrollType.Smooth))
    //               )
    //           let onSelect (n: Node) = dispatch (Graph <| GraphMsg.SetRoot n)
    //
    //           let root = model.Graph.RootsPath |> List.tryHead |> Option.defaultValue node
    //           let graphButtons =
    //               Columns.columns [ ]
    //                   [ Column.column [ Column.Width(Screen.All, Column.Is3) ]
    //                       [if not model.Graph.RootsPath.IsEmpty then
    //                           yield button [Button.Color IsPrimary; Button.Size IsSmall] "Back" (fun _ -> dispatch (Graph <| GraphMsg.RootBack))]
    //                     Column.column [ Column.Width(Screen.All, Column.Is3) ]
    //                       [button [Button.Color IsDark; Button.Size IsSmall] "Free view" (fun _ -> dispatch (Graph <| GraphMsg.SetOptions {model.Graph.Options with Layout = Graph.Free}))]
    //                     Column.column [ Column.Width(Screen.All, Column.Is3) ]
    //                       [button [Button.Color IsDark; Button.Size IsSmall] "Left-Right view" (fun _ -> dispatch (Graph <| GraphMsg.SetOptions {model.Graph.Options with Layout = Graph.HierarchicalLeftRight}))]
    //                     Column.column [ Column.Width(Screen.All, Column.Is3) ]
    //                       [button [Button.Color IsDark; Button.Size IsSmall] "Upper-Down view" (fun _ -> dispatch (Graph <| GraphMsg.SetOptions {model.Graph.Options with Layout = Graph.HierarchicalUpDown}))]
    //                   ]
    //           div [] [
    //               graphButtons
    //               GraphView.graph [
    //                   GraphView.Props.Tree (buildTree root |> limitTreeByNodes model.Graph.Options.MaxNodes)
    //                   GraphView.Props.OnHover onHover
    //                   GraphView.Props.OnSelect onSelect
    //                   GraphView.Props.Options model.Graph.Options
    //                   ] ]
    | Result.Error errors ->
        FantomasTools.Client.Editor.editorInTab
            [ EditorProp.Language "fsharp"
              EditorProp.IsReadOnly true
              EditorProp.Value errors ]
    | Ok None -> str ""

let view model dispatch =
    if model.IsLoading then
        FantomasTools.Client.Loader.loader
    else
        results model dispatch

let commands dispatch =
    fragment []
        [ Button.button
            [ Button.Color Primary
              Button.Custom [ OnClick(fun _ -> dispatch DoParse) ] ] [ str "Show Untyped AST" ]
          Button.button
              [ Button.Color Primary
                Button.Custom [ OnClick(fun _ -> dispatch DoTypeCheck) ] ] [ str "Show Typed AST" ] ]

let settings model dispatch =
    fragment []
        [ FantomasTools.Client.VersionBar.versionBar (sprintf "FSC - %s" model.Version)
          SettingControls.input (DefinesUpdated >> dispatch) "Defines" "Enter your defines separated with a space"
              model.Defines
          SettingControls.toggleButton (fun _ -> dispatch (SetFsiFile false)) (fun _ -> dispatch (SetFsiFile true))
              "*.fsi" "*.fs" "File extension" model.IsFsi
          SettingControls.multiButton "Mode"
              [ { IsActive = (isJsonView model.View)
                  Label = "JsonViewer"
                  OnClick = (fun _ -> dispatch ShowJsonViewer) }
                { IsActive = (isEditorView model.View)
                  Label = "Editor"
                  OnClick = (fun _ -> dispatch ShowEditor) }
                { IsActive = (isRawView model.View)
                  Label = "Raw"
                  OnClick = (fun _ -> dispatch ShowRaw) }
                { IsActive = (isGraph model.View)
                  Label = "Graph"
                  OnClick = (fun _ -> dispatch ShowGraph) } ] ]
