module ASTViewer.Server.TastTransformer

open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Range

module Helpers =
    let r (r: range): range option = Some r

    let p = Map.ofList
    let inline (==>) a b = (a, box b)

    let noRange = None

module Tast =
    open Helpers
    type FsAstNode = obj

    type Node =
        { Type: string
          Range: range option
          Properties: Map<string, obj>
          Childs: Node list
          FsAstNode: FsAstNode }


    let fsharpTypeToProps (t: FSharpType) = sprintf "%A" t

    let fsharpMemberToProps (t: FSharpMemberOrFunctionOrValue) = sprintf "%A" t

    let fsharpUnionCaseToProps (t: FSharpUnionCase) = sprintf "%A" t

    let fsharpFieldToProps (t: FSharpField) = sprintf "%A" t

    let fsharpGenericParameterToProps (t: FSharpGenericParameter) = sprintf "%A" t

    let fsharpObjectExprOverrideToProps (t: FSharpObjectExprOverride) = sprintf "%A" t

    let fsharpEntityToProps (t: FSharpEntity) = sprintf "%A" t

    let rec visitExpr (e: FSharpExpr): Node =
        match e with
        | BasicPatterns.AddressOf (lvalueExpr) ->
            { Type = "BasicPatterns.AddressOf"
              Range = r e.Range
              Properties = p []
              Childs = [ visitExpr lvalueExpr ]
              FsAstNode = box e }
        | BasicPatterns.AddressSet (lvalueExpr, rvalueExpr) ->
            { Type = "BasicPatterns.AddressSet"
              Properties = p []
              Range = r e.Range
              Childs =
                  [ visitExpr lvalueExpr
                    visitExpr rvalueExpr ]
              FsAstNode = box e }
        | BasicPatterns.Application (funcExpr, typeArgs, argExprs) ->
            { Type = "BasicPatterns.Application"
              Range = r e.Range
              Properties =
                  p [ yield!
                          typeArgs
                          |> List.map (fun n -> "typeArg" ==> fsharpTypeToProps n) ]
              Childs =
                  [ yield visitExpr funcExpr
                    yield! argExprs |> List.map visitExpr ]
              FsAstNode = box e }
        | BasicPatterns.Call (objExprOpt, memberOrFunc, typeArgs1, typeArgs2, argExprs) ->
            { Type = "BasicPatterns.Call"
              Range = r e.Range
              Properties =
                  p [ yield!
                          typeArgs1
                          |> List.map (fun n -> "typeArg" ==> fsharpTypeToProps n)
                      yield!
                          typeArgs2
                          |> List.map (fun n -> "typeArg2" ==> fsharpTypeToProps n)
                      yield
                          "memberOrFunc"
                          ==> fsharpMemberToProps memberOrFunc ]
              Childs =
                  [ if objExprOpt.IsSome
                    then yield visitExpr objExprOpt.Value
                    yield! argExprs |> List.map visitExpr ]
              FsAstNode = box e }
        | BasicPatterns.Coerce (targetType, inpExpr) ->
            { Type = "BasicPatterns.AddressSet"
              Range = r e.Range
              Properties = p [ yield "targetType" ==> fsharpTypeToProps targetType ]
              Childs = [ visitExpr inpExpr ]
              FsAstNode = box e }
        | BasicPatterns.FastIntegerForLoop (startExpr, limitExpr, consumeExpr, isUp) ->
            { Type = "BasicPatterns.FastIntegerForLoop"
              Range = r e.Range
              Properties = p [ yield "isUp" ==> isUp ]
              Childs =
                  [ visitExpr startExpr
                    visitExpr limitExpr
                    visitExpr consumeExpr ]
              FsAstNode = box e }
        | BasicPatterns.ILAsm (asmCode, typeArgs, argExprs) ->
            { Type = "BasicPatterns.ILAsm"
              Range = r e.Range
              Properties =
                  p [ yield "asmCode" ==> asmCode
                      yield!
                          typeArgs
                          |> List.map (fun n -> "typeArg" ==> fsharpTypeToProps n) ]
              Childs = [ yield! argExprs |> List.map visitExpr ]
              FsAstNode = box e }
        | BasicPatterns.ILFieldGet (objExprOpt, fieldType, fieldName) ->
            { Type = "BasicPatterns.ILFieldGet"
              Range = r e.Range
              Properties =
                  p [ yield "fieldType" ==> fsharpTypeToProps fieldType
                      yield "fieldName" ==> fieldName ]
              Childs =
                  [ if objExprOpt.IsSome
                    then yield visitExpr objExprOpt.Value ]
              FsAstNode = box e }
        | BasicPatterns.ILFieldSet (objExprOpt, fieldType, fieldName, valueExpr) ->
            { Type = "BasicPatterns.ILFieldGet"
              Range = r e.Range
              Properties =
                  p [ yield "fieldType" ==> fsharpTypeToProps fieldType
                      yield "fieldName" ==> fieldName ]
              Childs =
                  [ if objExprOpt.IsSome
                    then yield visitExpr objExprOpt.Value
                    yield visitExpr valueExpr ]
              FsAstNode = box e }
        | BasicPatterns.IfThenElse (guardExpr, thenExpr, elseExpr) ->
            { Type = "BasicPatterns.IfThenElse"
              Range = r e.Range
              Properties = p []
              Childs =
                  [ visitExpr guardExpr
                    visitExpr thenExpr
                    visitExpr elseExpr ]
              FsAstNode = box e }
        | BasicPatterns.Lambda (lambdaVar, bodyExpr) ->
            { Type = "BasicPatterns.Lambda"
              Range = r e.Range
              Properties = p [ "lambdaVar" ==> fsharpMemberToProps lambdaVar ]
              Childs = [ visitExpr bodyExpr ]
              FsAstNode = box e }
        | BasicPatterns.Let ((bindingVar, bindingExpr), bodyExpr) ->
            { Type = "BasicPatterns.Let"
              Range = r e.Range
              Properties = p [ "bindingVar" ==> fsharpMemberToProps bindingVar ]
              Childs =
                  [ visitExpr bindingExpr
                    visitExpr bodyExpr ]
              FsAstNode = box e }
        | BasicPatterns.LetRec (recursiveBindings, bodyExpr) ->
            { Type = "BasicPatterns.LetRec"
              Range = r e.Range
              Properties =
                  p [ "recursiveBindings"
                      ==> (recursiveBindings
                           |> List.map (fst >> fsharpMemberToProps)) ]
              Childs =
                  [ yield! recursiveBindings |> List.map (snd >> visitExpr)
                    yield visitExpr bodyExpr ]
              FsAstNode = box e }
        | BasicPatterns.NewArray (arrayType, argExprs) ->
            { Type = "BasicPatterns.NewArray"
              Range = r e.Range
              Properties = p [ "arrayType" ==> fsharpTypeToProps arrayType ]
              Childs = [ yield! List.map visitExpr argExprs ]
              FsAstNode = box e }
        | BasicPatterns.NewDelegate (delegateType, delegateBodyExpr) ->
            { Type = "BasicPatterns.NewDelegate"
              Range = r e.Range
              Properties = p [ "delegateType" ==> fsharpTypeToProps delegateType ]
              Childs = [ visitExpr delegateBodyExpr ]
              FsAstNode = box e }
        | BasicPatterns.NewObject (objType, typeArgs, argExprs) ->
            { Type = "BasicPatterns.NewObject"
              Range = r e.Range
              Properties =
                  p [ "objType" ==> fsharpMemberToProps objType
                      "typeArgs" ==> List.map fsharpTypeToProps typeArgs ]
              Childs = [ yield! List.map visitExpr argExprs ]
              FsAstNode = box e }
        | BasicPatterns.NewRecord (recordType, argExprs) ->
            { Type = "BasicPatterns.NewRecord"
              Range = r e.Range
              Properties = p [ "recordType" ==> fsharpTypeToProps recordType ]
              Childs = [ yield! List.map visitExpr argExprs ]
              FsAstNode = box e }
        | BasicPatterns.NewTuple (tupleType, argExprs) ->
            { Type = "BasicPatterns.NewTuple"
              Range = r e.Range
              Properties = p [ "tupleType" ==> fsharpTypeToProps tupleType ]
              Childs = [ yield! List.map visitExpr argExprs ]
              FsAstNode = box e }
        | BasicPatterns.NewUnionCase (unionType, unionCase, argExprs) ->
            { Type = "BasicPatterns.NewUnionCase"
              Range = r e.Range
              Properties =
                  p [ "unionType" ==> fsharpTypeToProps unionType
                      "unionCase" ==> fsharpUnionCaseToProps unionCase ]
              Childs = [ yield! List.map visitExpr argExprs ]
              FsAstNode = box e }
        | BasicPatterns.Quote (quotedExpr) ->
            { Type = "BasicPatterns.Quote"
              Range = r e.Range
              Properties = p []
              Childs = [ visitExpr quotedExpr ]
              FsAstNode = box e }
        | BasicPatterns.FSharpFieldGet (objExprOpt, recordOrClassType, fieldInfo) ->
            { Type = "BasicPatterns.FSharpFieldGet"
              Range = r e.Range
              Properties =
                  p [ "recordOrClassType"
                      ==> fsharpTypeToProps recordOrClassType
                      "fieldInfo" ==> fsharpFieldToProps fieldInfo ]
              Childs =
                  [ if objExprOpt.IsSome
                    then yield visitExpr objExprOpt.Value ]
              FsAstNode = box e }
        | BasicPatterns.FSharpFieldSet (objExprOpt, recordOrClassType, fieldInfo, argExpr) ->
            { Type = "BasicPatterns.FSharpFieldSet"
              Range = r e.Range
              Properties =
                  p [ "recordOrClassType"
                      ==> fsharpTypeToProps recordOrClassType
                      "fieldInfo" ==> fsharpFieldToProps fieldInfo ]
              Childs =
                  [ if objExprOpt.IsSome
                    then yield visitExpr objExprOpt.Value
                    yield visitExpr argExpr ]
              FsAstNode = box e }
        | BasicPatterns.Sequential (firstExpr, secondExpr) ->
            { Type = "BasicPatterns.Sequential"
              Range = r e.Range
              Properties = p []
              Childs =
                  [ visitExpr firstExpr
                    visitExpr secondExpr ]
              FsAstNode = box e }
        | BasicPatterns.TryFinally (bodyExpr, finalizeExpr) ->
            { Type = "BasicPatterns.TryFinally"
              Range = r e.Range
              Properties = p []
              Childs =
                  [ visitExpr bodyExpr
                    visitExpr finalizeExpr ]
              FsAstNode = box e }
        | BasicPatterns.TryWith (bodyExpr, _, _, catchVar, catchExpr) ->
            { Type = "BasicPatterns.TryWith"
              Range = r e.Range
              Properties = p [ "catchVar" ==> fsharpMemberToProps catchVar ]
              Childs =
                  [ visitExpr bodyExpr
                    visitExpr catchExpr ]
              FsAstNode = box e }
        | BasicPatterns.TupleGet (tupleType, tupleElemIndex, tupleExpr) ->
            { Type = "BasicPatterns.TupleGet"
              Range = r e.Range
              Properties =
                  p [ "tupleType" ==> fsharpTypeToProps tupleType
                      "tupleElemIndex" ==> tupleElemIndex ]
              Childs = [ visitExpr tupleExpr ]
              FsAstNode = box e }
        | BasicPatterns.DecisionTree (decisionExpr, decisionTargets) ->
            { Type = "BasicPatterns.DecisionTree"
              Range = r e.Range
              Properties =
                  p [ "decisionTargets"
                      ==> (List.map (fst >> List.map fsharpMemberToProps) decisionTargets) ]
              Childs =
                  [ yield visitExpr decisionExpr
                    yield! List.map (snd >> visitExpr) decisionTargets ]
              FsAstNode = box e }
        | BasicPatterns.DecisionTreeSuccess (decisionTargetIdx, decisionTargetExprs) ->
            { Type = "BasicPatterns.DecisionTreeSuccess"
              Range = r e.Range
              Properties = p [ "decisionTargetIdx" ==> decisionTargetIdx ]
              Childs = [ yield! List.map visitExpr decisionTargetExprs ]
              FsAstNode = box e }
        | BasicPatterns.TypeLambda (genericParam, bodyExpr) ->
            { Type = "BasicPatterns.TypeLambda"
              Range = r e.Range
              Properties =
                  p [ "genericParam"
                      ==> (List.map fsharpGenericParameterToProps genericParam) ]
              Childs = [ visitExpr bodyExpr ]
              FsAstNode = box e }
        | BasicPatterns.TypeTest (ty, inpExpr) ->
            { Type = "BasicPatterns.TypeTest"
              Range = r e.Range
              Properties = p [ "ty" ==> fsharpTypeToProps ty ]
              Childs = [ visitExpr inpExpr ]
              FsAstNode = box e }
        | BasicPatterns.UnionCaseSet (unionExpr, unionType, unionCase, unionCaseField, valueExpr) ->
            { Type = "BasicPatterns.UnionCaseSet"
              Range = r e.Range
              Properties =
                  p [ "unionType" ==> fsharpTypeToProps unionType
                      "unionCase" ==> fsharpUnionCaseToProps unionCase
                      "unionCaseField"
                      ==> fsharpFieldToProps unionCaseField ]
              Childs =
                  [ visitExpr unionExpr
                    visitExpr valueExpr ]
              FsAstNode = box e }
        | BasicPatterns.UnionCaseGet (unionExpr, unionType, unionCase, unionCaseField) ->
            { Type = "BasicPatterns.UnionCaseGet"
              Range = r e.Range
              Properties =
                  p [ "unionType" ==> fsharpTypeToProps unionType
                      "unionCase" ==> fsharpUnionCaseToProps unionCase
                      "unionCaseField"
                      ==> fsharpFieldToProps unionCaseField ]
              Childs = [ visitExpr unionExpr ]
              FsAstNode = box e }
        | BasicPatterns.UnionCaseTest (unionExpr, unionType, unionCase) ->
            { Type = "BasicPatterns.UnionCaseTest"
              Range = r e.Range
              Properties =
                  p [ "unionType" ==> fsharpTypeToProps unionType
                      "unionCase" ==> fsharpUnionCaseToProps unionCase ]
              Childs = [ visitExpr unionExpr ]
              FsAstNode = box e }
        | BasicPatterns.UnionCaseTag (unionExpr, unionType) ->
            { Type = "BasicPatterns.UnionCaseTag"
              Range = r e.Range
              Properties = p [ "unionType" ==> fsharpTypeToProps unionType ]
              Childs = [ visitExpr unionExpr ]
              FsAstNode = box e }
        | BasicPatterns.ObjectExpr (objType, baseCallExpr, overrides, interfaceImplementations) ->
            { Type = "BasicPatterns.ObjectExpr"
              Range = r e.Range
              Properties =
                  p [ "objType" ==> fsharpTypeToProps objType
                      "overrides"
                      ==> (List.map fsharpObjectExprOverrideToProps overrides)
                      "interfaceImplementations"
                      ==> (List.map
                               (fun (n, s) -> (fsharpTypeToProps n, List.map fsharpObjectExprOverrideToProps s))
                               interfaceImplementations) ]
              Childs = [ yield visitExpr baseCallExpr ]
              FsAstNode = box e }
        | BasicPatterns.TraitCall (sourceTypes, traitName, _, typeInstantiation, argTypes, argExprs) ->
            { Type = "BasicPatterns.TraitCall"
              Range = r e.Range
              Properties =
                  p [ "sourceTypes"
                      ==> List.map (fsharpTypeToProps) sourceTypes
                      "traitName" ==> traitName
                      "typeInstantiation"
                      ==> List.map (fsharpTypeToProps) typeInstantiation
                      "argTypes"
                      ==> List.map (fsharpTypeToProps) argTypes ]
              Childs = [ yield! List.map visitExpr argExprs ]
              FsAstNode = box e }
        | BasicPatterns.ValueSet (valToSet, valueExpr) ->
            { Type = "BasicPatterns.ValueSet"
              Range = r e.Range
              Properties = p [ "valToSet" ==> fsharpMemberToProps valToSet ]
              Childs = [ visitExpr valueExpr ]
              FsAstNode = box e }
        | BasicPatterns.WhileLoop (guardExpr, bodyExpr) ->
            { Type = "BasicPatterns.WhileLoop"
              Range = r e.Range
              Properties = p []
              Childs =
                  [ visitExpr guardExpr
                    visitExpr bodyExpr ]
              FsAstNode = box e }
        | BasicPatterns.BaseValue baseType ->
            { Type = "BasicPatterns.BaseValue"
              Range = r e.Range
              Properties = p [ "baseType" ==> fsharpTypeToProps baseType ]
              Childs = []
              FsAstNode = box e }
        | BasicPatterns.DefaultValue defaultType ->
            { Type = "BasicPatterns.DefaultValue"
              Range = r e.Range
              Properties = p [ "baseType" ==> fsharpTypeToProps defaultType ]
              Childs = []
              FsAstNode = box e }
        | BasicPatterns.ThisValue thisType ->
            { Type = "BasicPatterns.ThisValue"
              Range = r e.Range
              Properties = p [ "thisType" ==> fsharpTypeToProps thisType ]
              Childs = []
              FsAstNode = box e }
        | BasicPatterns.Const (constValueObj, constType) ->
            { Type = "BasicPatterns.Const"
              Range = r e.Range
              Properties =
                  p [ "constValueObj" ==> constValueObj
                      "constType" ==> fsharpTypeToProps constType ]
              Childs = []
              FsAstNode = box e }
        | BasicPatterns.Value (valueToGet) ->
            { Type = "BasicPatterns.Value"
              Range = r e.Range
              Properties = p [ "valueToGet" ==> fsharpMemberToProps valueToGet ]
              Childs = []
              FsAstNode = box e }
        | _ ->
            { Type = "Unknown"
              Range = r e.Range
              Properties = p []
              Childs = []
              FsAstNode = box e }

    let rec visitDeclaration d: Node option =
        match d with
        | FSharpImplementationFileDeclaration.Entity (e, subDecls) ->
            Some
                { Type = "FSharpImplementationFileDeclaration.Entity"
                  Range = None
                  Properties = p [ "e" ==> fsharpEntityToProps e ]
                  Childs = [ yield! List.choose visitDeclaration subDecls ]
                  FsAstNode = box d }
        | FSharpImplementationFileDeclaration.MemberOrFunctionOrValue (v, vs, e) when not v.IsCompilerGenerated ->
            Some
                { Type = "FSharpImplementationFileDeclaration.MemberOrFunctionOrValue"
                  Range = r e.Range
                  Properties =
                      p [ "v" ==> fsharpMemberToProps v
                          "vs"
                          ==> (List.map (List.map fsharpMemberToProps) vs) ]
                  Childs = [ visitExpr e ]
                  FsAstNode = box d }
        | FSharpImplementationFileDeclaration.InitAction (e) ->
            Some
                { Type = "FSharpImplementationFileDeclaration.InitAction"
                  Range = r e.Range
                  Properties = p []
                  Childs = [ visitExpr e ]
                  FsAstNode = box d }
        | _ -> None

let tastToNode tast: Tast.Node =
    let child = tast |> List.choose Tast.visitDeclaration

    { Type = "File"
      Range = None
      Properties = Map.empty
      Childs = child
      FsAstNode = box tast }
